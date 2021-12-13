using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//--------------------------------------------------------------------
//LedgeHang module is a movement ability
//When a ledge is detected, ledgehang sticks to it as long as the ledge is valid and no other input is given (or it is overriden by another module)
//--------------------------------------------------------------------
public class LedgeHangModule : GroundedControllerAbilityModule
{
    [SerializeField] bool m_DisableWhenGoingUpwards = false;
    [SerializeField] float m_LedgeHangCoolDown = 0.0f;

    float m_LastLedgeHangTime;

    //Reset all state when this module gets initialized
    protected override void ResetState(){
        base.ResetState();
        m_LastLedgeHangTime = 0.0f;
    }

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl(){
        m_ControlledCollider.SetUpCenter(m_ControlledCollider.GetEdgeCastInfo().GetProposedHeadPoint());
        m_ControlledCollider.RotateToAlignWithNormal(m_ControlledCollider.GetEdgeCastInfo().GetUpDirection(), RotateMethod.FromTop);
        m_ControlledCollider.UpdateContextInfo();
    }

    //Called whenever this module is ended (was active, now is inactive)
    protected override void EndModuleImpl(){
        m_LastLedgeHangTime = Time.time;
    }

    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule(){
        m_ControlledCollider.SetUpCenter(m_ControlledCollider.GetEdgeCastInfo().GetProposedHeadPoint());
        m_ControlledCollider.RotateToAlignWithNormal(m_ControlledCollider.GetEdgeCastInfo().GetUpDirection(), RotateMethod.FromTop);

        if (m_ControlledCollider.GetSideCastInfo().m_WallCastCount >= 2)
        {
            Vector3 normal = m_ControlledCollider.GetSideCastInfo().GetSideNormal();
            float distance = m_ControlledCollider.GetSideCastInfo().GetDistance();
            m_ControlledCollider.GetCapsuleTransform().Move(-normal * distance);
        }
        m_ControlledCollider.UpdateWithVelocity(Vector2.zero);

    }

    //Called to place a MovingColPoint to support moving colliders
    public override void PlaceMovingColPoint()
    {
        //MOVINGCOLPOINT, see CapsuleMovingColliderSolver for more details
        CEdgeCastInfo info = m_ControlledCollider.GetEdgeCastInfo();
        m_ControlledCollider.AddColPoint(info.GetEdgeTransform(), info.GetEdgePoint(), info.GetEdgeNormal());
    }

    //Query whether this module can be active, given the current state of the character controller (velocity, isGrounded etc.)
    //Called every frame when inactive (to see if it could be) and when active (to see if it should not be)
    public override bool IsApplicable(){
        if (m_ControlledCollider.IsGrounded() ||
            (DoesInputExist("Crouch") && GetButtonInput("Crouch").m_IsPressed) ||
            GetDirInput("Move").m_Direction == DirectionInput.Direction.Down)
        {
            return false;
        }
        if (m_DisableWhenGoingUpwards && m_ControlledCollider.GetVelocity().y > 0.0f)
        {
            return false;
        }
        if (!m_IsActive && Time.time - m_LastLedgeHangTime < m_LedgeHangCoolDown)
        {
            return false;
        }
        if (m_ControlledCollider.IsTouchingEdge())
        {
            return true;
        }
        return false;
    }

    //Get the name of the animation state that should be playing for this module. 
    public override string GetSpriteState(){
        //Reach out in anticipation of walljump
        if (m_ControlledCollider.IsPartiallyTouchingWall() && GetDirInput("Move").IsInThisDirection(m_ControlledCollider.GetEdgeCastInfo().GetWallNormal()))
        {
            return "LedgeHangSideReach";
        }
        return "LedgeHang";
    }
}
