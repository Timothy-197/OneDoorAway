using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal pair;

    public Transform srcTransform;          // set in inspector (prefab)
    public Transform desTransform;          // set in inspector (prefab)

    [Tooltip("Transferable Object implemented by Unity Physics")]
    public LayerMask transferableObjectUnityPhysics;            // set in inspector (prefab) - those objects should have Rigidbody2D
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

    private void portalTransfer(Collider2D collision)
    {
        if (pair == null) return;

        int layerNum = collision.gameObject.layer;

        // if hit by "Gravity Ball", use Unity Physics API
        if (IsInLayerMask(layerNum, transferableObjectUnityPhysics))
        {
            //Debug.Log("Detect gravity ball");

            GameObject go = collision.gameObject;
            Rigidbody2D rig = go.GetComponent<Rigidbody2D>();

            float speedMagnitude = rig.velocity.magnitude;

            // move `go` position and set speed
            rig.position = pair.desTransform.position;
            //rig.rotation = pair.desTransform.rotation;
            rig.velocity = speedMagnitude * pair.getPortalDirection();
        }

        // if hit by "player"
        // "player" is not implemented with Unity Physics
        else if (IsInLayerMask(layerNum, transferableObjectCustomizedPhysics))
        {
            PlayerEvents.current.EnterPortal();
            //Debug.Log("Detect player");

            GameObject go = collision.gameObject;

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

    private void OnTriggerStay2D(Collider2D collision)
    {
        portalTransfer(collision);
        //Debug.Log("portalTransfer call from OnTriggerStay2D");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        portalTransfer(collision);
    }
}
