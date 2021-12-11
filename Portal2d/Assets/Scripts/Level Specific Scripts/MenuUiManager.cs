using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuUiManager : MonoBehaviour
{
    public GameObject ClearSavePanel;
    public GameObject CheatPanel;


    #region ClearSavePanel
    public void Activate_ClearSavePanel()
    {
        ClearSavePanel.SetActive(true);
    }
    public void Deactivate_ClearSavePanel()
    {
        ClearSavePanel.SetActive(false);
    }
    #endregion


    #region CheatPanel
    public void Activate_CheatPanel()
    {
        CheatPanel.SetActive(true);
    }
    public void Deactivate_CheatPanel()
    {
        CheatPanel.SetActive(false);
    }
    #endregion
}
