using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Small class to rotate an object with a certain velocity. Used for moving platforms in levels.
//--------------------------------------------------------------------
public class Rotator : MonoBehaviour
{
    [SerializeField] float m_MovementSpeed = 0.0f;
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, Time.fixedDeltaTime * m_MovementSpeed);
    }
}
