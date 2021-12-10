using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Small class to rotate an object back and forth a certain angle. Used for moving platforms in levels
//--------------------------------------------------------------------
public class Balancer: MonoBehaviour
{
    [SerializeField] float m_MovementSpeed = 0.0f;
    [SerializeField] float m_MaxAngle = 0.0f;
    [SerializeField] AnimationCurve m_RotationCurve = null;

    protected float m_Time;

    void FixedUpdate()
    {
        m_Time += Time.fixedDeltaTime * m_MovementSpeed;
        float angle = m_RotationCurve.Evaluate(m_Time) * m_MaxAngle * 2.0f - m_MaxAngle;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
