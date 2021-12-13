using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to draw a line where the Zipline is. (uses built in LineRenderer component). Works for both curved and straight ziplines

//We execute in edit mode just so we can see where our zipline is
[ExecuteInEditMode]
public class ZiplineRenderer : MonoBehaviour {
	[SerializeField] LineRenderer m_Renderer = null;
    [SerializeField] Zipline m_Zipline = null;
	[SerializeField] int m_Subdivisions = 2;
	Vector3[] m_PositionsT1;
	Vector3[] m_PositionsT0;
	void Start () 
	{
		if (m_Zipline)
		{
			m_PositionsT0 = new Vector3[m_Subdivisions];
			m_PositionsT1 = new Vector3[m_Subdivisions];
			for (int i = 0; i < m_Subdivisions; i ++)
			{
				float t = (float)i/(float)(m_Subdivisions-1);
				m_PositionsT0[i] = m_Zipline.GetPositionAtUnitLength(t);
				m_PositionsT1[i] = m_Zipline.GetPositionAtUnitLength(t);
			}
			UpdateRenderPositions(m_PositionsT1);
		}
	}

	void FixedUpdate()
	{
		for (int i = 0; i < m_Subdivisions; i ++)
		{
			m_PositionsT0[i] = m_PositionsT1[i];
		}
		for (int i = 0; i < m_Subdivisions; i ++)
		{
			float t = (float)i/(float)(m_Subdivisions-1);
			m_PositionsT1[i] = m_Zipline.GetPositionAtUnitLength(t);
		}
	}
	void Update () 
	{
		if (!Application.isPlaying)
		{
			if (m_Subdivisions >= 2)
			{
				Vector3[] positions = new Vector3[m_Subdivisions];
				for (int i = 0; i < m_Subdivisions; i ++)
				{
					float t = (float)i/(float)(m_Subdivisions-1);
					positions[i] = m_Zipline.GetPositionAtUnitLength(t);
				}
				UpdateRenderPositions(positions);
			}
		}
		else
		{
			float interpolationFactor = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
			Vector3[] positions = new Vector3[m_PositionsT0.Length];
			for (int i = 0; i < m_Subdivisions; i ++)
			{
				positions[i] = Vector3.Lerp(m_PositionsT0[i], m_PositionsT1[i], interpolationFactor);
			}
			UpdateRenderPositions(positions);
		}
	}

	void UpdateRenderPositions(Vector3[] a_Positions)
	{
		if (m_Renderer)
		{
			m_Renderer.positionCount = a_Positions.Length;
			m_Renderer.SetPositions(a_Positions);
		}
	}
}
