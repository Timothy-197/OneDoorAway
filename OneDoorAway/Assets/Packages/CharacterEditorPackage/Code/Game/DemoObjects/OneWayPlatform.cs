using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Script based on solution found by Screenhog on answers.unity3d.com
//http://answers.unity3d.com/answers/362043/view.html
//Use a trigger in combination with Physics.ignorecollision to make the platform intangible.

public class OneWayPlatform : MonoBehaviour {
    [SerializeField] Collider m_PlatformCollider = null;

    List<Collider> m_IgnoredColliders = new List<Collider>();

    void OnTriggerEnter(Collider a_Collider)
    {
        DisableForObject(a_Collider);
    }

    void OnTriggerExit(Collider a_Collider)
    {
        EnableForObject(a_Collider);
    }

    void OnEnable()
    {
        InSceneLevelSwitcher.OnLevelStart += ResetStart;
    }

    void OnDisable()
    {
        InSceneLevelSwitcher.OnLevelStart -= ResetStart;
    }

    void ResetStart()
    {
        m_PlatformCollider.gameObject.layer = LayerMask.NameToLayer("Default");
        for (int i = 0; i < m_IgnoredColliders.Count; i++)
        {
            if (m_IgnoredColliders != null)
            {
                Physics.IgnoreCollision(m_PlatformCollider, m_IgnoredColliders[i], false);
            }
        }
        m_IgnoredColliders.Clear();
    }

    public void DisableForObject(Collider a_Collider)
    {
        if (a_Collider != null && !IsIgnoringCollider(a_Collider))
        {
            m_PlatformCollider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            Physics.IgnoreCollision(m_PlatformCollider, a_Collider, true);
            m_IgnoredColliders.Add(a_Collider);
        }
    }

    public void EnableForObject(Collider a_Collider)
    {
        if (a_Collider != null && IsIgnoringCollider(a_Collider))
        {
            m_PlatformCollider.gameObject.layer = LayerMask.NameToLayer("Default");
            Physics.IgnoreCollision(m_PlatformCollider, a_Collider, false);
            m_IgnoredColliders.Remove(a_Collider);
        }
    }

    bool IsIgnoringCollider(Collider a_Collider)
    {
        if (m_IgnoredColliders != null)
        {
            for (int i = 0; i <m_IgnoredColliders.Count; i ++)
            {
                if (m_IgnoredColliders[i] == a_Collider)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
