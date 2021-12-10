using UnityEngine;
using System.Collections;

public class WorldTurner : MonoBehaviour {
    CharacterControllerBase m_Character;
    Transform m_CharacterAnchor;
    Transform m_AxisTransform;
    Transform m_TriggerTransform;
    bool m_IsRotating;

    float m_RotationDuration;
    float m_StartRotationY;
    float m_EndRotationY;

    float m_StartTime;

    void Start () 
	{
        m_Character = FindObjectOfType<CharacterControllerBase>();

        m_CharacterAnchor = new GameObject().transform;
        m_CharacterAnchor.gameObject.name = "CharacterAnchor";
        m_CharacterAnchor.SetParent(transform);

        m_AxisTransform = new GameObject().transform;
        m_AxisTransform.gameObject.name = "WorldAxisTransform";
        m_AxisTransform.SetParent(transform);
    }

    void Update()
    {
        if (m_IsRotating)
        {
            if (Time.time - m_StartTime < m_RotationDuration)
            {
                float factor = (Time.time - m_StartTime) / m_RotationDuration;
                float currentAngle = Mathf.Lerp(m_StartRotationY, m_EndRotationY, factor);

                m_AxisTransform.rotation = Quaternion.AngleAxis(currentAngle, Vector3.up);
                // Set the default character new position
                m_Character.transform.position = m_CharacterAnchor.position;
            }
            else
            {
                EndTurning();
            }
        }
    }

    //For scripts to see if the right object is triggering a transition
    public GameObject GetCharacterObject()
    {
        if (m_Character == null)
        {
            return null;
        }
        return m_Character.gameObject;
    }

    public void StartTurning(Vector3 a_AxisPoint, Transform a_TriggerTransform, float a_Angle, float a_Time)
    {
        if (m_Character == null)
        {
            Debug.LogError("No reference to character in WorldTurner script. Cannot turn without character");
            return;
        }

        //Move the axispoint for the world axis
        transform.SetParent(null);
        m_AxisTransform.SetParent(null);
        m_AxisTransform.position = a_AxisPoint;
        transform.SetParent(m_AxisTransform);

        //Save the object that has triggered the transition, to make sure character ends up in the right place
        if (a_TriggerTransform == null)
        {
            Debug.Log("No trigger transform given, cannot rotate");
            return;
        }
        m_TriggerTransform = a_TriggerTransform;

        //In the case that the axis point is not on the character or trigger, make sure the character moves smoothly during transition
        m_CharacterAnchor.position = m_Character.transform.position;

        m_IsRotating = true;
        m_StartTime = Time.time;
        m_RotationDuration = a_Time;
        m_StartRotationY = m_AxisTransform.eulerAngles.y;
        m_EndRotationY = m_StartRotationY + a_Angle;

        //Lock movement during rotation
        m_Character.LockMovement(true);
    }

    public void EndTurning()
    {
        m_IsRotating = false;
        //Prevent minor errors as result of deltaTime differences, snap to end state
        m_AxisTransform.rotation = Quaternion.AngleAxis(m_EndRotationY, Vector3.up);
        // Set the default character new position
        // Also snap the character to the turning trigger's Z position to prevent the character from getting stuck
        Vector3 newPosition = m_CharacterAnchor.position;
        newPosition.z = m_TriggerTransform.position.z;
        m_Character.transform.position = newPosition;
        //Unlock movement now that we're done.
        m_Character.LockMovement(false);
    }
}
