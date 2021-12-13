using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//--------------------------------------------------------------------
//A CapsuleMovementPath is used by GroundedAnimatedAbilityModule and its derivatives
//A movement path is a series of nodes which define a capsuletransform state as well as a duration
//A CapsuleTransform can be moved along this path over time
//The path and its nodes are created by animated movement modules
//--------------------------------------------------------------------
public class CapsuleMovementPath {
    List<CapsuleMovementPathNode> m_PathNodes = new List<CapsuleMovementPathNode>();
    public CapsuleMovementPathNode m_CurrentPathNode;
    int m_CurrentIndex;

    public void IncrementCurrentNode()
    {
        m_CurrentIndex++;
        if (m_CurrentIndex < m_PathNodes.Count)
        { 
            m_CurrentPathNode = m_PathNodes[m_CurrentIndex];
        }
        else
        {
            m_CurrentPathNode = null;
        }
    }
    //Check if current motion can be executed (to see if it was interrupted
    public bool CanApplyMotion(CapsuleTransform a_Transform, float a_Time)
    {
        if (m_CurrentPathNode == null)
        {
            return false;
        }
        if (m_CurrentIndex == 0)
        {
            return true;
        }
        if (m_CurrentPathNode.m_Duration == 0)
        {
            return m_CurrentPathNode.CanApplyEntireMovement(a_Transform);
        }
        else
        {
            float factor = Mathf.Clamp01(a_Time / m_CurrentPathNode.m_Duration);

            CapsuleMovementPathNode newPoint = new CapsuleMovementPathNode();
            newPoint.InterpolationBetweenTwoNodes(m_PathNodes[m_CurrentIndex - 1], m_CurrentPathNode, factor);

            return newPoint.CanApplyEntireMovement(a_Transform);
        }
    }
    //Calld by GroundedAnimatedAbilityModule to move a capsuletransform along the path
    public void ApplyMotion(CapsuleTransform a_Transform, float a_Time)
    {
        if (m_CurrentPathNode == null)
        {
            return;
        }
        if (m_CurrentIndex == 0)
        {
            m_CurrentPathNode.ApplyEntireMovement(a_Transform);
            return;
        }
        if (m_CurrentPathNode.m_Duration == 0)
        {
            m_CurrentPathNode.ApplyEntireMovement(a_Transform);
        }
        else
        {
            float factor = Mathf.Clamp01(a_Time / m_CurrentPathNode.m_Duration);

            CapsuleMovementPathNode newPoint = new CapsuleMovementPathNode();
            newPoint.InterpolationBetweenTwoNodes(m_PathNodes[m_CurrentIndex - 1], m_CurrentPathNode, factor);

            newPoint.ApplyEntireMovement(a_Transform);
        }
    }
    //Apply the final transform state (when exiting the path)
    public void ApplyFinalNode(CapsuleTransform a_Transform)
    {
        if (m_PathNodes.Count == 0)
        {
            return;
        }
        m_PathNodes[m_PathNodes.Count - 1].ApplyEntireMovement(a_Transform);
    }

    public bool IsDone()
    {
        return (m_CurrentPathNode == null);
    }

    public float GetTotalTime()
    {
        float time = 0.0f;
        for (int i = 0; i < m_PathNodes.Count; i++)
        {
            time += m_PathNodes[i].m_Duration;
        }
        return time;
    }
    //Move the entire path (when the collider it started on is moving)
    public void Move(Vector3 a_Difference)
    {
        for (int i = 0; i < m_PathNodes.Count; i ++)
        {
            m_PathNodes[i].m_Position += a_Difference;
        }
    }
    //Rotate the entire path (when the collider it started on is moving)
    public void Rotate(Quaternion a_Rotation, Vector3 a_Pivot)
    {
        for (int i = 0; i < m_PathNodes.Count; i++)
        {
            Vector3 offsetToPivot = m_PathNodes[i].m_Position - a_Pivot;
            Vector3 newRelativePoint = a_Rotation * offsetToPivot;
            m_PathNodes[i].m_Position = a_Pivot + newRelativePoint;

            m_PathNodes[i].m_UpDirection = a_Rotation * m_PathNodes[i].m_UpDirection;
        }
    }
    //Creation functions for the path
    public CapsuleMovementPathNode CreateFirstNode(CapsuleTransform a_Transform)
    {
        CapsuleMovementPathNode node = new CapsuleMovementPathNode();
        node.CopyFromTransform(a_Transform);
        AddNode(node);
        m_CurrentPathNode = node;
        m_CurrentIndex = 0;
        return node;
    }

    public CapsuleMovementPathNode DuplicateAndAddLastNode()
    {
        if (m_PathNodes.Count == 0)
        {
            Debug.Log("Path is empty!");
            return new CapsuleMovementPathNode();
        }
        CapsuleMovementPathNode node = new CapsuleMovementPathNode();
        node.CopyFromPathNode(m_PathNodes[m_PathNodes.Count - 1]);
        AddNode(node);
        return node;
    }

    public void Clear()
    {
        m_PathNodes.Clear();
        m_CurrentPathNode = null;
        m_CurrentIndex = 0;
    }

    void AddNode(CapsuleMovementPathNode a_Node)
    {
        m_PathNodes.Add(a_Node);
    }
    //Check if the entire path can be applied
    public bool IsPossible(CapsuleTransform a_Transform)
    {
        CapsuleTransform transform = a_Transform.CreateCopy();
        bool isPossible = true;
        for (int i = 0; i < m_PathNodes.Count; i++)
        {
            //if (m_PathNodes[i].CanApplyEntireMovement(transform))
            {
                m_PathNodes[i].ApplyEntireMovement(transform);
            }
            //else
            {
               // return false;
            }
        }
        if (!transform.CanExistHere())
            return false;
        return isPossible;
    }
}
