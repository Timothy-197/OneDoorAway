using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AccomplishmentPanel : MonoBehaviour
{
    public Transform AccomplishmentSet;

    // PlayerPrefs.GetInt(AccomplishmentName, 0)
    //    - 0: Do not collect this accomplishment
    //    - 1: Have already collected this accomplishment
    static private List<string> AccomplishmentList = new List<string> {
        "Accomplishment 1",
        "Accomplishment 2",
        "Accomplishment 3",
        "Accomplishment 4",
        "Accomplishment 5",
        "Accomplishment 6",
    };

    private Color LOCKED_COLOR = new Color(0.2f, 0.2f, 0.2f);

    public void ActivateAccomplishmentPanel()
    {
        // go through all accomplishments (children of AccomplishmentSet)
        for (int i = 0; i < AccomplishmentSet.childCount; i++)
        {
            string AccomplishmentName = AccomplishmentSet.GetChild(i).name;

            // if we have already collected this accomplishment, do nothing
            if (PlayerPrefs.GetInt(AccomplishmentName, 0) != 1)
            {
                Transform accomplishment = AccomplishmentSet.GetChild(i);
                accomplishment.Find("Lock Image").gameObject.SetActive(true);  // activate the lock
                accomplishment.GetComponent<Image>().color = LOCKED_COLOR;     // make this accomplishment grey
            }
        }

        this.gameObject.SetActive(true);
    }

    public void DeactivateAccomplishmentPanel()
    {
        this.gameObject.SetActive(false);
    }

    static public void UnlockAllAccomplishments()
    {
        for (int i = 0; i < AccomplishmentList.Count; i++)
        {
            PlayerPrefs.SetInt(AccomplishmentList[i], 1);
        }
    }

    static public void LockAllAccomplishments()
    {
        for (int i = 0; i < AccomplishmentList.Count; i++)
        {
            PlayerPrefs.SetInt(AccomplishmentList[i], 0);
        }
    }
}
