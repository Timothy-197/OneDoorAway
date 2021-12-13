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
    public GameObject MemoryUI;

    private bool shouldFade;
    private float t = 0f;

    void Start() {
        shouldFade = false;
        if (AccomplishmentPanel.IsAccomplishmentActive(memoryIndex)) this.gameObject.SetActive(false);
    }

    void Update() {
        if (shouldFade) {
            this.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, Mathf.Lerp(255, 0, t));
            t += Time.deltaTime*0.5f;
            if (t > 1.0f)
            {
                shouldFade = false;
                MemoryUI.SetActive(false);
                this.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
                this.gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (IsInLayerMask(col.gameObject.layer, playerLayer)) {
            // activate UI
            MemoryUI.SetActive(true);
            // player unlock achevement
            AccomplishmentPanel.ActivateAccomplishment(memoryIndex);
            // make the memory chip disappear forever
            shouldFade = true;
        }
    }

    private bool IsInLayerMask(int layerNum, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << layerNum)) != 0);
    }
}
