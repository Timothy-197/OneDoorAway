using UnityEngine;
using System.Collections;

//--------------------------------------------------------------------
//InSceneLevelSwitcher keeps track of spawnpoints and respawning
//Switches camera to te one used in that level
//--------------------------------------------------------------------
public class InSceneLevelSwitcher : MonoBehaviour {
    //Level start event (for other scripts to use when the level is changed)
    public delegate void OnLevelStartEvent();
    public static event OnLevelStartEvent OnLevelStart;
    [SerializeField] CharacterControllerBase m_Character = null;
    [SerializeField] InSceneLevel[] m_Levels = null;
    [SerializeField] int m_ButtonSize = 0;
    [SerializeField] int m_ButtonsPerRow = 0;
    [SerializeField] Transform m_Camera = null;
    int m_CurrentIndex;
    
    static InSceneLevelSwitcher g_InSceneLevelSwitcher;
    public static InSceneLevelSwitcher Get()
    {
        if (g_InSceneLevelSwitcher == null)
        {
            g_InSceneLevelSwitcher = FindObjectOfType<InSceneLevelSwitcher>();
            if (g_InSceneLevelSwitcher == null)
            {
                return null;
            }
        }
        return g_InSceneLevelSwitcher;
    }
	void Start () 
	{
		StartLevel(0);
        CorrectCamera();
	}

    void OnGUI()
    {
        for (int i = 0; i < m_Levels.Length; i ++)
        {
            int xIndex = (i) % (m_ButtonsPerRow);
            int yIndex = i / m_ButtonsPerRow;
            int xPos = Screen.width - m_ButtonsPerRow * m_ButtonSize + (xIndex) * m_ButtonSize;
            int yPos = yIndex * m_ButtonSize;

            int index = i;
            if (GUI.Button(new Rect(xPos, yPos, m_ButtonSize, m_ButtonSize), (index+1).ToString()))
            {
                StartLevel(index);
                CorrectCamera();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
    }

    public void SetIndex(int a_Index)
    {
        m_CurrentIndex = a_Index;
    }

    public void Respawn()
    {
        StartLevel(m_CurrentIndex);
        CorrectCamera();
    }
    void CorrectCamera()
    {
        Vector3 diff = m_Character.transform.position - m_Camera.transform.position;
        diff.z = 0;
        m_Camera.transform.position += diff;
    }
    void StartLevel(int a_Index)
    {
        if (a_Index >= m_Levels.Length)
        {
            return;
        }
        m_Character.SpawnAndResetAtPosition(m_Levels[a_Index].m_StartPoint.position);
		m_CurrentIndex = a_Index;
        if (OnLevelStart != null)
        {
            OnLevelStart();
        }    
    }
}
