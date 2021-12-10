using UnityEngine;
using System.Collections;
//WARNING
//--------------------------------------------------------------------
//Is going to be deprecated and replaced completely by NetClimbModule, since Net objects can be used to create ladders
//--------------------------------------------------------------------
//--------------------------------------------------------------------
//LadderClimbModule is a movement module related to the Ladder objects.
//This ability allows players to climb on Ladder objects
//--------------------------------------------------------------------
public class LadderClimbModule : GroundedControllerAbilityModule
{
    [SerializeField] float m_LadderReachRadius = 0.0f;
    [SerializeField] float m_Speed = 0.0f;
    [SerializeField] LayerMask m_LadderMask = 0;
    [SerializeField] float m_LadderClimbCoolDown = 0.0f;
    [SerializeField] bool m_CanPassThroughOneWayPlatforms = false;
    [SerializeField] bool m_CanExitTopOfLadder = false;
    [SerializeField] bool m_CanExitSidesOfLadder = false;
    [SerializeField] bool m_CanExitBottomOfLadder = false;
    [SerializeField] bool m_CanReleaseLadderWithButton = false;
    [SerializeField] bool m_CanJumpFromLadder = false;
    [SerializeField] float m_VerticalJumpVelocity = 0.0f;
    [SerializeField] float m_HorizontalJumpVelocity = 0.0f;
    [SerializeField] bool m_UseNormalVerticalJumpVelocity = false;

    float m_LastLadderClimbTime;

    //Reset all state when this module gets initialized
    protected override void ResetState()
    {
        base.ResetState();
        m_LastLadderClimbTime = 0.0f;
    }

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl()
    {
        if (m_CanPassThroughOneWayPlatforms)
        {
            TryDisablingPlatforms();
        }
        Ladder ladder = FindLadder();
        if (ladder)
        {
            Vector3 newPoint = ladder.GetClosestPointOnLadder(m_ControlledCollider.GetCapsuleTransform().GetPosition());
            m_ControlledCollider.GetCapsuleTransform().SetPosition(newPoint);
        }
    }

    //Called whenever this module is ended (was active, now is inactive)
    protected override void EndModuleImpl()
    {
        m_LastLadderClimbTime = Time.time;
    }

    //Move up, down ladder
    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule()
    {
        if (m_CanPassThroughOneWayPlatforms)
        {
            TryDisablingPlatforms();
        }
        if (m_CanJumpFromLadder)
        {
            if (TryLadderJump())
            {
                EndModule();
                return;
            }
        }
        if (m_CanReleaseLadderWithButton)
        {
            if (DoesInputExist("ClimbRelease") && GetButtonInput("ClimbRelease").m_WasJustPressed)
            {
                m_ControlledCollider.SetVelocity(Vector2.zero);
                EndModule();
                return;
            }
        }

        Ladder ladder = FindLadder();
        if (ladder == null)
        {
            return;
        }
        Vector2 newVel = Vector2.zero;

        if (GetDirInput("Move").m_Direction == DirectionInput.Direction.Down || GetDirInput("Move").m_Direction == DirectionInput.Direction.Up)
        {
            newVel = ladder.GetUpDirection() * GetDirInput("Move").m_ClampedInput.y * m_Speed;
        }
        m_ControlledCollider.UpdateWithVelocity(newVel);
        Vector3 position = m_ControlledCollider.GetCapsuleTransform().GetPosition();
        if (!m_CanExitTopOfLadder)
        {
            if (ladder.HasExceededUpperBound(position))
            {
                Vector3 newPosition = ladder.GetClosestPointOnLadder(position);
                m_ControlledCollider.GetCapsuleTransform().SetPosition(newPosition);
                m_ControlledCollider.UpdateContextInfo();
            }
        }
        if (!m_CanExitBottomOfLadder)
        { 
            if (ladder.HasExceededLowerBound(position))
            {
                Vector3 newPosition = ladder.GetClosestPointOnLadder(position);
                m_ControlledCollider.GetCapsuleTransform().SetPosition(newPosition);
                m_ControlledCollider.UpdateContextInfo();
            }
        }
    }

    //Called to place a MovingColPoint to support moving colliders
    public override void PlaceMovingColPoint()
    {
        //MOVINGCOLPOINT, see CapsuleMovingColliderSolver for more details
        Ladder ladder = FindLadder();
        if (ladder)
        { 
            m_ControlledCollider.AddColPoint(ladder.transform, m_ControlledCollider.GetCapsuleTransform().GetPosition(), ladder.GetUpDirection());
        }
    }

    //Character needs to be touching a Ladder object
    public override bool IsApplicable()
    {
        if (!m_IsActive && Time.time - m_LastLadderClimbTime < m_LadderClimbCoolDown)
        {
            return false;
        }
        if (m_ControlledCollider.IsGrounded() && GetDirInput("Move").m_Direction != DirectionInput.Direction.Up)
        {
            if (!m_CanPassThroughOneWayPlatforms || !m_ControlledCollider.GetGroundedInfo().GetGroundTransform().GetComponentInChildren<OneWayPlatform>())
            {
                return false;
            }
        }
        Ladder ladder = FindLadder();
        if (ladder)
        {
            if (!m_IsActive)
            { 
                if (GetDirInput("Move").m_Direction == DirectionInput.Direction.Down || GetDirInput("Move").m_Direction == DirectionInput.Direction.Up)
                { 
                    Vector3 newPoint = ladder.GetClosestPointOnLadder(m_ControlledCollider.GetCapsuleTransform().GetPosition());
                    if (m_ControlledCollider.GetCapsuleTransform().CanMove(newPoint - m_ControlledCollider.GetCapsuleTransform().GetPosition()))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (m_CanExitSidesOfLadder && (GetDirInput("Move").m_Direction == DirectionInput.Direction.Left || GetDirInput("Move").m_Direction == DirectionInput.Direction.Right))
                {
                    return false;
                }
                return true;
            }
        }
        return false;
    }

    Ladder FindLadder()
    {
        Collider[] results = Physics.OverlapSphere(m_CharacterController.transform.position, m_LadderReachRadius, m_LadderMask, QueryTriggerInteraction.Collide);

        if (results.Length > 0)
        {
            for (int i = 0; i < results.Length; i++)
            {
                Ladder ladder = results[i].GetComponent<Ladder>();
                if (ladder != null)
                {
                    return ladder;
                }
            }
        }
        return null;
    }

    void TryDisablingPlatforms()
    {
        if (m_ControlledCollider.IsGrounded() && m_CanPassThroughOneWayPlatforms && GetDirInput("Move").m_Direction == DirectionInput.Direction.Down)
        {
            OneWayPlatform oneWayPlatform = m_ControlledCollider.GetGroundedInfo().GetGroundTransform().GetComponentInChildren<OneWayPlatform>();
            if (oneWayPlatform)
            {
                oneWayPlatform.DisableForObject(m_CharacterController.GetComponent<Collider>());
            }
        }
    }

    bool TryLadderJump()
    {
        if (m_CharacterController.DidJustJump())
        {
            return false;
        }

        if ((m_CharacterController.GetJumpIsCached()))
        {
            float jumpVelocity = m_VerticalJumpVelocity;
            if (m_UseNormalVerticalJumpVelocity)
            {
                jumpVelocity = m_CharacterController.GetJumpVelocity();
            }
            Vector2 newVelocity = Vector2.up * jumpVelocity + GetDirInput("Move").m_ClampedInput.x * m_HorizontalJumpVelocity * Vector2.right;

            m_CharacterController.Jump(newVelocity);
            m_ControlledCollider.UpdateWithVelocity(m_ControlledCollider.GetVelocity());
            return true;
        }
        return false;
    }

    //Get the name of the animation state that should be playing for this module. 
    public override string GetSpriteState()
    {
        if (Mathf.Abs(GetDirInput("Move").m_ClampedInput.y) >= 0.05f)
        {
            return "Climb";
        }
        else
        {
            return "ClimbIdle";
        }
    }
}
