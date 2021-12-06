using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public static LevelManager _instance;           //singleton
    public static int currentLevel { get; set; }

    public Animator animator;                       // set in inspector

    static Dictionary<int, string> levelNameMapping = new Dictionary<int, string> {
        {-1, "Menu" }, {0, "Level00" }, {1, "Level01" }, {2, "Level02" }, {3, "Level03" }, {4, "Level04" }, {5, "Level05" }
    };

    private void Awake()
    {
        if (_instance != null) Destroy(_instance);

        _instance = this;
    }

    private void Start()
    {
        //Debug.Log("currentLevel = " + currentLevel);
    }

    public void RestartCurrentLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void BackToMenu()
    {
        loadLevel(-1);
    }

    public void LoadCurrentLevel()
    {
        loadLevel(currentLevel);
    }

    public void loadLevel(int levelIndex)
    {
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
            Debug.Log("No next level available. Reload current level");
            loadLevel(currentLevel);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
