using UnityEngine;
using System.Collections;

//--------------------------------------------------------------------
//Base class for all ability modules. Stores information about name, priority, and being active. Provides the handles to deal with ability modules.
//--------------------------------------------------------------------
[System.Serializable]
public class AbilityModule : MonoBehaviour {
    [SerializeField] int m_Priority = 0;
    [SerializeField] string m_ModuleName = "";
    protected bool m_IsActive;
    protected bool m_IsLocked;
    protected CharacterControllerBase m_CharacterControllerBase;
    protected ControlledCollider m_ControlledColliderBase;
    //Called once when instantiating the ability module
    public virtual void InitModule(CharacterControllerBase a_CharacterControllerBase)
    {
        if (a_CharacterControllerBase == null)
        {
            Debug.LogError("Character controller not found for " + GetName() + "!");
            return;
        }
        m_CharacterControllerBase = a_CharacterControllerBase;
        m_ControlledColliderBase = a_CharacterControllerBase.GetCollider();
        ResetState();
    }
    //Called from within modules to see if the input they require is set up
    protected bool DoesInputExist(string a_Name)
    {
        return m_CharacterControllerBase.GetPlayerInput().DoesInputExist(a_Name);
    }
    //Called from within modules to get a directional input by name
    protected DirectionInput GetDirInput(string a_Name)
    {
        if (m_CharacterControllerBase == null)
        {
            Debug.LogError("Character controller not found for " + GetName() + "!");
            return null;
        }
        if (m_CharacterControllerBase.GetPlayerInput() == null)
        {
            Debug.LogError("Player input not found for " + GetName() + "!");
            return null;
        }
        return m_CharacterControllerBase.GetPlayerInput().GetDirectionInput(a_Name);
    }
    //Called from within modules to get a button input by name
    protected ButtonInput GetButtonInput(string a_Name)
    {
        if (m_CharacterControllerBase == null)
        {
            Debug.LogError("Character controller not found for " + GetName() + "!");
            return null;
        }
        if (m_CharacterControllerBase.GetPlayerInput() == null)
        {
            Debug.LogError("Player input not found for " + GetName() + "!");
            return null;
        }
        return m_CharacterControllerBase.GetPlayerInput().GetButton(a_Name);
    }
    //Check if this ability can even start
    public bool IsLocked()
    {
        return m_IsLocked;
    }
    //Lock/unlock this ability
    public void SetLocked(bool a_Locked)
    {
        m_IsLocked = a_Locked;
        if (!a_Locked)
        {
            ResetState();
        }
    }
    //Reset when initialized
    protected virtual void ResetState()
    {
        m_IsActive = false;
    }
    //Called whenever the module is started
    public void StartModule()
    {
        m_IsActive = true;
        StartModuleImpl();
    }
    //Called whenever the module is ended
    public void EndModule()
    {
        m_IsActive = false;
        EndModuleImpl();
    }
    //StartModule for implementation by child modules
    protected virtual void StartModuleImpl()
    {
    }
    //Endodule for implementation by child modules
    protected virtual void EndModuleImpl()
    {
    }
    //Called whenever the module is active and updating (implementation by child modules)
    public virtual void FixedUpdateModule()
    {
    }
    //Called whenever the module is inactive and updating (implementation by child modules)
    public virtual void InactiveUpdateModule()
    {
    }
    //Called to place a MovingColPoint to support moving colliders (implementation by child modules)
    public virtual void PlaceMovingColPoint()
    {
    }
    //Query whether this module can be activated, given the current state of the character controller (velocity, isgrounded etc.)
    public virtual bool IsApplicable()
    {
        return false;
    }
    //Query whether this module can be ended without bad results (clipping etc.)
    public virtual bool CanEnd()
    {
        return true;
    }
    
    public bool GetIsActive()
    {
        return m_IsActive;
    }

    public int GetPriority()
    {
        return m_Priority;
    }

    public string GetName()
    {
        return m_ModuleName;
    }

    //Get the name of the animation state that should be playing for this module. Default is the current module name, but children can override.
    public virtual string GetSpriteState()
    {
        return m_ModuleName;
    }
	public virtual Vector2 GetCurrentVisualUp()
	{
		return Vector2.up;
	}
}
