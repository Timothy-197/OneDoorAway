using UnityEngine;
using UnityEditor;
using System.Collections;

public class CharacterFixEditor
{

    public static void ShowFixCharacterButton(GameObject a_Object)
    {
        if (a_Object == null)
        {
            GUILayout.Label("No object found", EditorStyles.boldLabel);
            return;
        }
        SerializedObject serializedObject = new SerializedObject(a_Object);
        EditorGUI.BeginChangeCheck();

        

        string fixText = "Create character on this object";
        if (a_Object.GetComponent<GroundedCharacterController>())
        {
            fixText = "Fix character";
            GUILayout.Label("Character is missing components, or links to components. Fix it?");
        }
        else
        {
            GUILayout.Label("Character script could not be found on this object. Create it?");
        }
        GUIContent content = new GUIContent(fixText, "Tries to find missing components of character controller and link them up. Creates default components if none can be found");
        if (GUILayout.Button(content))
        {
            FixCharacterOnObject(a_Object);
        }


        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    public static bool IsCharacterAlright(GameObject a_Object)
    {
        if (a_Object == null)
        {
            return false;
        }
        GroundedCharacterController character = a_Object.GetComponent<GroundedCharacterController>();
        if (character == null)
        {
            return false;
        }
        ControlledCapsuleCollider controlledCapsuleCollider = a_Object.GetComponent<ControlledCapsuleCollider>();
        if (controlledCapsuleCollider == null)
        {
            return false;
        }
        PlayerInput input = a_Object.GetComponent<PlayerInput>();
        if (input == null)
        {
            return false;
        }
        if (!input.DoesInputExist("Move") || !input.DoesInputExist("Jump"))
        {
            return false;
        }
        AbilityModuleManager abilityModuleManager = a_Object.GetComponent<AbilityModuleManager>();
        if (abilityModuleManager == null)
        {
            return false;
        }
        CapsuleMovingColliderSolver capsuleMovingColliderSolver = a_Object.GetComponent<CapsuleMovingColliderSolver>();
        if (capsuleMovingColliderSolver == null)
        {
            return false;
        }
        CapsuleCollider capsuleCollider = a_Object.GetComponent<CapsuleCollider>();
        if (capsuleCollider == null)
        {
            return false;
        }
        Rigidbody rigidbody = a_Object.GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            return false;
        }
        if (rigidbody.constraints != RigidbodyConstraints.FreezeAll || rigidbody.useGravity != false || rigidbody.angularDrag != 0.0f)
        {
            return false;
        }
        CapsuleVolumeIntegrity volumeIntegrity = a_Object.GetComponent<CapsuleVolumeIntegrity>();
        if (volumeIntegrity == null)
        {
            return false;
        }

        SerializedObject serializedObject = new SerializedObject(character);
        if (serializedObject.FindProperty("m_ControlledCollider").objectReferenceValue == null) return false;
        if (serializedObject.FindProperty("m_AbilityManager").objectReferenceValue == null) return false;

        serializedObject = new SerializedObject(controlledCapsuleCollider);
        if (serializedObject.FindProperty("m_CapsuleMovingColliderSolver").objectReferenceValue == null) return false;
        if ((1 << a_Object.layer & serializedObject.FindProperty("m_LayerMask").intValue) != 0 ||
            (1 << LayerMask.NameToLayer("Ignore Raycast") & serializedObject.FindProperty("m_LayerMask").intValue) != 0)
        {
            return false;
        }

        serializedObject = new SerializedObject(input);
        if (serializedObject.FindProperty("m_CharacterController").objectReferenceValue == null) return false;

        serializedObject = new SerializedObject(capsuleMovingColliderSolver);
        if (serializedObject.FindProperty("m_ControlledCollider").objectReferenceValue == null) return false;

        serializedObject = new SerializedObject(volumeIntegrity);
        if (serializedObject.FindProperty("m_RigidBody").objectReferenceValue == null) return false;
        if (serializedObject.FindProperty("m_ControlledCollider").objectReferenceValue == null) return false;
        if (serializedObject.FindProperty("m_CapsuleCollider").objectReferenceValue == null) return false;

        return true;
    }


    static void FixCharacterOnObject(GameObject a_Object)
    {
        //Find, or create missing components
        GroundedCharacterController character = a_Object.GetComponent<GroundedCharacterController>();
        if (character == null)
        {
            character = a_Object.AddComponent<GroundedCharacterController>();
        }
        ControlledCapsuleCollider controlledCapsuleCollider = a_Object.GetComponent<ControlledCapsuleCollider>();
        if (controlledCapsuleCollider == null)
        {
            controlledCapsuleCollider = a_Object.AddComponent<ControlledCapsuleCollider>();
        }
        PlayerInput input = a_Object.GetComponent<PlayerInput>();
        if (input == null)
        {
            input = a_Object.AddComponent<PlayerInput>();
        }
        input.EnsureJumpAndMoveInputsAreSet();
        AbilityModuleManager abilityModuleManager = a_Object.GetComponent<AbilityModuleManager>();
        if (abilityModuleManager == null)
        {
            abilityModuleManager = a_Object.AddComponent<AbilityModuleManager>();
        }
        CapsuleMovingColliderSolver capsuleMovingColliderSolver = a_Object.GetComponent<CapsuleMovingColliderSolver>();
        if (capsuleMovingColliderSolver == null)
        {
            capsuleMovingColliderSolver = a_Object.AddComponent<CapsuleMovingColliderSolver>();
        }
        CapsuleCollider capsuleCollider = a_Object.GetComponent<CapsuleCollider>();
        if (capsuleCollider == null)
        {
            capsuleCollider = a_Object.AddComponent<CapsuleCollider>();
        }
        Rigidbody rigidbody = a_Object.GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            rigidbody = a_Object.AddComponent<Rigidbody>();
        }
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        rigidbody.useGravity = false;
        rigidbody.angularDrag = 0.0f;
        CapsuleVolumeIntegrity volumeIntegrity = a_Object.GetComponent<CapsuleVolumeIntegrity>();
        if (volumeIntegrity == null)
        {
            volumeIntegrity = a_Object.AddComponent<CapsuleVolumeIntegrity>();
        }


        //Link up the components to one another, using the serializedobject interface
        SerializedObject serializedObject = new SerializedObject(character);
        serializedObject.FindProperty("m_ControlledCollider").objectReferenceValue = controlledCapsuleCollider as ControlledCollider;
        serializedObject.FindProperty("m_AbilityManager").objectReferenceValue = abilityModuleManager;
        serializedObject.ApplyModifiedProperties();

        serializedObject = new SerializedObject(controlledCapsuleCollider);
        serializedObject.FindProperty("m_CapsuleMovingColliderSolver").objectReferenceValue = capsuleMovingColliderSolver;
        if ((1 << a_Object.layer & serializedObject.FindProperty("m_LayerMask").intValue) != 0 ||
            (1 << LayerMask.NameToLayer("Ignore Raycast") & serializedObject.FindProperty("m_LayerMask").intValue) != 0)
        {
            serializedObject.FindProperty("m_LayerMask").intValue = ~((1 << a_Object.layer) + (1 << LayerMask.NameToLayer("Ignore Raycast")));
        }
        serializedObject.ApplyModifiedProperties();

        serializedObject = new SerializedObject(input);
        serializedObject.FindProperty("m_CharacterController").objectReferenceValue = character as CharacterControllerBase;
        serializedObject.ApplyModifiedProperties();

        serializedObject = new SerializedObject(capsuleMovingColliderSolver);
        serializedObject.FindProperty("m_ControlledCollider").objectReferenceValue = controlledCapsuleCollider;
        serializedObject.ApplyModifiedProperties();

        serializedObject = new SerializedObject(volumeIntegrity);
        serializedObject.FindProperty("m_RigidBody").objectReferenceValue = rigidbody;
        serializedObject.FindProperty("m_ControlledCollider").objectReferenceValue = controlledCapsuleCollider;
        serializedObject.FindProperty("m_CapsuleCollider").objectReferenceValue = capsuleCollider;
        serializedObject.ApplyModifiedProperties();
    }
}
