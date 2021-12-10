using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Main class definition for ControlledCapsuleCollider.
//Secondary class definitions exist in:
//CapsuleParameters.cs for the parameters of the capsule and its movement
//--------------------------------------------------------------------
//ControlledCapsuleCollider is a class which stores information about the capsule shape, transform and state.
//This includes context information (via CCState) and collision information
//--------------------------------------------------------------------

public partial class ControlledCapsuleCollider : ControlledCollider
{
    List<CapsuleCollisionOccurrance> m_Collisions = new List<CapsuleCollisionOccurrance>();

    CCState m_State = new CCState();

    CapsuleTransform m_CapsuleTransform = new CapsuleTransform();
    void Awake () 
	{
        m_State.Init(this);
        m_PrevLength = m_Length;
        m_CapsuleTransform.SetLength(m_Length, CapsuleResizeMethod.FromCenter);
        m_CapsuleTransform.m_CapsuleCollider = this;
        m_CapsuleTransform.SetPosition(transform.position);
        m_CapsuleTransform.SetUpDirection(transform.up);
        UpdateContextInfo();
        if ((1 << gameObject.layer & m_LayerMask.value) != 0)
        {
            Debug.LogError("ControlledCapsuleCollider Layermask will hit the player itself! Please uncheck the \"" + LayerMask.LayerToName(gameObject.layer) + "\" layer in the ControlledCapsuleCollider collision LayerMask, or switch the player GameObject layer to one not covered by that LayerMask");
        }
    }

    void Update()
    {
        CheckForUpdatedPosition();
    }

    //The character has been moved, either via script or via editor. Adjust position to make up for this
    void CheckForUpdatedPosition()
    {
        if (transform.position != m_CapsuleTransform.GetPosition())
        {
            m_CapsuleTransform.SetPosition(transform.position);
        }
    }
//Main movement update
//Used by CharacterControllerBase to move the collider and update its velocity in the process
//This will collide against geometry and take appropriate action
    public override void UpdateWithVelocity(Vector2 a_Velocity)
    {
#if UNITY_EDITOR
        //If the length has been edited in the inspector since last update, see if the capsule transform needs to be updated
        if (m_Length != m_PrevLength)
        {
            if (m_PrevLength == m_CapsuleTransform.GetLength())
            {
                m_CapsuleTransform.SetLength(m_Length, CapsuleResizeMethod.FromBottom);
            }
            m_PrevLength = m_Length;
        }
#endif
        //The character may have been moved, either via script or via editor. Adjust position to make up for this
        CheckForUpdatedPosition();

        Vector3 realVel = new Vector3(a_Velocity.x, a_Velocity.y, 0) * Time.fixedDeltaTime;
        m_Collisions.Clear();
        if (m_CollisionsActive)
        { 
            TryMove(realVel, ref a_Velocity);
        }
        else
        {
            m_CapsuleTransform.Move(realVel);
        }

        m_PrevVelocity = m_Velocity;
        m_Velocity = a_Velocity;

        UpdateContextInfo();

        //MOVINGCOLPOINT, see CapsuleMovingColliderSolver for more details
        if (IsGrounded())
        {
            AddColPoint(m_State.m_GroundedInfo.GetGroundTransform(), m_State.m_GroundedInfo.GetPoint(), m_State.m_GroundedInfo.GetNormal());
        }
    }
    //Moves the collider with the current update velocity unti it hits something. Resolves the collision with position and velocity, then tries again.
    //Max iteration count to prevent infinite loop
    void TryMove(Vector3 a_Velocity, ref Vector2 o_PureVel)
    {
        for (int iteration = 0; iteration < 15; iteration ++)
        {
            float distanceToTravel = a_Velocity.magnitude + m_MovementCapsuleCastMargin;
            float shortestDistance = distanceToTravel;
            int shortestHitIndex = 0;
            Vector3 direction = a_Velocity.normalized;
            Vector3 margin = -direction * m_MovementCapsuleCastMargin;

            List<RaycastHit> hits = CCState.CorrectCapsuleCastAll(GetDownCenter() + margin, GetUpCenter() + margin, m_Radius, direction, distanceToTravel, m_LayerMask);

            bool blocked = false;
            for (int i = hits.Count - 1; i >= 0; i--)
            {
                float dot = Vector3.Dot(a_Velocity, hits[i].normal);
                if (dot >= 0)//Perpendicular or not blocking (relative velocity?)
                {
                    continue;
                }
                if (hits[i].distance < shortestDistance && hits[i].distance > 0 && hits[i].distance >= m_MovementCapsuleCastMargin * 0.9f)
                {
                    shortestDistance = hits[i].distance;
                    shortestHitIndex = i;
                    blocked = true;
                }
            }
            distanceToTravel = shortestDistance - m_MovementCapsuleCastMargin;
            m_CapsuleTransform.Move(direction * distanceToTravel);
            if (!blocked)
            {
                break;
            }
            else//Reorient, try moving again, correct velocity
            {

                CapsuleCollisionOccurrance collision = new CapsuleCollisionOccurrance();
                collision.m_Point = hits[shortestHitIndex].point;
                collision.m_Normal = hits[shortestHitIndex].normal;
                collision.m_Transform = hits[shortestHitIndex].transform;
                collision.m_IncomingVelocity = a_Velocity;
                collision.m_IncomingVelocityPure = o_PureVel;

                a_Velocity -= direction * distanceToTravel;
                
                Vector3 normal = hits[shortestHitIndex].normal;
                Vector3 aligned = (new Vector3(normal.y, -normal.x, 0)).normalized;
                float alignDot = Vector3.Dot(a_Velocity, aligned);
                a_Velocity = aligned * alignDot;
                Vector2 pureAligned = aligned;
                alignDot = Vector2.Dot(o_PureVel, pureAligned);
                o_PureVel = pureAligned * alignDot;

                collision.m_OutgoingVelocity = a_Velocity;
                collision.m_OutgoingVelocityPure = o_PureVel;
                collision.m_VelocityLoss = collision.m_IncomingVelocity - collision.m_OutgoingVelocity;
                collision.m_VelocityLossPure = collision.m_IncomingVelocityPure - collision.m_OutgoingVelocityPure;
                m_Collisions.Add(collision);
            }
            //Escape when the leftover velocity isn't worthwhile
            if (a_Velocity.magnitude <= m_MinimumViableVelocity)
            {
                break;
            }
        }        
    }

    public override void SetPosition(Vector3 a_Position)
    {
        m_CapsuleTransform.SetPosition(a_Position);
    }
    public override void SetRotation(Quaternion a_Rotation)
    {
        m_CapsuleTransform.SetUpDirection(a_Rotation * Vector3.up);
    }

    //Capsule transform get
    public CapsuleTransform GetCapsuleTransform()
    {
        CheckForUpdatedPosition();
        return m_CapsuleTransform;
    }
    //Get a copy if edits need to be made but not applied to the collider (in case of checking a movement for example)
    public CapsuleTransform GetCapsuleTransformCopy()
    {
        CheckForUpdatedPosition();
        return m_CapsuleTransform.CreateCopy();
    }
    //Apply a capsule transform to the actual collider object. Update contextual information
    public void ApplyCapsuleTransform(CapsuleTransform a_CapsuleTransform)
    {
        m_CapsuleTransform.OverrideValues(a_CapsuleTransform);

        UpdateContextInfo();
    }

    public override void UpdateContextInfo()
    {
        List<RaycastHit> raycastHits = new List<RaycastHit>(Physics.SphereCastAll(m_CapsuleTransform.GetPosition(), m_Radius - m_GroundedMargin, -m_CapsuleTransform.GetUpDirection(), (m_CapsuleTransform.GetLength() * 0.5f) + m_GroundedCheckDistance + m_GroundedMargin, m_LayerMask));
        m_State.UpdateGroundedInfo(raycastHits);
        List<RaycastHit> rightCastResults = CCState.CorrectCapsuleCastAll(GetDownCenter(true), GetUpCenter(true), GetRadius() - m_SideCastMargin, m_CapsuleTransform.GetRightDirection(), m_SideCastDistance + m_SideCastMargin, m_LayerMask);
        List<RaycastHit> leftCastResults = CCState.CorrectCapsuleCastAll(GetDownCenter(true), GetUpCenter(true), GetRadius() - m_SideCastMargin, -m_CapsuleTransform.GetRightDirection(), m_SideCastDistance + m_SideCastMargin, m_LayerMask);
        m_State.UpdateSideCastInfo(leftCastResults, rightCastResults);
        m_State.UpdateEdgeCastInfo();
    }
    public bool CanBeResized(float a_Length, CapsuleResizeMethod a_Method)
    {
        return m_CapsuleTransform.CanBeResized(a_Length, a_Method);
    }

    public void SetLength(float a_Length, CapsuleResizeMethod a_Method, CapsuleResizeMethod a_RestoreMethod = CapsuleResizeMethod.None)
    {
        m_CapsuleTransform.SetLength(a_Length, a_Method, a_RestoreMethod);
    }

    public float GetLength()
    {
        return m_CapsuleTransform.GetLength();
    }

    public override bool CanAlignWithNormal(Vector3 a_Normal, RotateMethod a_Method = RotateMethod.FromBottom)
    {
        return m_CapsuleTransform.CanRotate(a_Normal, a_Method);
    }

    public override void RotateToAlignWithNormal(Vector3 a_Normal, RotateMethod a_Method = RotateMethod.FromBottom)
    {
        if (m_CapsuleTransform.CanRotate(a_Normal, a_Method))
        { 
            m_CapsuleTransform.Rotate(a_Normal, a_Method);
            UpdateContextInfo();
        }
    }

    //Get/Set positional information
    public Vector3 GetUpDirection()
    {
        return m_CapsuleTransform.GetUpDirection();
    }

    public void SetUpCenter(Vector3 a_Position)
    {
        m_CapsuleTransform.SetUpCenter(a_Position);
    }

    public void SetDownCenter(Vector3 a_Position)
    {
        m_CapsuleTransform.SetDownCenter(a_Position);
    }

    public Vector3 GetUpCenter(bool a_UseOriginalLength = false)
    {
        return m_CapsuleTransform.GetUpCenter(a_UseOriginalLength);
    }
    public Vector3 GetDownCenter(bool a_UseOriginalLength = false)
    {
        return m_CapsuleTransform.GetDownCenter(a_UseOriginalLength);
    }
//Get collision information
    public List<CapsuleCollisionOccurrance> GetCollisionInfo()
    {
        return m_Collisions;
    }
//Get grounded, sidecast and edgecast information
    public override bool IsGrounded()
    {
        return m_State.m_GroundedInfo.m_IsGrounded;
    }

    public override CGroundedInfo GetGroundedInfo()
    {
        return m_State.m_GroundedInfo;
    }

    public override bool IsCompletelyTouchingWall()
    {
        return (m_State.m_SideCastInfo.m_WallCastCount >= 3);
    }
    public override bool IsPartiallyTouchingWall()
    {
        return (m_State.m_SideCastInfo.m_WallCastCount >= 2);
    }

    public override CSideCastInfo GetSideCastInfo()
    {
        return m_State.m_SideCastInfo;
    }

    public override bool IsTouchingEdge()
    {
        return m_State.m_EdgeCastInfo.m_HasHitEdge;
    }

    public override CEdgeCastInfo GetEdgeCastInfo()
    {
        return m_State.m_EdgeCastInfo;
    }
//Get/use the movement solver
    public CapsuleMovingColliderSolver GetMovementSolver()
    {
        return m_CapsuleMovingColliderSolver;
    }

    public override void AddColPoint(Transform a_Parent, Vector3 a_Point, Vector3 a_Normal)
    {
        if (m_CapsuleMovingColliderSolver != null)
        {
            m_CapsuleMovingColliderSolver.AddColPoint(a_Parent, a_Point, a_Normal);
        }
    }
    public override void ClearColPoints()
    {
        if (m_CapsuleMovingColliderSolver != null)
        {
            m_CapsuleMovingColliderSolver.ClearColPoints();
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            if (m_CapsuleTransform.GetLength() != m_Length)
            m_CapsuleTransform.SetLength(m_Length);
            m_CapsuleTransform.m_CapsuleCollider = this;
            if (m_CapsuleTransform.GetPosition() != transform.position)
            m_CapsuleTransform.SetPosition(transform.position);
            if (m_CapsuleTransform.GetUpDirection() != transform.up)
            m_CapsuleTransform.SetUpDirection(transform.up);
        }
        Color preColor = Gizmos.color;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(GetUpCenter(), m_Radius);
        Gizmos.DrawWireSphere(GetDownCenter(), m_Radius);
        Gizmos.DrawLine(GetUpCenter() + transform.right * m_Radius, GetDownCenter() + transform.right * m_Radius);
        Gizmos.DrawLine(GetUpCenter() - transform.right * m_Radius, GetDownCenter() - transform.right * m_Radius);
        Gizmos.color = preColor;
    }
}
