using UnityEngine;
using UnityEditor;
using System.Collections;

public class CharacterEditorWindow : EditorWindow
{
    enum WindowState
    {
        Character = 0,
        Abilities = 1,
        Input = 2,
        MovingColliders = 3,
        WindowStateCount = 4
    }
    WindowState m_WindowState;
    GameObject m_SelectedObject;
    CharacterControllerBase m_CharacterController;
    GUIStyle m_TabPressedStyle;
    Vector2 m_ScrollPosition;
    [MenuItem("Window/CharacterEditor")]
    static void ShowWindow()
    {
        CharacterEditorWindow window = (CharacterEditorWindow)GetWindow(typeof(CharacterEditorWindow));
        window.UpdateSelection();

        Vector2 size = window.minSize;
        size.x = 300.0f;
        window.minSize = size;
    }

    void OnDestroy()
    {
        m_CharacterController = null;
    }

    void OnSelectionChange()
    {
        UpdateSelection();
    }

    void UpdateSelection()
    {
        m_CharacterController = null;
        m_SelectedObject = Selection.activeGameObject;
        if (m_SelectedObject != null)
        { 
            if (m_SelectedObject.GetComponent<CharacterControllerBase>())
            { 
                m_CharacterController = Selection.activeGameObject.GetComponent<CharacterControllerBase>();
            }
        }
        
        Repaint();
    }

    void OnGUI()
    {
        if (m_SelectedObject == null)
        {
            GUILayout.Label("No object selected", EditorStyles.boldLabel);
            return;
        }
        if (m_SelectedObject != null && m_CharacterController == null)
        {
            if (m_SelectedObject.GetComponent<CharacterControllerBase>())
            {
                m_CharacterController = m_SelectedObject.GetComponent<CharacterControllerBase>();
            }
            else
            { 
                GUILayout.Label("Selected object is not a character", EditorStyles.boldLabel);
                CharacterFixEditor.ShowFixCharacterButton(m_SelectedObject);
                return;
            }
        }
        if (!CharacterFixEditor.IsCharacterAlright(m_SelectedObject))
        {
            CharacterFixEditor.ShowFixCharacterButton(m_SelectedObject);
        }
        GUILayout.Label(m_CharacterController.transform.name, EditorStyles.boldLabel);
        if (Application.isPlaying)
        {
            GUILayout.Label("Edits made during play will not be saved!", EditorStyles.boldLabel);
        }
        DrawSwitchTabs();
        m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
        switch (m_WindowState)
        {
            case WindowState.Character:
                BasicCharacterEditor.ShowTab(m_CharacterController);
                break;
            case WindowState.Abilities:
                CharacterAbilityEditor.ShowTab(m_CharacterController);
                break;
            case WindowState.Input:
                CharacterInputEditor.ShowTab(m_CharacterController);
                break;
            case WindowState.MovingColliders:
                CharacterMovingColliderEditor.ShowTab(m_CharacterController);
                break;
        }
        EditorGUILayout.EndScrollView();
    }

    void DrawSwitchTabs()
    {
        GUILayout.BeginHorizontal();

        for (int i = 0; i < (int)WindowState.WindowStateCount; i ++)
        {
            WindowState state = (WindowState)i;
            GUIStyle style = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button;
            if (m_WindowState == state)
            {
                style = GetTabPressedStyle();
            }
            if (GUILayout.Button(state.ToString(), style))
            {
                m_WindowState = state;
            }
            GUILayout.Space(-6.5f);
        }

        GUILayout.EndHorizontal();
    }

    GUIStyle GetTabPressedStyle()
    {
        GUISkin editorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
        m_TabPressedStyle = new GUIStyle(editorSkin.button);
        m_TabPressedStyle.normal.textColor = m_TabPressedStyle.active.textColor;
        m_TabPressedStyle.normal.background = m_TabPressedStyle.active.background;
        return m_TabPressedStyle;
    }
}
