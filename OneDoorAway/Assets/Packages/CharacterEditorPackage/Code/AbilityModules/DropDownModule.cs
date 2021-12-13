using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//DropDown is a movement module related to the OneWayPlatform objects.
//This ability allows players to pass through OneWayPlatform objects they are standing on by pressing crouch/holding down, and pressing jump
//Optionally a jump speed is added.
//--------------------------------------------------------------------
public class DropDownModule : GroundedControllerAbilityModule
{
    [SerializeField] float m_JumpDownVelocity = 0.0f;

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl(){
        if (m_ControlledCollider != null)
        {
            m_ControlledCollider.GetGroundedInfo().GetGroundTransform().GetComponentInChildren<OneWayPlatform>().DisableForObject(m_CharacterController.GetComponent<Collider>());
        }
    }

    //Execute jump (lasts one update)
    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule()
    {
        Vector2 jumpVelocity = Vector3.down * m_JumpDownVelocity;
        m_CharacterController.Jump(jumpVelocity, false, false);

        m_ControlledCollider.UpdateWithVelocity(m_ControlledCollider.GetVelocity());
    }

    //Query whether this module can be active, given the current state of the character controller (velocity, isGrounded etc.)
    //Called every frame when inactive (to see if it could be) and when active (to see if it should not be)
    public override bool IsApplicable(){
        if (m_ControlledCollider.IsGrounded() &&
            m_CharacterController.GetJumpIsCached() &&
            ((DoesInputExist("Crouch") && GetButtonInput("Crouch").m_IsPressed) || GetDirInput("Move").m_Direction == DirectionInput.Direction.Down))
        {
            if (m_ControlledCollider.GetGroundedInfo().GetGroundTransform().GetComponentInChildren<OneWayPlatform>())
            {
                return true;
            }
        }
        return false;
    }
}
