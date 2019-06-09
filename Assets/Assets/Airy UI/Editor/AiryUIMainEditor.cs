using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AiryUIMainEditor : EditorWindow
{
    private static EditorWindow window;
    private GUIStyle buttonContentStyle;

    [MenuItem("Airy UI/Main Editor &%e", priority = 0)]
    private static void ShowWindow()
    {
        window = GetWindow<AiryUIMainEditor>("Airy UI");
        window.Show();
        window.maxSize = new Vector2(325, 500);
        window.minSize = new Vector2(325, 500);
    }

    private void OnGUI()
    {
        buttonContentStyle = new GUIStyle(GUI.skin.button);
        buttonContentStyle.normal.textColor = Color.white;
        buttonContentStyle.fontSize = 17;
        buttonContentStyle.fixedHeight = 50;

        GUI.color = Color.gray;

        WindowTitle_LABEL();

        GUI.color = Color.white;
        GUI.backgroundColor = Color.black;

        AddRemoveAnimationManager_BUTTONS();

        GUI.color = Color.white;
        GUI.backgroundColor = Color.gray;

        AddRemoveAnimation_BUTTONS();

        GUI.color = Color.white;
        GUI.backgroundColor = Color.blue;

        AddRemoveBackBtn_BUTTONS();
    }

    private void WindowTitle_LABEL()
    {
        GUILayout.Space(10);

        var titleLabelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter, fontSize = 25, fontStyle = FontStyle.Bold, fixedHeight = 50 };

        EditorGUILayout.LabelField("Airy UI Main Window", titleLabelStyle);
        GUILayout.Space(50);
    }

    private void AddRemoveAnimationManager_BUTTONS()
    {
        if (GUILayout.Button("Add Animation Manager", buttonContentStyle))
        {
            foreach (GameObject g in Selection.gameObjects)
            {
                if (g.GetComponent<AiryUIAnimationManager>() == null)
                {
                    g.AddComponent<AiryUIAnimationManager>();
                }
            }
        }
        if (GUILayout.Button("Remove Animation Manager", buttonContentStyle))
        {
            foreach (GameObject g in Selection.gameObjects)
            {
                if (g.GetComponent<AiryUIAnimationManager>() != null)
                    DestroyImmediate(g.GetComponent<AiryUIAnimationManager>());
            }
        }

        GUILayout.Space(20);
    }

    private void AddRemoveAnimation_BUTTONS()
    {
        if (GUILayout.Button("Add Animated Element", buttonContentStyle))
        {
            foreach (GameObject g in Selection.gameObjects)
            {
                if (g.GetComponent<AiryUIAnimatedElement>() == null)
                {
                    g.AddComponent<AiryUIAnimatedElement>();
                }
            }
        }
        if (GUILayout.Button("Add Custom Animated Element", buttonContentStyle))
        {
            foreach (GameObject g in Selection.gameObjects)
            {
                if (g.GetComponent<AiryUICustomAnimationElement>() == null)
                {
                    g.AddComponent<AiryUICustomAnimationElement>();
                }
            }
        }
        if (GUILayout.Button("Remove Animated Element", buttonContentStyle))
        {
            foreach (GameObject g in Selection.gameObjects)
            {
                if (g.GetComponent<AiryUIAnimatedElement>() != null)
                    DestroyImmediate(g.GetComponent<AiryUIAnimatedElement>());

                if (g.GetComponent<AiryUICustomAnimationElement>() != null)
                    DestroyImmediate(g.GetComponent<AiryUICustomAnimationElement>());
            }
        }

        GUILayout.Space(20);
    }

    private void AddRemoveBackBtn_BUTTONS()
    {
        if (GUILayout.Button("Add Back Button Functionality", buttonContentStyle))
        {
            foreach (GameObject g in Selection.gameObjects)
            {
                if (g.GetComponent<AiryUIBackButton>() == null)
                {
                    g.AddComponent<AiryUIBackButton>();
                }
            }
        }

        if (GUILayout.Button("Remove Back Button Functionality", buttonContentStyle))
        {
            foreach (GameObject g in Selection.gameObjects)
            {
                if (g.GetComponent<AiryUIBackButton>() != null)
                    DestroyImmediate(g.GetComponent<AiryUIBackButton>());
            }
        }

        GUILayout.Space(20);
    }
}