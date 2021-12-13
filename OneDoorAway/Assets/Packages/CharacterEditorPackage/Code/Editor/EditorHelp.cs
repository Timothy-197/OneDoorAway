using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorHelp {
    public const int c_LineHeight = 16;
    //http://answers.unity3d.com/questions/684414/custom-editor-foldout-doesnt-unfold-when-clicking.html
    //For the foldout on clicking label solution
    public static bool Foldout(string a_DisplayText, string a_ToolTip = "")
    {
        GUIContent content = new GUIContent(a_DisplayText, a_ToolTip);
        
        Rect rect = GUILayoutUtility.GetRect(40f, 40f, 16f, 16f, EditorStyles.foldout);
        bool result = EditorGUI.Foldout(rect, EditorPrefs.GetBool(a_DisplayText + "FoldoutStorage", true), content, true, EditorStyles.foldout);

        EditorPrefs.SetBool(a_DisplayText + "FoldoutStorage", result);
        return result;
    }
    public static void SerializeRelativeField(SerializedProperty a_MainProperty, string a_RelativeFieldName, string a_ToolTipText = "", string a_DisplayName = "", string a_PropertyType = "")
    {
        SerializedProperty property = a_MainProperty.FindPropertyRelative(a_RelativeFieldName);
        if (property == null)
        {
            Debug.Log("Property " + a_RelativeFieldName + " not found!");
            return;
        }
        SerializeProperty(property, a_ToolTipText, a_DisplayName, a_PropertyType);
    }
    public static void SerializeRelativeField(SerializedObject a_Object, string a_FieldName, string a_ToolTipText = "", string a_DisplayName = "", string a_PropertyType = "")
    {
        SerializedProperty property = a_Object.FindProperty(a_FieldName);
        if (property == null)
        {
            Debug.Log("Property " + a_FieldName + " not found!");
            return;
        }
        SerializeProperty(property, a_ToolTipText, a_DisplayName, a_PropertyType);
    }
    public static void SerializeProperty(SerializedProperty a_Property, string a_ToolTipText = "", string a_DisplayName = "", string a_PropertyType = "")
    { 
        string displayName = (a_DisplayName != "") ? a_DisplayName : a_Property.displayName;
        string toolTipText = (a_ToolTipText != "") ? a_ToolTipText : displayName;
        string propertyType = (a_PropertyType != "") ? a_PropertyType : a_Property.type;
        GUIContent displayContent = new GUIContent(displayName, toolTipText);
        switch (propertyType)
        {
            default:
                EditorGUILayout.PropertyField(a_Property, displayContent);
            break;
            case "range01":
                EditorGUILayout.Slider(a_Property, 0.0f, 1.0f, displayContent);
                break;

        }
    }
    public static void SerializeArray(SerializedProperty a_Property)
    {
        EditorGUILayout.PropertyField(a_Property, true);
    }


    public static void PropertyDrawerLineWithVar(SerializedProperty a_BaseProperty, string a_RelativeVar, ref Rect a_Rect, string a_ToolTip = "", string a_DisplayName = "")
    {
        PropertyDrawerNewLine(ref a_Rect);
        SerializedProperty relativeVarProperty = a_BaseProperty.FindPropertyRelative(a_RelativeVar);
        string displayName = (a_DisplayName != "") ? a_DisplayName : relativeVarProperty.displayName;
        GUIContent content = new GUIContent(displayName, a_ToolTip);
        EditorGUI.PropertyField(a_Rect, relativeVarProperty, content);
    }
    public static bool PropertyDrawerButton(string a_ButtonText, ref Rect a_Rect, string a_ToolTip = "")
    {
        PropertyDrawerNewLine(ref a_Rect);
        Rect drawRect = EditorGUI.IndentedRect(a_Rect);
        drawRect.width = 50.0f;
        drawRect.x = EditorGUIUtility.labelWidth;
        GUIContent content = new GUIContent(a_ButtonText, a_ToolTip);
        return GUI.Button(drawRect, content);
    }
    public static void PropertyDrawerNewLine(ref Rect a_Rect)
    {
        a_Rect.y += c_LineHeight;
    }
}
