using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntrancePortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 31)
        {
            LevelManager._instance.loadNextLevel();
        }
    }
}
