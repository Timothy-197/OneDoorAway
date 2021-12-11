// the script should be attached to the memory chip prefab
// the script control the behavior of the memory chip
// 1. detect player collision, invoke ui, make player collect (automatically collect)
// 2. unlock corresponding achievement
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryChip : MonoBehaviour
{
    public LayerMask playerLayer;
    public int memoryIndex;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (IsInLayerMask(col.gameObject.layer, playerLayer)) {
            // activate UI
            GameObject.Find("Find Memory UI").SetActive(true);
            // player unlock achevement

        }
    }

    private bool IsInLayerMask(int layerNum, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << layerNum)) != 0);
    }
}
