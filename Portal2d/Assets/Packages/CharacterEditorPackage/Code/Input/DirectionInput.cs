using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Gets arrow key or analogue stick input from Unity's input system. Used by PlayerInput
//--------------------------------------------------------------------
public class DirectionInput
{
    public enum Direction
    {
        Neutral,
        Up,
        Down,
        Left,
        Right
    }
    public Vector2 m_RawInput;
    public Vector2 m_ClampedInput;
    public Direction m_Direction;

    string m_HorizontalName;
    string m_VerticalName;
    float m_DirectionThreshold;

    //Override
    bool m_IsBeingOverridden;
    Vector2 m_OverriddenInput;

    public bool GetIsBeingOverridden()
    {
        return m_IsBeingOverridden;
    }
    public void SetOverride(bool a_Override, Vector2 a_OverriddenInput = default(Vector2))
    {
        if (a_Override)
        {
            m_OverriddenInput = a_OverriddenInput;
        }
        m_IsBeingOverridden = a_Override;
    }

    public DirectionInput(string a_HorizontalName, string a_VerticalName, float a_DirectionThreshold)
    {
        m_HorizontalName = a_HorizontalName;
        m_VerticalName = a_VerticalName;
        m_DirectionThreshold = a_DirectionThreshold;
    }

    public bool IsInThisDirection(Vector2 a_Direction)
    {
        float dot = Vector2.Dot(a_Direction, m_ClampedInput);
        if (dot >= m_DirectionThreshold)
        {
            return true;
        }
        return false;
    }

    public bool HasSurpassedThreshold()
    {
        return (m_ClampedInput.magnitude >= m_DirectionThreshold);
    }

	public void Update ()
    {
        if (m_IsBeingOverridden)
        {
            m_RawInput = m_OverriddenInput.normalized;
        }
        else
        { 
            m_RawInput.x = Input.GetAxisRaw(m_HorizontalName);
            m_RawInput.y = Input.GetAxisRaw(m_VerticalName);
        }
        m_ClampedInput = (m_RawInput.magnitude > 1) ? m_RawInput.normalized : m_RawInput;

        if (Mathf.Abs(m_ClampedInput.x) > m_DirectionThreshold || Mathf.Abs(m_ClampedInput.y) > m_DirectionThreshold)
        {
            if (Mathf.Abs(m_ClampedInput.x) > Mathf.Abs(m_ClampedInput.y))
            {
                if (m_ClampedInput.x > 0)
                {
                    m_Direction = Direction.Right;
                }
                else
                {
                    m_Direction = Direction.Left;
                }
            }
            else
            {
                if (m_ClampedInput.y > 0)
                {
                    m_Direction = Direction.Up;
                }
                else
                {
                    m_Direction = Direction.Down;
                }
            }
        }
        else
        {
            m_Direction = Direction.Neutral;
        }
	}
}
