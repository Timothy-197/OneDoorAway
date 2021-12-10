using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//ZiplineModule is a movement module related to the Zipline objects.
//This ability allows players to make use of ziplines
//--------------------------------------------------------------------
public class ZiplineModule : GroundedControllerAbilityModule
{
    [SerializeField] float m_ZiplineReachRadius = 0.0f;
    [SerializeField] float m_MaxSpeed = 0.0f;
	[SerializeField] float m_DownhillAcceleration = 0.0f;
	[SerializeField] float m_UphillAcceleration = 0.0f;
	[SerializeField] float m_Deceleration = 0.0f;
    [SerializeField] LayerMask m_ZiplineMask = 0;
    [SerializeField] float m_ZiplineClimbCoolDown = 0.0f;
	[SerializeField] bool m_CanReleaseZiplineWithInput = false;
    [SerializeField] bool m_CanJumpFromZipline = false;

    float m_LastZiplineClimbTime;
	bool m_IsSkating;
	Zipline m_CurrentZipline;

    //Reset all state when this module gets initialized
    protected override void ResetState()
    {
        base.ResetState();
        m_LastZiplineClimbTime = 0.0f;
		m_IsSkating = false;
		m_CurrentZipline= null;
    }

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl()
    {
        Zipline zipline = FindZipline();
        if (zipline)
        {
			//Find our zipline, attach ourselves to it from the bottom or top depending on whether or not it is marked as a "skateline"
			m_CurrentZipline = zipline;
			m_IsSkating = zipline.IsSkateLine();
			Vector3 newPoint = zipline.GetClosestPointOnZipline(GetAttachPoint());
			SetAttachPoint(newPoint);
        }
    }

    //Called whenever this module is ended (was active, now is inactive)
    protected override void EndModuleImpl()
    {
        m_LastZiplineClimbTime = Time.time;
		m_CurrentZipline = null;
    }

    //Move along Zipline
    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule()
    {
        if (m_CanJumpFromZipline)
        {
            if (TryZiplineJump())
            {
                EndModule();
                return;
            }
        }

		if (m_CurrentZipline == null)
        {
            return;
        }

		Vector3 travelSpeed = m_CurrentZipline.GetTravelVelocity (GetAttachPoint(), m_ControlledCollider.GetVelocity ());
		Vector3 dir = m_CurrentZipline.GetTravelDirection (GetAttachPoint(), travelSpeed);
		Vector3 acceleration;
		if (travelSpeed.magnitude < m_MaxSpeed) 
		{
			acceleration = dir * m_DownhillAcceleration * Time.fixedDeltaTime;
			if (dir.y > 0.0f)  //We're going uphill, so use our uphillacceleration (usually negative so that we start sliding down)
			{
				acceleration = dir * m_UphillAcceleration * Time.fixedDeltaTime;
			}
		}
		else//We're going too fast, so let's slow down
		{
			acceleration = -dir * m_Deceleration * Time.fixedDeltaTime;
		}
		travelSpeed += acceleration;
		if (!m_ControlledCollider.GetCapsuleTransform ().CanMove (travelSpeed * Time.fixedDeltaTime))
		{
			//We're probably being blocked by something, reset our forward velocity
			travelSpeed *= 0.0f;
		}

		m_ControlledCollider.UpdateWithVelocity(travelSpeed);

		//Clamp to the Zipline after we've moved
		Vector3 position = GetAttachPoint();   
		Vector3 newPosition = m_CurrentZipline.GetClosestPointOnZipline(position);
		SetAttachPoint(newPosition);
        m_ControlledCollider.UpdateContextInfo();
    }

    //Called to place a MovingColPoint to support moving colliders
	//If our zipline is moving, then we might get an issue where it moves too far from us
	//This helps us stay on top of it
    public override void PlaceMovingColPoint()
    {
        //MOVINGCOLPOINT, see CapsuleMovingColliderSolver for more details
		if (m_CurrentZipline)
        { 
			m_ControlledCollider.AddColPoint(m_CurrentZipline.GetColPointTransform(), GetAttachPoint (), Vector3.up);
        }
    }

    //Character needs to be touching a Zipline object
    public override bool IsApplicable()
	{
		if (!m_IsActive && ((Time.time - m_LastZiplineClimbTime < m_ZiplineClimbCoolDown) || m_ControlledCollider.GetVelocity().y > 0.0f))
        {
            return false;
        }
		if (m_CanReleaseZiplineWithInput)
		{
			if ((DoesInputExist("ClimbRelease") && GetButtonInput("ClimbRelease").m_IsPressed) ||
				GetDirInput("Move").m_Direction == DirectionInput.Direction.Down)
			{
				return false;
			}
		}
		if (m_ControlledCollider.IsGrounded ()) 
		{
			return false;
		}
		Zipline zipline = null;
		if (m_IsActive)
			zipline = m_CurrentZipline;
		else
			zipline = FindZipline();
        if (zipline)
        {
            if (!m_IsActive)
            { 
				m_IsSkating = zipline.IsSkateLine();
				Vector3 newPoint = zipline.GetClosestPointOnZipline(GetAttachPoint ());

				Vector3 diff = newPoint - GetAttachPoint ();
				if (m_ControlledCollider.GetCapsuleTransform ().CanMove (diff)) 
				{
					return true;
				}
            }
            else
            {
				//We might have been blocked from attaching too closely to the zipline. Let's make sure that we can actually attach
				if (zipline.GetDistToClosestPointOnZipline(GetAttachPoint()) > m_ZiplineReachRadius)
				{
					return false;
				}
				Vector3 travelSpeed = zipline.GetTravelVelocity (GetAttachPoint(), m_ControlledCollider.GetVelocity ());
				if (zipline.GetDistToEnd (GetAttachPoint (), travelSpeed) < zipline.GetLetGoDistance()) 
				{
					return false;
				}
                return true;
            }
        }
        return false;
    }

    Zipline FindZipline()
    {
		Zipline best = null;
		float best_dist = m_ZiplineReachRadius;

		//Check for ziplines first (lines we hang from), so check around the top of our capsule
		Vector3 point = m_ControlledCollider.GetUpCenter();
		Collider[] results = Physics.OverlapSphere(point, m_ZiplineReachRadius * 2.0f, m_ZiplineMask, QueryTriggerInteraction.Collide);
        if (results.Length > 0)
        {
            for (int i = 0; i < results.Length; i++)
            {
                Zipline zipline = results[i].GetComponent<Zipline>();
				if (zipline != null && !zipline.IsSkateLine())
                {
					float dist = zipline.GetDistToClosestPointOnZipline (point);
					if (dist <= best_dist)
					{
						best_dist = dist;
						best = zipline;
					}
                }
            }
        }
		//Now check for skatelines (lines we stand on top of), so check around the bottom of our capsule
		point = m_ControlledCollider.GetDownCenter();
		results = Physics.OverlapSphere(point, m_ZiplineReachRadius * 2.0f, m_ZiplineMask, QueryTriggerInteraction.Collide);
		if (results.Length > 0)
		{
			for (int i = 0; i < results.Length; i++)
			{
				Zipline zipline = results[i].GetComponent<Zipline>();
				if (zipline != null && zipline.IsSkateLine())
				{
					float dist = zipline.GetDistToClosestPointOnZipline (point);
					if (dist <= best_dist)
					{
						best_dist = dist;
						best = zipline;
					}
				}
			}
		}
        return best;
    }
    bool TryZiplineJump()
    {
        if (m_CharacterController.DidJustJump())
        {
            return false;
        }

        if ((m_CharacterController.GetJumpIsCached()))
        {
            float jumpVelocity  = m_CharacterController.GetJumpVelocity();
			Vector2 newVelocity = Vector2.up * jumpVelocity + m_ControlledCollider.GetVelocity ().x * Vector2.right;

            m_CharacterController.Jump(newVelocity);
            m_ControlledCollider.UpdateWithVelocity(m_ControlledCollider.GetVelocity());
            return true;
        }
        return false;
    }

	Vector3 GetAttachPoint()
	{
		if (m_IsSkating)
		{
			return m_ControlledCollider.GetDownCenter();
		}
		else
		{
			return m_ControlledCollider.GetUpCenter ();
		}
	}
	void SetAttachPoint(Vector3 point)
	{
		if (m_IsSkating)
		{
			m_ControlledCollider.SetDownCenter(point);
		}
		else
		{
			m_ControlledCollider.SetUpCenter(point);
		}
	}

    //Get the name of the animation state that should be playing for this module. 
    public override string GetSpriteState()
    {
		if (m_IsSkating) 
		{
			return "Skateline";
		} 
		else
		{
			return "Zipline";
		}
    }

	//We're going to follow the up of our current zipline
	public override Vector2 GetCurrentVisualUp()
	{
		if (m_CurrentZipline != null)
		{
			return (Vector2)m_CurrentZipline.GetUpDirection(GetAttachPoint());
		}
		return Vector2.up;
	}
}
