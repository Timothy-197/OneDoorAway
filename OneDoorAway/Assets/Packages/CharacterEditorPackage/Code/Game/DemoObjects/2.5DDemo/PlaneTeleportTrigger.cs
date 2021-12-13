using UnityEngine;
using System.Collections;

public class PlaneTeleportTrigger : MonoBehaviour {
    [SerializeField] Transform m_Goal = null;
    [SerializeField] bool m_ResetVelocityAfterTeleport  = false;

    CharacterControllerBase m_Character;

    void Start()
    {
        m_Character = FindObjectOfType<CharacterControllerBase>();
    }

    void OnTriggerEnter(Collider a_Collider)
    {
        TryTeleport(a_Collider.gameObject);
    }
    void OnTriggerStay(Collider a_Collider)
    {
        TryTeleport(a_Collider.gameObject);
    }

    void TryTeleport(GameObject a_Object)
    {
        if (a_Object == m_Character.gameObject)
        {
            if (m_Character.GetPlayerInput().DoesInputExist("TeleportEnter") && m_Character.GetPlayerInput().GetButton("TeleportEnter").m_WasJustPressed)
            {
                Teleport();
            }
        }
    }

    void Teleport()
    {
        if (m_Goal == null)
        {
            Debug.Log("Goal of PlaneTeleportTrigger is not assigned. Can't teleport");
            return;
        }
        m_Character.SetPosition(m_Goal.position);
        if (m_ResetVelocityAfterTeleport)
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
        Gizmos.DrawLine(transform.position, m_Goal.position);
    }
}
