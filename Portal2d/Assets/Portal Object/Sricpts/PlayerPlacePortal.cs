using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerPlacePortal : MonoBehaviour
{
    static Dictionary<int, Color> portalColorMap = new Dictionary<int, Color> {
        {0, new Color(1, 0, 0) }, {1, new Color(0, 0, 1) }
    };

    public bool allowedToPlacePortal = true;    // reset to false in some level
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


        if (Input.GetMouseButtonDown(0) && allowedToPlacePortal)
        {
            Shoot(0);
        }
        else if (Input.GetMouseButtonDown(1) && allowedToPlacePortal)
        {
            Shoot(1);
        }
    }

    private void Shoot(int index)
    {
        // check invalid input
        if (index != 0 && index != 1)
        {
            Debug.Log("InstantiatePortal: invalid portalIndex");
            return;
        }

        Vector3 destination = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        Vector3 direction = (destination - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * bulletSpeed, ForceMode2D.Impulse);

        // change color of the bullet
        bullet.GetComponent<SpriteRenderer>().color = portalColorMap[index];

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

            // set color of instantiated portal
            portalObj.GetComponent<SpriteRenderer>().color = portalColorMap[portalIndex];

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

            // set to the child of the ground
            if (IsInLayerMask(hit.collider.gameObject.layer, canPortalBePlaced))
            {
                portalObj.transform.SetParent(hit.collider.gameObject.transform); // attach to the rotate door
            }
        }
        else
            Debug.Log("InstantiatePortal: raycast not hit");

    }

    private bool IsInLayerMask(int layerNum, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << layerNum)) != 0);
    }
}
