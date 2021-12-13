using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//ButtonInput registers keyboard or controller buttoninput to be used by PlayerInput
//--------------------------------------------------------------------

public class ButtonInput {
    public enum UnityInputType
    {
        Button,
        Axis,
        ButtonAndAxis
    }
    public enum UnityAxisRecognition
    {
        PositiveOrNegative,
        PositiveOnly,
        NegativeOnly
    }
    public bool m_IsPressed;
    public bool m_WasJustPressed;
    public bool m_WasJustReleased;

    UnityInputType m_UnityInputType;
    UnityAxisRecognition m_UnityAxisRecognition;
    string m_ButtonName;

    float m_PreviousAxisVal;
    bool m_IsActuallyPressed;
    bool m_IsActuallyJustPressed;
    bool m_IsActuallyJustReleased;

    //Override
    bool m_IsBeingOverridden;

    public bool GetIsBeingOverridden()
    {
        return m_IsBeingOverridden;
    }
    public void SetOverride(bool a_Override, bool a_Input = false)
    {
        if (a_Override)
        {
            HandleButtonUpdate(a_Input);
        }
        m_IsBeingOverridden = a_Override;
    }

    public ButtonInput(UnityInputType a_InputType, UnityAxisRecognition a_AxisRecognition, string a_ButtonName)
    {
        m_UnityInputType = a_InputType;
        m_UnityAxisRecognition = a_AxisRecognition;
        m_ButtonName = a_ButtonName;    
    }

    public void Update()
    {
        float axisVal = 0;
        if (m_IsBeingOverridden)
        { 
            return;
        }
        //In case of the triggers on a controller, still register them as button presses
        if (m_UnityInputType == UnityInputType.Axis || m_UnityInputType == UnityInputType.ButtonAndAxis)
        {
            axisVal = Input.GetAxisRaw(m_ButtonName);
        }
        switch (m_UnityInputType)
        {
            case UnityInputType.Button:
                if (Input.GetButtonDown(m_ButtonName))
                {
                    HandleButtonUpdate(true);
                }
                if (Input.GetButtonUp(m_ButtonName))
                {
                    HandleButtonUpdate(false);
                }
                break;
            case UnityInputType.Axis:
                if (IsAxisPressed(axisVal) && !IsAxisPressed(m_PreviousAxisVal))
                {
                    HandleButtonUpdate(true);
                }
                if (!IsAxisPressed(axisVal) && IsAxisPressed(m_PreviousAxisVal))
                {
                    HandleButtonUpdate(false);
                }
                m_PreviousAxisVal = axisVal;
                break;
            case UnityInputType.ButtonAndAxis:
                if (Input.GetButtonDown(m_ButtonName))
                {
                    HandleButtonUpdate(true);
                }
                if (Input.GetButtonUp(m_ButtonName))
                {
                    HandleButtonUpdate(false);
                }
                if (IsAxisPressed(axisVal) && !IsAxisPressed(m_PreviousAxisVal))
                {
                    HandleButtonUpdate(true);
                }
                if (!IsAxisPressed(axisVal) && IsAxisPressed(m_PreviousAxisVal))
                {
                    HandleButtonUpdate(false);
                }
                m_PreviousAxisVal = axisVal;
                break;
        }
    }

    bool IsAxisPressed(float a_Value)
    {
        switch (m_UnityAxisRecognition)
        {
            case UnityAxisRecognition.PositiveOrNegative:
                return (a_Value != 0.0f);
            case UnityAxisRecognition.PositiveOnly:
                return (a_Value > 0.0f);
            case UnityAxisRecognition.NegativeOnly:
                return (a_Value < 0.0f);
        }
        return false;
    }
	
    public void FixedUpdate()
    {
        m_WasJustPressed = false;
        m_WasJustReleased = false;
        if (m_IsActuallyJustPressed)
        {
            m_WasJustPressed = true;
            m_IsActuallyJustPressed = false;
            m_IsPressed = true;
        }
        else if (m_IsActuallyJustReleased)//Stagger just pressed and released if they happened within a single fixed update
        {
            m_WasJustReleased = true;
            m_IsActuallyJustReleased = false;
            m_IsPressed = false;
        }
        else
        {
            m_IsPressed = m_IsActuallyPressed;
        }
    }

    void HandleButtonUpdate(bool a_Pressed)
    {
        if (a_Pressed && !m_IsActuallyPressed)
        {
            m_IsActuallyJustPressed = true;
        }
        else if (!a_Pressed && m_IsActuallyPressed)
        {
            m_IsActuallyJustReleased = true;
        }
        m_IsActuallyPressed = a_Pressed;
    }
}
