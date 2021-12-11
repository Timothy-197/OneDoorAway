using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndStory : MonoBehaviour
{
    public Text storyText;          // set in inspector

    private void Start()
    {
        storyText.text = "";
        StartCoroutine("TellEndStory");
    }

    IEnumerator TellEndStory()
    {
        yield return new WaitForSeconds(0.5f);
        storyText.text = "I finially get the truth... The sea is not real!";
        yield return new WaitForSeconds(3f);
        storyText.text = "I remember... I was trapped in an infinite dream!";
        yield return new WaitForSeconds(3f);
        storyText.text = "Everytime I reach the sea, my memory get cleared... And I need to go through this hell again...";
        yield return new WaitForSeconds(3f);
        storyText.text = "Wait... I am lying on an hard bed... I feel so cold";
        yield return new WaitForSeconds(2.5f);
        storyText.text = "What is connect to my head? I need to drag that stuff off!";
        yield return new WaitForSeconds(2.5f);
        storyText.text = "... S**t!... That hurts!";
        yield return new WaitForSeconds(2.5f);
        storyText.text = "Ah, my brain was controlled by the f***** computer! ";
        yield return new WaitForSeconds(2.5f);
        storyText.text = "It is time to fight back... and gain real freedom... and see the real sea...";
        yield return new WaitForSeconds(3f);

        Destroy(this.gameObject);
    }
}
