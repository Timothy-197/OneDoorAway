using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Class used on triggers which signify the end of a level. The InSceneLevelSwitcher will move the player to the next level
//--------------------------------------------------------------------
public class LevelTransition : MonoBehaviour {

    [SerializeField] int m_Index = 0;

    void OnTriggerEnter()
    {
        if (InSceneLevelSwitcher.Get())
        {
            InSceneLevelSwitcher.Get().SetIndex(m_Index);
            InSceneLevelSwitcher.Get().Respawn();
        }
    }
}
