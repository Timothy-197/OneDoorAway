using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonGas : MonoBehaviour
{
    public Transform spawnPos;
    public Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layerNum = collision.gameObject.layer;

        if (layerNum == 31)
        {
            collision.transform.position = spawnPos.position;
            animator.SetTrigger("FadeOut");

            StartCoroutine("ResetTrigger");
        }
    }

    IEnumerator ResetTrigger()
    {
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger("FadeOut");
        animator.SetBool("AutoBackToFadeIn", true);
    }
}
