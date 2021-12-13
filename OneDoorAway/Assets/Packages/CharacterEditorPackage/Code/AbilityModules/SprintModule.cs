using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Sprint module is a movement ability
//If the Sprint input is pressed, and the character is on the ground, speed up character motion
//--------------------------------------------------------------------
public class SprintModule : GroundedControllerAbilityModule
{
    [SerializeField] float m_SprintAcceleration = 0.0f;
    [SerializeField] float m_MaxSprintSpeed = 0.0f;
    [SerializeField] float m_SprintBrakeDeceleration = 0.0f;
    [SerializeField] float m_SprintFriction = 0.0f;

    //Walk across the floor, but with different values for speed and friction
    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule(){
        if (m_CharacterController.TryDefaultJump())
        {
            m_ControlledCollider.UpdateWithVelocity(m_ControlledCollider.GetVelocity());
            return;
        }    
        Vector2 currentVel = m_ControlledCollider.GetVelocity();

        Vector2 fInput = m_CharacterController.GetDirectedInputMovement() * m_SprintAcceleration;
        fInput = m_CharacterController.ClampInputVelocity(fInput, currentVel, m_MaxSprintSpeed);
        Vector2 fGravity = m_CharacterController.GetGravity();
        Vector2 fDrag = -0.5f * (currentVel.sqrMagnitude) * m_CharacterController.GetDragConstant() * currentVel.normalized;

        Vector2 summedF = fInput + fGravity + fDrag;

        Vector2 newVel = currentVel + summedF * Time.fixedDeltaTime;

        newVel += m_CharacterController.GetStoppingForce(newVel, m_SprintBrakeDeceleration);
        Vector2 friction = m_CharacterController.GetFriction(newVel, summedF, m_SprintFriction);
        newVel += friction;

        m_ControlledCollider.UpdateWithVelocity(newVel);
        m_CharacterController.TryAligningWithGround();
    }
    //Character needs to be on the floor and pressing the Sprint button
    //Query whether this module can be active, given the current state of the character controller (velocity, isGrounded etc.)
    //Called every frame when inactive (to see if it could be) and when active (to see if it should not be)
    public override bool IsApplicable(){
        if (!DoesInputExist("Sprint"))
        {
            Debug.LogError("Input for module " + GetName() + " not set up");
            return false;
        }
        if (GetButtonInput("Sprint").m_IsPressed && m_ControlledCollider.IsGrounded())
        {
            return true;
        }
        return false;
    }

    //Get the name of the animation state that should be playing for this module. 
    public override string GetSpriteState(){
        if (m_CharacterController.GetDirectedInputMovement().magnitude != 0)
        {
            if (m_ControlledCollider.GetVelocity().magnitude >= 5.0f && Vector3.Dot(m_CharacterController.GetDirectedInputMovement(), m_ControlledCollider.GetVelocity()) < -0.1f)
            {
                return "BrakeRun";
            }
            else
            {
                return "Sprint";
            }
        }
        else
        {
            if (m_ControlledCollider.GetVelocity().magnitude >= 5.0f)
            {
                return "BrakeRun";
            }
            return "Idle";
        }
    }
}
