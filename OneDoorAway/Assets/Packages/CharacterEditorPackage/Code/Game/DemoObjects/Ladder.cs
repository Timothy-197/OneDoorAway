using UnityEngine;
using System.Collections;
//WARNING
//--------------------------------------------------------------------
//Is going to be deprecated and replaced completely by Net, since Net objects can be used to create ladders
//--------------------------------------------------------------------
public class Ladder : MonoBehaviour {
    [SerializeField] float m_Height = 0.0f;
    [SerializeField] bool m_ScaleToLocalScale = true;

    float GetHeight()
    {
        return m_Height * (m_ScaleToLocalScale ? transform.localScale.y : 1.0f);
    }

    Vector3 GetLowestPoint()
    {
        return (transform.position - GetHeight() * 0.5f * transform.up);
    }

    Vector3 GetHighestPoint()
    {
        return (transform.position + GetHeight() * 0.5f * transform.up);
    }

    public Vector3 GetUpDirection()
    {
        return transform.up;
    }

    public bool HasExceededUpperBound(Vector3 a_Position)
    {
        Vector3 ladderLine = GetHighestPoint() - GetLowestPoint();
        Vector3 positionLine = a_Position - GetLowestPoint();

        float dot = Vector3.Dot(ladderLine.normalized, positionLine);

        if (dot >= GetHeight())
        {
            return true;
        }
        return false;
    }

    public bool HasExceededLowerBound(Vector3 a_Position)
    {
        Vector3 ladderLine = GetHighestPoint() - GetLowestPoint();
        Vector3 positionLine = a_Position - GetLowestPoint();

        float dot = Vector3.Dot(ladderLine.normalized, positionLine);

        if (dot <= 0.0f)
        {
            return true;
        }
        return false;
    }


    public Vector3 GetClosestPointOnLadder(Vector3 a_Position)
    {
        Vector3 ladderLine = GetHighestPoint() - GetLowestPoint();
        Vector3 positionLine = a_Position - GetLowestPoint();

        float dot = Vector3.Dot(ladderLine.normalized, positionLine);

        if (dot <= 0.0f)
        {
            return GetLowestPoint();
        }
        else if (dot >= GetHeight())
        {
            return GetHighestPoint();
        }
        else
        {
            return GetLowestPoint() + ladderLine.normalized * dot;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawLine(GetLowestPoint() + Vector3.back, GetHighestPoint() + Vector3.back);
    }
}
