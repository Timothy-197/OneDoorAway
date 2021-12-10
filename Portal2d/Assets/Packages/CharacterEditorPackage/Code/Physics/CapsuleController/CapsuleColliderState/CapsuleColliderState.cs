using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum CapsuleResizeMethod
{
    FromCenter,
    FromBottom,
    FromTop,
    None
}
//--------------------------------------------------------------------
//Class derived from CState which stores context information for a capsule collider. This includes grounded, sidecast and edgecast information. This is updated by ControlledCapsulerCollider.
//--------------------------------------------------------------------
public class CCState : CState
{
    //Helpers
    //The reason for encapsulation is that it might be necessary to process the capsulecast results further.
    //There were also rumours that capsulecasts could break when the top and bottom point were switched when calling capsulecast
    //Keeping these calls here allows me to change them once, change them everywhere in case one of the above things becomes an issue.
    public static List<RaycastHit> CorrectCapsuleCastAll(Vector3 a_Bot, Vector3 a_Top, float a_Rad, Vector3 a_Direction, float a_Distance, LayerMask a_LayerMask)
    {
        RaycastHit[] raycastHits = Physics.CapsuleCastAll(a_Bot, a_Top, a_Rad, a_Direction, a_Distance, a_LayerMask);
        return new List<RaycastHit>(raycastHits);
    }

    public static bool CorrectCapsuleCast(Vector3 a_Bot, Vector3 a_Top, float a_Rad, Vector3 a_Direction, float a_Distance, LayerMask a_LayerMask)
    {
        return Physics.CapsuleCast(a_Bot, a_Top, a_Rad, a_Direction, a_Distance, a_LayerMask);
    }

    public CCGroundedInfo m_GroundedInfo = new CCGroundedInfo();
    public CCSideCastInfo m_SideCastInfo = new CCSideCastInfo();
    public CCEdgeCastInfo m_EdgeCastInfo = new CCEdgeCastInfo();
    public void Init(ControlledCapsuleCollider a_CapsuleCollider)
    {
        m_GroundedInfo.Init(a_CapsuleCollider);
        m_SideCastInfo.Init(a_CapsuleCollider);
        m_EdgeCastInfo.Init(a_CapsuleCollider);
    }

    public void UpdateGroundedInfo(List<RaycastHit> a_RaycastHits)
    {
        m_GroundedInfo.UpdateWithCollisions(a_RaycastHits);
    }

    public void UpdateSideCastInfo(List<RaycastHit> leftHitResults, List<RaycastHit> rightHitResults)
    {
        m_SideCastInfo.UpdateWithCollisions(leftHitResults, rightHitResults);
    }

    public void UpdateEdgeCastInfo()
    {
        m_EdgeCastInfo.UpdateWithCollisions();
    }
}