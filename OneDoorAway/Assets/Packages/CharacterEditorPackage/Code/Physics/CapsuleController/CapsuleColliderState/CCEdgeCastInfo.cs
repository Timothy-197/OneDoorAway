using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//CCEdgeCastInfo stores the edge information for a ControlledCapsuleCollider. Is kept by the CCState class.
//This class gets updated whenever the ControlledCapsuleCollider is moved.
//--------------------------------------------------------------------
public class CCEdgeCastInfo : CEdgeCastInfo
{
    ControlledCapsuleCollider m_CapsuleCollider;
    Vector3 m_UpDirection;
    Vector3 m_ProposedHeadPosition;
    Vector3 m_WallNormal;
    Vector3 m_EdgePoint;
    Vector3 m_EdgeNormal;
    Transform m_EdgeTransform;
    public void Init(ControlledCapsuleCollider a_CapsuleCollider)
    {
        m_CapsuleCollider = a_CapsuleCollider;
    }
    //This context uses raycasts to determine if a ledge is present.
    //The raycasts originate from above and to the left/right of the character, and are casted downwards
    public void UpdateWithCollisions()
    {
        RaycastHit rightHit;
        RaycastHit leftHit;
        bool rightHasHit = false;
        bool leftHasHit = false;
        m_HasHitEdge = false;
        Vector3 upCenter = m_CapsuleCollider.GetUpCenter();
        Vector3 upDirection = m_CapsuleCollider.GetUpDirection();
        Vector3 transformRight = m_CapsuleCollider.transform.right;
        //Orient against wall to the side, even if not able to wallsliding
        //This allows the character to grab onto ledges on slanted walls
        CSideCastInfo sideInfo = m_CapsuleCollider.GetSideCastInfo();
        if (sideInfo.m_HasHitSide)
        {
            Vector3 sideNormal = sideInfo.GetSideNormal();
            Vector3 sidePoint = sideInfo.GetSidePoint();
            Vector3 sideUp = CState.GetDirectionAlongNormal(Vector3.up, sideNormal);

            RaycastHit properSideHit;
            if (Physics.Raycast(sidePoint + sideNormal * 0.2f + sideUp * 0.05f, -sideNormal, out properSideHit, 0.3f, m_CapsuleCollider.GetLayerMask()))
            {
                Vector3 newPoint = properSideHit.point;
                Vector3 newNormal = properSideHit.normal;
                Vector3 newUp = CState.GetDirectionAlongNormal(Vector3.up, newNormal);

                Vector3 pivot = upCenter - newPoint;
                Quaternion rotation = Quaternion.FromToRotation(upDirection, newUp);
                Vector3 newUpCenter = newPoint + rotation * pivot;
                CapsuleTransform transform = m_CapsuleCollider.GetCapsuleTransformCopy();
                transform.SetUpDirection(newUp);
                transform.SetUpCenter(newUpCenter);

                upCenter = newUpCenter;
                upDirection = newUp;
                transformRight = transform.GetRightDirection();
            }
        }
        //Determine the starting positions of the raycasts
        Vector3 centerStart = upCenter + upDirection * (m_CapsuleCollider.GetRadius() + m_CapsuleCollider.GetEdgeCastVerticalMargin());
        Vector3 sideOffSet = transformRight * (m_CapsuleCollider.GetRadius() + m_CapsuleCollider.GetEdgeCastHorizontalDistance());
        Vector3 grabLength = -upDirection * (m_CapsuleCollider.GetEdgeCastVerticalDistance() + m_CapsuleCollider.GetEdgeCastVerticalMargin());

        //The raycasts originate from above and to the left/right of the character, and are casted downwards
        //The angle of the slope they hit can't be too steep, or the character will slide off.
        if (Physics.Raycast(centerStart + sideOffSet, -upDirection, out rightHit, grabLength.magnitude, m_CapsuleCollider.GetLayerMask()))
        {
            if (Vector3.Angle(rightHit.normal, upDirection) < m_CapsuleCollider.GetMaxGrabAngle())
                rightHasHit = true;
        }
        if (Physics.Raycast(centerStart - sideOffSet, -upDirection, out leftHit, grabLength.magnitude, m_CapsuleCollider.GetLayerMask()))
        {
            if (Vector3.Angle(leftHit.normal, upDirection) < m_CapsuleCollider.GetMaxGrabAngle())
                leftHasHit = true;
        }
        if (rightHasHit || leftHasHit)
        {
            RaycastHit castToUse;
            Vector3 wallProbeDirection;
            if (rightHasHit && leftHasHit)
            {
                if (leftHit.distance < rightHit.distance)
                {
                    castToUse = leftHit;
                    wallProbeDirection = -transformRight;
                }
                else
                {
                    castToUse = rightHit;
                    wallProbeDirection = transformRight;
                }
            }
            else if (rightHasHit)
            {
                castToUse = rightHit;
                wallProbeDirection = transformRight;
            }
            else
            {
                castToUse = leftHit;
                wallProbeDirection = -transformRight;
            }

            //Check if the character can even hold on. The distance between the character and the start of the raycastpoint should not be blocked by a collider (no grabbing on ledges on the other side of walls
            if (Physics.Raycast(centerStart, wallProbeDirection, m_CapsuleCollider.GetRadius() + m_CapsuleCollider.GetEdgeCastHorizontalDistance(), m_CapsuleCollider.GetLayerMask()))
            {
                return;
            }

            wallProbeDirection = CState.GetDirectionAlongNormal(wallProbeDirection, castToUse.normal);
            //After detecting that collider can hang onto edge, orient properly against surface
            RaycastHit probeHit;
            Vector3 probedNormal = CState.GetDirectionAlongNormal(-wallProbeDirection, Vector3.up);
            if (Physics.Raycast(upCenter, wallProbeDirection, out probeHit, m_CapsuleCollider.GetRadius() + m_CapsuleCollider.GetEdgeAlignProbeDistance(), m_CapsuleCollider.GetLayerMask()))
            {
                float angle = Vector3.Angle(probeHit.normal, Vector3.up);
                if (angle < m_CapsuleCollider.GetMaxEdgeAlignAngle())
                {
                    probedNormal = probeHit.normal;
                }
            }
            Vector3 newUpDirection = CState.GetDirectionAlongNormal(Vector3.up, probedNormal);
            Vector3 headPoint = castToUse.point + probedNormal * (m_CapsuleCollider.GetRadius() + m_CapsuleCollider.GetEdgeCastHorizontalDistance()) - newUpDirection * m_CapsuleCollider.GetRadius();
            //Block check
            //Check if something is obstructing the proposed headposition (and fix if so)
            RaycastHit blockHit;
            if (Physics.Raycast(headPoint, -probedNormal, out blockHit, m_CapsuleCollider.GetRadius(), m_CapsuleCollider.GetLayerMask()))
            {
                if (blockHit.distance < m_CapsuleCollider.GetRadius())
                {
                    Vector3 alongNormal = -CState.GetDirectionAlongNormal(wallProbeDirection, castToUse.normal);
                    castToUse.point += alongNormal * (m_CapsuleCollider.GetRadius() - blockHit.distance) / Vector3.Dot(alongNormal, blockHit.normal);

                    headPoint = castToUse.point + probedNormal * (m_CapsuleCollider.GetRadius() + m_CapsuleCollider.GetEdgeCastHorizontalDistance()) - newUpDirection * m_CapsuleCollider.GetRadius();
                }
            }

            //Rotation check, check if character is aligned to wall or dangling
            //then check if that rotation can be achieved
            CapsuleTransform copy = m_CapsuleCollider.GetCapsuleTransformCopy();
            copy.SetUpCenter(headPoint);
            if (Vector3.Angle(Vector3.up, probedNormal) > m_CapsuleCollider.GetMaxWallAngle() || Vector3.Angle(Vector3.up, probedNormal) < m_CapsuleCollider.GetMaxGroundedAngle())
            {
                return;
            }

            if (copy.CanRotate(newUpDirection, RotateMethod.FromTop))
            {
                m_EdgePoint = castToUse.point;
                m_EdgeNormal = castToUse.normal;
                m_EdgeTransform = castToUse.transform;
                m_WallNormal = probedNormal;
                m_UpDirection = newUpDirection;
                m_ProposedHeadPosition = headPoint;
                m_HasHitEdge = true;
            }
        }
    }

    public override Vector3 GetUpDirection()
    {
        return m_UpDirection;
    }

    public override Vector3 GetProposedHeadPoint()
    {
        return m_ProposedHeadPosition;
    }

    public override Vector3 GetWallNormal()
    {
        return m_WallNormal;
    }

    public override Vector3 GetEdgeNormal()
    {
        return m_EdgeNormal;
    }
    public override Vector3 GetEdgePoint()
    {
        return m_EdgePoint;
    }

    public override Transform GetEdgeTransform()
    {
        return m_EdgeTransform;
    }
}
