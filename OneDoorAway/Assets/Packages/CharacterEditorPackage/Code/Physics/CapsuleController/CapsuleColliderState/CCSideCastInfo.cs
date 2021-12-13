using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//--------------------------------------------------------------------
//CCSideCastInfo stores the sidecast and wallcast information for a ControlledCapsuleCollider. Is kept by the CCState class.
//This class gets updated whenever the ControlledCapsuleCollider is moved.
//--------------------------------------------------------------------
public class CCSideCastInfo : CSideCastInfo
{
    ControlledCapsuleCollider m_CapsuleCollider;
    public void Init(ControlledCapsuleCollider a_CapsuleCollider)
    {
        m_CapsuleCollider = a_CapsuleCollider;
    }
    public override Vector2 GetSideNormal()
    {
        return m_MostValidHit.normal;
    }
    public override Vector3 GetSidePoint()
    {
        return m_MostValidHit.point;
    }
    public override float GetDistance()
    {
        return m_Distance;
    }

    //Side cast receives the results of two capsulecasts (to the left and the right of the capsule).
    //It uses this first to determine whether there is a hit to the side of the capsule, and then whether or not this hit could signify a wall.
    //Mostvalidhit stores information on this sidecast. Sidecast information can sometimes be used without wallcast information
    public void UpdateWithCollisions(List<RaycastHit> leftHitResults, List<RaycastHit> rightHitResults)
    {
        m_HasHitSide = false;
        m_WallCastCount = 0;
        if (leftHitResults.Count == 0 && rightHitResults.Count == 0)
        {
            return;
        }
        List<RaycastHit> resultsToUse = null;
        //First determine which side to use (based on distance)
        if (leftHitResults.Count == 0)
        {
            resultsToUse = rightHitResults;
        }
        else if (rightHitResults.Count == 0)
        {
            resultsToUse = leftHitResults;
        }
        else
        {
            float lowestSideDistance = float.MaxValue;
            for (int i = 0; i < leftHitResults.Count; i++)
            {
                if (leftHitResults[i].point.y < lowestSideDistance)
                {
                    lowestSideDistance = leftHitResults[i].distance;
                    resultsToUse = leftHitResults;
                }
            }
            for (int i = 0; i < rightHitResults.Count; i++)
            {
                if (rightHitResults[i].point.y < lowestSideDistance)
                {
                    lowestSideDistance = rightHitResults[i].distance;
                    resultsToUse = rightHitResults;
                }
            }
        }
        m_HasHitSide = true;
        m_MostValidHit = resultsToUse[0];
        float lowestDistance = float.MaxValue;
        for (int i = 0; i < resultsToUse.Count; i++)
        {
            if (resultsToUse[i].distance < lowestDistance)
            {
                lowestDistance = resultsToUse[i].distance;
                m_MostValidHit = resultsToUse[i];
            }
        }

        //Capsulecasts can give unusable results on corners.
        //Cast a little bit up and down from the initial point to get a better normal.
        RaycastHit correctingCast1, correctingCast2;
        Vector3 dirAlongNormal = CState.GetDirectionAlongNormal(Vector3.up, m_MostValidHit.normal);
        bool correctCast1Hit = Physics.Raycast(m_MostValidHit.point + m_MostValidHit.normal * 0.02f + dirAlongNormal * 0.01f, -m_MostValidHit.normal, out correctingCast1, 0.03f, m_CapsuleCollider.GetLayerMask());
        bool correctCast2Hit = Physics.Raycast(m_MostValidHit.point + m_MostValidHit.normal * 0.02f - dirAlongNormal * 0.01f, -m_MostValidHit.normal, out correctingCast2, 0.03f, m_CapsuleCollider.GetLayerMask());
        
        if (correctCast1Hit || correctCast2Hit)
        {
            RaycastHit correctCastToUse = m_MostValidHit;
            if (correctCast1Hit && correctCast2Hit)
            {
                float dot1 = Vector3.Dot(m_MostValidHit.normal, correctingCast1.normal);
                float dot2 = Vector3.Dot(m_MostValidHit.normal, correctingCast2.normal);
                if (dot1 < 0.95f && dot2 < 0.95f)
                {
                    if (dot1 > dot2)
                    {
                        correctCastToUse = correctingCast1;
                    }
                    else
                    {
                        correctCastToUse = correctingCast2;
                    }
                }
            }
            else if (correctCast1Hit)
            {
                if (Vector3.Dot(m_MostValidHit.normal, correctingCast1.normal) < 0.95f)
                {
                    correctCastToUse = correctingCast1;
                }
            }
            else
            {
                if (Vector3.Dot(m_MostValidHit.normal, correctingCast2.normal) < 0.95f)
                {
                    correctCastToUse = correctingCast2;
                }
            }
            m_MostValidHit = correctCastToUse;
        }
        m_Distance = lowestDistance - m_CapsuleCollider.GetSideCastMargin();
        WallCast(m_MostValidHit);
    }
    //This takes the results of a sidecast and tries to determine if a wall is found.
    //Using the sidecast information, it sends out three probes (from the center of the top and bottom hemispheres and the center of the capsule).
    //It counts the amount of hits that are valid and stores this for other classes to access.
    void WallCast(RaycastHit a_SidecastHit)
    {
        if (Vector3.Angle(Vector3.up, a_SidecastHit.normal) > m_CapsuleCollider.GetMaxWallAngle() || Vector3.Angle(Vector3.up, a_SidecastHit.normal) < m_CapsuleCollider.GetMaxGroundedAngle())
        {
            m_WallCastCount = 0;
            return;
        }
        float wallCastMargin = m_CapsuleCollider.GetWallCastMargin();
        float wallCastDistance = m_CapsuleCollider.GetWallCastDistance();
        Vector3 hitPos = a_SidecastHit.point;
        Vector3 normal = a_SidecastHit.normal;
        Vector3 currentUp = m_CapsuleCollider.GetUpDirection();
        Vector3 direction = CState.GetDirectionAlongNormal(Vector3.up, normal);
        Vector3 startHitPos1 = Vector3.zero;
        Vector3 startHitPos2 = Vector3.zero;
        Vector3 startHitPos3 = Vector3.zero;
        Vector3 normalOff = normal * (m_CapsuleCollider.GetRadius() - wallCastMargin);

        //Determines where the sidecast has hit. This to orient the probes in the correct way.
        if (Vector3.Dot(currentUp, hitPos) <= Vector3.Dot(currentUp, m_CapsuleCollider.GetDownCenter()))//Hit on the lower hemisphere
        {
            startHitPos1 = m_CapsuleCollider.GetDownCenter() + direction * m_CapsuleCollider.GetDefaultLength() - normalOff;
            startHitPos2 = m_CapsuleCollider.GetDownCenter() - normalOff;
            startHitPos3 = m_CapsuleCollider.GetDownCenter() + direction * m_CapsuleCollider.GetDefaultLength() * 0.5f - normalOff;
        }
        else if (Vector3.Dot(currentUp, hitPos) >= Vector3.Dot(currentUp, m_CapsuleCollider.GetUpCenter()))//Hit on the upper hemisphere
        {
            startHitPos1 = m_CapsuleCollider.GetUpCenter() - direction * m_CapsuleCollider.GetDefaultLength() - normalOff;
            startHitPos2 = m_CapsuleCollider.GetUpCenter() - normalOff;
            startHitPos3 = m_CapsuleCollider.GetUpCenter() - direction * m_CapsuleCollider.GetDefaultLength() * 0.5f - normalOff;
        }
        else //Hit somewhere in the middle
        {
            startHitPos1 = m_CapsuleCollider.GetDownCenter(true) + direction * m_CapsuleCollider.GetDefaultLength() - normalOff;
            startHitPos2 = m_CapsuleCollider.GetDownCenter(true) - normalOff;
            startHitPos3 = m_CapsuleCollider.GetDownCenter(true) + direction * m_CapsuleCollider.GetDefaultLength() * 0.5f - normalOff;
        }

        RaycastHit newHit;
        m_WallCastCount = 0;
        if (Physics.Raycast(startHitPos1, -normal, out newHit, wallCastDistance + wallCastMargin, m_CapsuleCollider.GetLayerMask()))
        {
            m_WallCastCount++;
        }
        if (Physics.Raycast(startHitPos2, -normal, out newHit, wallCastDistance + wallCastMargin, m_CapsuleCollider.GetLayerMask()))
        {
            m_WallCastCount++;
        }
        if (Physics.Raycast(startHitPos3, -normal, out newHit, wallCastDistance + wallCastMargin, m_CapsuleCollider.GetLayerMask()))
        {
            m_WallCastCount++;
        }
    }

    public override Transform GetWallTransform()
    {
        return m_MostValidHit.transform;
    }
}