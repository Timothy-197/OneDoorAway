using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Alternative CharacterControllerBase derived class. Uses a more simple model than GroundedCharacterController to move a character in the air.
//Mainly used for debugging
//--------------------------------------------------------------------
public class HoverCharacterController : CharacterControllerBase
{
    [SerializeField] float m_InputModifier = 0.0f;
    [SerializeField] float m_DragConstant = 0.0f;
    [SerializeField] float m_StoppingPower = 0.0f;

    protected override void DefaultUpdateMovement()
    {
        Vector2 previousVel = m_ControlledCollider.GetVelocity();

        Vector2 acceleration = m_MovementInput.m_ClampedInput * m_InputModifier;

        Vector2 drag = -0.5f * (previousVel.sqrMagnitude) * m_DragConstant * previousVel.normalized;

        Vector2 newVel = previousVel + (acceleration + drag) * Time.fixedDeltaTime;
        
        if (Vector2.Dot(m_MovementInput.m_ClampedInput.normalized, newVel) <= 0.0f)
        {

            Vector2 stop = -newVel.normalized * m_StoppingPower * Time.fixedDeltaTime;
            if (stop.magnitude >= newVel.magnitude)
            {
                stop = -newVel;
            }
            newVel += stop;
        }

        m_ControlledCollider.UpdateWithVelocity(newVel);
    }
}
