﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera cam;
    public Vector3 UIoffset;
    public GameObject MemoryUI;

    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeInHierarchy) {
            // follow the player if the UI is active
            Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(cam, MemoryUI.transform.position);
            this.GetComponent<RectTransform>().anchoredPosition = screenPosition + UIoffset;
        }
    }
}
