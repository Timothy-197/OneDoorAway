using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//EdgeClimb module is an animated, grounded ability
//When a ledge is detected, the module checks if the player wants to get up there (via input)
//If so, it tries to create a path which moves the player over the ledge and checks if it's valid.
//Then it follows GroundedAnimatedAbility's fixedupdate
//--------------------------------------------------------------------
public class EdgeClimbModule : GroundedAnimatedAbilityModule {
    [SerializeField] float m_MoveUpTime = 0.0f;
    [SerializeField] float m_MoveSideTime = 0.0f;
    [SerializeField] float m_MoveUpMargin = 0.0f;
    [SerializeField] float m_MoveSideDistance = 0.0f;

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl(){
        base.StartModuleImpl();
        //MOVINGCOLPOINT, see GroundedAnimatedAbilityModules for more details
        CEdgeCastInfo info = m_ControlledCollider.GetEdgeCastInfo();

        Transform transform = new GameObject().transform;
        transform.position = info.GetEdgePoint();
        transform.parent = info.GetEdgeTransform();
        if (m_ReferencePoint == null)
        { 
            m_ReferencePoint = new MovingColPoint();
        }
        m_ReferencePoint.m_Transform = transform;
        m_ReferencePoint.m_PrevPoint = transform.position;
        m_ReferencePoint.m_PrevRot = transform.rotation;
        m_ReferencePoint.m_Normal = info.GetEdgeNormal();
        m_ReferencePoint.m_PointRelativeToThis = m_ControlledCollider.GetCapsuleTransform().GetPosition() - info.GetEdgePoint();
    }

    //Called whenever this module is ended (was active, now is inactive)
    protected override void EndModuleImpl(){
        base.EndModuleImpl();
        if (!m_WasInterrupted && (Time.time - m_StartTime >= m_Path.GetTotalTime() || m_Path.IsDone()))
        {
            m_ControlledCollider.SetVelocity(Vector2.zero);
        }
    }
    //Query whether this module can be active, given the current state of the character controller (velocity, isGrounded etc.)
    //Called every frame when inactive (to see if it could be) and when active (to see if it should not be)
    public override bool IsApplicable(){
        if (!m_IsActive)
        {
            //Character can't touch ground, has to touch edge and might be disabled if it is moving upwards
            if (m_ControlledCollider.IsGrounded())
            {
                return false;
            }   
            if (!m_ControlledCollider.IsTouchingEdge())
            {
                return false;
            }
            CEdgeCastInfo info = m_ControlledCollider.GetEdgeCastInfo();
            if (GetDirInput("Move").m_Direction == DirectionInput.Direction.Up || GetDirInput("Move").IsInThisDirection(-info.GetWallNormal()))
            { 
                float angle = Vector3.Angle(info.GetEdgeNormal(), Vector3.up);
                if (angle >= m_ControlledCollider.GetMaxGroundedAngle())
                {
                    return false;
                }

                GeneratePath();
                if (m_Path.IsPossible(m_ControlledCollider.GetCapsuleTransform()))
                {
                    return true;
                }
            }
        }
        else
        {
            //If the referencepoint slope is too steep to cling on to (during motion), interrupt movement
            float angle = Vector3.Angle(m_ReferencePoint.m_Transform.up, Vector3.up);
            if (angle >= m_ControlledCollider.GetMaxGroundedAngle())
            {
                return false;
            }
            if (m_WasInterrupted)
            {
                return false;
            }
            if (Time.time - m_StartTime >= m_Path.GetTotalTime() || m_Path.IsDone())
            {
                return false;
            }
            return true;
        }
        return false;
    }

    protected override void GeneratePath()
    {
        CEdgeCastInfo info = m_ControlledCollider.GetEdgeCastInfo();
        m_Path.Clear();
        CapsuleTransform copy = m_ControlledCollider.GetCapsuleTransformCopy();

        //First node is in edgehang alignment, moving away from the edge mildly
        CapsuleMovementPathNode newNode = m_Path.CreateFirstNode(copy);
        Vector3 upCenter = info.GetProposedHeadPoint();
        upCenter += m_ControlledCollider.GetEdgeCastInfo().GetWallNormal() * 0.03f;
        copy.SetUpCenter(upCenter);
        copy.Rotate(info.GetUpDirection(), RotateMethod.FromTop);
        newNode = m_Path.DuplicateAndAddLastNode();
        newNode.CopyFromTransform(copy);

        //Second node moves up along local up, until the bottom can slide over the edge
        newNode = m_Path.DuplicateAndAddLastNode();
        newNode.m_Duration = m_MoveUpTime;
        float contactDot = Vector3.Dot(info.GetEdgeNormal(), info.GetEdgePoint());
        float bottomDot = Vector3.Dot(info.GetEdgeNormal(), copy.GetDownCenter()) - m_ControlledCollider.GetRadius() - m_MoveUpMargin;
        float normalDot = Vector3.Dot(info.GetEdgeNormal(), copy.GetUpDirection());
        float rawDistance = contactDot - bottomDot;
        float distance = rawDistance / normalDot;
        newNode.m_Position += copy.GetUpDirection() * distance;
        newNode.ApplyEntireMovement(copy);
        //Third node snaps the capsule to the "upright" position for the ground it's going to stand on
        newNode = m_Path.DuplicateAndAddLastNode();
        newNode.m_Duration = 0.0f;
        newNode.m_UpDirection = (m_CharacterController.GetAlignsToGround()) ? info.GetEdgeNormal() : Vector3.up;
        newNode.m_RotateMethod = RotateMethod.FromBottom;
        newNode.ApplyEntireMovement(copy);
        newNode.m_Position = copy.GetPosition();
        //Final node moves the capsule over the ground
        newNode = m_Path.DuplicateAndAddLastNode();
        newNode.m_Duration = m_MoveSideTime;
        Vector3 direction = CState.GetDirectionAlongNormal(-info.GetWallNormal(), info.GetEdgeNormal());
        newNode.m_Position += direction * m_MoveSideDistance;
        newNode.ApplyEntireMovement(copy);
    }

    //Get the name of the animation state that should be playing for this module. 
    public override string GetSpriteState(){
        if (Time.time - m_StartTime < m_MoveUpTime)
        {
            return "WallRun";
        }
        else
        {
            return "Run";
        }
    }
}
