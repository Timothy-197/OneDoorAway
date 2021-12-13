using UnityEngine;
using System.Collections;

public class AbilityUnlockTrigger : MonoBehaviour {
    [SerializeField] AbilityUnlockList m_List = null;
    [SerializeField] bool m_DisableAfterTriggering = false;
    void OnTriggerEnter(Collider a_Collider)
    {
        AbilityModuleManager abilityManager = a_Collider.GetComponent<AbilityModuleManager>();
        if (abilityManager)
        {
            abilityManager.ApplyAbilityUnlockList(m_List);
            if (m_DisableAfterTriggering)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
