using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Follows the player along the 2d plane, using a continuous lerp
//--------------------------------------------------------------------
public class BasicCameraTracker : MonoBehaviour {
    [SerializeField] GameObject m_Target = null;
    [SerializeField] float m_InterpolationFactor = 0.0f;
    [SerializeField] bool m_UseFixedUpdate = false;
    [SerializeField] float m_ZDistance = 10.0f;

	void FixedUpdate () 
	{
        if (m_UseFixedUpdate)
        {
            Interpolate(Time.fixedDeltaTime);
        }
	}

    void LateUpdate()
    {
        if (!m_UseFixedUpdate)
        {
            Interpolate(Time.deltaTime);
        }
    }

    void Interpolate(float a_DeltaTime)
    {
        if (m_Target == null)
        {
            return;
        }
        Vector3 diff = m_Target.transform.position + Vector3.back * m_ZDistance - transform.position;
        transform.position += diff * m_InterpolationFactor * a_DeltaTime;
    }
}
