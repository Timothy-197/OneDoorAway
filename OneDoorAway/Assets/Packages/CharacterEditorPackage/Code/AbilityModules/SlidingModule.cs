using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Sliding is a movement ability
//It triggers when the character is pressing crouch when on the ground and moving above a certain threshold speed
//Sliding has different values for gravity and friction than the default movement
//--------------------------------------------------------------------
public class SlidingModule : GroundedControllerAbilityModule
{
    [SerializeField] float m_SlideHeight = 0.0f;
    [SerializeField] float m_SlideFriction = 0.0f;
    [SerializeField] float m_SlideGravity = 0.0f;
    [SerializeField] float m_SpeedNeededToStart = 0.0f;
    [SerializeField] float m_LowestPossibleSlideSpeed = 0.0f;

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl(){
        if (m_ControlledCollider != null)
        {
            m_ControlledCollider.SetLength(m_SlideHeight, CapsuleResizeMethod.FromBottom);
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
    //Moves similar to default, but does not process input and overrides friction and gravity
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

        Vector2 fGravity = Vector2.down * m_SlideGravity;
        Vector2 fDrag = -0.5f * (currentVel.sqrMagnitude) * m_CharacterController.GetDragConstant() * currentVel.normalized;

        Vector2 summedF = fGravity + fDrag;

        Vector2 newVel = currentVel + summedF * Time.fixedDeltaTime;

        Vector2 friction = m_CharacterController.GetFriction(newVel, summedF, m_SlideFriction);
        newVel += friction;
        m_ControlledCollider.UpdateWithVelocity(newVel);
        m_CharacterController.TryAligningWithGround();
    }

    //Query whether this module can be active, given the current state of the character controller (velocity, isGrounded etc.)
    //Called every frame when inactive (to see if it could be) and when active (to see if it should not be)
    public override bool IsApplicable(){
        if (m_ControlledCollider.IsGrounded() &&
            ((DoesInputExist("Crouch") && GetButtonInput("Crouch").m_IsPressed) || GetDirInput("Move").m_Direction == DirectionInput.Direction.Down))
        {
            if (m_IsActive)
            {
                if (m_ControlledCollider.GetVelocity().magnitude >= m_LowestPossibleSlideSpeed)
                {
                    return true;
                }
            }
            else
            {
                if (m_ControlledCollider.GetVelocity().magnitude >= m_SpeedNeededToStart)
                {
                    return true;
                }
            }
        }
        return false;
    }
    //Query whether this module can be deactivated without bad results (clipping etc.)
    public override bool CanEnd(){
        if (m_ControlledCollider != null)
        {
            if (m_ControlledCollider.CanBeResized(m_ControlledCollider.GetDefaultLength(), CapsuleResizeMethod.FromBottom))
            {
                return true;
            }            
            else
            {
                if (m_CharacterController.GetAbilityModuleManager().GetModuleWithName("Crouch") != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }
}
