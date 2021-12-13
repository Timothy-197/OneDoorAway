using UnityEngine;
using System.Collections;

//--------------------------------------------------------------------
//Small class to move an object along a path (specified by nodes). Used for moving platforms in levels
//Has reset capabilities when a respawn happens
//--------------------------------------------------------------------
public class RunnerDemoPathMover : MonoBehaviour {
    [System.Serializable]
    public class Node
    {
        public Vector3 m_Position;
        public float m_Duration;
    }
    [SerializeField] Node[] m_Nodes = null;
    [SerializeField] float m_StartFactor = 0.0f;
    [SerializeField] bool m_ResetAfterLastNode = false;
    [SerializeField] bool m_UseRotationForMovement = false;
    float m_NodeStartTime;
    int m_CurrentNode;
    Vector3 m_StartPosition;

    void OnEnable()
    {
        m_StartPosition = transform.position;
        InSceneLevelSwitcher.OnLevelStart += ResetStart;
        ResetStart();
    }

    void OnDisable()
    {
        InSceneLevelSwitcher.OnLevelStart -= ResetStart;
    }

    void ResetStart()
    {
        float totalTime = 0;
        for (int i = 0; i < m_Nodes.Length; i++)
        {
            totalTime += m_Nodes[i].m_Duration;
        }
        float startTime = totalTime * Mathf.Clamp01(m_StartFactor);
        for (int i = 0; i < m_Nodes.Length; i++)
        {
            if (startTime >= m_Nodes[i].m_Duration)
            {
                startTime -= m_Nodes[i].m_Duration;
                m_CurrentNode = i;
            }
            else
            {
                StartNode(i);
                m_NodeStartTime -= startTime;
                break;
            }
        }
    }

    void FixedUpdate () 
	{
        if (m_Nodes.Length < 2) return;

        if (Time.fixedTime - m_NodeStartTime >= m_Nodes[m_CurrentNode].m_Duration)
        {
            int nextIndex = m_CurrentNode + 1;
            if (nextIndex >= m_Nodes.Length || (m_ResetAfterLastNode && nextIndex == m_Nodes.Length-1))
            {
                nextIndex = 0;
            }
            StartNode(nextIndex);
        }
        float factor = (Time.fixedTime - m_NodeStartTime) / Mathf.Max(m_Nodes[m_CurrentNode].m_Duration,0.0001f);
        Vector3 position = Vector3.Lerp(m_Nodes[m_CurrentNode].m_Position, GetNextNode().m_Position, factor);
        if (m_UseRotationForMovement)
        {
            position = transform.rotation * position;
        }
        transform.position = m_StartPosition + position;
	}

    void StartNode(int a_Node)
    {
        a_Node = Mathf.Clamp(a_Node, 0, m_Nodes.Length);
        m_CurrentNode = a_Node;
        m_NodeStartTime = Time.fixedTime;
    }

    Node GetNextNode()
    {
        int index = m_CurrentNode + 1;
        if (index >= m_Nodes.Length)
        {
            index = 0;
        }
        return m_Nodes[index];
    }

    void OnDrawGizmos()
    {
        if (m_Nodes == null ||  m_Nodes.Length < 2) return;
        Vector3 startPosition = transform.position;
        if (Application.isPlaying)
        {
            startPosition = m_StartPosition;
        }

        Gizmos.color = Color.green;
        for (int i = 0; i < m_Nodes.Length; i ++)
        {
            Vector3 position = m_Nodes[i].m_Position;
            if (m_UseRotationForMovement)
            {
                position = transform.rotation * position;
            }
            if (i > 0)
            {
                Vector3 secondPosition = m_Nodes[i - 1].m_Position;
                if (m_UseRotationForMovement)
                {
                    secondPosition = transform.rotation * secondPosition;
                }
                Gizmos.DrawLine(startPosition + position, startPosition + secondPosition);
            }
            Gizmos.matrix = Matrix4x4.TRS(startPosition + position, transform.rotation, transform.localScale);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
