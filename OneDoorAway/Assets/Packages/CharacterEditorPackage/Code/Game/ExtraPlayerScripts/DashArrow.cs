using UnityEngine;
using System.Collections;

public class DashArrow : MonoBehaviour {
    [SerializeField] PlayerInput m_PlayerInput = null;
    [SerializeField] AbilityModuleManager m_ModuleManager = null;
    [SerializeField] Transform m_Arrow = null;
    [SerializeField] float m_MaxLength = 0.0f;
	
	void Update () 
	{
        AbilityModule moduleObject = m_ModuleManager.GetModuleWithName("Dash");
        if (moduleObject == null || moduleObject.GetType() != typeof(DashModule))
        {
            return;
        }
        DashModule module = moduleObject as DashModule;
        if (module.CanStartDash() && m_PlayerInput.GetDirectionInput("Aim") != null)
        {
            m_Arrow.gameObject.SetActive(true);    
            Vector2 aimDir = m_PlayerInput.GetDirectionInput("Aim").m_ClampedInput;
            Vector3 aimDir3d = new Vector3(aimDir.x, aimDir.y, 0.0f);
            aimDir3d.Normalize();
            m_Arrow.LookAt(m_Arrow.transform.position + aimDir3d, Vector3.back);
            m_Arrow.localScale = new Vector3(1.0f, 1.0f, m_MaxLength * aimDir.magnitude);
        }
        else
        {
            m_Arrow.gameObject.SetActive(false);
        }
	}
}
