using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Base information for context types and collider state
//Overriden by CCState (for capsules) and capsule based context info
//--------------------------------------------------------------------
public enum RotateMethod
{
    FromCenter,
    FromBottom,
    FromTop
}

public abstract class CGroundedInfo
{
    public bool m_IsGrounded;
    public abstract Vector3 GetPoint();
    public abstract Vector2 GetNormal();
    public abstract Vector2 GetWalkDirection(Vector2 a_Speed);
    public abstract Transform GetGroundTransform();
    public abstract bool IsDangling();
}

public abstract class CSideCastInfo
{
    public bool m_HasHitSide;
    public int m_WallCastCount;
    public RaycastHit m_MostValidHit;
    public float m_Distance;
    public abstract Vector2 GetSideNormal();
    public abstract Vector3 GetSidePoint();
    public abstract float GetDistance();
    public abstract Transform GetWallTransform();
}

public abstract class CEdgeCastInfo
{
    public bool m_HasHitEdge;
    public abstract Vector3 GetUpDirection();
    public abstract Vector3 GetProposedHeadPoint();
    public abstract Vector3 GetWallNormal();
    public abstract Vector3 GetEdgeNormal();
    public abstract Vector3 GetEdgePoint();
    public abstract Transform GetEdgeTransform();
}

public class CState
{
    public static Vector2 GetDirectionAlongNormal(Vector2 a_InitialDirection, Vector2 a_Normal)
    {
        Vector2 direction = new Vector2(a_Normal.y, -a_Normal.x);
        float dirDot = Vector2.Dot(direction, a_InitialDirection);
        if (dirDot < 0)
        {
            direction *= -1.0f;
        }
        return direction.normalized;
    }

    public static Vector3 GetDirectionAlongNormal(Vector3 a_InitialDirection, Vector3 a_Normal)
    {
        Vector3 direction = new Vector3(a_Normal.y, -a_Normal.x);
        float dirDot = Vector3.Dot(direction, a_InitialDirection);
        if (dirDot < 0)
        {
            direction *= -1.0f;
        }
        return direction.normalized;
    }
}