using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Attach to objects to be used by InSceneLevelSwitcher as starting points
//--------------------------------------------------------------------
[System.Serializable]
public class InSceneLevel : MonoBehaviour {
    public Transform m_StartPoint;

    void OnDrawGizmos()
    {
        if (m_StartPoint != null)
        {
            Gizmos.color = Color.Lerp(Color.red, Color.white, 0.7f);
            Gizmos.DrawWireSphere(m_StartPoint.position, 0.5f);
        }
    }
}
