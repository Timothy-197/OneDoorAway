// this script is for advanced palyer control
// this script should be attached to the Player of the player prefab
// 1. Eject portals
// 2. Picking boxes
// 3. climb ladders (may be)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvanceMove : MonoBehaviour
{
    public GameObject pickUpText;
    void Start()
    {
        PlayerEvents.current.onDetectObject += showPickUpText;
        PlayerEvents.current.onCarryObject += hidePickUpText;
        pickUpText.SetActive(false);
    }

    void OnDestroy()
    {
        PlayerEvents.current.onDetectObject -= showPickUpText;
        PlayerEvents.current.onCarryObject += hidePickUpText;
    }

    private void showPickUpText()
    {
        //Debug.Log("show called!");
        pickUpText.SetActive(true);
    }

    private void hidePickUpText()
    {
        //Debug.Log("hide called!");
        pickUpText.SetActive(false);
    }
}
