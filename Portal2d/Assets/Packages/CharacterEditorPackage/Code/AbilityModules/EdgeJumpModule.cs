using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//--------------------------------------------------------------------
//EdgeJump module is a movement ability
//When a ledge is detected, and the appropriate input is given, jump upwards along the ledge.
//--------------------------------------------------------------------
public class EdgeJumpModule : GroundedControllerAbilityModule
{
    [SerializeField] float m_JumpVelocity = 0.0f;
    Vector3 m_ProposedNewPosition;

    //Reset all state when this module gets initialized
    protected override void ResetState(){
        base.ResetState();
        m_ProposedNewPosition = Vector3.zero;
    }

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl(){
        //First move up
        m_ControlledCollider.GetCapsuleTransform().SetPosition(m_ProposedNewPosition);
    }
    //Execute jump (lasts one update)
    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule(){
        Vector3 up = m_ControlledCollider.GetEdgeCastInfo().GetUpDirection();
        Vector2 jumpVelocity = up * m_JumpVelocity;
        m_CharacterController.Jump(jumpVelocity);

        m_ControlledCollider.UpdateWithVelocity(m_ControlledCollider.GetVelocity());
    }

    //Query whether this module can be active, given the current state of the character controller (velocity, isGrounded etc.)
    //Called every frame when inactive (to see if it could be) and when active (to see if it should not be)
    public override bool IsApplicable(){
        if (m_ControlledCollider.IsGrounded() || (DoesInputExist("Crouch") && GetButtonInput("Crouch").m_IsPressed) || GetDirInput("Move").m_Direction == DirectionInput.Direction.Down || m_CharacterController.DidJustJump())
        {
            return false;
        }
        //Prevent overriding walljumps
        if (GetDirInput("Move").IsInThisDirection(m_ControlledCollider.GetEdgeCastInfo().GetWallNormal()))
        {
            return false;
        }
        if ((m_CharacterController.GetJumpIsCached()) && m_ControlledCollider.IsTouchingEdge())
        {
            //Move up to avoid transitioning into a wallrun.
            //Also used to prevent small distance jumps.
            CEdgeCastInfo edgeInfo = m_ControlledCollider.GetEdgeCastInfo();
            CapsuleTransform copy = m_ControlledCollider.GetCapsuleTransformCopy();
            Vector3 headStartDisplacement = edgeInfo.GetEdgeNormal() * (m_ControlledCollider.GetRadius() + 0.1f) + edgeInfo.GetWallNormal() * 0.015f;
            if (!copy.CanMove(headStartDisplacement, true))
            {
                return false;
            }
            copy.Move(headStartDisplacement);
            m_ProposedNewPosition = copy.GetPosition();
            return true;
        }
        return false;
    }
    //Get the name of the animation state that should be playing for this module. 
    public override string GetSpriteState(){
        return "LedgeHang";
    }
}
