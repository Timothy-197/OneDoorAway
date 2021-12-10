using UnityEngine;
using System.Collections;

public class FixedUpdateRenderInterpolator : MonoBehaviour {
    public enum SpriteInterpolation
    {
        None,
        Interpolate,
        Extrapolate
    }
    [SerializeField] Transform m_ActualMover = null;
    [SerializeField] SpriteInterpolation m_SpriteInterpolation = SpriteInterpolation.None;
    Vector3 m_LastFixedUpdatePosition;
    Vector3 m_CurrentFixedUpdatePosition;
    Vector3 m_LastDisplacement;
    Quaternion m_LastFixedUpdateRotation;
    Quaternion m_CurrentFixedUpdateRotation;
    Quaternion m_LastRotation;

    void FixedUpdate()
    { 
        m_LastFixedUpdatePosition = m_CurrentFixedUpdatePosition;
        m_CurrentFixedUpdatePosition = m_ActualMover.position;
        float difference = (m_CurrentFixedUpdatePosition - m_LastFixedUpdatePosition).magnitude;
        //We have likely been teleported. Interpolation would be annoying at this point.
        if (difference > 1)
        {
            m_LastFixedUpdatePosition = m_CurrentFixedUpdatePosition;
        }

        m_LastFixedUpdateRotation = m_CurrentFixedUpdateRotation;
        m_CurrentFixedUpdateRotation = m_ActualMover.rotation;

        m_LastDisplacement = m_CurrentFixedUpdatePosition - m_LastFixedUpdatePosition;
        m_LastRotation = Quaternion.Inverse(m_CurrentFixedUpdateRotation) * m_LastFixedUpdateRotation;
    }

    void LateUpdate () 
	{
        //Interpolation of rendered object
        float interpolationFactor = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        interpolationFactor = Mathf.Clamp01(interpolationFactor);
        transform.position = m_ActualMover.position;
        transform.rotation = m_ActualMover.rotation;
        switch (m_SpriteInterpolation)
        {
            case SpriteInterpolation.Interpolate:
                transform.position = Vector3.Lerp(m_LastFixedUpdatePosition, m_CurrentFixedUpdatePosition, interpolationFactor);
                transform.rotation = Quaternion.Slerp(m_LastFixedUpdateRotation, m_CurrentFixedUpdateRotation, interpolationFactor);
                break;
            case SpriteInterpolation.Extrapolate:
                transform.position = Vector3.Lerp(m_CurrentFixedUpdatePosition, m_CurrentFixedUpdatePosition + m_LastDisplacement, interpolationFactor);
                transform.rotation = Quaternion.Slerp(m_CurrentFixedUpdateRotation, m_CurrentFixedUpdateRotation * m_LastRotation, interpolationFactor);
                break;
        }
    }
}
