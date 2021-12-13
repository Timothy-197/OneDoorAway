using UnityEngine;
using UnityEditor;
using System.Collections;

public class CharacterInputEditor {

	public static void ShowTab(CharacterControllerBase a_CharacterController)
    {
        if (a_CharacterController == null)
        {
            GUILayout.Label("CharacterController script not found on object", EditorStyles.boldLabel);
            return;
        }
        PlayerInput playerInput = a_CharacterController.GetComponent<PlayerInput>();
        if (playerInput)
        {
            SerializedObject inputObject = new SerializedObject(playerInput);
            EditorGUI.BeginChangeCheck();

            float prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth += 15.0f;

            GUILayout.Label("List of inputs", EditorStyles.boldLabel);
            GUILayout.Space(4.0f);

            SerializedProperty array = inputObject.FindProperty("m_Inputs");
            if (array != null)
            {
                EditorHelp.SerializeArray(array);
            }

            EditorGUIUtility.labelWidth = prevLabelWidth;
            
            if (EditorGUI.EndChangeCheck())
            {
                inputObject.ApplyModifiedProperties();
            }
        }
        else
        {
            GUILayout.Label("PlayerInput script not found on object", EditorStyles.boldLabel);
        }     
    }
}
