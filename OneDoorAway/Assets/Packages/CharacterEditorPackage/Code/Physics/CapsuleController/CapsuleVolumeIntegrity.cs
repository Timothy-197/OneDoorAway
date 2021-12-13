using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//This class is in charge of preventing capsule clipping.
//It uses a Unity capsule collider and rigidbody to track this).
//It tries to detect shallow clipping as it happens (for example due to moving colliders) and resolving it
//It also keeps track of whether the capsule is being squished, which means that it is unrecoverably clipping.
//Other scripts can fix it then (for example via a death state)
//--------------------------------------------------------------------
public class CapsuleVolumeIntegrity : MonoBehaviour {
    [SerializeField] Rigidbody m_RigidBody;
    [SerializeField] ControlledCapsuleCollider m_ControlledCollider;
    [SerializeField] CapsuleCollider m_CapsuleCollider;
    [SerializeField] float m_CheckDistance = 0.0f;
    [SerializeField] float m_SquishRadius = 0.0f;
    bool m_IsActive = true;
    bool m_IsBeingSquished;

    //Called by Unity upon adding a new component to an object, or when Reset is selected in the context menu. Used here to provide default values.
    //Also used when fixing up components using the CharacterFixEditor button
    void Reset()
    {
        m_RigidBody = transform.GetComponent<Rigidbody>();
        m_ControlledCollider = transform.GetComponent<ControlledCapsuleCollider>();
        m_CapsuleCollider = transform.GetComponent<CapsuleCollider>();
        m_CheckDistance = 0.02f;
        m_SquishRadius = 0.05f;
    }
    void Awake()
    {
        if (m_ControlledCollider == null || m_CapsuleCollider == null || m_RigidBody == null)
        {
            Debug.LogError("Missing references on CapsuleVolumeIntegrity");
            enabled = false;
            return;
        }

        m_CapsuleCollider.radius = m_ControlledCollider.GetRadius();
    }
    void FixedUpdate()
    {
        //If the core of the capsule is clipping, the capsule is likely being squished.
        if (m_IsActive && m_ControlledCollider.GetCapsuleTransform().IsBeingSquished(m_SquishRadius))
        {
            m_IsBeingSquished = true;
        }
        else
        {
            m_IsBeingSquished = false;
        }
        //Apply ControlledCapsuleCollider transform
        m_CapsuleCollider.height = m_ControlledCollider.GetLength() + m_ControlledCollider.GetRadius() * 2.0f;

        //Detect collisions
        if (m_RigidBody)
        {
            m_RigidBody.MovePosition(transform.position);
        }
    }
    void OnCollisionStay(Collision a_Collision)
    {
        if (!m_IsActive || !enabled)
        {
            return;
        }
        ResolveCollision(a_Collision);
    }
    void OnCollisionEnter(Collision a_Collision)
    {
        if (!m_IsActive || !enabled)
        {
            return;
        }
        ResolveCollision(a_Collision);
    }

    void ResolveCollision(Collision a_Collision)
    {
		for (int i = 0; i < a_Collision.contacts.Length; i++) 
		{
			Vector3 normal = a_Collision.contacts [i].normal;
			Vector3 point = a_Collision.contacts [i].point -normal * a_Collision.contacts[i].separation;

			//Resolution of shallow clipping
			Vector3 castStartPoint = point - normal * m_CheckDistance;
			RaycastHit result;
			LayerMask mask = 1 << gameObject.layer;
			bool raycastSuccesful = Physics.Raycast (castStartPoint, normal, out result, m_CheckDistance + 0.1f, mask);
			if (raycastSuccesful) {
				if (result.distance <= m_CheckDistance) {
					m_ControlledCollider.GetCapsuleTransform ().Move (-normal * (result.distance - m_CheckDistance));
					m_ControlledCollider.UpdateContextInfo ();
				}
			}
		}
    }

    public void ToggleCollisionsActive()
    {
        m_IsActive = !m_IsActive;
    }

    public bool IsBeingSquished()
    {
        return m_IsBeingSquished;
    }
}
