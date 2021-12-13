using UnityEngine;
using System.Collections;

public class AnimatedMover : MonoBehaviour {
    Animator m_Animator;
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!m_Animator)
        {
            Debug.Log("Animator not found on object");
            this.enabled = false;
            return;
        }
        m_Animator.Update(Time.fixedDeltaTime);
        m_Animator.enabled = false;
    }
}
