using UnityEngine;
using UnityEditor;
using System.Collections;

public class BasicCharacterEditor {

	public static void ShowTab(CharacterControllerBase a_CharacterController)
    {
        if (a_CharacterController == null)
        {
            GUILayout.Label("CharacterController script not found on object", EditorStyles.boldLabel);
            return;
        }
        
        GroundedCharacterController groundedChar = (GroundedCharacterController)a_CharacterController;
        ControlledCapsuleCollider capsuleCol = (ControlledCapsuleCollider)a_CharacterController.GetCollider();
        if (groundedChar && capsuleCol)
        {
            SerializedObject charObject = new SerializedObject(groundedChar);
            SerializedObject colObject = new SerializedObject(capsuleCol);
            EditorGUI.BeginChangeCheck();

            float prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth += 50.0f;

            GUILayout.Label("Variables for movement", EditorStyles.boldLabel);
            GUILayout.Space(4.0f);

            if (EditorHelp.Foldout("Basic character settings"))
            {
                EditorGUI.indentLevel++;
                EditorHelp.SerializeRelativeField(colObject, "m_LayerMask","","Collision mask");
                EditorHelp.SerializeRelativeField(colObject, "m_Length", "The length of the capsule (not including both demispheres)", "Capsule length");
                EditorHelp.SerializeRelativeField(colObject, "m_Radius", "The radius of the capsule", "Capsule radius");
                EditorHelp.SerializeRelativeField(charObject, "m_Gravity", "How fast a character falls downwards");
                EditorHelp.SerializeRelativeField(charObject, "m_DragConstant", "A force that slows down the character (low values work best!)", "Drag");
                GUILayout.Space(4.0f);
                EditorGUI.indentLevel--;
            }

            if (EditorHelp.Foldout("Running"))
            {
                EditorGUI.indentLevel++;
                EditorHelp.SerializeRelativeField(charObject, "m_WalkForce", "How fast a character can accelerate", "Character acceleration");
                EditorHelp.SerializeRelativeField(charObject, "m_WalkForceApplyLimit", "The maximum speed a character can accelerate to by running", "Max running speed");
                EditorHelp.SerializeRelativeField(charObject, "m_StoppingForce", "How fast a character can stop running when the player releases run input", "Brake deceleration");
                EditorHelp.SerializeRelativeField(charObject, "m_ApplyStoppingForceWhenActivelyBraking", "Whether the character uses Brake deceleration when the player turns around mid-run (prevents sliding)", "Brake when changing direction");
                EditorHelp.SerializeRelativeField(charObject, "m_FrictionConstant", "Slows down the character when its on the ground", "Friction");
                GUILayout.Space(4.0f);
                EditorGUI.indentLevel--;
            }

            if (EditorHelp.Foldout("Jumping"))
            {
                EditorGUI.indentLevel++;
                EditorHelp.SerializeRelativeField(charObject, "m_JumpVelocity", "The velocity that is applied to the character on jump", "Jump velocity");
                EditorHelp.SerializeRelativeField(charObject, "m_JumpCutVelocity", "The vertical velocity that the character will use when the jump button is let go after jumping (this leads to different jump heights)", "Low jump velocity");
                EditorHelp.SerializeRelativeField(charObject, "m_MinAllowedJumpCutVelocity", "If the jump button is let go after jumping, and its vertical velocity is below this threshold, the character will switch to Low jump velocity (value above this one) ", "Velocity threshold for low jump");
                EditorHelp.SerializeRelativeField(charObject, "m_HorizontalJumpBoostFactor", "How much of the horizontal velocity should be added to the character's velocity when jumping", "Horizontal jump boost factor", "range01");
                EditorHelp.SerializeRelativeField(charObject, "m_JumpAlignedToGroundFactor", "How much the jump direction should be influenced by the ground (0 is jumping straight up, 1 is jumping away from the ground)", "Jump aligned to ground factor", "range01");
                EditorHelp.SerializeRelativeField(charObject, "m_GroundedToleranceTime", "The time (in seconds) after not being grounded where a jump press is still valid. If a player presses jump just after falling off a platform, the character will still jump. ", "Jump grounded tolerance time");
                EditorHelp.SerializeRelativeField(charObject, "m_JumpCacheTime", "The time (in seconds) after which a jump press is still valid. If a player presses jump before standing on the ground, but lands within this amount of seconds, the character will still jump", "Jump input valid time");
                EditorHelp.SerializeRelativeField(charObject, "m_ResetVerticalSpeedOnJumpIfMovingDown", "If the character is running downhill and the player presses jump, should the downwards velocity be reset before jumping? Could otherwise lead to low jumps in this situation", "Reset down velocity on jump");
                GUILayout.Space(4.0f);
                EditorGUI.indentLevel--;
            }
            if (EditorHelp.Foldout("Air control"))
            {
                EditorGUI.indentLevel++;
                EditorHelp.SerializeRelativeField(charObject, "m_AirControl", "How fast a character can accelerate in the air (as a factor of its regular acceleration). 0 is no acceleration, 1 is just as fast as on the ground", "In-air character acceleration", "range01");
                EditorHelp.SerializeRelativeField(charObject, "m_AirForceApplyLimit", "The maximum speed a character can accelerate to by moving in the air. ", "Max horizontal in-air speed");
                GUILayout.Space(4.0f);
                EditorGUI.indentLevel--;
            }
            if (EditorHelp.Foldout("Environmental context"))
            {
                EditorGUI.indentLevel++;
                EditorHelp.SerializeRelativeField(colObject, "m_MaximumGroundedAngle", "The maximum angle a character can still walk on(from straight up)", "Max ground angle");
                EditorHelp.SerializeRelativeField(colObject, "m_MaximumWallAngle", "The maximum angle a character can still align to a wall with(from straight up)", "Max wall angle");
                EditorHelp.SerializeRelativeField(colObject, "m_MaxGrabAngle", "The maximum angle of the surface on a ledge where a character can still grab on to (from straight up)", "Max edge ground angle");
                EditorHelp.SerializeRelativeField(colObject, "m_MaxEdgeAlignAngle", "The maximum angle of the wall before a ledge where a character can still align to (from straight up)", "Max edge wall angle");
                GUILayout.Space(4.0f);
                EditorGUI.indentLevel--;
            }
            if (EditorHelp.Foldout("Various"))
            {
                EditorGUI.indentLevel++;
                EditorHelp.SerializeRelativeField(charObject, "m_ApplyGravityOnGround", "Should the character apply gravity when it's standing on the ground?", "Gravity active on ground");
                if (charObject.FindProperty("m_ApplyGravityOnGround").boolValue)
                {
                    EditorHelp.SerializeRelativeField(charObject, "m_ApplyGravityIntoGroundNormal", "Should the gravity on the ground be pointed in the direction of the ground (when checked), or should it point straight down (when unchecked)? If unchecked, gravity will pull the character downhill", "Gravity directed into ground");
                }
                GUILayout.Space(4.0f);
                EditorGUI.indentLevel--;
            }
            
            EditorGUIUtility.labelWidth = prevLabelWidth;

            if (EditorGUI.EndChangeCheck())
            {
                charObject.ApplyModifiedProperties();
                colObject.ApplyModifiedProperties();
            }
        }
        else
        {
            if (!groundedChar)
                GUILayout.Label("This is not a grounded character controller", EditorStyles.boldLabel);
            if (!capsuleCol)
                GUILayout.Label("This character does not have a controlled capsule collider script", EditorStyles.boldLabel);
        }
    }
}
