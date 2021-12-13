using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//--------------------------------------------------------------------
//This class solves moving collider support for ControlledCapsuleColliders
//It uses anchor points (designated by AbilityModules and the CharacterControllerBases) to tether the capsule to a surface.
//When that anchor point is moved, due to the surface moving, this class will move the ControlledCapsuleCollider along with it
//--------------------------------------------------------------------
public class MovingColPoint
{
    public Transform m_Transform;
    public Vector3 m_PrevPoint;
    public Quaternion m_PrevRot;
    public Vector3 m_Normal;
    public Vector3 m_PointRelativeToThis;
}
public class CapsuleMovingColliderSolver : MonoBehaviour
{
    [SerializeField] ControlledCapsuleCollider m_ControlledCollider;
    [SerializeField] bool m_ApplyRotationCorrection;
    [SerializeField] bool m_ApplyLastMovementAsVelocity;
    [SerializeField] bool m_DisableDownwardsLastMovement;
    [SerializeField] bool m_UseEscapeAcceleration;
    [SerializeField] float m_EscapeAcceleration;

    List<MovingColPoint> m_ColPoints = new List<MovingColPoint>();
    int m_PreviousAmountOfColPoints;
    Vector3 m_LastMovement;

    //Called by Unity upon adding a new component to an object, or when Reset is selected in the context menu. Used here to provide default values.
    //Also used when fixing up components using the CharacterFixEditor button
    void Reset()
    {
        m_ControlledCollider = transform.GetComponent<ControlledCapsuleCollider>();
        m_ApplyRotationCorrection = true;
        m_ApplyLastMovementAsVelocity = false;
        m_DisableDownwardsLastMovement = false;
        m_UseEscapeAcceleration = false;
        m_EscapeAcceleration = 10.0f;
    }

    void FixedUpdate()
    {
        //If no colpoints have been added this frame, but there were colpoints in the previous frame, then exit colpoints (with optional velocity boost)
        if (m_ColPoints.Count == 0)
        {
            if (m_PreviousAmountOfColPoints > 0)
            {
                ExitColpoints();
            }
            m_PreviousAmountOfColPoints = 0;
            m_LastMovement = Vector3.zero;
            m_ColPoints.Clear();
            return;
        }   
        //Determine the total movement for the current frame by colpoints
        Vector3 currentMovement = Vector3.zero;
        for (int i = 0; i < m_ColPoints.Count; i++)
        {
            Vector3 diff = m_ColPoints[i].m_Transform.position - m_ColPoints[i].m_PrevPoint;
            currentMovement += diff;
            Quaternion rotationDifference = Quaternion.FromToRotation(m_ColPoints[i].m_PrevRot * Vector3.up, m_ColPoints[i].m_Transform.rotation * Vector3.up);
            if (m_ApplyRotationCorrection)
            {
                Vector3 newRelativePoint = rotationDifference * m_ColPoints[i].m_PointRelativeToThis;
                Vector3 relativeDifference = newRelativePoint - m_ColPoints[i].m_PointRelativeToThis;
                currentMovement += relativeDifference;
            }
        }
        //If the total movement for this frame (compared to the previous frame's colpoint movement) is too large, detach the colpoints
        if (m_UseEscapeAcceleration && m_PreviousAmountOfColPoints > 0)
        {
            Vector3 velDifference = (currentMovement / Time.fixedDeltaTime) - (m_LastMovement / Time.fixedDeltaTime);
            for (int i = 0; i < m_ColPoints.Count; i ++)
            {
                float normalDot = Vector3.Dot(m_ColPoints[i].m_Normal, velDifference);
                if (normalDot > 0)
                {
                    velDifference -= m_ColPoints[i].m_Normal * normalDot;
                }
            }
            
            if (velDifference.magnitude >= m_EscapeAcceleration)
            {
                m_ColPoints.Clear();
                ExitColpoints();
            }
        }
        m_LastMovement = currentMovement;
        //Apply ColPoints transformation to the character controller
        for (int i = 0; i < m_ColPoints.Count; i++)
        {
            //Position
            Vector3 diff = m_ColPoints[i].m_Transform.position - m_ColPoints[i].m_PrevPoint;
            m_ControlledCollider.GetCapsuleTransform().Move(diff);

            //Rotation
            Quaternion rotationDifference = Quaternion.FromToRotation(m_ColPoints[i].m_PrevRot * Vector3.up, m_ColPoints[i].m_Transform.rotation * Vector3.up);
            m_ControlledCollider.GetCapsuleTransform().SetUpDirection(rotationDifference * m_ControlledCollider.GetCapsuleTransform().GetUpDirection());
            //Offset for rotation
            if (m_ApplyRotationCorrection)
            { 
                Vector3 newRelativePoint = rotationDifference * m_ColPoints[i].m_PointRelativeToThis;
                Vector3 relativeDifference = newRelativePoint - m_ColPoints[i].m_PointRelativeToThis;
                m_ControlledCollider.GetCapsuleTransform().Move(-relativeDifference);
            }
            m_ControlledCollider.UpdateContextInfo();
        }
        m_PreviousAmountOfColPoints = m_ColPoints.Count;
        //Clear ColPoints
        for (int i = 0; i < m_ColPoints.Count; i++)
        {
            Destroy(m_ColPoints[i].m_Transform.gameObject);
        }
        m_ColPoints.Clear();
    }

    void ExitColpoints()
    {
        //Apply last movement (if out of colpoints)
        if (m_ApplyLastMovementAsVelocity)
        {
            if (m_DisableDownwardsLastMovement)
            {
                m_LastMovement.y = Mathf.Max(0.0f, m_LastMovement.y);
            }
            Vector2 velocity = m_ControlledCollider.GetVelocity();
            Vector2 addVel = new Vector2(m_LastMovement.x, m_LastMovement.y) / Time.fixedDeltaTime;
            velocity += addVel;

            
            m_ControlledCollider.SetVelocity(velocity);
        }
        m_LastMovement = Vector3.zero;
    }

    public void AddColPoint(Transform a_Parent, Vector3 a_Point, Vector3 a_Normal)
    {
        Transform transform = new GameObject().transform;
        transform.position = a_Point;
        transform.parent = a_Parent;
        
        MovingColPoint point = new MovingColPoint();
        point.m_Transform = transform;
        point.m_PrevPoint = transform.position;
        point.m_PrevRot = transform.rotation;
        point.m_Normal = a_Normal;
        point.m_PointRelativeToThis = a_Point - m_ControlledCollider.GetCapsuleTransform().GetPosition();
        
        m_ColPoints.Add(point);
    }

    public void ClearColPoints()
    {
        m_ColPoints.Clear();
    }
}
