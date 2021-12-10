using UnityEngine;
using System.Collections;

public class PlaneLaunchTrigger : MonoBehaviour {
    [SerializeField] Transform m_Goal = null;
    [SerializeField] float m_Duration = 0.0f;
    [SerializeField] float m_LaunchHeight = 0.0f;
    [SerializeField] bool m_ResetVelocityAfterLaunch = false;

    CharacterControllerBase m_Character;
    float m_StartTime;
    bool m_IsLaunching;
    Vector3 m_CharacterStartPosition;

    void Start()
    {
        m_Character = FindObjectOfType<CharacterControllerBase>();
    }

    void OnTriggerEnter(Collider a_Collider)
    {
        if (a_Collider.gameObject == m_Character.gameObject)
        {
            StartLaunch();
        }
    }

    void Update () 
	{
	    if (m_IsLaunching)
        {
            if (Time.time - m_StartTime < m_Duration)
            {
                float factor = (Time.time - m_StartTime) / m_Duration;
                Vector3 heightOffset = Mathf.Sin(factor * Mathf.PI) * m_LaunchHeight * Vector3.up;

                Vector3 position = Vector3.Lerp(m_CharacterStartPosition, m_Goal.position, factor) + heightOffset;
                m_Character.SetPosition(position);
                m_Character.LockMovement(true);
            }
            else
            {
                EndLaunch();
            }
        }
	}

    void StartLaunch()
    {
        if (m_Goal == null)
        {
            Debug.Log("Goal of PlaneLaunchTrigger is not assigned. Can't launch");
            return;
        }
        m_IsLaunching = true;
        m_StartTime = Time.time;
        //Lock motion while in flight
        m_Character.LockMovement(true);
        m_CharacterStartPosition = m_Character.transform.position;
    }
    void EndLaunch()
    {
        m_IsLaunching = false;
        //Unlock motion now that we're done
        m_Character.LockMovement(false);
        if (m_ResetVelocityAfterLaunch)
        {
            m_Character.GetCollider().SetVelocity(Vector2.zero);
        }
    }

    void OnDrawGizmos()
    {
        if (m_Goal == null)
        {
            return;
        }
        Gizmos.color = Color.green;

        Vector3 halfWayPoint = transform.position + (m_Goal.position - transform.position) * 0.5f + Vector3.up * m_LaunchHeight;
        Gizmos.DrawLine(transform.position, halfWayPoint);
        Gizmos.DrawLine(halfWayPoint, m_Goal.position);
    }
}
