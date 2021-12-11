using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AccomplishmentPanel : MonoBehaviour
{
    public Transform AccomplishmentSet;
    public GameObject AccomplishmentInfoPanel;
    public Text AccomplishmentStory;

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
    private Color UNLOCKED_COLOR = new Color(1f, 1f, 1f);

    #region AccomplishmentPanel
    public void ActivateAccomplishmentPanel()
    {
        // go through all accomplishments (children of AccomplishmentSet)
        for (int i = 0; i < AccomplishmentSet.childCount; i++)
        {
            Transform accomplishment = AccomplishmentSet.GetChild(i);
            string AccomplishmentName = accomplishment.name;

            if (!IsAccomplishmentActive(AccomplishmentName))
            {
                accomplishment.Find("Lock Image").gameObject.SetActive(true);  // activate the lock
                accomplishment.GetComponent<Image>().color = LOCKED_COLOR;     // make this accomplishment grey
                accomplishment.GetComponent<Button>().interactable = false;    // do not allow click
            }
            else
            {
                accomplishment.Find("Lock Image").gameObject.SetActive(false);
                accomplishment.GetComponent<Image>().color = UNLOCKED_COLOR;
                accomplishment.GetComponent<Button>().interactable = true;
            }
        }

        this.gameObject.SetActive(true);
    }

    public void DeactivateAccomplishmentPanel()
    {
        this.gameObject.SetActive(false);
    }
    #endregion


    #region AccomplishmentInfoPanel
    public void Activate_AccomplishmentInfoPanel(string storyText)
    {
        //update story text
        AccomplishmentStory.text = storyText;

        AccomplishmentInfoPanel.SetActive(true);
    }
    public void Deactivate_AccomplishmentInfoPanel()
    {
        AccomplishmentStory.text = "No story here...";

        AccomplishmentInfoPanel.SetActive(false);
    }
    #endregion


    #region SAVE
    static public bool IsAllAccomplishmentsActive()
    {
        for (int i = 0; i < AccomplishmentList.Count; i++)
        {
            int status = PlayerPrefs.GetInt(AccomplishmentList[i], 0);

            if (status != 1)
                return false;
        }

        return true;
    }

    static public bool IsAccomplishmentActive(int accomplishmentIndex)
    {
        if (accomplishmentIndex >= 0 && accomplishmentIndex < AccomplishmentList.Count)
        {
            int status = PlayerPrefs.GetInt(AccomplishmentList[accomplishmentIndex], 0);
            return status == 1;
        }
        else
        {
            Debug.Log("IsAccomplishmentActive: invalid accomplishmentIndex");
            return false;
        }
    }

    static public bool IsAccomplishmentActive(string accomplishmentName)
    {
        if (AccomplishmentList.Contains(accomplishmentName))
        {
            int status = PlayerPrefs.GetInt(accomplishmentName, 0);
            return status == 1;
        }
        else
        {
            Debug.Log("IsAccomplishmentActive: invalid accomplishmentName");
            return false;
        }
    }

    static public void ActivateAccomplishment(int accomplishmentIndex)
    {
        if (accomplishmentIndex >= 0 && accomplishmentIndex < AccomplishmentList.Count)
        {
            PlayerPrefs.SetInt(AccomplishmentList[accomplishmentIndex], 1);
        }
        else
        {
            Debug.Log("ActivateAccomplishment: invalid accomplishmentIndex");
        }
    }

    static public void ActivateAccomplishment(string accomplishmentName)
    {
        if (AccomplishmentList.Contains(accomplishmentName))
        {
            PlayerPrefs.SetInt(accomplishmentName, 1);
        }
        else
        {
            Debug.Log("ActivateAccomplishment: invalid accomplishmentName");
        }
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
    #endregion

}
