using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Small class to rotate an object back and forth a certain angle. Used for moving platforms in levels
//This child class adds reset capabilities when a respawn happens
//--------------------------------------------------------------------
public class RunnerDemoBalancer: Balancer
{
    [SerializeField] float m_StartTime = 0.0f;

    void OnEnable()
    {
        m_Time = m_StartTime;
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
