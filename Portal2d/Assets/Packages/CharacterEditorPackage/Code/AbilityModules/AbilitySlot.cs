using UnityEngine;
using System.Collections;

[System.Serializable]
public class AbilitySlot
{
    public AbilityModule m_AbilityModulePrefab;

    [HideInInspector]
    public bool m_IsFoldingOut;
}
