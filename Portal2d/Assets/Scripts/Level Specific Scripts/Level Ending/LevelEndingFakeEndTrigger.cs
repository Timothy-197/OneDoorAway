using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelEndingFakeEndTrigger : MonoBehaviour
{
    public Transform spawnPos;

    public GameObject BlackPanel;
    public Text storyText;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 31)
        {
            if (AccomplishmentPanel.CanGoToBonusLevel())
            {
                collision.transform.position = spawnPos.position;
            }
            else
            {
                // player fake ending story
                StartCoroutine("TellFakeEndStory");
            }
        }
    }

    IEnumerator TellFakeEndStory()
    {
        BlackPanel.SetActive(true);

        storyText.text = "";
        yield return new WaitForSeconds(2f);
        storyText.text = "Finally... Finally... I get out of the house...";
        yield return new WaitForSeconds(3f);
        storyText.text = "YEAH!!! I see the sea!";
        yield return new WaitForSeconds(2f);
        storyText.text = "*I run to the sea, wanting to feel the waves flapping my tried body*";
        yield return new WaitForSeconds(3f);
        storyText.text = "Wait... Why... Why I can't feel the water?";
        yield return new WaitForSeconds(3f);
        storyText.text = "... Is it real?";
        yield return new WaitForSeconds(2f);
        storyText.text = "Why...";
        yield return new WaitForSeconds(2f);
        storyText.text = "*Suddenly, I feel so tired and fall asleep*";
        yield return new WaitForSeconds(3f);
        storyText.text = "*Right before I lose consciousness, I felt that... this seems to have happened before...*";
        yield return new WaitForSeconds(4f);

        LevelManager._instance.loadLevel(0);     //go back to level 0 again
    }
}
