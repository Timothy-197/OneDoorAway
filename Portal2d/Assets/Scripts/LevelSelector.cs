using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LevelSelector : MonoBehaviour
{
    public GameObject CloseBtn;

    public GameObject levelHolder;
    public GameObject levelIcon;
    public GameObject thisCanvas;
    public int numberOfLevels;
    public Vector2 iconSpacing;

    private Rect panelDimensions;
    private Rect iconDimensions;
    private int amountPerPage;
    private int currentLevelCount;

    public void OpenLevelSelectPanel()
    {
        CloseBtn.SetActive(true);
        levelHolder.SetActive(true);
    }

    public void CloseLevelSelectPanel()
    {
        CloseBtn.SetActive(false);
        levelHolder.SetActive(false);
    }

    private void Start()
    {
        numberOfLevels = LevelManager.levelNameMapping.Count - 1;

        panelDimensions = levelHolder.GetComponent<RectTransform>().rect;
        iconDimensions = levelIcon.GetComponent<RectTransform>().rect;
        int maxInARow = Mathf.FloorToInt((panelDimensions.width + iconSpacing.x) / iconDimensions.width);
        int maxInACol = Mathf.FloorToInt((panelDimensions.height + iconSpacing.y) / iconDimensions.height);
        amountPerPage = maxInARow * maxInACol;
        int totalPages = Mathf.CeilToInt((float)numberOfLevels / amountPerPage);
        LoadPanels(totalPages);

        CloseLevelSelectPanel();
    }

    private void LoadPanels(int numberOfPanels)
    {
        GameObject panelClone = Instantiate(levelHolder) as GameObject;

        PageSwiper swiper = levelHolder.AddComponent<PageSwiper>();
        swiper.totalPages = numberOfPanels;

        for (int i = 1; i <= numberOfPanels; i++)
        {
            GameObject panel = Instantiate(panelClone) as GameObject;
            panel.transform.SetParent(thisCanvas.transform, false);
            panel.transform.SetParent(levelHolder.transform);
            panel.name = "Page-" + i;
            panel.GetComponent<RectTransform>().localPosition = new Vector2(panelDimensions.width * (i - 1), 0);
            SetUpGrid(panel);
            int numberOfIcons = (i == numberOfPanels) ? (numberOfLevels - currentLevelCount) : amountPerPage;
            LoadIcons(numberOfIcons, panel);
        }
        Destroy(panelClone);
    }

    void SetUpGrid(GameObject panel)
    {
        GridLayoutGroup grid = panel.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(iconDimensions.width, iconDimensions.height);
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.spacing = iconSpacing;
    }

    void LoadIcons(int numberOfIcons, GameObject parentObject)
    {
        for (int i = 0; i < numberOfIcons; i++)
        {
            currentLevelCount++;
            GameObject icon = Instantiate(levelIcon) as GameObject;
            icon.transform.SetParent(thisCanvas.transform, false);
            icon.transform.SetParent(parentObject.transform);
            icon.name = "Level " + i;

            int levelIndex = currentLevelCount - 1;
            icon.GetComponentInChildren<TextMeshProUGUI>().SetText("Level " + levelIndex);
            icon.GetComponent<LevelSelectElement>().levelIndex = levelIndex;
            if (levelIndex > PlayerPrefs.GetInt(LevelManager.LEVEL_PROGRESS, 0))
            {
                icon.transform.Find("Lock Image").gameObject.SetActive(true);
            }
        }
    }
}
