using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//Displays the name of the current module above the character
//--------------------------------------------------------------------
public class MovementStateText : MonoBehaviour {
    [SerializeField] AbilityModuleManager m_AbilityModuleManager = null;
    [SerializeField] TextMesh m_TextMesh = null;

    void FixedUpdate () 
	{
        if (m_AbilityModuleManager != null)
        { 
            AbilityModule module = m_AbilityModuleManager.GetCurrentModule();
            if (module != null)
            {
                m_TextMesh.text = module.GetName();
            }
            else
            {
                m_TextMesh.text = "Default";
            }
        }
    }
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
