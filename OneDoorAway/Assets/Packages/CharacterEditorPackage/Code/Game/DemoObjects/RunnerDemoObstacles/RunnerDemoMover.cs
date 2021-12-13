using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Small class to move an object back and forth along a path. Used for moving platforms in levels
//This child class adds reset capabilities when a respawn happens
//--------------------------------------------------------------------
public class RunnerDemoMover: Mover
{
    [SerializeField] float m_StartTime = 0.0f;

    void OnEnable()
    {
        m_Time = m_StartTime;
        m_StartPosition = transform.position;
        InSceneLevelSwitcher.OnLevelStart += ResetStart;
    }

    void OnDisable()
    {
        InSceneLevelSwitcher.OnLevelStart -= ResetStart;
    }

    void ResetStart()
    {
        m_Time = m_StartTime;
    }

}
