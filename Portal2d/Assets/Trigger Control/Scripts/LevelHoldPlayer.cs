using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHoldPlayer : MonoBehaviour
{

    public LayerMask carryObjectLayers;

    void OnCollisionEnter2D(Collision2D collision) {
        if (IsInLayerMask(collision.gameObject.layer, carryObjectLayers)) {
            Debug.Log("carry the object with layer: " + collision.gameObject.layer);
            collision.gameObject.transform.SetParent(this.transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (IsInLayerMask(collision.gameObject.layer, carryObjectLayers))
        {
            collision.gameObject.transform.SetParent(null);
        }
    }

    private bool IsInLayerMask(int layerNum, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << layerNum)) != 0);
    }
}
