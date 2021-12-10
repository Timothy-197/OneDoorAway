using UnityEngine;
using System.Collections;

public class WorldTurnPoint : MonoBehaviour
{
    [SerializeField] float m_RotationDuration = 0.0f;
    [SerializeField] float m_LeftAngle = 0.0f;
    [SerializeField] float m_RightAngle = 0.0f;
    WorldTurner m_WorldTurner;
    void Start()
    {
        m_WorldTurner = FindObjectOfType<WorldTurner>();
    }

    public void TryTurning(GameObject a_Object)
    {
        //See that we don't trigger on a different gameobject entering a trigger
        if (a_Object != m_WorldTurner.GetCharacterObject())
        {
            return;
        }
        if (m_WorldTurner == null)
        {
            Debug.LogError("Could not find WorldTurner object on WorldTurnPoint script. Cannot turn the world");
            return;
        }
        //See if playerCollider was approaching from left or right
        Vector3 difference = m_WorldTurner.GetCharacterObject().transform.position - transform.position;
        difference.y = 0.0f;
        difference.Normalize();
        float leftDot = Mathf.Abs(Vector3.Dot(difference, GetLeftDirection()));
        float rightDot = Mathf.Abs(Vector3.Dot(difference, GetRightDirection()));
        float rotateAngle = 0.0f;
        if (leftDot > rightDot)
        {
            rotateAngle = m_RightAngle - m_LeftAngle;
        }
        else
        {
            rotateAngle = m_LeftAngle - m_RightAngle;
        }
        if (Mathf.Abs(rotateAngle) > 180)
        {
            if (rotateAngle > 0.0f)
            {
                rotateAngle -= 360.0f;
            }
            else
            {
                rotateAngle += 360.0f;
            }
        }
        m_WorldTurner.StartTurning(transform.position, transform, rotateAngle, m_RotationDuration);
    }

    Vector3 GetLeftDirection()
    {
        return Quaternion.AngleAxis(m_LeftAngle, Vector3.up) * transform.right;
    }
    Vector3 GetRightDirection()
    {
        return Quaternion.AngleAxis(m_RightAngle, Vector3.up) * transform.right;
    }

    void OnDrawGizmos()
    {
        Vector3 leftVec = GetLeftDirection();
        Vector3 rightVec = GetRightDirection();
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + leftVec * 3.0f);
        Gizmos.DrawWireSphere(transform.position + leftVec * 3.0f, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + rightVec * 3.0f);
        Gizmos.DrawWireSphere(transform.position + rightVec * 3.0f, 0.5f);
    }
}

