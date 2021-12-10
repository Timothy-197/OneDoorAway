using UnityEngine;
using UnityEditor;
using System.Collections;

public class CharacterMovingColliderEditor {

	public static void ShowTab(CharacterControllerBase a_CharacterController)
    {
        if (a_CharacterController == null)
        {
            GUILayout.Label("CharacterController script not found on object", EditorStyles.boldLabel);
            return;
        }
        CapsuleMovingColliderSolver movingColliderSolver = a_CharacterController.GetComponent<CapsuleMovingColliderSolver>();
        if (movingColliderSolver)
        {
            SerializedObject solverObject = new SerializedObject(movingColliderSolver);
            EditorGUI.BeginChangeCheck();

            float prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth += 120.0f;

            GUILayout.Label("Handling moving colliders", EditorStyles.boldLabel);
            GUILayout.Space(4.0f);

            EditorHelp.SerializeRelativeField(solverObject, "m_ApplyLastMovementAsVelocity", "Should the character inherit velocity from the moving object it was connected with, if the character loses connection with that object? For example: If the character jumps from a moving platform, should it keep the velocity of the moving platform", "Inherit velocity from moving platforms");
            if (solverObject.FindProperty("m_ApplyLastMovementAsVelocity").boolValue)
            {
                EditorHelp.SerializeRelativeField(solverObject, "m_DisableDownwardsLastMovement", "If the character inherits velocity from the moving object, should it prevent getting any downwards velocity? If the character is jumping from a platform that is moving down, should it ignore the downward velocity (checked), or should it also inherit that (unchecked)", "Inherit downward velocity or ignore it");
            }
            EditorHelp.SerializeRelativeField(solverObject, "m_UseEscapeAcceleration", "Should this character detach from a moving platform if the acceleration is too large? For example: if a character is standing on a platform which suddenly drops down, should the character detach from that platform (checked), or should the character stick to that platform (unchecked). Does not work if the platform is (partially) moving in the direction of the character (if a platform the character is standing on suddenly moves up, the character will stay connected)", "Detach from fast moving platforms");
            if (solverObject.FindProperty("m_UseEscapeAcceleration").boolValue)
            {
                EditorHelp.SerializeRelativeField(solverObject, "m_EscapeAcceleration", "If the above is checked, what is the acceleration needed for the character to detach? If the character is standing on a platform that moves away, how fast should it drop before the character detaches?", "Acceleration needed to detach");
            }

            EditorGUIUtility.labelWidth = prevLabelWidth;

            if (EditorGUI.EndChangeCheck())
            {
                solverObject.ApplyModifiedProperties();
            }
        }
        else
        {
            GUILayout.Label("CapsuleMovingColliderSolver script not found on object", EditorStyles.boldLabel);
        }     
    }
}
