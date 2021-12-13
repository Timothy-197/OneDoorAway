using UnityEngine;
using System.Collections;

public class Net : MonoBehaviour {
    [SerializeField] float m_Width = 0.0f;
    [SerializeField] float m_Height = 0.0f;
    [SerializeField] bool m_ScaleToLocalScale = true;

    float GetWidth()
    {
        return m_Width * (m_ScaleToLocalScale ? transform.localScale.x : 1.0f);
    }

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

    Vector3 GetLeftMostPoint()
    {
        return (transform.position - GetWidth() * 0.5f * transform.right);
    }

    Vector3 GetRightMostPoint()
    {
        return (transform.position + GetWidth() * 0.5f * transform.right);
    }

    public Vector3 GetUpDirection()
    {
        return transform.up;
    }
    public Vector3 GetRightDirection()
    {
        return transform.right;
    }

    public bool HasExceededUpperBound(Vector3 a_Position)
    {
        Vector3 verticalPositionLine = a_Position - GetLowestPoint();
        float heightDot = Vector3.Dot(transform.up, verticalPositionLine);
        if (heightDot > GetHeight())
        {
            return true;
        }
        return false;
    }

    public bool HasExceededLowerBound(Vector3 a_Position)
    {
        Vector3 verticalPositionLine = a_Position - GetLowestPoint();
        float heightDot = Vector3.Dot(transform.up, verticalPositionLine);
        if (heightDot < 0.0f)
        {
            return true;
        }
        return false;
    }

    public bool HasExceededHorizontalBounds(Vector3 a_Position)
    {
        Vector3 horizontalPositionLine = a_Position - GetLeftMostPoint();
        float widthDot = Vector3.Dot(transform.right, horizontalPositionLine);
        if (widthDot < 0.0f || widthDot > GetWidth())
        {
            return true;
        }
        return false;
    }

    public bool IsInBounds(Vector3 a_Position)
    {
        Vector3 horizontalPositionLine = a_Position - GetLeftMostPoint();
        Vector3 verticalPositionLine = a_Position - GetLowestPoint();

        float widthDot = Vector3.Dot(transform.right, horizontalPositionLine);
        float heightDot = Vector3.Dot(transform.up, verticalPositionLine);

        if (widthDot >= 0.0f && widthDot < GetWidth() && heightDot >= 0.0f && heightDot < GetHeight())
        {
            return true;
        }
        return false;
    }

    public Vector3 GetClosestPointOnNet(Vector3 a_Position)
    {
        Vector3 horizontalPositionLine = a_Position - GetLeftMostPoint();
        Vector3 verticalPositionLine = a_Position - GetLowestPoint();

        float widthDot = Vector3.Dot(transform.right, horizontalPositionLine);
        float heightDot = Vector3.Dot(transform.up, verticalPositionLine);

        float relativeX = Mathf.Clamp(widthDot, 0.0f, GetWidth()) - GetWidth() * 0.5f;
        float relativeY = Mathf.Clamp(heightDot, 0.0f, GetHeight()) - GetHeight() * 0.5f;

        return transform.position + relativeX * transform.right + relativeY * transform.up;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(GetWidth(), GetHeight(), transform.localScale.z));
    }
}
