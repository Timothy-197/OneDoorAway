using UnityEngine;
using UnityEditor;
using System.Collections;

public class CharacterAbilityEditor {

	public static void ShowTab(CharacterControllerBase a_CharacterController)
    {
        if (Application.isPlaying)
        {
            GUILayout.Label("Abilities cannot be edited during play!", EditorStyles.boldLabel);
            return;
        }
        if (a_CharacterController == null)
        {
            GUILayout.Label("CharacterController script not found on object", EditorStyles.boldLabel);
            return;
        }
        AbilityModuleManager abilityModuleManager = a_CharacterController.GetAbilityModuleManager();
        if (abilityModuleManager)
        {
            SerializedObject abilityManagerObject = new SerializedObject(abilityModuleManager);
            EditorGUI.BeginChangeCheck();

            float prevLabelWidth = EditorGUIUtility.labelWidth;
            //EditorGUIUtility.labelWidth += 50.0f;

            GUILayout.Label("List of abilities", EditorStyles.boldLabel);
            GUILayout.Space(4.0f);

            SerializedProperty array = abilityManagerObject.FindProperty("m_AbilitySlots");
            if (array != null)
            {
                EditorHelp.SerializeArray(array);
            }

            EditorGUIUtility.labelWidth = prevLabelWidth;
            
            if (EditorGUI.EndChangeCheck())
            {
                abilityManagerObject.ApplyModifiedProperties();
            }
        }
        else
        {
            GUILayout.Label("AbilityModuleManager script not found on object", EditorStyles.boldLabel);
        }     
    }
}
