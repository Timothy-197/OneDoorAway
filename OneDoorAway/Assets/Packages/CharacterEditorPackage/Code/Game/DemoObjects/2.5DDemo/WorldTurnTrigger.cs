using UnityEngine;
using System.Collections;

public class WorldTurnTrigger : MonoBehaviour
{
    [SerializeField] WorldTurnPoint m_ActualTurnPoint = null;
    void OnTriggerEnter(Collider a_Collider)
    { 
        m_ActualTurnPoint.TryTurning(a_Collider.gameObject);
    }

    void OnDrawGizmos()
    { 
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }
}

