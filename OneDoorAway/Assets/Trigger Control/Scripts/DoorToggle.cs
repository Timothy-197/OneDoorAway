using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorToggle : MonoBehaviour
{
    [Tooltip("the index should maps to a unique door")]
    public int triggerIndex;
    public LayerMask gravityInstances;

    private GameObject activeSp;
    private GameObject inactiveSp;
    private bool isToggled;
    void Start()
    {
        activeSp = this.transform.GetChild(2).gameObject;
        inactiveSp = this.transform.GetChild(1).gameObject;
        activeSp.SetActive(false);
        inactiveSp.SetActive(true);
        isToggled = false;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("enter trigger, layer is: " + other.gameObject.layer);
        if (IsInLayerMask(other.gameObject.layer, gravityInstances))
        {
            if (isToggled)
            {
                isToggled = false;
                // trigger the event to close the the door
                DoorEvents.current.LeaveTrigger(triggerIndex);
                // invoke inactive button sprite
                activeSp.SetActive(false);
                inactiveSp.SetActive(true);
            }
            else {
                isToggled = true;
                // trigger the event to open the door
                DoorEvents.current.PressTrigger(triggerIndex);
                // invoke active button sprite
                activeSp.SetActive(true);
                inactiveSp.SetActive(false);
            }
        }
    }

    private bool IsInLayerMask(int layerNum, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << layerNum)) != 0);
    }
}
