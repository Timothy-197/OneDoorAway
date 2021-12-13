using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityField : MonoBehaviour
{
    public LayerMask PlayerLayer;
    public LayerMask GravityObjLayer;
    private void OnTriggerEnter2D(Collider2D collider) {
        if (IsInLayerMask(collider.gameObject.layer, PlayerLayer)) {
            // player change gravity direction in the air
            BasicMove.Instance.SetGravityDirection(true);
        }
        if (IsInLayerMask(collider.gameObject.layer, GravityObjLayer)) {
            // change the gravity direction of the object
            collider.gameObject.GetComponent<Rigidbody2D>().gravityScale = -collider.gameObject.GetComponent<Rigidbody2D>().gravityScale;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (IsInLayerMask(collider.gameObject.layer, PlayerLayer))
        {
            // player change gravity direction in the air
            BasicMove.Instance.SetGravityDirection(true);
        }
        if (IsInLayerMask(collider.gameObject.layer, GravityObjLayer))
        {
            // change the gravity direction of the object
            collider.gameObject.GetComponent<Rigidbody2D>().gravityScale = -Mathf.Abs(collider.gameObject.GetComponent<Rigidbody2D>().gravityScale);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (IsInLayerMask(collider.gameObject.layer, PlayerLayer))
        {
            // player change gravity direction in the air
            BasicMove.Instance.SetGravityDirection(false);
        }
        if (IsInLayerMask(collider.gameObject.layer, GravityObjLayer))
        {
            // change the gravity direction of the object
            collider.gameObject.GetComponent<Rigidbody2D>().gravityScale = -collider.gameObject.GetComponent<Rigidbody2D>().gravityScale;
        }
    }

    private bool IsInLayerMask(int layerNum, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << layerNum)) != 0);
    }
}
