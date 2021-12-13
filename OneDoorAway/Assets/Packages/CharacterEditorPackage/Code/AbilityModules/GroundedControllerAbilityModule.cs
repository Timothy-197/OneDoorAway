using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Base class for ability modules.
//This subclass is specifically for modules working with GroundedCharacterControllers
//Also stores ControlledCapsuleCollider instead of just ControlledCollider
//--------------------------------------------------------------------
public class GroundedControllerAbilityModule : AbilityModule {

    protected GroundedCharacterController m_CharacterController;
    protected ControlledCapsuleCollider m_ControlledCollider;
    public override void InitModule(CharacterControllerBase a_CharacterController)
    {
        base.InitModule(a_CharacterController);
        m_CharacterController = a_CharacterController as GroundedCharacterController;
        m_ControlledCollider = a_CharacterController.GetCollider() as ControlledCapsuleCollider;
    }
}
