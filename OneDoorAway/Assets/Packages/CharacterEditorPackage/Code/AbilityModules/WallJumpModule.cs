using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//--------------------------------------------------------------------
//WallJump module is a movement ability
//When a wall is detected, and the appropriate input is given, jump away from the wall
//Has a variation: MeatBoyWallJump
//--------------------------------------------------------------------
public class WallJumpModule : GroundedControllerAbilityModule
{
    [SerializeField] float m_JumpVelocity = 0.0f;
    [SerializeField] float m_JumpAlignedToWallFactor = 0.0f;
    [SerializeField] bool m_UseWallUpDirection = false;
    [SerializeField] bool m_ResetVerticalSpeedIfFalling = false;
    [SerializeField] float m_VerticalVelocityInheritanceFactor = 0.0f;
    [SerializeField] float m_HorizontalVelocityInheritanceFactor = 0.0f;

    Vector2 m_SideNormal;

    //Reset all state when this module gets initialized
    protected override void ResetState(){
        base.ResetState();
        m_SideNormal = Vector2.zero;
    }

    //Execute jump (lasts one update)
    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule(){
        Vector2 currentVel = m_ControlledCollider.GetVelocity();
        //Make sure that current velocity is aligned to the side wall
        currentVel = CState.GetDirectionAlongNormal(currentVel, m_SideNormal) * currentVel.magnitude;
        //If sliding down the wall when jumping, reset that velocity
        if (m_ResetVerticalSpeedIfFalling)
        {
            currentVel.y = Mathf.Clamp(currentVel.y, 0.0f, float.MaxValue);
        }
        //Possibly inherit some velocity from before jumping, leading to increased velocity on jump
        currentVel.y *= m_VerticalVelocityInheritanceFactor;
        currentVel.x *= m_HorizontalVelocityInheritanceFactor;
        Vector2 up = (m_UseWallUpDirection) ? CState.GetDirectionAlongNormal(Vector2.up, m_SideNormal) : Vector2.up;
        Vector2 jumpDirection = Vector2.Lerp(up, m_SideNormal, m_JumpAlignedToWallFactor).normalized;
        Vector2 jumpVelocity = jumpDirection * m_JumpVelocity;

        m_CharacterController.Jump(currentVel + jumpVelocity);

        m_ControlledCollider.UpdateWithVelocity(m_ControlledCollider.GetVelocity());
    }

    //Query whether this module can be active, given the current state of the character controller (velocity, isGrounded etc.)
    //Called every frame when inactive (to see if it could be) and when active (to see if it should not be)
    public override bool IsApplicable(){
        if (m_ControlledCollider.IsGrounded())
        {
            return false;
        }

        if (m_ControlledCollider.GetSideCastInfo().m_HasHitSide)
        {
            if (!m_ControlledCollider.IsPartiallyTouchingWall())
            {
                return false;
            }
            if (m_CharacterController.GetJumpIsCached())
            {
                if ((Time.time - m_CharacterController.GetLastTouchingSurfaceTime() <= m_CharacterController.GetGroundedToleranceTime()) && !m_CharacterController.DidJustJump())
                {
                    m_SideNormal = m_ControlledCollider.GetSideCastInfo().GetSideNormal();
                    return true;
                }
            }
        }
        return false;
    }

    //Get the name of the animation state that should be playing for this module. 
    public override string GetSpriteState(){
        return "JumpSide";
    }
}
