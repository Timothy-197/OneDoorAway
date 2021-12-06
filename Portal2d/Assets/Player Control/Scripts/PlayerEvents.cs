using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    public static PlayerEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action onDetectObject; // detect the pickable object in range
    public event Action onCarryObject;
    public event Action onEnterPortal; // enter the portal

    public void DetectObject()
    {
        if (onDetectObject != null)
        {
            onDetectObject();
        }
    }

    public void CarryObject()
    {
        if (onCarryObject != null)
        {
            onCarryObject();
        }
    }

    public void EnterPortal()
    {
        if (onEnterPortal != null)
        {
            onEnterPortal();
        }
    }
}