using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMove_Slide : MonoBehaviour
{
    public int doorIndex; // unique index of the current door
    public float Speed; // speed to rotate the door
    public Vector3 direction; // direction to move
    public float distance; // distance to move

    private Transform tr;
    private Vector3 initialPosition;
    private bool shouldMove;
    private bool shouldBack;

    private void Start()
    {
        DoorEvents.current.onPressTrigger += SlideDoorOpen;
        DoorEvents.current.onLeaveTrigger += SlideDoorClose;
        tr = this.transform;
        initialPosition = tr.position;
    }

    private void OnDestroy()
    {
        DoorEvents.current.onPressTrigger -= SlideDoorOpen;
        DoorEvents.current.onLeaveTrigger -= SlideDoorClose;
        shouldMove = false;
        shouldBack = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) { SlideDoorOpen(doorIndex); }
        if (Input.GetKeyDown(KeyCode.I)) { SlideDoorClose(doorIndex); }
        if (shouldBack)
        {
            if (tr.position == initialPosition) {
                shouldBack = false;
            }
            tr.position = Vector3.Lerp(tr.position, initialPosition, Time.deltaTime*Speed);
        }
        else if (shouldMove) {
            if (tr.position == initialPosition + direction * distance) {
                shouldMove = false;
            }
            tr.position = Vector3.Lerp(tr.position, initialPosition+direction*distance, Time.deltaTime*Speed);
        }
    }

    private void SlideDoorOpen(int index)
    {
        if (index == doorIndex) {
            shouldMove = true;
            shouldBack = false;
        }
    }

    private void SlideDoorClose(int index)
    {
        if (index == doorIndex) {
            shouldBack = true;
            shouldMove = false;
        }
    }
}
