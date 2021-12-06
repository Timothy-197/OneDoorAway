﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    // normal of the glass is transform.right

    public float minSpeedToBreak;
    public float forceScale;

    public LayerMask canBreakGlassUnityPhysics;
    public LayerMask cabBreakGlassCustomizedPhysics;

    public bool IsInLayerMask(int layerNum, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << layerNum)) != 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layerNum = collision.gameObject.layer;

        if (IsInLayerMask(layerNum, canBreakGlassUnityPhysics))
        {
            Vector2 vel = collision.gameObject.GetComponent<Rigidbody2D>().velocity;
            breakGlass(vel);
        }
        else if (IsInLayerMask(layerNum, cabBreakGlassCustomizedPhysics))
        {
            Vector2 vel = collision.gameObject.GetComponent<BasicMove>().GetSpeed_Vector();
            breakGlass(vel);
        }

    }

    private void breakGlass(Vector2 vel)
    {
        float normalVel = Vector3.Project(vel, transform.right).magnitude;

        // break the glass
        if (normalVel > minSpeedToBreak)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Rigidbody2D rb = transform.GetChild(i).GetComponent<Rigidbody2D>();
                rb.simulated = true;
                rb.AddForce(vel.normalized * forceScale, ForceMode2D.Impulse);
            }

            // Destroy Itself
            StartCoroutine("DestroyItself");
        }
    }

    IEnumerator DestroyItself()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
