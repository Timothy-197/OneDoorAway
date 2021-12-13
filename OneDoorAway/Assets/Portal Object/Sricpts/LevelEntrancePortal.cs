using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntrancePortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 31)
        {
            // disactive all player object's child (to make the game look better, not compulsory)
            Transform player = collision.gameObject.transform;
            int childCount = player.childCount;
            for (int i = 0; i < childCount; i++)
            {
                player.GetChild(i).gameObject.SetActive(false);
            }

            LevelManager._instance.loadNextLevel();
        }
    }
}
