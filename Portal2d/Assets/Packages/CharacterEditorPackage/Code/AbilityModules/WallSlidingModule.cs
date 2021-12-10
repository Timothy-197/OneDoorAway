using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//--------------------------------------------------------------------
//WallSliding module is a movement ability
//If a wall is detected, wallslide uses gravity and friction values to make the character slide along the wall
//--------------------------------------------------------------------
public class WallSlidingModule : GroundedControllerAbilityModule
{
    [SerializeField] float m_SlideFriction = 0.0f;
    [SerializeField] float m_SlideGravity = 0.0f;
    [SerializeField] bool m_ApplyFrictionIfGoingUpwards = false;
    [SerializeField] float m_EscapeCornerRaycastDistance = 0.0f;
    [SerializeField] float m_EscapeVelocity = 0.0f;
    [SerializeField] bool m_HonorJumpCut  = false;
    [SerializeField] bool m_UseSameGravityForAllSlopes  = false;
    Vector3 m_WallNormal;
    Vector3 m_UpDirection;
    RotateMethod m_RotateMethod;

    //Reset all state when this module gets initialized
    protected override void ResetState(){
        base.ResetState();
        m_WallNormal = Vector3.zero;
        m_UpDirection = Vector3.zero;
        m_RotateMethod = RotateMethod.FromBottom;
    }

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl(){
        m_ControlledCollider.RotateToAlignWithNormal(m_UpDirection, m_RotateMethod);
        m_ControlledCollider.UpdateContextInfo();
    }

    //Called whenever this module is ended (was active, now is inactive)
    protected override void EndModuleImpl(){
        //Reset rotation
        m_ControlledCollider.RotateToAlignWithNormal(Vector3.up, m_RotateMethod);
        m_ControlledCollider.UpdateContextInfo();
    }

    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule(){
        m_WallNormal = m_ControlledCollider.GetSideCastInfo().GetSideNormal();
        m_RotateMethod = (m_WallNormal.y < 0) ? RotateMethod.FromTop : RotateMethod.FromBottom;
        m_UpDirection = CState.GetDirectionAlongNormal(Vector3.up, m_WallNormal);
        m_ControlledCollider.RotateToAlignWithNormal(m_UpDirection, m_RotateMethod);

        //if touching a wall during a jump, cut it short when jump is released
        if (m_HonorJumpCut)
        {
            m_CharacterController.UpdateJumpCut();
        }

        Vector2 currentVel = m_ControlledCollider.GetVelocity();

        Vector2 fInput = m_CharacterController.GetDirectedInputMovement() * m_EscapeVelocity;
        if (m_WallNormal.y >= 0.1f)
        {
            fInput = Vector2.zero;
        }
        float inputDotToNormal = Vector2.Dot(fInput, m_WallNormal);
        if (inputDotToNormal <= 0)//Moving into wall, or not moving at all
        {
            fInput = Vector2.zero;
            float distance = m_ControlledCollider.GetSideCastInfo().GetDistance();
            m_ControlledCollider.GetCapsuleTransform().Move(-m_WallNormal * distance);
        }

        Vector2 fGravity = -m_UpDirection * m_SlideGravity;// 
        if (!m_UseSameGravityForAllSlopes)
        {
            fGravity *= Vector2.Dot(-m_UpDirection, Vector2.down);//Gravity along wall, but with corrected gravity
        }
        Vector2 fDrag = -0.5f * (currentVel.sqrMagnitude) * m_CharacterController.GetDragConstant() * currentVel.normalized;

        Vector2 summedF = fInput + fGravity + fDrag;

        Vector2 newVel = currentVel + (summedF * Time.fixedDeltaTime);

        newVel += GetFrictionAlongWall(newVel);
        
        m_ControlledCollider.UpdateWithVelocity(newVel);
    }

    //Called to place a MovingColPoint to support moving colliders
    public override void PlaceMovingColPoint()
    {
        //MOVINGCOLPOINT, see CapsuleMovingColliderSolver for more details
        CSideCastInfo info = m_ControlledCollider.GetSideCastInfo();
        m_ControlledCollider.AddColPoint(info.GetWallTransform(), info.GetSidePoint(), info.GetSideNormal());
    }

    Vector2 GetFrictionAlongWall(Vector2 a_Velocity)
    {
        Vector2 directionAlongWall = CState.GetDirectionAlongNormal(a_Velocity, new Vector2(m_WallNormal.x, m_WallNormal.y));
        if (!m_ApplyFrictionIfGoingUpwards && directionAlongWall.y > 0)//Going upwards
        {
            return Vector2.zero;
        }
        Vector2 frictionDirection = -directionAlongWall;

        Vector2 maxFrictionSpeedChange = frictionDirection * m_SlideFriction * Time.fixedDeltaTime;

        Vector2 velInDirection = Mathf.Abs(Vector2.Dot(a_Velocity, frictionDirection)) * frictionDirection;

        if (velInDirection.magnitude > maxFrictionSpeedChange.magnitude)
        {
            return maxFrictionSpeedChange;
        }
        else
        {
            return velInDirection;
        }
    }

    //Query whether this module can be active, given the current state of the character controller (velocity, isGrounded etc.)
    //Called every frame when inactive (to see if it could be) and when active (to see if it should not be)
    public override bool IsApplicable(){
        if (m_ControlledCollider.IsGrounded())
        {
            return false;
        }
        //Wall needs to be hit for the module to be active
        if (m_ControlledCollider.GetSideCastInfo().m_HasHitSide)
        {
            if (m_IsActive)
            {
                if (!m_ControlledCollider.IsPartiallyTouchingWall())
                {
                    return false;
                }
            }
            else
            {
                if (!m_ControlledCollider.IsCompletelyTouchingWall())
                {
                    return false;
                }
            }
            m_WallNormal = m_ControlledCollider.GetSideCastInfo().GetSideNormal();
            m_RotateMethod = (m_WallNormal.y < 0) ? RotateMethod.FromTop : RotateMethod.FromBottom;
            m_UpDirection = CState.GetDirectionAlongNormal(Vector3.up, m_WallNormal);
            if (!m_ControlledCollider.CanAlignWithNormal(m_UpDirection))
            {
                return false;
            }
            RaycastHit hit;
            //If moving down, and possibly entering a position where it cannot rotate back (and therefore escape), stop before this happens
            if (Vector3.Dot(m_ControlledCollider.GetVelocity(), m_UpDirection) < 0 && Physics.Raycast(m_ControlledCollider.GetDownCenter(), -m_UpDirection, out hit, m_ControlledCollider.GetRadius() + m_EscapeCornerRaycastDistance, m_ControlledCollider.GetLayerMask()))
            {
                return false;
            }
            return true;
        }
        return false;
    }
    //Query whether this module can be deactivated without bad results (clipping etc.)
    public override bool CanEnd(){
        if (!m_ControlledCollider.CanAlignWithNormal(Vector3.up, m_RotateMethod))
        {
            return false;
        }
        return true;
    }
}
