using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(AiryUICustomAnimationElement))]
[CanEditMultipleObjects]
public class AiryUICustomAnimationElementInspector : Editor
{
    private AiryUICustomAnimationElement customAnimationElement;

    private SerializedProperty _showItemOnMenuEnable;
    private SerializedProperty _loop;
    private SerializedProperty _componentsToAnimate_SHOW;
    private SerializedProperty _transformAnimationRecords_SHOW;
    private SerializedProperty _graphicAnimationRecords_SHOW;
    private SerializedProperty _transformAndGraphicAnimationRecords_SHOW;

    private SerializedProperty _componentsToAnimate_HIDE;
    private SerializedProperty _transformAnimationRecords_HIDE;
    private SerializedProperty _graphicAnimationRecords_HIDE;
    private SerializedProperty _transformAndGraphicAnimationRecords_HIDE;

    private SerializedProperty _currentRecordDuration;
    private SerializedProperty _currentRecordDelay;

    private SerializedProperty _withDelay;
    private SerializedProperty _showDelay;
    private SerializedProperty _hideDelay;

    private SerializedProperty _onShowEvent;
    private SerializedProperty _onHideEvent;
    private SerializedProperty _onShowCompleteEvent;
    private SerializedProperty _onHideCompleteEvent;

    private string[] tabsTexts = { "Show", "Hide" };
    private int currentTabIndex = 0;

    public float currentRecordDuration = 0.5f;
    public float currentRecordStartsAt = 0;

    private bool recordModeActive;

    private void OnEnable()
    {
        customAnimationElement = (AiryUICustomAnimationElement)target;

        _showItemOnMenuEnable = serializedObject.FindProperty("showItemOnMenuEnable");
        _loop = serializedObject.FindProperty("loop");
        _componentsToAnimate_SHOW = serializedObject.FindProperty("componentsToAnimate_SHOW");
        _transformAnimationRecords_SHOW = serializedObject.FindProperty("TransformAnimationRecords_SHOW");
        _graphicAnimationRecords_SHOW = serializedObject.FindProperty("GraphicAnimationRecords_SHOW");
        _transformAndGraphicAnimationRecords_SHOW = serializedObject.FindProperty("TransformAndGraphicAnimationRecords_SHOW");

        _componentsToAnimate_HIDE = serializedObject.FindProperty("componentsToAnimate_HIDE");
        _transformAnimationRecords_HIDE = serializedObject.FindProperty("TransformAnimationRecords_HIDE");
        _graphicAnimationRecords_HIDE = serializedObject.FindProperty("GraphicAnimationRecords_HIDE");
        _transformAndGraphicAnimationRecords_HIDE = serializedObject.FindProperty("TransformAndGraphicAnimationRecords_HIDE");

        _currentRecordDuration = serializedObject.FindProperty("currentRecordDuration");
        _currentRecordDelay = serializedObject.FindProperty("currentRecordDelay");

        _withDelay = serializedObject.FindProperty("withDelay");
        _showDelay = serializedObject.FindProperty("showDelay");
        _hideDelay = serializedObject.FindProperty("hideDelay");

        _onShowEvent = serializedObject.FindProperty("OnShow");
        _onHideEvent = serializedObject.FindProperty("OnHide");
        _onShowCompleteEvent = serializedObject.FindProperty("OnShowComplete");
        _onHideCompleteEvent = serializedObject.FindProperty("OnHideComplete");
    }

    public override void OnInspectorGUI()
    {
        Title_LABEL();

        currentTabIndex = GUILayout.Toolbar(currentTabIndex, tabsTexts);

        GUILayout.Space(20);

        if (currentTabIndex == 0)
        {
            AnimationWorksOnStart_TOGGLE();
            Loop_TOGGLE();
            ComponentsToAnimateShow_DROPDOWN();
            RecordMode_BUTTONS();
            ShowAnimationDelay_PROPS();
            Duration_PROPS();
            TransformAnimationRecordsShow_LIST();
            GraphicAnimationRecordsShow_LIST();
            TransformAndGraphicAnimationRecordsShow_LIST();
            OnShow_EVENTS();
        }
        else if (currentTabIndex == 1)
        {
            ComponentsToAnimateHide_DROPDOWN();
            RecordMode_BUTTONS();
            HideAnimationDelay_PROPS();
            Duration_PROPS();
            TransformAnimationRecordsHide_LIST();
            GraphicAnimationRecordsHide_LIST();
            TransformAndGraphicAnimationRecordsHide_LIST();
            OnHide_EVENTS();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void Loop_TOGGLE()
    {
        EditorGUILayout.PropertyField(_loop);

        if (customAnimationElement.loop == true)
            EditorGUILayout.HelpBox("Note: OnShowComplete() event will not be executed when the animation is looping", MessageType.None);

        GUILayout.Space(10);
    }

    private void Title_LABEL()
    {
        GUILayout.Space(20);
        var titleLabelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter, fontSize = 20, fontStyle = FontStyle.Bold, fixedHeight = 50 };

        EditorGUILayout.LabelField("Custom Animated Element", titleLabelStyle);

        GUILayout.Space(30);
    }

    private void AnimationWorksOnStart_TOGGLE()
    {
        EditorGUILayout.PropertyField(_showItemOnMenuEnable, new GUIContent("Show Animation On Menu Enable"));

        GUILayout.Space(10);
    }

    private void ComponentsToAnimateShow_DROPDOWN()
    {
        GUI.color = Color.cyan;
        EditorGUILayout.PropertyField(_componentsToAnimate_SHOW, true);
        GUI.color = Color.white;
        GUILayout.Space(20);
    }

    private void ComponentsToAnimateHide_DROPDOWN()
    {
        GUI.color = Color.cyan;
        EditorGUILayout.PropertyField(_componentsToAnimate_HIDE, true);
        GUI.color = Color.white;
        GUILayout.Space(20);
    }

    private void Duration_PROPS()
    {
        GUI.color = Color.yellow;

        EditorGUILayout.PropertyField(_currentRecordDuration);

        GUI.color = Color.green;

        EditorGUILayout.PropertyField(_currentRecordDelay);

        GUI.color = Color.white;
        GUILayout.Space(20);
    }

    private void TransformAnimationRecordsShow_LIST()
    {
        if (customAnimationElement.componentsToAnimate_SHOW == AiryUICustomAnimationElement.ComponentsToAnimate.Transform)
        {
            EditorGUILayout.PropertyField(_transformAnimationRecords_SHOW, new GUIContent("Animation Records"), true);
            GUILayout.Space(20);
        }
    }

    private void TransformAnimationRecordsHide_LIST()
    {
        if (customAnimationElement.componentsToAnimate_HIDE == AiryUICustomAnimationElement.ComponentsToAnimate.Transform)
        {
            EditorGUILayout.PropertyField(_transformAnimationRecords_HIDE, new GUIContent("Animation Records"), true);
            GUILayout.Space(20);
        }
    }

    private void GraphicAnimationRecordsShow_LIST()
    {
        if (customAnimationElement.componentsToAnimate_SHOW == AiryUICustomAnimationElement.ComponentsToAnimate.Graphic)
        {
            EditorGUILayout.PropertyField(_graphicAnimationRecords_SHOW, new GUIContent("Animation Records"), true);
            GUILayout.Space(20);
        }
    }

    private void GraphicAnimationRecordsHide_LIST()
    {
        if (customAnimationElement.componentsToAnimate_HIDE == AiryUICustomAnimationElement.ComponentsToAnimate.Graphic)
        {
            EditorGUILayout.PropertyField(_graphicAnimationRecords_HIDE, new GUIContent("Animation Records"), true);
            GUILayout.Space(20);
        }
    }

    private void TransformAndGraphicAnimationRecordsShow_LIST()
    {
        if (customAnimationElement.componentsToAnimate_SHOW == AiryUICustomAnimationElement.ComponentsToAnimate.Both)
        {
            EditorGUILayout.PropertyField(_transformAndGraphicAnimationRecords_SHOW, new GUIContent("Animation Records"), true);
            GUILayout.Space(20);
        }
    }

    private void TransformAndGraphicAnimationRecordsHide_LIST()
    {
        if (customAnimationElement.componentsToAnimate_HIDE == AiryUICustomAnimationElement.ComponentsToAnimate.Both)
        {
            EditorGUILayout.PropertyField(_transformAndGraphicAnimationRecords_HIDE, new GUIContent("Animation Records"), true);
            GUILayout.Space(20);
        }
    }

    private void RecordMode_BUTTONS()
    {
        EditorGUILayout.BeginHorizontal();

        GUI.color = Color.cyan;
        if (GUILayout.Button("Enter Record Mode"))
        {
            foreach (var g in Selection.gameObjects)
            {
                AiryUICustomAnimationElement aniamtedElement = g.GetComponent<AiryUICustomAnimationElement>();
                if (aniamtedElement)
                {
                    aniamtedElement.EnterRecordMode(g, (AiryUICustomAnimationElement.AnimationShowOrHide)currentTabIndex);
                }
            }
            recordModeActive = true;
            GUI.color = Color.red;
        }

        GUILayout.Space(5);

        GUI.color = Color.yellow;
        if (GUILayout.Button("Exit Record Mode"))
        {
            foreach (var g in Selection.gameObjects)
            {
                AiryUICustomAnimationElement aniamtedElement = g.GetComponent<AiryUICustomAnimationElement>();
                if (aniamtedElement)
                {
                    aniamtedElement.ExitRecordMode(g, (AiryUICustomAnimationElement.AnimationShowOrHide)currentTabIndex);
                }
            }
            recordModeActive = false;
            GUI.color = Color.white;
        }

        GUILayout.Space(15);

        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginDisabledGroup(!recordModeActive);

        Record_BUTTON();

        EditorGUI.EndDisabledGroup();

        GUILayout.Space(20);
    }

    private void Record_BUTTON()
    {
        GUI.color = Color.red;

        if (GUILayout.Button("Record"))
        {
            if (currentTabIndex == 0)
            {
                foreach (var g in Selection.gameObjects)
                {
                    AiryUICustomAnimationElement aniamtedElement = g.GetComponent<AiryUICustomAnimationElement>();
                    if (aniamtedElement)
                    {
                        aniamtedElement.Record(g, AiryUICustomAnimationElement.AnimationShowOrHide.Show);
                    }
                }
            }
            else if (currentTabIndex == 1)
            {
                foreach (var g in Selection.gameObjects)
                {
                    AiryUICustomAnimationElement aniamtedElement = g.GetComponent<AiryUICustomAnimationElement>();
                    if (aniamtedElement)
                    {
                        aniamtedElement.Record(g, AiryUICustomAnimationElement.AnimationShowOrHide.Hide);
                    }
                }
            }
        }
    }

    private void ShowAnimationDelay_PROPS()
    {
        GUI.color = Color.white;
        EditorGUILayout.PropertyField(_withDelay, new GUIContent("With Delay"));

        if (customAnimationElement.withDelay)
        {
            EditorGUILayout.PropertyField(_showDelay, new GUIContent("Show Delay"));
        }

        EditorGUILayout.Space(); EditorGUILayout.Space();
    }

    private void HideAnimationDelay_PROPS()
    {
        GUI.color = Color.white;
        EditorGUILayout.PropertyField(_withDelay, new GUIContent("With Delay"));

        if (customAnimationElement.withDelay)
        {
            EditorGUILayout.PropertyField(_hideDelay, new GUIContent("Hide Delay"));
        }

        EditorGUILayout.Space(); EditorGUILayout.Space();
    }

    private void OnShow_EVENTS()
    {
        EditorGUILayout.PropertyField(_onShowEvent, new GUIContent("On Show"));
        EditorGUILayout.PropertyField(_onShowCompleteEvent, new GUIContent("On Show Complete"));
    }

    private void OnHide_EVENTS()
    {
        EditorGUILayout.PropertyField(_onHideEvent, new GUIContent("On Hide"));
        EditorGUILayout.PropertyField(_onHideCompleteEvent, new GUIContent("On Hide Complete"));
    }
}