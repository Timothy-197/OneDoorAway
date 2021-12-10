using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//--------------------------------------------------------------------
//CCGroundedInfo stores the grounded information for a ControlledCapsuleCollider. Is kept by the CCState class.
//This class gets updated whenever the ControlledCapsuleCollider is moved.
//--------------------------------------------------------------------
public class CCGroundedInfo : CGroundedInfo
{
    ControlledCapsuleCollider m_CapsuleCollider;
    List<RaycastHit> m_ValidHits = new List<RaycastHit>();
    RaycastHit m_MostValidHit;
    public void Init(ControlledCapsuleCollider a_CapsuleCollider)
    {
        m_CapsuleCollider = a_CapsuleCollider;
    }
    //This takes a list of raycasthits (which have been obtained from a downwards spherecast) and tries to decide if the character is grounded.
    //The raycasthit which determines the grounded state also contains information about normals.
    public void UpdateWithCollisions(List<RaycastHit> a_Collisions)
    {
        m_ValidHits.Clear();
        m_IsGrounded = false;
        Vector3 downCenter = m_CapsuleCollider.GetDownCenter();
        //Discard all hitresults that are above the lower hemisphere (locally), or have an inacceptable angle (locally or globally)
        for (int i = 0; i < a_Collisions.Count; i++)
        {
            float hitPointDot = Vector3.Dot(a_Collisions[i].point, Vector3.up);
            float downCenterDot = Vector3.Dot(downCenter, Vector3.up);
            float reqAngle = m_CapsuleCollider.GetMaxGroundedAngle();
            float angle = Vector3.Angle(Vector3.up, a_Collisions[i].normal);
            if (hitPointDot < downCenterDot && Mathf.Abs(angle) < reqAngle)
            {
                m_ValidHits.Add(a_Collisions[i]);
            }
        }

        if (m_ValidHits.Count != 0)
        {
            float shortestDistance = m_ValidHits[0].distance;
            m_MostValidHit = m_ValidHits[0];
            for (int i = 1; i < m_ValidHits.Count; i++)
            {
                if (m_ValidHits[i].distance < shortestDistance)
                {
                    shortestDistance = m_ValidHits[i].distance;
                    m_MostValidHit = m_ValidHits[i];
                }
            }
            m_IsGrounded = true;
        }
    }

    public override Vector3 GetPoint()
    {
        if (!m_IsGrounded)
        {
            return Vector3.zero;
        }
        else
        {
            return m_MostValidHit.point;
        }
    }

    public override Vector2 GetNormal()
    {
        if (!m_IsGrounded)
        {
            return Vector2.up;
        }
        else
        {
            return m_MostValidHit.normal;
        }
    }
    //Easy way to get a direction along the ground
    public override Vector2 GetWalkDirection(Vector2 a_Speed)
    {
        if (a_Speed.x == 0 || !m_IsGrounded)
        {
            return Vector2.zero;
        }
        return CState.GetDirectionAlongNormal(a_Speed, GetNormal());
    }

    public override Transform GetGroundTransform()
    {
        return m_MostValidHit.transform;
    }

    public override bool IsDangling()
    {
        bool hit = Physics.Raycast(m_CapsuleCollider.GetCapsuleTransform().GetDownCenter(), Vector3.down, m_CapsuleCollider.GetRadius() + m_CapsuleCollider.GetGroundedMargin(), m_CapsuleCollider.GetLayerMask());
        if (hit)
        {
            return false;
        }
        else
        {
            return true;
        }     
    }
}
