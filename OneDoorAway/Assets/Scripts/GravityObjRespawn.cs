// this script should be attached to the gravity object prefabs
// the script can prevent the objects disapperar the the screen
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObjRespawn : MonoBehaviour
{
    public Transform objRespawnPoint;
    [Tooltip("the maximum falling distance of the gameobject ")]
    public float maxFallingDistance;

    void Start()
    {
        StartCoroutine(onCoroutine());
    }

    IEnumerator onCoroutine()
    {
        while (true)
        {
            if (Mathf.Abs(this.transform.position.y) > maxFallingDistance) {
                Respawn();    
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void Respawn()
    {
        this.transform.position = objRespawnPoint.position;
        if (this.GetComponent<Rigidbody2D>() != null) this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

}
