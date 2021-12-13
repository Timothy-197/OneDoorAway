using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Class which keeps all of the abilitymodules for a character controller
//The CharacterControllerBase class controls this and asks if there are any applicable modules.
//Starting, running and ending AbilityModules is done through here, but the current abilitymodule can be requested to get information from.
//--------------------------------------------------------------------
public class AbilityModuleManager : MonoBehaviour {
    [SerializeField] AbilitySlot[] m_AbilitySlots = null;
    Transform m_ModuleParent;
    AbilityModule[] m_AbilityModules;
    AbilityModule m_CurrentlyUsedModule;
    CharacterControllerBase m_CharacterController;

    public bool IsAbilityModuleRunning()
    {
        return (m_CurrentlyUsedModule != null);
    }

    public AbilityModule GetCurrentModule()
    {
        return m_CurrentlyUsedModule;
    }

    public AbilityModule GetModuleWithName(string a_Name)
    {
        if (m_AbilityModules == null)
        {
            return null;
        }
        for (int i = 0; i < m_AbilityModules.Length; i ++)
        {
            if (m_AbilityModules[i].GetName() == a_Name)
            {
                return m_AbilityModules[i];
            }
        }
        return null;
    }

    public void ApplyAbilityUnlockList(AbilityUnlockList a_List)
    {
        for (int i = 0; i < a_List.m_List.Length; i ++)
        {
            AbilityModule module = GetModuleWithName(a_List.m_List[i].m_AbilityName);
            if (module != null)
            {
                module.SetLocked(a_List.m_List[i].m_Lock);
            }
            else
            {
                Debug.Log("Trying to unlock module with name " + a_List.m_List[i].m_AbilityName + " but this ability cannot be found on the character");
            }
        }
    }
    
    public void InitAllModules(CharacterControllerBase a_CharacterController)
    {
        m_ModuleParent = (new GameObject()).transform;
        m_ModuleParent.name = "Modules";
        m_ModuleParent.parent = transform;
        m_CharacterController = a_CharacterController;
        int actualAbilityModules = 0;
        for (int i = 0; i < m_AbilitySlots.Length; i ++)
        {
            if (m_AbilitySlots[i].m_AbilityModulePrefab != null)
            {
                actualAbilityModules++;
            }
        }
        m_AbilityModules = new AbilityModule[actualAbilityModules];
        int counter = 0;
        for (int i = 0; i < m_AbilitySlots.Length; i++)
        {
            if (m_AbilitySlots[i].m_AbilityModulePrefab != null)
            {
                m_AbilityModules[counter] = Instantiate(m_AbilitySlots[i].m_AbilityModulePrefab);
                m_AbilityModules[counter].InitModule(a_CharacterController);
                m_AbilityModules[counter].transform.parent = m_ModuleParent;
                counter++;
            }
        }
    }

    public void UpdateBestApplicableModule()
    {
        if (m_CurrentlyUsedModule != null && (!m_CurrentlyUsedModule.IsApplicable() || m_CurrentlyUsedModule.IsLocked()))
        {
            if (m_CurrentlyUsedModule.CanEnd())
            {
                m_CurrentlyUsedModule.EndModule();
                m_CurrentlyUsedModule = null;
            }
        }
        AbilityModule bestApplicableModule = GetBestApplicableModule();
        if (m_CurrentlyUsedModule != null && bestApplicableModule != m_CurrentlyUsedModule)
        {
            if (!m_CurrentlyUsedModule.CanEnd())
            {
                return;
            }
        }
        SetNewModule(bestApplicableModule);
    }

    public void UpdateInactiveModules()
    {
        for (int i = 0; i < m_AbilityModules.Length; i ++)
        {
            if (!m_AbilityModules[i].GetIsActive())
            {
                m_AbilityModules[i].InactiveUpdateModule();
            }
        }
    }

    public void FixedUpdateCurrentModule()
    {
        if (m_CurrentlyUsedModule != null)
        { 
            m_CurrentlyUsedModule.FixedUpdateModule();
            m_CurrentlyUsedModule.PlaceMovingColPoint();
        }
    }

    public void PostFixedUpdateModuleSelection()
    {
        AbilityModule preSelectMod = GetCurrentModule();
        UpdateBestApplicableModule();
        AbilityModule postSelectMod = GetCurrentModule();
        if (preSelectMod != postSelectMod && postSelectMod != null)
        {
            m_CharacterController.GetCollider().ClearColPoints();
            postSelectMod.PlaceMovingColPoint();
        }
    }

    public void ForceExitModules()
    {
        if (m_CurrentlyUsedModule != null)
        {
            m_CurrentlyUsedModule.EndModule();
            m_CurrentlyUsedModule = null;
        }
    }

    void SetNewModule(AbilityModule a_Module)
    {
        if (a_Module == m_CurrentlyUsedModule)
        {
            return;
        }

        if (m_CurrentlyUsedModule != null)
        {
            m_CurrentlyUsedModule.EndModule();
        }
        if (a_Module != null)
        {
            a_Module.StartModule();
        }
        m_CurrentlyUsedModule = a_Module;
    }

    AbilityModule GetBestApplicableModule()
    {
        int bestPriority = int.MinValue;
        AbilityModule returnModule = null;

        for (int i = 0; i < m_AbilityModules.Length; i++)
        {
            if (m_AbilityModules[i].GetPriority() > bestPriority)
            {
                if (!m_AbilityModules[i].IsLocked() && m_AbilityModules[i].IsApplicable())
                {
                    returnModule = m_AbilityModules[i];
                    bestPriority = m_AbilityModules[i].GetPriority();
                }
            }
        }

        return returnModule;
    }
}
