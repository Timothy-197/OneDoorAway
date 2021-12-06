// this script is attached to the door event gameobject
// the door event game object should be added to a certain scene if the scene contains the triggers
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEvents : MonoBehaviour
{
    public static DoorEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action<int> onPressTrigger; // player press down the trigger
    public event Action<int> onLeaveTrigger; // player leave the trigger

    public void PressTrigger(int index)
    {
        if (onPressTrigger != null)
        {
            onPressTrigger(index);
        }
    }

    public void LeaveTrigger(int index)
    {
        if (onLeaveTrigger != null)
        {
            onLeaveTrigger(index);
        }
    }
}
