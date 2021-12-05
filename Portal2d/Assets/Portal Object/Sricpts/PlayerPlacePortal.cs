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

    private int getPairPortalIndex(int thisPortalIndex)
    {
        // check invalid input
        if (thisPortalIndex != 0 && thisPortalIndex != 1)
        {
            Debug.LogError("InstantiatePortal: invalid portalIndex");
            return -1;
        }

        if (thisPortalIndex == 1) 
            return 0;
        else 
            return 1;
    }

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
        else if (Input.GetMouseButtonDown(1))
        {
            Shoot(1);
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

        /*
        // Implement with raycast
        Vector2 src = (Vector2)this.transform.position;
        Vector3 destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(destination, Vector2.zero);

        if (hit.collider == null)
        {
            Debug.Log("do not hit anything");
            return;
        }

        GameObject go = hit.collider.gameObject;
        int layerNum = go.layer;

        if (IsInLayerMask(layerNum, canPortalBePlaced))
        {
            Debug.Log("Place Portal.");
        }
        */
    }

    public void InstantiatePortal(int portalIndex, Vector3 bulletPos)
    {
        // check invalid input
        if (portalIndex != 0 && portalIndex != 1)
        {
            Debug.Log("InstantiatePortal: invalid portalIndex");
            return;
        }

        // destroy old portal
        if (portals[portalIndex] != null)
        {
            Destroy(portals[portalIndex].gameObject);

            portals[portalIndex] = null;

            if (portals[getPairPortalIndex(portalIndex)] != null)
                portals[getPairPortalIndex(portalIndex)].pair = null;
        }

        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, (bulletPos - this.transform.position).normalized, 50f, canPortalBePlaced);

        if (hit)
        {
            // instantiate new portal
            GameObject portalObj = Instantiate(portalPrefab, hit.point + hit.normal * 0.1f, Quaternion.identity);
            Quaternion toRotation = Quaternion.FromToRotation(portalObj.transform.right, hit.normal);
            portalObj.transform.rotation = toRotation;

            // update `portals` array
            portals[portalIndex] = portalObj.GetComponent<Portal>();

            // set pair info for the 2 portals
            int anotherPortalIndex = getPairPortalIndex(portalIndex);
            if (portals[anotherPortalIndex] != null)
            {
                portals[portalIndex].pair = portals[anotherPortalIndex];
                portals[anotherPortalIndex].pair = portals[portalIndex];
            }
            else
            {
                portals[portalIndex].pair = null;
            }
        }
        else
            Debug.LogError("InstantiatePortal: raycast not hit");

    }
}
