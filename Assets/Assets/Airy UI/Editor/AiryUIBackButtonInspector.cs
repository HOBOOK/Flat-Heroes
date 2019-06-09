using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AiryUIBackButton))]
[CanEditMultipleObjects]
public class AiryUIBackButtonInspector : Editor
{
    private AiryUIBackButton backButtonP;

    private SerializedProperty _withGraphic;
    private SerializedProperty _showDelay;
    private SerializedProperty _font;
    private SerializedProperty _position;
    private SerializedProperty _graphicType;
    private SerializedProperty _imageColor;
    private SerializedProperty _textColor;
    private SerializedProperty _buttonText;
    private SerializedProperty _graphicSprite;
    private SerializedProperty _scale;
    private SerializedProperty _offsetX;
    private SerializedProperty _offsetY;

    private void OnEnable()
    {
        backButtonP = (AiryUIBackButton)target;

        _withGraphic = serializedObject.FindProperty("withGraphic");
        _showDelay = serializedObject.FindProperty("showDelay");
        _position = serializedObject.FindProperty("position");
        _graphicType = serializedObject.FindProperty("graphicType");
        _imageColor = serializedObject.FindProperty("imageColor");
        _textColor = serializedObject.FindProperty("textColor");
        _font = serializedObject.FindProperty("font");
        _buttonText = serializedObject.FindProperty("buttonText");
        _graphicSprite = serializedObject.FindProperty("graphicSprite");
        _scale = serializedObject.FindProperty("scale");
        _offsetX = serializedObject.FindProperty("offsetX");
        _offsetY = serializedObject.FindProperty("offsetY");
    }

    public override void OnInspectorGUI()
    {
        ShowGraphics();

        serializedObject.ApplyModifiedProperties();
    }

    private void ShowGraphics()
    {
        GUILayout.Space(20);

        GUI.color = Color.cyan;
        EditorGUILayout.PropertyField(_withGraphic);
        GUI.color = Color.white;

        GUILayout.Space(20);

        if (backButtonP.withGraphic)
        {
            EditorGUILayout.PropertyField(_showDelay);
            EditorGUILayout.PropertyField(_graphicType);

            EditorGUILayout.PropertyField(_scale);
            EditorGUILayout.PropertyField(_offsetX);
            EditorGUILayout.PropertyField(_offsetY);

            if (backButtonP.graphicType == AiryUIBackButton.GraphicType.Image)
            {
                EditorGUILayout.PropertyField(_graphicSprite);
                EditorGUILayout.PropertyField(_imageColor);
            }
            else if (backButtonP.graphicType == AiryUIBackButton.GraphicType.Text)
            {
                EditorGUILayout.PropertyField(_buttonText);
                EditorGUILayout.PropertyField(_font);
                EditorGUILayout.PropertyField(_textColor);
            }
            else if (backButtonP.graphicType == AiryUIBackButton.GraphicType.Both)
            {
                EditorGUILayout.PropertyField(_graphicSprite);
                EditorGUILayout.PropertyField(_imageColor);
                EditorGUILayout.PropertyField(_textColor);
                EditorGUILayout.PropertyField(_font);
                EditorGUILayout.PropertyField(_buttonText);
            }

            GUILayout.Space(20);

            EditorGUILayout.PropertyField(_position);

            GUILayout.Space(20);
            
            EditorGUILayout.HelpBox("if with graphic is false, it will work only on devices that has back button, and ESC button on standalone devices. If true, there will be a button on the screen", MessageType.Info);
        }
    }
}