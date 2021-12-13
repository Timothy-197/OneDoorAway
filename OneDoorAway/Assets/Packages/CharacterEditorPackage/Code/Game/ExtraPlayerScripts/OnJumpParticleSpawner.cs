using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//--------------------------------------------------------------------
//This class registers to an CharacterControllerBase's OnJump event and spawns particles when a jump occurs
//--------------------------------------------------------------------
public class OnJumpParticleSpawner : MonoBehaviour {
    [SerializeField] GroundedCharacterController m_CharacterController = null;
    [SerializeField] ControlledCapsuleCollider m_Collider = null;
    [SerializeField] ParticleSystem m_ParticleSystem = null;
    [SerializeField] int m_EmissionCount = 0;
    void OnEnable()
    {
        m_CharacterController.OnJump += SpawnParticles;
    }


    void OnDisable()
    {
        m_CharacterController.OnJump -= SpawnParticles;
    }

    void SpawnParticles()
    {
        Vector3 position = Vector3.zero;
        Vector3 normal = Vector3.zero;

        CGroundedInfo groundInfo = m_CharacterController.GetCollider().GetGroundedInfo();
        CSideCastInfo sideCastInfo = m_CharacterController.GetCollider().GetSideCastInfo();
        bool valid = false;
        if (groundInfo.m_IsGrounded)
        { 
            position = groundInfo.GetPoint();
            normal = new Vector3(groundInfo.GetNormal().x, groundInfo.GetNormal().y, 0.0f);
            valid = true;
        }
        else if (sideCastInfo.m_WallCastCount >= 2)
        {
            position = sideCastInfo.GetSidePoint();
            normal = sideCastInfo.GetSideNormal();
            valid = true;
        }
        else
        {
            position = m_Collider.GetDownCenter() - m_Collider.GetUpDirection() * m_Collider.GetRadius();
            normal = m_Collider.GetUpDirection();
            valid = true;
        }
        if (valid)
        { 
            m_ParticleSystem.transform.position = position;
            m_ParticleSystem.transform.LookAt(position + normal, Vector3.back);
            m_ParticleSystem.Emit(m_EmissionCount);
        }
    }
}
