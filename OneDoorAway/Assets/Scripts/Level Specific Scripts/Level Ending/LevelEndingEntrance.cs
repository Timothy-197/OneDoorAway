using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndingEntrance : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 31)
        {
            if (AccomplishmentPanel.CanGoToBonusLevel())
            {
                LevelManager._instance.loadNextLevel();  // load next which is bonus
            }
            else
            {
                Debug.Log("Not all accomplishment collected, cannot go to bonus");
            }
        }
    }
}
