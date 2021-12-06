using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectElement : MonoBehaviour
{
    public int levelIndex { get; set; }

    public void LoadCorrespondingLevel()
    {
        if (levelIndex <= PlayerPrefs.GetInt(LevelManager.LEVEL_PROGRESS, 0))
            LevelManager._instance.loadLevel(levelIndex);
        else
            Debug.Log("LevelSelectElement: this level is locked");
    }
}
