using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMove_Rotate : MonoBehaviour
{
    public int doorIndex; // unique index of the current door
    public float angle; // angle of rotation
    public float rotateSpeed; // speed to rotate the door
    [Tooltip("counter 1->clockwise / -1->counter clockwise")]
    public int direction; // counter 0->clockwise / 1->clockwise

    private Transform tr;
    private Vector3 initilaEulerAngle;
    private bool shouldRotate;
    private bool rotateBack; // 1: rotate back, 0: do not rotate, have higher priority

    private void Start() 
    {
        DoorEvents.current.onPressTrigger += RotateDoorOpen;
        DoorEvents.current.onLeaveTrigger += RotateDoorClose;
        tr = this.transform;
        initilaEulerAngle = tr.eulerAngles;
        shouldRotate = false;
        rotateBack = false;
    }

    private void OnDestroy()
    {
        DoorEvents.current.onPressTrigger -= RotateDoorOpen;
        DoorEvents.current.onLeaveTrigger -= RotateDoorClose;
    }

    private void Update()
    {
        if (shouldRotate) {
            if (rotateBack) {
                if (tr.eulerAngles.z == initilaEulerAngle.z) {
                    shouldRotate = false;
                    rotateBack = false; // clear
                }
                tr.eulerAngles = new Vector3(
                        initilaEulerAngle.x, initilaEulerAngle.y,
                        Mathf.LerpAngle(tr.eulerAngles.z, initilaEulerAngle.z, Time.deltaTime * rotateSpeed));
            }
            else if (direction == 1)
            {
                if (Vector3.Angle(tr.eulerAngles, initilaEulerAngle) >= angle)
                {
                    //Debug.Log("reset");
                    shouldRotate = false;
                    tr.eulerAngles = new Vector3(initilaEulerAngle.x, initilaEulerAngle.y, initilaEulerAngle.z + angle);
                }
                else
                {
                    tr.eulerAngles = new Vector3(
                        initilaEulerAngle.x, initilaEulerAngle.y,
                        Mathf.LerpAngle(tr.eulerAngles.z, initilaEulerAngle.z + (angle + 5), Time.deltaTime * rotateSpeed));
                }
            }
            else if (direction == -1)
            {
                if (Vector3.Angle(tr.eulerAngles, initilaEulerAngle) >= angle)
                {
                    //Debug.Log("reset");
                    shouldRotate = false;
                    tr.eulerAngles = new Vector3(initilaEulerAngle.x, initilaEulerAngle.y, initilaEulerAngle.z - angle);
                }
                else
                {
                    tr.eulerAngles = new Vector3(
                        initilaEulerAngle.x, initilaEulerAngle.y,
                        Mathf.LerpAngle(tr.eulerAngles.z, initilaEulerAngle.z - (angle + 5), Time.deltaTime * rotateSpeed));
                }
            }
        }
    }

    /*
     * to rotate the door, should be invoked if the player trigger the door open switch
     * --------------------------------------------------------------------------------
     * parameters: int direction: 1->clockwise, 0->counter clockwise
     */
    private void RotateDoorOpen(int index) 
    {
        if (index == doorIndex) { // check the index of the trigger
            //Debug.Log("door called by trigger!");
            shouldRotate = true;
            rotateBack = false;
        }
    }
    /*
     * rotate the door to the original state (close the door)
     */
    private void RotateDoorClose(int index)
    {
        //Debug.Log("rotate back called");
        if (index == doorIndex) {
            shouldRotate = true;
            rotateBack = true;
        }
    }


    //private IEnumerator RotateObject(int dir, float overTime)
    //{
    //    float startTime = Time.time;
    //    float initZ;
    //    float tempZ;
    //    initZ = tr.eulerAngles.z;
    //    tempZ = initZ;
    //    while (Time.time < startTime + overTime)
    //    {
    //        Debug.Log("now the tempZ is " + tempZ);
    //        tempZ = Mathf.LerpAngle(initZ, initZ+angle, Time.deltaTime)+tempZ;
    //        tr.eulerAngles = new Vector3(tr.eulerAngles.x, tr.eulerAngles.y, tempZ);
    //        yield return null;
    //    }
    //}
}
