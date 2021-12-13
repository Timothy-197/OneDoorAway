using UnityEngine;
using System.Collections;

//class used to define Ziplines, which a character can hang from with the ZiplineModule. Also used to define Skatelines; a variation of a zipline where the character slides on top instead.

public class Zipline : MonoBehaviour {
	[SerializeField] protected Transform m_Anchor1;
	[SerializeField] protected Transform m_Anchor2;
	[SerializeField] protected Transform m_ColPointTransform;
	[SerializeField] bool m_IsSkateLine = false;
	[SerializeField] float m_LetGoDistance = 0.0f;
	void Start()
	{
		UpdateCollider ();
	}
	void FixedUpdate()
	{
		UpdateCollider ();
	}
	//We only want to test against ziplines we're reasonably close to. We create a bounding collider which covers the entire zipline so that we can quickly see if we're close or not
	protected virtual void UpdateCollider()
	{
		Vector3 line = GetLinearLine ();
		transform.position = m_Anchor1.position + line * 0.5f;
		transform.up = GetLinearUpDirection ();
		transform.localScale = new Vector3 (GetLength (), transform.localScale.y, transform.localScale.z);

		m_ColPointTransform.position = transform.position;
	}

	Vector3 GetLinearLine()
	{
		return m_Anchor2.position - m_Anchor1.position;
	}
	Vector3 GetLinearNLine()
	{
		return GetLinearLine().normalized;
	}
	float GetLength()
	{
		return Vector3.Distance (m_Anchor1.position, m_Anchor2.position);
	}
	public virtual Vector3 GetPositionAtUnitLength(float a_Unitlength)
	{
		a_Unitlength = Mathf.Clamp01(a_Unitlength);
		return m_Anchor1.position + GetLinearLine() * a_Unitlength;
	}
	public virtual Vector3 GetUpDirection(Vector3 a_Pos)
    {
		return GetLinearUpDirection();
    }
	Vector3 GetLinearUpDirection()
	{
		Vector3 line = GetLinearNLine ();
		float x = -line.y;
		line.y = line.x;
		line.x = x;
		return line.y > 0.0f ? line : -line;
	}
    Vector3 GetLinearRightDirection()
    {
		Vector3 line =  GetLinearNLine ();
		return line.x > 0.0f ? line : -line;
    }

	public Vector3 GetDownwardDirection(Vector3 a_Pos)
	{
		Vector3 dir = GetLinearRightDirection ();
		return dir.y < 0.0f ? dir : -dir;
	}
	public virtual float GetDistToEnd(Vector3 a_Pos, Vector3 a_Vel)
	{
		Vector3 pos = GetClosestPointOnZipline (a_Pos);

		Vector3 direction_end = Vector3.Dot (a_Vel, GetLinearNLine ()) > 0.0f ? m_Anchor2.position : m_Anchor1.position;
		return Vector3.Distance(direction_end, pos);
	}
	public virtual Vector3 GetTravelVelocity(Vector3 a_Pos, Vector3 a_Vel)
	{
		Vector3 right = GetLinearRightDirection ();
		float dot = Vector3.Dot (right, a_Vel);
		return dot * right;
	}
	public virtual Vector3 GetTravelDirection(Vector3 a_Pos, Vector3 a_Dir)
	{
		Vector3 right = GetLinearRightDirection ();
		float dot = Vector3.Dot (right, a_Dir);
		return (dot > 0.0f) ? right : -right;
	}
		
	public float GetDistToClosestPointOnZipline(Vector3 a_Position)
	{
		Vector3	pos = GetClosestPointOnZipline (a_Position);
		return Vector3.Distance (pos, a_Position);
	}
	//Since this is a straight zipline, simply find the dot product along our line to find the closest point
    public virtual Vector3 GetClosestPointOnZipline(Vector3 a_Position)
    {
		Vector3 line =  GetLinearNLine ();
		Vector3 rel_pos = a_Position - m_Anchor1.position;
        
		float dot = Vector3.Dot(rel_pos, line);
		dot = Mathf.Clamp (dot, 0.0f, GetLength ());

		return m_Anchor1.position + dot * line;
    }
	//Returns a transform which won't rotate, so that if the zipline moves/rotates the capsule won't
	public Transform GetColPointTransform()
	{
		return m_ColPointTransform;
	}
	public bool IsSkateLine()
	{
		return m_IsSkateLine;
	}
	public float GetLetGoDistance()
	{
		return m_LetGoDistance;
	}
}
