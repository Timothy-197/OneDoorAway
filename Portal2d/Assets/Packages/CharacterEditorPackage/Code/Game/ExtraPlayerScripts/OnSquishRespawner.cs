using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Detects when the capsule is being squished (using CapsuleVolumeIntegrity) and attempts to respawn
//Use timer to prevent overzealous respawning
//--------------------------------------------------------------------
public class OnSquishRespawner : MonoBehaviour {
    [SerializeField] CapsuleVolumeIntegrity m_VolumePreserver = null;
    [SerializeField] float m_SquishTime = 0.0f;
    float m_LastSquishTime;
    bool m_IsSquishing;
	void FixedUpdate () 
	{
	    if (m_VolumePreserver.IsBeingSquished())
        {
            if (!m_IsSquishing)
            {
                m_LastSquishTime = Time.fixedTime;
            }
            m_IsSquishing = true;
        }
        else
        {
            m_IsSquishing = false;
        }
        if (m_IsSquishing && Time.fixedTime - m_LastSquishTime > m_SquishTime)
        {
            Respawn();
        }
	}

    void Respawn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
