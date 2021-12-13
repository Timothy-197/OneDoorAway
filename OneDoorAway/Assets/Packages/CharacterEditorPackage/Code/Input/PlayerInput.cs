using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//PlayerInput uses Unity input (keyboard or controller) to register player input and send it to an CharacterControllerBase
//--------------------------------------------------------------------
public class PlayerInput : MonoBehaviour
{
    [SerializeField] CharacterControllerBase m_CharacterController;

    [SerializeField] InputElement[] m_Inputs;

    //Called by Unity upon adding a new component to an object, or when Reset is selected in the context menu. Used here to provide default values.
    //Also used when fixing up components using the CharacterFixEditor button
    void Reset()
    {
        m_CharacterController = transform.GetComponent<CharacterControllerBase>();
        EnsureJumpAndMoveInputsAreSet();
    }

    public void EnsureJumpAndMoveInputsAreSet()
    {
        if (m_Inputs == null)
        {
            m_Inputs = new InputElement[0];
        }
        bool moveFound = false;
        bool jumpFound = false;
        
        for (int i = 0; i < m_Inputs.Length; i ++)
        {
            if (m_Inputs[i].m_Name == "Move")
            {
                moveFound = true;
            }
            if (m_Inputs[i].m_Name == "Jump")
            {
                jumpFound = true;
            }
        }
        if (!moveFound)
        {
            InputElement[] newInputs = new InputElement[m_Inputs.Length + 1];
            for (int i = 0; i < m_Inputs.Length; i ++)
            {
                newInputs[i] = m_Inputs[i];
            }
            newInputs[m_Inputs.Length] = new InputElement();
            newInputs[m_Inputs.Length].m_Name = "Move";
            newInputs[m_Inputs.Length].m_InputType = InputElement.InputType.Direction;
            newInputs[m_Inputs.Length].m_HorizontalAxisName = "Horizontal";
            newInputs[m_Inputs.Length].m_VerticalAxisName = "Vertical";
            newInputs[m_Inputs.Length].m_DirectionThreshold = 0.6f;

            m_Inputs = newInputs;
        }
        if (!jumpFound)
        {
            InputElement[] newInputs = new InputElement[m_Inputs.Length + 1];
            for (int i = 0; i < m_Inputs.Length; i++)
            {
                newInputs[i] = m_Inputs[i];
            }
            newInputs[m_Inputs.Length] = new InputElement();
            newInputs[m_Inputs.Length].m_Name = "Jump";
            newInputs[m_Inputs.Length].m_InputType = InputElement.InputType.Button;
            newInputs[m_Inputs.Length].m_UnityInputType = ButtonInput.UnityInputType.Button;
            newInputs[m_Inputs.Length].m_ButtonName = "Jump";

            m_Inputs = newInputs;
        }
    }

    void Awake()
    {
        //Update entity controller inputs
        if (m_CharacterController != null)
        {
            m_CharacterController.SetPlayerInput(this);
        }
    }

    public void SetCharacterControllerBase(CharacterControllerBase a_CharacterController)
    {
        m_CharacterController = a_CharacterController;
        if (m_CharacterController != null)
        {
            m_CharacterController.SetPlayerInput(this);
        }
    }

    public CharacterControllerBase GetCharacterControllerBase()
    {
        return m_CharacterController;
    }

    public bool DoesInputExist(string a_Name)
    {
        for (int i = 0; i < m_Inputs.Length; i++)
        {
            if (m_Inputs[i].m_Name == a_Name)
            {
                return true;
            }
        }
        return false;
    }

    public ButtonInput GetButton(string a_Name)
    {
        for (int i = 0; i < m_Inputs.Length; i++)
        {
            if (m_Inputs[i].m_Name == a_Name)
            { 
                if (m_Inputs[i].m_InputType == InputElement.InputType.Button)
                {
                    return m_Inputs[i].GetButtonInput();
                }
                else
                {
                    Debug.LogError("Requested input " + a_Name + " is not of Button type but of " +m_Inputs[i].m_InputType.ToString() + " type.");
                    return null;
                }
            }
        }
        Debug.LogError("Requesting a button input that is not defined: " + a_Name);
        return null;
    }
    public DirectionInput GetDirectionInput(string a_Name)
    {
        for (int i = 0; i < m_Inputs.Length; i++)
        {
            if (m_Inputs[i].m_Name == a_Name)
            {
                if (m_Inputs[i].m_InputType == InputElement.InputType.Direction)
                {
                    return m_Inputs[i].GetDirectionInput();
                }
                else
                {
                    Debug.LogError("Requested input " + a_Name + " is not of Direction type but of " + m_Inputs[i].m_InputType.ToString() + " type.");
                    return null;
                }
            }
        }
        Debug.LogError("Requesting a Direction input that is not defined: " + a_Name);
        return null;
    }
	void Update ()
    {    
        for (int i = 0; i < m_Inputs.Length; i ++)
        {
            m_Inputs[i].Update();
        }
	}

    void FixedUpdate()
    {
        for (int i = 0; i < m_Inputs.Length; i++)
        {
            m_Inputs[i].FixedUpdate();
        }
    }
}
