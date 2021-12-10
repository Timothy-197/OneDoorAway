using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//--------------------------------------------------------------------
//WallStick module is a movement ability
//If a wall is detected, WallStick will let the character cling on to that position, as long as input is moving into the wall
//--------------------------------------------------------------------
public class WallStickModule : GroundedControllerAbilityModule
{
    Vector3 m_WallNormal;
    Vector3 m_UpDirection;
    RotateMethod m_RotateMethod;

    //Reset all state when this module gets initialized
    protected override void ResetState()
    {
        base.ResetState();
        m_WallNormal = Vector3.zero;
        m_UpDirection = Vector3.zero;
        m_RotateMethod = RotateMethod.FromBottom;
    }

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl()
    {
        m_ControlledCollider.RotateToAlignWithNormal(m_UpDirection, m_RotateMethod);
        m_ControlledCollider.UpdateContextInfo();
    }

    //Called whenever this module is ended (was active, now is inactive)
    protected override void EndModuleImpl()
    {
        //Reset rotation
        m_ControlledCollider.RotateToAlignWithNormal(Vector3.up, m_RotateMethod);
        m_ControlledCollider.UpdateContextInfo();
    }

    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule()
    {
        m_WallNormal = m_ControlledCollider.GetSideCastInfo().GetSideNormal();
        m_RotateMethod = (m_WallNormal.y < 0) ? RotateMethod.FromTop : RotateMethod.FromBottom;
        m_UpDirection = CState.GetDirectionAlongNormal(Vector3.up, m_WallNormal);
        m_ControlledCollider.RotateToAlignWithNormal(m_UpDirection, m_RotateMethod);

        float distance = m_ControlledCollider.GetSideCastInfo().GetDistance();
        m_ControlledCollider.GetCapsuleTransform().Move(-m_WallNormal * distance);

        m_ControlledCollider.UpdateWithVelocity(Vector2.zero);
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
    public override bool IsApplicable()
    {
        if (m_ControlledCollider.IsGrounded())
        {
            return false;
        }
        //Wall needs to be hit for the module to be active
        if (m_ControlledCollider.GetSideCastInfo().m_HasHitSide)
        {
            if (!m_ControlledCollider.IsPartiallyTouchingWall())
            {
                return false;
            }
            m_WallNormal = m_ControlledCollider.GetSideCastInfo().GetSideNormal();
            m_RotateMethod = (m_WallNormal.y < 0) ? RotateMethod.FromTop : RotateMethod.FromBottom;
            m_UpDirection = CState.GetDirectionAlongNormal(Vector3.up, m_WallNormal);

            float inputDot = Vector2.Dot(GetDirInput("Move").m_ClampedInput, m_WallNormal);
            if (inputDot >= 0.0f)
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
    public override bool CanEnd()
    {
        if (!m_ControlledCollider.CanAlignWithNormal(Vector3.up, m_RotateMethod))
        {
            return false;
        }
        return true;
    }

    //Get the name of the animation state that should be playing for this module. 
    public override string GetSpriteState()
    {
        return "WallSlide";
    }
}
