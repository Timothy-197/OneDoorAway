using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal pair;

    public Transform srcTransform;          // set in inspector (prefab)
    public Transform desTransform;          // set in inspector (prefab)

    [Tooltip("Transferable Object implemented by Unity Physics")]
    public LayerMask transferableObjectUnityPhysics;            // set in inspector (prefab) - those objects should have Rigidbody
    [Tooltip("Transferable Object implemented by Customized Physics")]
    public LayerMask transferableObjectCustomizedPhysics;       // set in inspector (prefab) - those objects should have BasicMove (implemented by ourselves)


    public Vector3 getPortalDirection()
    {
        return (desTransform.position - srcTransform.position).normalized;
    }

    public bool IsInLayerMask(int layerNum, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << layerNum)) != 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pair == null) return;

        int layerNum = other.gameObject.layer;

        // if hit by "Gravity Ball", use Unity Physics API
        if (IsInLayerMask(layerNum, transferableObjectUnityPhysics))
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
        else if (IsInLayerMask(layerNum, transferableObjectCustomizedPhysics))
        {
            GameObject go = other.gameObject;

            //ControlledCollider controlledCollider = go.GetComponent<ControlledCollider>();
            BasicMove basicMove = go.GetComponent<BasicMove>();

            //float speedMagnitude = controlledCollider.GetVelocity().magnitude;
            float speedMagnitude = basicMove.GetSpeed();

            // move `go` position and set speed
            go.transform.position = pair.desTransform.position;
            go.transform.rotation = pair.desTransform.rotation;
            basicMove.SetVelocity(speedMagnitude * pair.getPortalDirection());
            //controlledCollider.SetVelocity(speedMagnitude * pair.getPortalDirection());  //implicitly converted to Vector2
        }
    }
}
