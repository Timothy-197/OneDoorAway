using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//--------------------------------------------------------------------
//Jetpack module is a movement ability
//Boost character in air while holding button
//--------------------------------------------------------------------
public class JetpackModule : GroundedControllerAbilityModule
{
    [SerializeField] float m_JetpackForce = 0.0f;
    [SerializeField] float m_MaxUpwardsVelocity = 0.0f;
    [SerializeField] float m_MinUpwardsVelocity = 0.0f;
    [SerializeField] bool m_UnlimitedFuel = false;
    [SerializeField] float m_MaxFuel = 0.0f;
    [SerializeField] float m_FuelConsumptionRate = 0.0f;
    [SerializeField] float m_FuelRegenerationRate = 0.0f;
    [SerializeField] bool m_AlwaysRefuel = false;
    [SerializeField] bool m_RefuelWhileTouchingGround = false;
    [SerializeField] bool m_RefuelWhileTouchingWall = false;
    [SerializeField] bool m_RefuelWhileTouchingEdge = false;
    [SerializeField] bool m_AllowStartWhileMovingUpwards = false;

    float m_Fuel;

    //Reset all state when this module gets initialized
    protected override void ResetState()
    {
        base.ResetState();
        m_Fuel = m_MaxFuel;
    }

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl()
    {
        m_CharacterController.StopJumpCut();
    }

    //Execute jump (lasts one update)
    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule()
    {
        if (!m_UnlimitedFuel)
        {
            m_Fuel = Mathf.Clamp(m_Fuel - m_FuelConsumptionRate * Time.fixedDeltaTime, 0.0f, m_MaxFuel);
        }

        Vector2 currentVel = m_ControlledCollider.GetVelocity();
        if (currentVel.y < m_MinUpwardsVelocity)
        {
            currentVel.y = m_MinUpwardsVelocity;
        }
        float upwardForce = m_JetpackForce * Time.fixedDeltaTime;
        if (currentVel.y + upwardForce >= m_MaxUpwardsVelocity && currentVel.y < m_MaxUpwardsVelocity)
        {
            upwardForce = m_MaxUpwardsVelocity - currentVel.y;
        }
        else if (currentVel.y >= m_MaxUpwardsVelocity)
        {
            upwardForce = 0.0f;
        }
        Vector2 fJetpack = Vector2.up * upwardForce;

        Vector2 fHorizontal = m_CharacterController.GetDirectedInputMovement() * m_CharacterController.GetInputForce();
        fHorizontal = m_CharacterController.ClampInputVelocity(fHorizontal, m_ControlledCollider.GetVelocity(), m_CharacterController.GetInputForceApplyLimit());

        Vector2 fGravity = m_CharacterController.GetGravity();

        Vector2 fDrag = -0.5f * (currentVel.sqrMagnitude) * m_CharacterController.GetDragConstant() * currentVel.normalized;
        Vector2 summedF = fHorizontal + fGravity + fDrag;
        Vector2 newVelocity = currentVel + fJetpack + summedF * Time.fixedDeltaTime;
        m_ControlledCollider.UpdateWithVelocity(newVelocity);
    }

    //Called whenever this module is inactive and updating (implementation by child modules), useful for cooldown updating etc.
    public override void InactiveUpdateModule()
    {
        if (m_AlwaysRefuel ||
           (m_ControlledCollider.IsGrounded() && m_RefuelWhileTouchingGround) ||
           (m_ControlledCollider.IsPartiallyTouchingWall() && m_RefuelWhileTouchingWall) ||
           (m_ControlledCollider.IsTouchingEdge() && m_RefuelWhileTouchingEdge))
        {
            m_Fuel = Mathf.Clamp(m_Fuel + m_FuelRegenerationRate * Time.fixedDeltaTime, 0.0f, m_MaxFuel);
        }
    }

    //Query whether this module can be active, given the current state of the character controller (velocity, isGrounded etc.)
    //Called every frame when inactive (to see if it could be) and when active (to see if it should not be)
    public override bool IsApplicable()
    {
        if (!m_UnlimitedFuel && m_Fuel <= 0)
        {
            return false;
        }

        //Prevent overriding the default jump
        if (m_ControlledCollider.IsGrounded())
        {
            return false;
        }
        if (!m_IsActive)
        {
            if (!m_AllowStartWhileMovingUpwards && m_ControlledCollider.GetVelocity().y > 0.0f)
            {
                return false;
            }
        }

        if ((m_CharacterController.GetJumpInput().m_IsPressed))
        {
            return true;
        }
        return false;
    }

    public float GetFuelAs01Factor()
    {
        if (m_UnlimitedFuel)
        {
            return 1.0f;
        }
        else
        {
            return Mathf.Clamp01(m_Fuel / m_MaxFuel);
        }
    }
}
