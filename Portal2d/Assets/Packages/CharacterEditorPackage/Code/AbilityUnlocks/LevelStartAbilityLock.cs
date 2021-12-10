using UnityEngine;
using System.Collections;

public class LevelStartAbilityLock : MonoBehaviour {
    [SerializeField] AbilityUnlockList m_List = null;
    [SerializeField] CharacterControllerBase m_Character = null;

    void Start () 
	{
        m_Character.GetAbilityModuleManager().ApplyAbilityUnlockList(m_List);
	}
}
