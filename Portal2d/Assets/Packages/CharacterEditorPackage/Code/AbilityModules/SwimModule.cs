using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Swim is a movement module related to the Water trigger objects.
//This ability allows characters to swim in all directions, as long as they are within water
//--------------------------------------------------------------------
public class SwimModule : GroundedControllerAbilityModule
{
    [SerializeField] LayerMask m_WaterLayer = 0;
    [SerializeField] float m_SwimRadius = 0.0f;
    [SerializeField] float m_Gravity = 0.0f;
    [SerializeField] float m_Acceleration = 0.0f;
    [SerializeField] float m_MaxSpeed = 0.0f;
    [SerializeField] float m_Deceleration = 0.0f;
    [SerializeField] float m_Friction = 0.0f;
    [SerializeField] float m_MinExitVel = 0.0f;

    //Called whenever this module is ended (was active, now is inactive)
    protected override void EndModuleImpl()
    {
        Vector2 currentVel = m_ControlledCollider.GetVelocity();
        float dot = Vector2.Dot(currentVel, m_CharacterController.GetInputMovement());
        if (currentVel.magnitude < m_MinExitVel && dot > 0.0f)
        {
            currentVel = currentVel.normalized * m_MinExitVel;
        }
        m_ControlledCollider.SetVelocity(currentVel);
    }
    //Swim around in the water
    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule()
    {
        if (m_CharacterController.TryDefaultJump())
        {
            m_ControlledCollider.UpdateWithVelocity(m_ControlledCollider.GetVelocity());
            return;
        }
        Vector2 currentVel = m_ControlledCollider.GetVelocity();
        Vector2 fInput = m_CharacterController.GetInputMovement() * m_Acceleration;
        fInput = m_CharacterController.ClampInputVelocity(fInput, currentVel, m_MaxSpeed);
        Vector2 fGravity = Vector2.down * m_Gravity;
        Vector2 fDrag = -0.5f * (currentVel.sqrMagnitude) * m_CharacterController.GetDragConstant() * currentVel.normalized;

        Vector2 summedF = fInput + fGravity + fDrag;

        Vector2 newVel = currentVel + summedF * Time.fixedDeltaTime;

        if (newVel.magnitude >= 0.0f)
        {
            Vector2 direction = -newVel.normalized; //Opposite direction of velocity
            Vector2 maxFrictionSpeedChange = direction * m_Friction * Time.fixedDeltaTime;

            Vector2 velInDirection = Mathf.Abs(Vector2.Dot(newVel, direction)) * direction;
            if (velInDirection.magnitude > maxFrictionSpeedChange.magnitude)
            {
                newVel+= maxFrictionSpeedChange;
            }
            else
            {
                newVel += velInDirection;
            }
        }

        Vector2 inputDirection = m_CharacterController.GetInputMovement();
        float dot = Vector2.Dot(inputDirection, newVel);
        if (dot <= 0)
        {
            Vector2 direction = -newVel.normalized; //Opposite direction of velocity

            Vector2 maxForceSpeedChange = direction * m_Deceleration * Time.fixedDeltaTime;

            Vector2 velInDirection = Mathf.Abs(Vector2.Dot(newVel, direction)) * direction;

            if (velInDirection.magnitude > maxForceSpeedChange.magnitude)
            {
                newVel += maxForceSpeedChange;
            }
            else
            {
                newVel += velInDirection;
            }
        }
        m_ControlledCollider.UpdateWithVelocity(newVel);
        m_ControlledCollider.ClearColPoints();
    }
    //Character needs to be touching something in the Water layer within a certain distance to be considered swimming.
    public override bool IsApplicable()
    {
        Collider[] results = Physics.OverlapSphere(m_CharacterController.transform.position, m_SwimRadius, m_WaterLayer, QueryTriggerInteraction.Collide);

        if (results.Length > 0)
        {
            return true;
        }
        return false;
    }

    //Get the name of the animation state that should be playing for this module. 
    public override string GetSpriteState()
    {
        if (m_ControlledCollider.IsGrounded())
        {
            if (Mathf.Abs(m_CharacterController.GetDirectedInputMovement().x) >= 0.05f)
            {
                return "Run";
            }
            else
            {
                return "Idle";
            }
        }
        else
        {
            if (m_CharacterController.GetInputMovement().magnitude >= 0.05f)
            {
                return "Swim";
            }
            else
            {
                return "SwimIdle";
            }
        }
    }
}
