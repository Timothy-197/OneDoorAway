using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal pair;

    public Transform srcTransform;          // set in inspector (prefab)
    public Transform desTransform;          // set in inspector (prefab)

    public Vector3 getPortalDirection()
    {
        return (desTransform.position - srcTransform.position).normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pair == null) return;

        int layerNum = other.gameObject.layer;

        // if hit by "Portal Transferable"
        if (layerNum == 9)
        {
            GameObject go = other.gameObject;
            Rigidbody rig = go.GetComponent<Rigidbody>();

            float speedMagnitude = rig.velocity.magnitude;

            // move `go` position and set speed
            rig.position = pair.desTransform.position;
            rig.rotation = pair.desTransform.rotation;
            rig.velocity = speedMagnitude * pair.getPortalDirection();
        }

        // if hit by "player"
        // "player" is not implemented with Unity Physics
        else if (layerNum == 31)
        {
            GameObject go = other.gameObject;
            ControlledCollider controlledCollider = go.GetComponent<ControlledCollider>();

            float speedMagnitude = controlledCollider.GetVelocity().magnitude;

            // move `go` position and set speed
            go.transform.position = pair.desTransform.position;
            go.transform.rotation = pair.desTransform.rotation;
            //controlledCollider.SetPosition(pair.desTransform.position);
            //controlledCollider.SetRotation(pair.desTransform.rotation);
            controlledCollider.SetVelocity(speedMagnitude * pair.getPortalDirection());  //implicitly converted to Vector2
        }
    }
}
