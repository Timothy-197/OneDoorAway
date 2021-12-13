using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Crouching module is a movement ability
//If the crouch input is pressed, and the character is on the ground, shrink the character's capsule
//--------------------------------------------------------------------
public class CrouchingModule : GroundedControllerAbilityModule
{
    [SerializeField] float m_CrouchHeight = 0.0f;
    [SerializeField] float m_CrouchAcceleration = 0.0f;
    [SerializeField] float m_MaxCrouchSpeed = 0.0f;
    [SerializeField] float m_CrouchBrakeDeceleration = 0.0f;
    [SerializeField] float m_CrouchFriction = 0.0f;

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl(){
        if (m_ControlledCollider != null)
        {      
            m_ControlledCollider.SetLength(m_CrouchHeight, CapsuleResizeMethod.FromBottom);
            m_ControlledCollider.UpdateContextInfo();
        }
    }

    //Called whenever this module is ended (was active, now is inactive)
    protected override void EndModuleImpl(){
        if (m_ControlledCollider != null)
        {
            m_ControlledCollider.SetLength(m_ControlledCollider.GetDefaultLength(), CapsuleResizeMethod.FromBottom);
            m_ControlledCollider.UpdateContextInfo();
        }
    }
    //Walk across the floor, but with different values for speed and friction
    //Can be used to slow down the movement when crouching
    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule(){        
        if (CanEnd())
        {
            if (m_CharacterController.TryDefaultJump())
            {
                m_ControlledCollider.UpdateWithVelocity(m_ControlledCollider.GetVelocity());
                return;
            }
        }
        Vector2 currentVel = m_ControlledCollider.GetVelocity();
        Vector2 fInput = m_CharacterController.GetDirectedInputMovement() * m_CrouchAcceleration;
        fInput = m_CharacterController.ClampInputVelocity(fInput, currentVel, m_MaxCrouchSpeed);
        Vector2 fGravity = m_CharacterController.GetGravity();
        Vector2 fDrag = -0.5f * (currentVel.sqrMagnitude) * m_CharacterController.GetDragConstant() * currentVel.normalized;

        Vector2 summedF = fInput + fGravity + fDrag;

        Vector2 newVel = currentVel + summedF * Time.fixedDeltaTime;

        newVel += m_CharacterController.GetStoppingForce(newVel, m_CrouchBrakeDeceleration);
        Vector2 friction = m_CharacterController.GetFriction(newVel, summedF, m_CrouchFriction);
        newVel += friction;

        m_ControlledCollider.UpdateWithVelocity(newVel);
        m_CharacterController.TryAligningWithGround();
    }
    //Character needs to be on the floor and pressing the crouch button, or moving down with arrow keys/analogue stick
    //Query whether this module can be active, given the current state of the character controller (velocity, isGrounded etc.)
    //Called every frame when inactive (to see if it could be) and when active (to see if it should not be)
    public override bool IsApplicable(){
        if (m_ControlledCollider.IsGrounded() &&
            ((DoesInputExist("Crouch") && GetButtonInput("Crouch").m_IsPressed) || GetDirInput("Move").m_Direction == DirectionInput.Direction.Down ||
            (!m_ControlledCollider.CanBeResized(m_ControlledCollider.GetDefaultLength(), CapsuleResizeMethod.FromBottom))))
        {
            return true;
        }
        return false;
    }
    //Query whether this module can be deactivated without bad results (clipping etc.)
    public override bool CanEnd(){
        if (m_ControlledCollider != null)
        {
            return m_ControlledCollider.CanBeResized(m_ControlledCollider.GetDefaultLength(), CapsuleResizeMethod.FromBottom);      
            
        }
        return true;
    }
    //Get the name of the animation state that should be playing for this module. 
    public override string GetSpriteState(){
        if (Mathf.Abs(m_ControlledCollider.GetVelocity().x) < 0.0001f)
        {
            return "CrouchIdle";
        }
        else
        {
            return "Crouch"; 
        }
    }
}
