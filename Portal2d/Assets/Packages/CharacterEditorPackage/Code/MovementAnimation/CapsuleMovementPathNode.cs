using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//A CapsuleMovementPathNode is a node in a CapsuleMovementPath
//It uses CapsuleManipulator to work on CapsuleTransforms
//CapsuleMovementPath controls it
//A node defines a state for a capsuletransform, as well as a duration (how long it should take to get to this position)
//--------------------------------------------------------------------
public class CapsuleMovementPathNode
{
    public float m_Duration;

    public Vector3 m_Position;
    public Vector3 m_UpDirection;
    public float m_Length;
    public CapsuleResizeMethod m_ResizeMethod;
    public RotateMethod m_RotateMethod;

    public bool CanApplyEntireMovement(CapsuleTransform a_Transform)
    {
        if (a_Transform.GetPosition() != m_Position)
        {
            if (!a_Transform.CanMove(m_Position - a_Transform.GetPosition(), true))
            {
                return false;
            }
        }
        if (a_Transform.GetUpDirection() != m_UpDirection)
        {
            if (!a_Transform.CanRotate(m_UpDirection, m_RotateMethod))
            {
                return false;
            }
        }
        if (a_Transform.GetLength() != m_Length)
        {
            if (!a_Transform.CanBeResized(m_Length, m_ResizeMethod))
            {
                return false;
            }
        }
        return true;
    }

    public void ApplyEntireMovement(CapsuleTransform a_Transform)
    {
        if (a_Transform.GetPosition() != m_Position)
        {
            a_Transform.SetPosition(m_Position);
        }
        if (a_Transform.GetUpDirection() != m_UpDirection)
        {
            a_Transform.Rotate(m_UpDirection, m_RotateMethod);
        }
        if (a_Transform.GetLength() != m_Length)
        {
            a_Transform.SetLength(m_Length, m_ResizeMethod);
        }
    }

    public void CopyFromTransform(CapsuleTransform a_Transform)
    {
        m_Position = a_Transform.GetPosition();
        m_UpDirection = a_Transform.GetUpDirection();
        m_Length = a_Transform.GetLength();
    }

    public void CopyFromPathNode(CapsuleMovementPathNode a_PathNode)
    {
        m_Position = a_PathNode.m_Position;
        m_UpDirection = a_PathNode.m_UpDirection;
        m_Length = a_PathNode.m_Length;
        m_Duration = a_PathNode.m_Duration;
        m_RotateMethod = a_PathNode.m_RotateMethod;
        m_ResizeMethod = a_PathNode.m_ResizeMethod;
    }

    public void InterpolationBetweenTwoNodes(CapsuleMovementPathNode a_PathNode1, CapsuleMovementPathNode a_PathNode2, float a_Factor)
    {
        a_Factor = Mathf.Clamp01(a_Factor);
        m_Position = Vector3.Lerp(a_PathNode1.m_Position, a_PathNode2.m_Position, a_Factor);
        m_UpDirection = Vector3.Lerp(a_PathNode1.m_UpDirection, a_PathNode2.m_UpDirection, a_Factor).normalized;
        m_Length = Mathf.Lerp(a_PathNode1.m_Length, a_PathNode2.m_Length, a_Factor);
    }
}
