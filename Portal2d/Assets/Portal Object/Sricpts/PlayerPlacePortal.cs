using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerPlacePortal : MonoBehaviour
{
    public GameObject bulletPrefab;             // set in inspector
    public Transform firePoint;                 // set in inspector
    public float bulletSpeed;                   // set in inspector


    void Update()
    {
        // gun aim at ...
        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 aimDirection = (mousePosition - transform.position).normalized;
        //float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;


        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Vector3 destination = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        Vector3 direction = (destination - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * bulletSpeed, ForceMode2D.Impulse);

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

}
