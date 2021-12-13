using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InputElement))]
public class InputPropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty a_Property, GUIContent a_Label)
    {
        bool foldOut = a_Property.FindPropertyRelative("m_IsFoldingOut").boolValue;
        int lineAmount = 1;
        if (foldOut)
        {
            lineAmount += 2;
            InputElement.InputType type = (InputElement.InputType)a_Property.FindPropertyRelative("m_InputType").enumValueIndex;
            switch(type)
            {
                case InputElement.InputType.Button:
                    { 
                        lineAmount += 2;
                        if (a_Property.FindPropertyRelative("m_UnityInputType").enumValueIndex != 0)
                        {
                            lineAmount++;
                        }
                    }
                    break;
                case InputElement.InputType.Direction:
                    lineAmount += 3;
                    break;
            }
        }
        return (float)(lineAmount * EditorHelp.c_LineHeight + ((foldOut) ? 4 : 0));
    }

    public override void OnGUI(Rect position, SerializedProperty a_Property, GUIContent a_Label)
    {
        position.height = EditorHelp.c_LineHeight;
        EditorGUI.BeginProperty(position, a_Label, a_Property);

        SerializedProperty foldOutProperty = a_Property.FindPropertyRelative("m_IsFoldingOut");
        string nameString = a_Property.FindPropertyRelative("m_Name").stringValue;
        if (nameString == "") nameString = "<InputName missing>";

        foldOutProperty.boolValue = EditorGUI.Foldout(position, foldOutProperty.boolValue, nameString, true);

        Rect pos = position;

        if (foldOutProperty.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorHelp.PropertyDrawerLineWithVar(a_Property, "m_Name", ref pos, "Name of this input, as referenced in code");
            EditorHelp.PropertyDrawerLineWithVar(a_Property, "m_InputType", ref pos,"Is this input a button (or key), or directional (analogue stick, arrow keys, WASD)?");

            InputElement.InputType type = (InputElement.InputType)a_Property.FindPropertyRelative("m_InputType").enumValueIndex;
            switch (type)
            {
                case InputElement.InputType.Button:
                    {
                        EditorHelp.PropertyDrawerLineWithVar(a_Property, "m_UnityInputType", ref pos, "Is this a button, or can it be triggered by an axis as well (triggers on a gamepad for example)");
                        if (a_Property.FindPropertyRelative("m_UnityInputType").enumValueIndex != 0)
                        {
                            EditorHelp.PropertyDrawerLineWithVar(a_Property, "m_UnityAxisRecognition", ref pos, "If this button can be triggered by an axis, should it trigger if the axis value is: positive only, negative only, or not zero");
                        }
                        EditorHelp.PropertyDrawerLineWithVar(a_Property, "m_ButtonName", ref pos, "In Unity's input manager, what is the name of the button/axis that this input uses?");
                    }
                    break;
                case InputElement.InputType.Direction:
                    {
                        EditorHelp.PropertyDrawerLineWithVar(a_Property, "m_HorizontalAxisName", ref pos, "In Unity's input manager, what is the name of the axis that this input uses for its horizontal value?");
                        EditorHelp.PropertyDrawerLineWithVar(a_Property, "m_VerticalAxisName", ref pos, "In Unity's input manager, what is the name of the axis that this input uses for its vertical value?");
                        EditorHelp.PropertyDrawerLineWithVar(a_Property, "m_DirectionThreshold", ref pos, "In the case of analogue input, how far should the input be pushed before a direction is recognized? For example: how far should an analogue stick be pulled down before it registers as a crouch command");
                    }
                    break;
            }
            EditorGUI.indentLevel--;
        }
        
        EditorGUI.EndProperty();
    }
}
