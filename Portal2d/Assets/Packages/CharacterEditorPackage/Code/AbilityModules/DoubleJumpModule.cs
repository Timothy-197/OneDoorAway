using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//--------------------------------------------------------------------
//DoubleJump module is a movement ability
//When in the air, allow more jumps
//--------------------------------------------------------------------
public class DoubleJumpModule : GroundedControllerAbilityModule
{
    [SerializeField] bool m_UseNormalJumpVelocity = false;
    [SerializeField] float m_JumpVelocity = 0.0f;
    [SerializeField] int m_AmountOfDoubleJumpsAllowed = 0;

    [SerializeField] bool m_ResetDoubleJumpsAfterTouchingWall = false;
    [SerializeField] bool m_ResetDoubleJumpsAfterTouchingEdge = false;

    int m_DoubleJumpsLeft;

    //Reset all state when this module gets initialized
    protected override void ResetState()
    {
        base.ResetState();
        m_DoubleJumpsLeft = m_AmountOfDoubleJumpsAllowed;
    }

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl()
    {
        m_DoubleJumpsLeft -= 1;
    }

    //Execute jump (lasts one update)
    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule()
    {
        float jumpVelocity = m_JumpVelocity;
        if (m_UseNormalJumpVelocity)
        {
            jumpVelocity = m_CharacterController.GetJumpVelocity();
        }
        Vector2 newVelocity = m_ControlledCollider.GetVelocity();
        newVelocity.y = Mathf.Max(0.0f, newVelocity.y);
        newVelocity += Vector2.up * jumpVelocity;

        m_CharacterController.Jump(newVelocity);
        m_ControlledCollider.UpdateWithVelocity(m_ControlledCollider.GetVelocity());
    }

    //Called whenever this module is inactive and updating (implementation by child modules), useful for cooldown updating etc.
    public override void InactiveUpdateModule()
    {
        if (m_ControlledCollider.IsGrounded() ||
           (m_ControlledCollider.IsPartiallyTouchingWall() && m_ResetDoubleJumpsAfterTouchingWall) ||
           (m_ControlledCollider.IsTouchingEdge() && m_ResetDoubleJumpsAfterTouchingEdge))
        {
            m_DoubleJumpsLeft = m_AmountOfDoubleJumpsAllowed;
        }
    }

    //Query whether this module can be active, given the current state of the character controller (velocity, isGrounded etc.)
    //Called every frame when inactive (to see if it could be) and when active (to see if it should not be)
    public override bool IsApplicable()
    {
        if (m_DoubleJumpsLeft <= 0)
        {
            return false;
        }

        //Prevent overriding the default jump, or triggering just after a previous jump
        if (m_ControlledCollider.IsGrounded() || m_CharacterController.DidJustJump() || (Time.fixedTime - m_CharacterController.GetLastGroundedTime() <= m_CharacterController.GetGroundedToleranceTime()))
        {
            return false;
        }

        if ((m_CharacterController.GetJumpIsCached()))
        {
            return true;
        }
        return false;
    }
}
