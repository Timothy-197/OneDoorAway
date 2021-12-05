using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public static LevelManager _instance;           //singleton
    public static int currentLevel { get; set; }

    static Dictionary<int, string> levelNameMapping = new Dictionary<int, string> {
        {0, "Level00" }
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

    public void loadLevel(int levelIndex)
    {
        currentLevel = levelIndex;

        SceneManager.LoadScene(levelNameMapping[levelIndex], LoadSceneMode.Single);
    }
}
