using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//NetClimbModule is a movement module related to the Net objects.
//This ability allows players to climb on Net objects
//--------------------------------------------------------------------
public class NetClimbModule : GroundedControllerAbilityModule
{
    [SerializeField] float m_NetReachRadius = 0.0f;
    [SerializeField] float m_Speed = 0.0f;
    [SerializeField] LayerMask m_NetMask = 0;
    [SerializeField] float m_NetClimbCoolDown = 0.0f;
    [SerializeField] bool m_CanPassThroughOneWayPlatforms = false;
    [SerializeField] bool m_CanExitTopOfNet = false;
    [SerializeField] bool m_CanExitSidesOfNet = false;
    [SerializeField] bool m_CanExitBottomOfNet = false;
    [SerializeField] bool m_CanReleaseNetWithButton = false;
    [SerializeField] bool m_CanJumpFromNet = false;
    [SerializeField] float m_VerticalJumpVelocity = 0.0f;
    [SerializeField] float m_HorizontalJumpVelocity = 0.0f;
    [SerializeField] bool m_UseNormalVerticalJumpVelocity = false;

    float m_LastNetClimbTime;

    //Reset all state when this module gets initialized
    protected override void ResetState()
    {
        base.ResetState();
        m_LastNetClimbTime = 0.0f;
    }

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl()
    {
        if (m_CanPassThroughOneWayPlatforms)
        {
            TryDisablingPlatforms();
        }
        Net Net = FindNet();
        if (Net)
        {
            Vector3 newPoint = Net.GetClosestPointOnNet(m_ControlledCollider.GetCapsuleTransform().GetPosition());
            m_ControlledCollider.GetCapsuleTransform().SetPosition(newPoint);
        }
    }

    //Called whenever this module is ended (was active, now is inactive)
    protected override void EndModuleImpl()
    {
        m_LastNetClimbTime = Time.time;
    }

    //Move up, down Net
    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule()
    {
        if (m_CanJumpFromNet)
        {
            if (TryNetJump())
            {
                EndModule();
                return;
            }
        }
        if (m_CanReleaseNetWithButton)
        {
            if (DoesInputExist("ClimbRelease") && GetButtonInput("ClimbRelease").m_WasJustPressed)
            {
                m_ControlledCollider.SetVelocity(Vector2.zero);
                EndModule();
                return;
            }
        }

        Net Net = FindNet();
        if (Net == null)
        {
            return;
        }
        if (m_CanPassThroughOneWayPlatforms)
        {
            TryDisablingPlatforms();
        }
        Vector2 newVel = GetDirInput("Move").m_ClampedInput * m_Speed;
        m_ControlledCollider.UpdateWithVelocity(newVel);

        //See if we moved past the bounds of any side of the net. Do we have to adjust?
        bool clampPosition = true;
        Vector3 position = m_ControlledCollider.GetCapsuleTransform().GetPosition();
        if (m_CanExitTopOfNet)
        {
            if (Net.HasExceededUpperBound(position))
            {
                clampPosition = false;
            }
        }
        if (m_CanExitBottomOfNet)
        { 
            if (Net.HasExceededLowerBound(position))
            {
                clampPosition = false;
            }
        }
        if (m_CanExitSidesOfNet)
        {
            if (Net.HasExceededHorizontalBounds(position))
            {
                clampPosition = false;
            }
        }
        if (clampPosition)
        { 
            //Clamp to the net.
            Vector3 newPosition = Net.GetClosestPointOnNet(position);
            m_ControlledCollider.GetCapsuleTransform().SetPosition(newPosition);
            m_ControlledCollider.UpdateContextInfo();
        }
    }

    //Called to place a MovingColPoint to support moving colliders
    public override void PlaceMovingColPoint()
    {
        //MOVINGCOLPOINT, see CapsuleMovingColliderSolver for more details
        Net Net = FindNet();
        if (Net)
        { 
            m_ControlledCollider.AddColPoint(Net.transform, m_ControlledCollider.GetCapsuleTransform().GetPosition(), Net.GetUpDirection());
        }
    }

    //Character needs to be touching a Net object
    public override bool IsApplicable()
    {
        if (!m_IsActive && Time.time - m_LastNetClimbTime < m_NetClimbCoolDown)
        {
            return false;
        }
        if (m_ControlledCollider.IsGrounded() && GetDirInput("Move").m_Direction == DirectionInput.Direction.Down)
        {
            if (!m_CanPassThroughOneWayPlatforms || !m_ControlledCollider.GetGroundedInfo().GetGroundTransform().GetComponentInChildren<OneWayPlatform>())
            {
                return false;
            }
        }
        Net Net = FindNet();
        if (Net)
        {
            if (!m_IsActive)
            { 
                if (GetDirInput("Move").m_Direction == DirectionInput.Direction.Down || GetDirInput("Move").m_Direction == DirectionInput.Direction.Up)
                { 
                    Vector3 newPoint = Net.GetClosestPointOnNet(m_ControlledCollider.GetCapsuleTransform().GetPosition());
                    if (m_ControlledCollider.GetCapsuleTransform().CanMove(newPoint - m_ControlledCollider.GetCapsuleTransform().GetPosition()))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    Net FindNet()
    {
        Collider[] results = Physics.OverlapSphere(m_CharacterController.transform.position, m_NetReachRadius, m_NetMask, QueryTriggerInteraction.Collide);

        if (results.Length > 0)
        {
            for (int i = 0; i < results.Length; i++)
            {
                Net Net = results[i].GetComponent<Net>();
                if (Net != null)
                {
                    return Net;
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

    bool TryNetJump()
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
        if (Mathf.Abs(GetDirInput("Move").m_ClampedInput.magnitude) >= 0.05f)
        {
            return "Climb";
        }
        else
        {
            return "ClimbIdle";
        }
    }
}
