using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Base class for all entity controllers.
//Uses a ControlledCollider for movement, and an AbilityModuleManager for movement modules
//Implemented further in GroundedCharacterController and HoverCharacterController
//--------------------------------------------------------------------
public abstract class CharacterControllerBase : MonoBehaviour
{
    protected DirectionInput m_MovementInput;
    protected PlayerInput m_PlayerInput;
    protected bool m_MovementIsLocked;

    [SerializeField] protected ControlledCollider m_ControlledCollider;
	[SerializeField] protected AbilityModuleManager m_AbilityManager;

    void Awake()
    {
        if (m_ControlledCollider == null)
        {
            Debug.LogError("Controlled collider not set up");
            return;
        }
        if (m_AbilityManager != null)
        {
            m_AbilityManager.InitAllModules(this);
        }
    }

    void FixedUpdate ()
    { 
        if (m_ControlledCollider == null)
        {
            return;
        }
        if (m_MovementIsLocked)
        {
            return;
        }
        m_ControlledCollider.UpdateContextInfo();
        UpdateController();
        if (m_AbilityManager != null)
        {
            m_AbilityManager.UpdateBestApplicableModule();
            m_AbilityManager.UpdateInactiveModules();
            if (m_AbilityManager.IsAbilityModuleRunning())
            {
                m_AbilityManager.FixedUpdateCurrentModule();
            }
            else
            {
                DefaultUpdateMovement();
            }
            m_AbilityManager.PostFixedUpdateModuleSelection();
        }
        else
        { 
            DefaultUpdateMovement();
        }
    }

    protected virtual void UpdateController()
    {
    }

    protected virtual void DefaultUpdateMovement ()
    {
	}

    public ControlledCollider GetCollider()
    {
        return m_ControlledCollider;
    }
    public AbilityModuleManager GetAbilityModuleManager()
    {
        return m_AbilityManager;
    }

    public void SetPosition(Vector3 a_Position)
    {
        if (m_ControlledCollider != null)
        {
            m_ControlledCollider.SetPosition(a_Position);
            m_ControlledCollider.UpdateContextInfo();
        }
    }

    public void SetRotation(Quaternion a_Rotation)
    {
        if (m_ControlledCollider != null)
        {
            m_ControlledCollider.SetRotation(a_Rotation);
            m_ControlledCollider.UpdateContextInfo();
        }
    }

    //Lock player in place
    public void LockMovement(bool a_Lock)
    {
        m_MovementIsLocked = a_Lock;
    }
    public bool IsMovementLocked()
    {
        return m_MovementIsLocked;
    }

    public void SpawnAndResetAtPosition(Vector3 a_Position)
    {
        if (m_AbilityManager != null)
        {
            m_AbilityManager.ForceExitModules();
        }
        if (m_ControlledCollider != null)
        {
            m_ControlledCollider.SetVelocity(Vector2.zero);
            m_ControlledCollider.SetPosition(a_Position);
            m_ControlledCollider.UpdateContextInfo();
            m_ControlledCollider.ClearColPoints();
        }
    }

    //Set inputs (by PlayerInput)
    public virtual void SetPlayerInput(PlayerInput a_PlayerInput)
    {
        m_PlayerInput = a_PlayerInput;
        if (a_PlayerInput.GetDirectionInput("Move") != null)
        {
            m_MovementInput = a_PlayerInput.GetDirectionInput("Move");
        }
        else
        {
            Debug.LogError("Move input not set up in character input");
        }
        
    }

    //Get inputs from player
    public PlayerInput GetPlayerInput()
    {
        return m_PlayerInput;
    }
    public Vector2 GetInputMovement()
    {
        if (m_MovementInput != null)
        {
            return m_MovementInput.m_ClampedInput;
        }
        else
        {
            return Vector2.zero;
        }
    }

    public string GetCurrentSpriteState()
    {
        if (m_AbilityManager != null)
        {
            if (m_AbilityManager.GetCurrentModule() != null)
            {
                return m_AbilityManager.GetCurrentModule().GetSpriteState();
            }
        }
        return GetCurrentSpriteStateForDefault();
    }
	//Used for ability modules to specify an "up" direction for whatever state they might be in
	public Vector2 GetCurrentVisualUp()
	{
		if (m_AbilityManager != null)
		{
			if (m_AbilityManager.GetCurrentModule() != null)
			{
				return m_AbilityManager.GetCurrentModule().GetCurrentVisualUp();
			}
		}
		return Vector2.up;
	}

    protected virtual string GetCurrentSpriteStateForDefault()
    {
        return "";
    }
}
