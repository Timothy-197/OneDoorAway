using UnityEngine;
using System.Collections;
[System.Serializable]
public class InputElement
{
    public enum InputType
    {
        Button,
        Direction
    }
    public string m_Name;
    public InputType m_InputType;
    public ButtonInput.UnityInputType m_UnityInputType;
    public ButtonInput.UnityAxisRecognition m_UnityAxisRecognition;
    public string m_ButtonName;
    public string m_HorizontalAxisName;
    public string m_VerticalAxisName;
    public float m_DirectionThreshold;
    [HideInInspector]
    public bool m_IsFoldingOut;

    ButtonInput m_ButtonInput;
    DirectionInput m_DirectionInput;

    public ButtonInput GetButtonInput()
    {
        if (m_ButtonInput == null)
        {
            m_ButtonInput = new ButtonInput(m_UnityInputType, m_UnityAxisRecognition, m_ButtonName);
        }
        return m_ButtonInput;
    }

    public DirectionInput GetDirectionInput()
    {
        if (m_DirectionInput == null)
        {
            m_DirectionInput = new DirectionInput(m_HorizontalAxisName, m_VerticalAxisName, m_DirectionThreshold);
        }
        return m_DirectionInput;
    }

    public void Update()
    {
        switch (m_InputType)
        {
            case InputType.Button:
                GetButtonInput().Update();
            break;
            case InputType.Direction:
                GetDirectionInput().Update();
            break;
        }
    }

    public void FixedUpdate()
    {
        switch (m_InputType)
        {
            case InputType.Button:
                GetButtonInput().FixedUpdate();
            break;
        }
    }
}
