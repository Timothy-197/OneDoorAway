using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLayerInit : MonoBehaviour
{
    public int layerNum;

    void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++) {
            this.transform.GetChild(i).gameObject.layer = layerNum;
        }
    }
}
