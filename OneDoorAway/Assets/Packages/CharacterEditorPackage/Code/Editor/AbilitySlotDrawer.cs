using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AbilitySlot))]
public class AbilitySlotDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty a_Property, GUIContent a_Label)
    {
        bool foldOut = a_Property.FindPropertyRelative("m_IsFoldingOut").boolValue;
        int lineAmount = 1;
        if (foldOut)
        {
            lineAmount++;
            SerializedProperty prefabProperty = a_Property.FindPropertyRelative("m_AbilityModulePrefab");
            if (prefabProperty != null)
            {
                lineAmount++;
            }
        }

        return (float)(lineAmount * EditorHelp.c_LineHeight + ((foldOut) ? 4 : 0));
    }

    public override void OnGUI(Rect position, SerializedProperty a_Property, GUIContent a_Label)
    {
        position.height = EditorHelp.c_LineHeight;
        EditorGUI.BeginProperty(position, a_Label, a_Property);

        SerializedProperty foldOutProperty = a_Property.FindPropertyRelative("m_IsFoldingOut");
        SerializedProperty prefabProperty = a_Property.FindPropertyRelative("m_AbilityModulePrefab");
        AbilityModule prefab = null;
        string nameString = "Ability slot empty";
        if (prefabProperty.objectReferenceValue != null)
        {
            prefab = prefabProperty.objectReferenceValue as AbilityModule;
            nameString = prefab.GetName();
        }
        foldOutProperty.boolValue = EditorGUI.Foldout(position, foldOutProperty.boolValue, nameString, true);

        Rect pos = position;

        if (foldOutProperty.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorHelp.PropertyDrawerLineWithVar(a_Property, "m_AbilityModulePrefab", ref pos, "", "Prefab");
            if (prefabProperty.objectReferenceValue != null)
            {
                if (EditorHelp.PropertyDrawerButton("Edit", ref pos))
                {
                    Selection.activeGameObject = prefab.gameObject;
                }
            }
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }
}
