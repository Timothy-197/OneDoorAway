using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//--------------------------------------------------------------------
//WallRunModule is a movement ability
//If a wall is detected, the character gets a boost upwards. This boost will decay over time due to gravity and possibly friction
//There are settings to avoid chaining these wallruns infinitely
//--------------------------------------------------------------------
public class WallRunModule : GroundedControllerAbilityModule
{
    [SerializeField] float m_WallRunGravity = 0.0f;
    [SerializeField] float m_WallRunForce = 0.0f;
    [SerializeField] bool m_DisableWhenBelowVelocity = false;
    [SerializeField] float m_MinimumUpVelocityOnStart = 0.0f;
    [SerializeField] bool m_AllowOnlyWhenJustTouchingWall = false;
    [SerializeField] bool m_ResetVelocityOnStart = false;
    [SerializeField] bool m_ApplyDrag = false;

    Vector3 m_WallNormal;
    Vector3 m_UpDirection;
    RotateMethod m_RotateMethod;
    bool m_IsAlreadyTouchingWall;

    //Reset all state when this module gets initialized
    protected override void ResetState(){
        base.ResetState();
        m_WallNormal = Vector3.zero;
        m_UpDirection = Vector3.zero;
        m_RotateMethod = RotateMethod.FromBottom;
        m_IsAlreadyTouchingWall = false;
    }

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl(){
        m_ControlledCollider.RotateToAlignWithNormal(m_UpDirection, m_RotateMethod);
        Vector2 currentVel = m_ControlledCollider.GetVelocity();

        if (m_ResetVelocityOnStart)
        {
            currentVel = Vector2.zero;
        }
            
        Vector2 wallRunVelocity = m_UpDirection * m_WallRunForce;

        currentVel += wallRunVelocity;

        m_ControlledCollider.SetVelocity(currentVel);
        m_ControlledCollider.UpdateContextInfo();
    }

    //Called whenever this module is ended (was active, now is inactive)
    protected override void EndModuleImpl(){
        m_ControlledCollider.RotateToAlignWithNormal(Vector3.up, m_RotateMethod);
        m_ControlledCollider.UpdateContextInfo();
    }

    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule(){
        if (!m_ControlledCollider.GetSideCastInfo().m_HasHitSide || (!m_ControlledCollider.IsPartiallyTouchingWall()))
        {
            return;
        }
        m_WallNormal = m_ControlledCollider.GetSideCastInfo().GetSideNormal();
        float distance = m_ControlledCollider.GetSideCastInfo().GetDistance();
        m_ControlledCollider.GetCapsuleTransform().Move(-m_WallNormal * distance);
        m_RotateMethod = (m_WallNormal.y < 0) ? RotateMethod.FromTop : RotateMethod.FromBottom;
        m_UpDirection = CState.GetDirectionAlongNormal(Vector3.up, m_WallNormal);
        m_ControlledCollider.RotateToAlignWithNormal(m_UpDirection, m_RotateMethod);

        Vector2 currentVel = m_ControlledCollider.GetVelocity();

        Vector2 fGravity = -m_UpDirection * m_WallRunGravity * Vector2.Dot(-m_UpDirection, Vector2.down);//Gravity along wall, but with correct velocity
        
        Vector2 fDrag = -0.5f * (currentVel.sqrMagnitude) * m_CharacterController.GetDragConstant() * currentVel.normalized;
        if (!m_ApplyDrag)
        {
            fDrag = Vector2.zero;
        }
        Vector2 summedF = fGravity + fDrag;

        Vector2 newVel = currentVel + (summedF * Time.fixedDeltaTime);

        m_ControlledCollider.UpdateWithVelocity(newVel);
    }

    //Called to place a MovingColPoint to support moving colliders
    public override void PlaceMovingColPoint()
    {
        //MOVINGCOLPOINT, see CapsuleMovingColliderSolver for more details
        CSideCastInfo info = m_ControlledCollider.GetSideCastInfo();
        m_ControlledCollider.AddColPoint(info.GetWallTransform(), info.GetSidePoint(), info.GetSideNormal());
    }

    //Query whether this module can be active, given the current state of the character controller (velocity, isGrounded etc.)
    //Called every frame when inactive (to see if it could be) and when active (to see if it should not be)
    public override bool IsApplicable(){
        if (m_ControlledCollider.IsGrounded())
        {
            return false;
        }
        //Do allow if we just jumped up
        if (m_CharacterController.DidJustJump())
        {
            m_IsAlreadyTouchingWall = false;
        }
        if ((DoesInputExist("Crouch") && GetButtonInput("Crouch").m_IsPressed) ||
            GetDirInput("Move").m_Direction == DirectionInput.Direction.Down)
        {
            return false;
        }
        //Disable when already touching wall beforehand, except when coming from standing position
        if (!m_IsActive && m_AllowOnlyWhenJustTouchingWall && (Time.time - m_CharacterController.GetLastGroundedTime() >= 0.02f))
        { 
            if (m_ControlledCollider.GetSideCastInfo().m_HasHitSide && (m_ControlledCollider.IsCompletelyTouchingWall()))
            {
                if (m_IsAlreadyTouchingWall)
                {
                    return false;
                }
                m_IsAlreadyTouchingWall = true;
            }
            else
            {
                m_IsAlreadyTouchingWall = false;
                return false;
            }
        }
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
            float upVelDot = Vector2.Dot(m_UpDirection, m_ControlledCollider.GetVelocity());
            if (m_DisableWhenBelowVelocity && upVelDot <= m_MinimumUpVelocityOnStart) //Already going downwards, not applicable
            {
                return false;
            }
            if (!m_ControlledCollider.CanAlignWithNormal(m_UpDirection))
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
