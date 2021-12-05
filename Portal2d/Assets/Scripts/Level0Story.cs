using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level0Story : MonoBehaviour
{
    public Text storyText;          // set in inspector

    private void Start()
    {
        storyText.text = "";
        StartCoroutine("TellBackgroundStory");
    }

    IEnumerator TellBackgroundStory()
    {
        yield return new WaitForSeconds(0.5f);
        storyText.text = "I woke up in this strange place, forgetting everything...";
        yield return new WaitForSeconds(3f);
        storyText.text = "Where is here... And... Who am I...";
        yield return new WaitForSeconds(3f);
        storyText.text = "I need to get out of this room, but where should I go...";
        yield return new WaitForSeconds(3f);
        storyText.text = "Wait... What is that sound...";
        yield return new WaitForSeconds(2.5f);
        storyText.text = "It sounds like sea waves!";
        yield return new WaitForSeconds(2.5f);
        storyText.text = "Maybe I can get out following the sea wave...";
        yield return new WaitForSeconds(3f);

        Destroy(this.gameObject);
    }
}
