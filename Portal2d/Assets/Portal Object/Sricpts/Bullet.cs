using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LayerMask canPortalBePlaced;         // set in inspector

    public int portalIndex { get; set; }

    public bool IsInLayerMask(int layerNum, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << layerNum)) != 0);
    }

    private void Start()
    {
        StartCoroutine("DestroySelf");
    }

    // Destroy itself if not hit anything
    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layerNum = collision.gameObject.layer;

        if (IsInLayerMask(layerNum, canPortalBePlaced))
        {
            PlayerPlacePortal playerPlacePortal = GameObject.Find("Player").GetComponent<PlayerPlacePortal>();
            playerPlacePortal.InstantiatePortal(portalIndex, this.transform.position);
            Destroy(this.gameObject);
        }
    }
}
