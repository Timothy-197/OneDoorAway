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
        {3, "03_PortalRespawn_tut" }, 
        {4, "n_GlassBreak" }, 
        {5, "n_ShotAcross" }, 
        {6, "n_SpeedSum 1" },
        {7, "Level-Ending" },
        {8, "Level-Bonus" }
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
            
            if (levelNameMapping[currentLevel] == "Level-Bonus")
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
        Application.OpenURL("https://github.com/Timothy-197/Portal2d");
    }
}
