using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public static LevelManager _instance;           //singleton
    public static int currentLevel { get; set; }

    public Animator animator;                       // set in inspector

    // save (PlayerPrefs)
    public const string LEVEL_PROGRESS = "LevelProgress";

    public static Dictionary<int, string> levelNameMapping = new Dictionary<int, string> {
        {-1, "Menu" },
        {0, "00_Start" },
        {1, "01_BasicTut" },
        {2, "02_Portal_tut" },
        {3, "03_portaltut" },
        {4, "04_GlassBreak" },
        {5, "05_shotAcross" },
        {6, "06_SpeedNumTut" },
        {7, "07_SpeedSum 1" },
        {8, "08_PortalRespawn" },
        {9, "09_SpeedSumFinal" },
        {10, "10_trigger_tut" },
        {11, "11_trigger2" },
        {12, "12_trigger3" },
        {13, "13_trigger4" },
        {14, "14_gravity_tut" },
        {15, "15_gravity2" },
        {16, "Level-Ending" },
        {17, "Level-Bonus" }
    };

    private void Awake()
    {
        if (_instance != null) Destroy(_instance);

        _instance = this;
    }

    #region SAVE_FEATURE
    private void ClearSave()
    {
        PlayerPrefs.SetInt(LEVEL_PROGRESS, 0);

        AccomplishmentPanel.LockAllAccomplishments();
    }

    private void UpdateLevelProgress(int level)
    {
        if (level > PlayerPrefs.GetInt(LEVEL_PROGRESS, 0))
            PlayerPrefs.SetInt(LEVEL_PROGRESS, level);
    }

    public void UnlockAllLevels()
    {
        Debug.Log("Unlock all levels: LevelProgress set to " + (levelNameMapping.Count - 1 - 1));
        PlayerPrefs.SetInt(LEVEL_PROGRESS, levelNameMapping.Count - 1 - 1);

        AccomplishmentPanel.UnlockAllAccomplishments();
    }
    #endregion

    public void RestartCurrentLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void BackToMenu()
    {
        loadLevel(-1);
    }
    public void StartOver()
    {
        // clear save
        ClearSave();

        loadLevel(0);
    }

    public void LoadCurrentLevel()
    {
        loadLevel(currentLevel);
    }

    public void loadLevel(int levelIndex)
    {
        // update save if needed
        UpdateLevelProgress(levelIndex);

        // player fade out animation
        if (animator != null)
            animator.SetTrigger("FadeOut");
        else
            Debug.Log("LevelManager: animator is not set.");

        if (levelIndex != -1)             //not menu
            currentLevel = levelIndex;

        StartCoroutine("WaitAndChangeLoadScene", levelIndex);
    }

    IEnumerator WaitAndChangeLoadScene(int levelIndex)
    {
        yield return new WaitForSeconds(0.9f);
        animator.ResetTrigger("FadeOut");
        SceneManager.LoadScene(levelNameMapping[levelIndex], LoadSceneMode.Single);
    }

    public void loadNextLevel()
    {
        if (levelNameMapping.ContainsKey(currentLevel + 1))
        {
            loadLevel(currentLevel + 1);
        } 
        else
        {
            Debug.Log("No next level available.");
            
            if (SceneManager.GetActiveScene().name == "Level-Bonus")
                BackToMenu();
            else
                loadLevel(currentLevel);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LinkToGithub()
    {
        Application.OpenURL("https://github.com/Timothy-197/OneDoorAway");
    }
}
