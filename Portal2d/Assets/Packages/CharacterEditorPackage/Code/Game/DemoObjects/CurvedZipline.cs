using UnityEngine;
using System.Collections;

//class used to define curved Ziplines, which differ from regular ziplines in that they can add a curve which impacts how the character traverses it
public class CurvedZipline : Zipline {
	[SerializeField] Transform m_BP1 = null;
    [SerializeField] Transform m_BP2 = null;

    //We only want to test against ziplines we're reasonably close to. 
    //We create a bounding collider which covers the entire zipline so that we can quickly see if we're close or not
    //Since the curved ziplines are created with a bezier curve, we know for a fact that if all points of the curve are inside the box, the entire curve will be as well
    protected override void UpdateCollider()
	{
		Vector3 min, max;
		min = m_Anchor1.position;
		max = m_Anchor1.position;
		min.x = Mathf.Min(Mathf.Min(m_Anchor1.position.x, m_Anchor2.position.x), Mathf.Min(m_BP1.position.x, m_BP2.position.x));
		min.y = Mathf.Min(Mathf.Min(m_Anchor1.position.y, m_Anchor2.position.y), Mathf.Min(m_BP1.position.y, m_BP2.position.y));
		min.z = Mathf.Min(Mathf.Min(m_Anchor1.position.z, m_Anchor2.position.z), Mathf.Min(m_BP1.position.z, m_BP2.position.z));
		max.x = Mathf.Max(Mathf.Max(m_Anchor1.position.x, m_Anchor2.position.x), Mathf.Max(m_BP1.position.x, m_BP2.position.x));
		max.y = Mathf.Max(Mathf.Max(m_Anchor1.position.y, m_Anchor2.position.y), Mathf.Max(m_BP1.position.y, m_BP2.position.y));
		max.z = Mathf.Max(Mathf.Max(m_Anchor1.position.z, m_Anchor2.position.z), Mathf.Max(m_BP1.position.z, m_BP2.position.z));

		Vector3 line = max - min;
		transform.position = min + line * 0.5f;
		transform.up = Vector3.up;
		transform.localScale = new Vector3 (line.x + 0.5f, line.y + 0.5f, transform.localScale.z);
		m_ColPointTransform.position = transform.position;
	}

	public override Vector3 GetPositionAtUnitLength(float a_Unitlength)
	{
		//Simple cubic Bezier curve from wikipedia serves to create our curve
		float t = Mathf.Clamp01(a_Unitlength);
		Vector3 P0 = m_Anchor1.position;
		Vector3 P1 = m_BP1.position;
		Vector3 P2 = m_BP2.position;
		Vector3 P3 = m_Anchor2.position;

		Vector3 pos = Mathf.Pow(1.0f-t, 3.0f)*P0 + 3.0f*Mathf.Pow(1.0f-t,2.0f)*t*P1 + 3.0f*(1.0f-t)*Mathf.Pow(t, 2.0f)*P2 + Mathf.Pow(t, 3.0f)*P3;

		return pos;
	}
	public override Vector3 GetUpDirection(Vector3 a_Pos)
    {
		float unit_pos = GetUnitPositionOnZipline(a_Pos);

		Vector3 leftpos = GetPositionAtUnitLength(unit_pos -0.01f );
		Vector3 rightpos = GetPositionAtUnitLength(unit_pos +0.01f );

		Vector3 line = (rightpos-leftpos).normalized;
		float x = -line.y;
		line.y = line.x;
		line.x = x;
		return line.y > 0.0f ? line : -line;
    }
	public override float GetDistToEnd(Vector3 a_Pos, Vector3 a_Vel)
	{

		float unit_pos = GetUnitPositionOnZipline(a_Pos);

		Vector3 leftpos = GetPositionAtUnitLength(unit_pos -0.01f );
		Vector3 rightpos = GetPositionAtUnitLength(unit_pos +0.01f );

		Vector3 right = (rightpos-leftpos).normalized;
		float dot = Vector3.Dot (right, a_Vel);

		Vector3 direction_end = dot > 0.0f ? m_Anchor2.position : m_Anchor1.position;

		Vector3 pos = GetClosestPointOnZipline (a_Pos);
		return Vector3.Distance(direction_end, pos);
	}
	public override Vector3 GetTravelVelocity(Vector3 a_Pos, Vector3 a_Vel)
	{
		float unit_pos = GetUnitPositionOnZipline(a_Pos);

		Vector3 leftpos = GetPositionAtUnitLength(unit_pos -0.01f );
		Vector3 rightpos = GetPositionAtUnitLength(unit_pos +0.01f );

		Vector3 right = (rightpos-leftpos).normalized;
		float dot = Vector3.Dot (right, a_Vel);
		return dot * right;
	}
	public override Vector3 GetTravelDirection(Vector3 a_Pos, Vector3 a_Dir)
	{
		float unit_pos = GetUnitPositionOnZipline(a_Pos);

		Vector3 leftpos = GetPositionAtUnitLength(unit_pos-0.01f );
		Vector3 rightpos = GetPositionAtUnitLength(unit_pos+0.01f );

		Vector3 right = (rightpos-leftpos).normalized;
		float dot = Vector3.Dot (right, a_Dir);
		return (dot > 0.0f) ? right : -right;
	}
	float GetUnitPositionOnZipline(Vector3 a_Pos)
	{
		float max_error = 0.001f;
		float step = 0.05f;
		return StepBezier(a_Pos, 0.0f, 1.0f, step, max_error,0);

	}

	//Recursive function to find the closest position (as a factor 0-1 of our curve) to a given position
	//First we step through the curve from beginning to end and find the closest point we stepped past
	//Then we step around the closest point we found with smaller steps to increase the accuracy
	//This continues until we're "close enough" (distance smaller than max_error) or until we think we stepped too often (iterations)
	float StepBezier(Vector3 pos, float start, float end, float step, float max_error, int iterations)
	{
		start = Mathf.Clamp01(start);
		end = Mathf.Clamp01(end);
		float best = start;
		float current_best_dist = 1000.0f;
		for (float t = start; t <= end; t += step)
		{
			Vector3 line_pos = GetPositionAtUnitLength(t);
			float dist = Vector3.Distance(pos, line_pos);
			if (dist < current_best_dist)
			{
				best = t;
				current_best_dist = dist;
			}
		}
		if (current_best_dist <= max_error || iterations >= 3)
		{
			return best;
		}
		else
		{
			return StepBezier(pos, best-step,best+step, step * 0.1f, max_error, iterations+1);
		}
	}
	public override Vector3 GetClosestPointOnZipline(Vector3 a_Pos)
	{
		return GetPositionAtUnitLength(GetUnitPositionOnZipline(a_Pos));
	}
}
