using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//A GroundedAnimatedAbilityModule is a special kind of AbilityModule
//It describes a motion over time, using a CapsuleMovementPath
//The EdgeClimb and EdgeVault modules are derived from this
//--------------------------------------------------------------------
public class GroundedAnimatedAbilityModule : GroundedControllerAbilityModule {

    protected CapsuleMovementPath m_Path = new CapsuleMovementPath();
    protected float m_StartTime;
    protected float m_TransitionStartTime;
    protected bool m_WasInterrupted;
    protected MovingColPoint m_ReferencePoint;

    //Reset all state when this module gets initialized
    protected override void ResetState(){
        base.ResetState();
        m_Path.Clear();
        m_StartTime = Time.time;
        m_TransitionStartTime = Time.time;
        m_WasInterrupted = false;
    }

    //Called whenever this module is started (was inactive, now is active)
    protected override void StartModuleImpl(){
        m_StartTime = Time.time;
        m_TransitionStartTime = Time.time;
        m_WasInterrupted = false;
        m_ControlledCollider.SetVelocity(Vector2.zero);
    }

    //Called whenever this module is ended (was active, now is inactive)
    protected override void EndModuleImpl(){
        if (Time.time - m_StartTime >= m_Path.GetTotalTime() || m_Path.IsDone())
        {
            if (m_ReferencePoint != null)
            { 
                //Position
                Vector3 diff = m_ReferencePoint.m_Transform.position - m_ReferencePoint.m_PrevPoint;
                m_Path.Move(diff);

                //Rotation
                Quaternion rotationDifference = Quaternion.FromToRotation(m_ReferencePoint.m_PrevRot * Vector3.up, m_ReferencePoint.m_Transform.rotation * Vector3.up);
                m_Path.Rotate(rotationDifference, m_ReferencePoint.m_PrevPoint);
            }
            m_Path.ApplyFinalNode(m_ControlledCollider.GetCapsuleTransform());
            m_ControlledCollider.UpdateContextInfo();
        }
        m_Path.Clear();
    }

    //Called for every fixedupdate that this module is active
    public override void FixedUpdateModule(){
        float currentTransitionTime = Time.time - m_TransitionStartTime;
        //A MovingColPoint is used to move and rotate the animation path when the collider it starts on is moved
        //This allows animated abilities to work on moving colliders
        if (m_ReferencePoint != null)
        {
            //Position
            CapsuleTransform copy = m_ControlledCollider.GetCapsuleTransformCopy();
            Vector3 diff = m_ReferencePoint.m_Transform.position - m_ReferencePoint.m_PrevPoint;
            copy.Move(diff);
            m_Path.Move(diff);

            //Rotation
            Quaternion rotationDifference = Quaternion.FromToRotation(m_ReferencePoint.m_PrevRot * Vector3.up, m_ReferencePoint.m_Transform.rotation * Vector3.up);
            copy.Rotate(rotationDifference * copy.GetUpDirection(), RotateMethod.FromCenter);
            m_Path.Rotate(rotationDifference, m_ReferencePoint.m_PrevPoint);

            //Offset for rotation
            Vector3 newRelativePoint = rotationDifference * m_ReferencePoint.m_PointRelativeToThis;
            Vector3 relativeDifference = newRelativePoint - m_ReferencePoint.m_PointRelativeToThis;

            copy.Move(relativeDifference);

            m_ReferencePoint.m_PrevPoint = m_ReferencePoint.m_Transform.position;
            m_ReferencePoint.m_PrevRot = m_ReferencePoint.m_Transform.rotation;
            m_ReferencePoint.m_PointRelativeToThis = m_ReferencePoint.m_Transform.position - m_ControlledCollider.GetCapsuleTransform().GetPosition();
            m_ReferencePoint.m_Normal = Quaternion.FromToRotation(m_ReferencePoint.m_PrevRot * Vector3.up, m_ReferencePoint.m_Transform.rotation * Vector3.up) * m_ReferencePoint.m_Normal;

            //if (copy.CanExistHere())
            {
                m_ControlledCollider.ApplyCapsuleTransform(copy);
            }
            //else
            //{
            //    m_WasInterrupted = true;
            //    return;
            //}
        }
        m_ControlledCollider.SetVelocity(Vector2.zero);
        //Move along the nodes as long as they can be applied (either they are finished or their duration is 0)
        while (!m_Path.IsDone() && m_Path.m_CurrentPathNode != null && (m_Path.m_CurrentPathNode.m_Duration == 0 || Time.time - m_TransitionStartTime >= m_Path.m_CurrentPathNode.m_Duration))
        {
            //if (m_Path.m_CurrentPathNode.CanApplyEntireMovement(m_ControlledCollider.GetCapsuleTransform()))
            {
                m_TransitionStartTime += m_Path.m_CurrentPathNode.m_Duration;
                currentTransitionTime -= m_Path.m_CurrentPathNode.m_Duration;
                m_Path.m_CurrentPathNode.ApplyEntireMovement(m_ControlledCollider.GetCapsuleTransform());
                m_Path.IncrementCurrentNode();
            }
           // else
            {
            //    m_WasInterrupted = true;
            //    return;
            }
        }
        //Move along the current node
       // if (m_Path.CanApplyMotion(m_ControlledCollider.GetCapsuleTransform(), currentTransitionTime))
        {
            m_Path.ApplyMotion(m_ControlledCollider.GetCapsuleTransform(), currentTransitionTime);
        }
        //else
        {
        //    m_WasInterrupted = true;
        //    return;
        }
        m_ControlledCollider.UpdateContextInfo();
    }

    protected virtual void GeneratePath()
    {
    }
}
