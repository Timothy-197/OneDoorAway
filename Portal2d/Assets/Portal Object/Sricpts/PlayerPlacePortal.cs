using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerPlacePortal : MonoBehaviour
{
    public GameObject portalPrefab;             // set in inspector
    public GameObject bulletPrefab;             // set in inspector
    public Transform firePoint;                 // set in inspector
    public float bulletSpeed;                   // set in inspector
    public LayerMask canPortalBePlaced;         // set in inspector

    public Portal[] portals;                    // init to len=2 in Start

    private void Start()
    {
        portals = new Portal[2];
    }

    void Update()
    {
        // gun aim at ...
        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 aimDirection = (mousePosition - transform.position).normalized;
        //float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;


        if (Input.GetMouseButtonDown(0))
        {
            Shoot(0);
        }
    }

    private void Shoot(int index)
    {
        Vector3 destination = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        Vector3 direction = (destination - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * bulletSpeed, ForceMode2D.Impulse);

        bullet.GetComponent<Bullet>().portalIndex = index;

        // Implement with raycast
        //Vector2 src = (Vector2)this.transform.position;
        //Vector3 destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //RaycastHit2D hit = Physics2D.Raycast(destination, Vector2.zero);

        //if (hit.collider == null)
        //{
        //    Debug.Log("do not hit anything");
        //    return;
        //}

        //GameObject go = hit.collider.gameObject;
        //int layerNum = go.layer;

        //if (IsInLayerMask(layerNum, canPortalBePlaced))
        //{
        //    Debug.Log("Place Portal.");
        //}
    }

    public void InstantiatePortal(int portalIndex, Collider2D collision, Transform bulletTransform)
    {
        if (portalIndex != 0 && portalIndex != 1)
        {
            Debug.Log("InstantiatePortal: invalid portalIndex");
            return;
        }

        // destroy old portal
        if (portals[portalIndex] != null)
        {
            Destroy(portals[portalIndex].gameObject);
        }

        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, (bulletTransform.position - this.transform.position).normalized, 50f, canPortalBePlaced);

        if (hit)
        {
            GameObject portalObj = Instantiate(portalPrefab, hit.point, Quaternion.identity);
            //portalObj.transform.LookAt(hit.point + hit.normal);
            portals[portalIndex] = portalObj.GetComponent<Portal>();
        } 
        else
        {
            Debug.Log("InstantiatePortal: raycast not hit");
        }
    }
}
