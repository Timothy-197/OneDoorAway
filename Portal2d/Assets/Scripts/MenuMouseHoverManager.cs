using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMouseHoverManager : MonoBehaviour
{
    public GameObject CheatBtnInfo;


    public void CheatBtn_ShowInfo()
    {
        CheatBtnInfo.SetActive(true);
    }
    public void CheatBtn_HideInfo()
    {
        CheatBtnInfo.SetActive(false);
    }
}
