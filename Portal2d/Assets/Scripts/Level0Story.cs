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
        storyText.text = "hello";
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
