using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Small class to rotate an object with a certain velocity. Used for moving platforms in levels.
//This child class adds reset capabilities when a respawn happens
//--------------------------------------------------------------------
public class RunnerDemoRotator : Rotator
{
    Quaternion m_StartRotation;

    void OnEnable()
    {
        m_StartRotation = transform.rotation;
        InSceneLevelSwitcher.OnLevelStart += ResetStart;
    }


    void OnDisable()
    {
        InSceneLevelSwitcher.OnLevelStart -= ResetStart;
    }

    void ResetStart()
    {
        transform.rotation = m_StartRotation;
    }
}
