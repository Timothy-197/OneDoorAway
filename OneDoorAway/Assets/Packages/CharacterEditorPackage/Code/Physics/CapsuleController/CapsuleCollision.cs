using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//CapsuleCollisionOccurance is a class which stores information about a collision occuring
//The ControlledCapsuleCollider registers these per update and other classes can request them
//--------------------------------------------------------------------
public class CapsuleCollisionOccurrance
{
    public Transform m_Transform;
    public Vector3 m_Point;
    public Vector3 m_Normal;
    public Vector3 m_IncomingVelocity;
    public Vector3 m_OutgoingVelocity;
    public Vector3 m_VelocityLoss;
    public Vector2 m_IncomingVelocityPure;
    public Vector2 m_OutgoingVelocityPure;
    public Vector2 m_VelocityLossPure;
}
