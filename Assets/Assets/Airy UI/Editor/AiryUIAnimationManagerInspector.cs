using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AiryUIAnimationManager))]
[CanEditMultipleObjects]
public class AiryUIAnimationManagerInspector : Editor
{
    private AiryUIAnimationManager animationPManager;

    private SerializedProperty _showMenuOnEnable;

    private void OnEnable()
    {
        animationPManager = (AiryUIAnimationManager)target;

        _showMenuOnEnable = serializedObject.FindProperty("showMenuOnEnable");
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(20);

        EditorGUILayout.HelpBox("Note: The children elements that have animation element must be enabled so that the manager can detect them", MessageType.Info);

        GUILayout.Space(20);

        EditorGUILayout.PropertyField(_showMenuOnEnable);

        GUILayout.Space(20);

        if (GUILayout.Button("Update Animated Elements"))
        {
            animationPManager.UpdateElementsInChildren();
            Debug.Log("Updated UI Elements - current UI animation elements in menu : " + animationPManager.childrenElements.Length);
        }

        GUILayout.Space(20);

        serializedObject.ApplyModifiedProperties();
    }
}