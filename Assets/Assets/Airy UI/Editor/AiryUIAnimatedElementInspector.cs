using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AiryUIAnimatedElement))]
[CanEditMultipleObjects]
public class AiryUIAnimatedElementInspector : Editor
{
    private AiryUIAnimatedElement AnimatedElement;
    private bool showFromCornerAnimation;

    private SerializedProperty _showItemOnMenuEnable;
    private SerializedProperty _animationShowDuration;
    private SerializedProperty _animationHideDuration;
    private SerializedProperty _pivotWhileRotating;
    private SerializedProperty _rotateFrom;
    private SerializedProperty _rotateTo;
    private SerializedProperty _showAnimationType;
    private SerializedProperty _hideAnimationType;
    private SerializedProperty _animationFromCornerStartFromType;
    private SerializedProperty _fadeChildren;
    private SerializedProperty _animationFromCornerType;
    private SerializedProperty _animationToCornerType;
    private SerializedProperty _elasticPower;
    private SerializedProperty _withDelay;
    private SerializedProperty _showDelay;
    private SerializedProperty _hideDelay;
    private SerializedProperty _onShowEvent;
    private SerializedProperty _onHideEvent;
    private SerializedProperty _onShowCompleteEvent;
    private SerializedProperty _onHideCompleteEvent;

    private int currentTabIndex = 0;
    private string[] tabsTexts = { "Show", "Hide" };

    private void OnEnable()
    {
        AnimatedElement = (AiryUIAnimatedElement)target;
        _showItemOnMenuEnable = serializedObject.FindProperty("showItemOnMenuEnable");
        _animationShowDuration = serializedObject.FindProperty("animationShowDuration");
        _animationHideDuration = serializedObject.FindProperty("animationHideDuration");
        _pivotWhileRotating = serializedObject.FindProperty("pivotWhileRotating");
        _rotateFrom = serializedObject.FindProperty("rotateFrom");
        _rotateTo = serializedObject.FindProperty("rotateTo");
        _showAnimationType = serializedObject.FindProperty("showAnimationType");
        _hideAnimationType = serializedObject.FindProperty("hideAnimationType");
        _animationFromCornerStartFromType = serializedObject.FindProperty("animationFromCornerStartFromType");
        _fadeChildren = serializedObject.FindProperty("fadeChildren");
        _animationFromCornerType = serializedObject.FindProperty("animationFromCornerType");
        _animationToCornerType = serializedObject.FindProperty("animationToCornerType");
        _elasticPower = serializedObject.FindProperty("elasticityPower");
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
        InspectorTitle_LABEL();

        GUILayout.Space(20);
        currentTabIndex = GUILayout.Toolbar(currentTabIndex, tabsTexts);
        GUILayout.Space(20);

        if (currentTabIndex == 0)
        {
            GUI.color = Color.cyan;

            AnimationWorksOnStart_TOGGLE();
            ShowAnimationType_DROPDOWN();
            AnimationCorner_DROPDOWN();
            ShowAnimationRotaion_PROPERTIES();
            FadeChildrenShow_TOGGLE();
            AnimationShowDuration_INPUT();
            ShowAnimationDelay_PROPERTIES();
            AutomateChildrenShowDelays_BUTTON();
            OnShow_EVENTS();
        }
        else if (currentTabIndex == 1)
        {
            GUI.color = Color.gray;

            HideAnimationType_DROPDOWN();
            HideAnimationRotaion_PROPERTIES();
            FadeChildrenHide_TOGGLE();
            AnimationHideDuration_INPUT();
            HideAnimationDelay_PROPERTIES();
            AutomateChildrenHideDelays_BUTTON();
            OnHide_EVENTS();
        }

        serializedObject.ApplyModifiedProperties();
    }


    private void AnimationCorner_DROPDOWN()
    {
        switch (AnimatedElement.showAnimationType)
        {
            case (AnimationType.FromCornerWithScale):
            case (AnimationType.FromCornerWithoutScale):
                EditorGUILayout.PropertyField(_animationFromCornerStartFromType, new GUIContent("Start & End Corner"));
                break;
        }
    }

    private void InspectorTitle_LABEL()
    {
        GUILayout.Space(20);
        var titleLabelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter, fontSize = 20, fontStyle = FontStyle.Bold, fixedHeight = 50 };

        EditorGUILayout.LabelField("Animated UI Element", titleLabelStyle);

        EditorGUILayout.Space(); EditorGUILayout.Space();
    }

    private void AnimationWorksOnStart_TOGGLE()
    {
        EditorGUILayout.PropertyField(_showItemOnMenuEnable, new GUIContent("Show Animation On Menu Enable"));

        GUILayout.Space(10);
    }

    private void ShowAnimationType_DROPDOWN()
    {
        EditorGUILayout.PropertyField(_showAnimationType, new GUIContent("Animation Type"));

        EditorGUILayout.Space(); EditorGUILayout.Space();

        switch (AnimatedElement.showAnimationType)
        {
            case (AnimationType.FromCornerWithScale):
            case (AnimationType.FromCornerWithoutScale):
                EditorGUILayout.PropertyField(_animationFromCornerType, new GUIContent("Animation From Corner Type"));
                break;
            case (AnimationType.ScaleElastic):
                EditorGUILayout.PropertyField(_elasticPower, new GUIContent("Elastic Power"));
                break;
        }
    }

    private void HideAnimationType_DROPDOWN()
    {
        EditorGUILayout.PropertyField(_hideAnimationType, new GUIContent("Animation Type"));

        EditorGUILayout.Space(); EditorGUILayout.Space();

        switch (AnimatedElement.hideAnimationType)
        {
            case (AnimationType.FromCornerWithScale):
            case (AnimationType.FromCornerWithoutScale):
                EditorGUILayout.PropertyField(_animationToCornerType, new GUIContent("Animation To Corner Type"));
                break;
            case (AnimationType.ScaleElastic):
                EditorGUILayout.PropertyField(_elasticPower, new GUIContent("Elastic Power"));
                break;
        }
    }

    private void FadeChildrenShow_TOGGLE()
    {
        if (AnimatedElement.showAnimationType == AnimationType.FadeColor)
            EditorGUILayout.PropertyField(_fadeChildren, new GUIContent("Fade Children With Parent"));
    }

    private void FadeChildrenHide_TOGGLE()
    {
        if (AnimatedElement.hideAnimationType == AnimationType.FadeColor)
            EditorGUILayout.PropertyField(_fadeChildren, new GUIContent("Fade Children With Parent"));
    }

    private void AnimationShowDuration_INPUT()
    {
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_animationShowDuration, new GUIContent("Animation Show Duration"));

        EditorGUILayout.Space(); EditorGUILayout.Space();
    }

    private void AnimationHideDuration_INPUT()
    {
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(_animationHideDuration, new GUIContent("Animation Hide Duration"));

        GUILayout.Space(10);
    }

    private void ShowAnimationDelay_PROPERTIES()
    {
        EditorGUILayout.PropertyField(_withDelay, new GUIContent("With Delay"));

        if (AnimatedElement.withDelay)
        {
            EditorGUILayout.PropertyField(_showDelay, new GUIContent("Show Delay"));
        }

        EditorGUILayout.Space(); EditorGUILayout.Space();
    }

    private void ShowAnimationRotaion_PROPERTIES()
    {
        if (AnimatedElement.showAnimationType == AnimationType.Rotate)
        {
            EditorGUILayout.PropertyField(_rotateFrom, new GUIContent("Rotate From"), true);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_pivotWhileRotating, new GUIContent("Rotaion Pivot"));
        }

        GUILayout.Space(10);
    }

    private void HideAnimationRotaion_PROPERTIES()
    {
        if (AnimatedElement.hideAnimationType == AnimationType.Rotate)
        {
            EditorGUILayout.PropertyField(_rotateTo, new GUIContent("Rotate To"), true);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_pivotWhileRotating, new GUIContent("Rotaion Pivot"));
        }

        GUILayout.Space(10);
    }

    private void HideAnimationDelay_PROPERTIES()
    {
        EditorGUILayout.PropertyField(_withDelay, new GUIContent("With Delay"));

        if (AnimatedElement.withDelay)
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

    private void AutomateChildrenShowDelays_BUTTON()
    {
        if (GUILayout.Button("Automate Show Delays In Children"))
        {
            AiryUIAnimatedElement[] elementsInChildren = Selection.activeGameObject.GetComponentsInChildren<AiryUIAnimatedElement>();

            float step = (AnimatedElement.animationShowDuration / elementsInChildren.Length);
            float currentValue = 0;

            for (int i = 1; i < elementsInChildren.Length; i++)
            {
                if (elementsInChildren[i].withDelay)
                    elementsInChildren[i].showDelay = AnimatedElement.animationShowDuration + AnimatedElement.showDelay + currentValue;

                currentValue += step;
            }
        }

        GUILayout.Space(10);
    }

    private void AutomateChildrenHideDelays_BUTTON()
    {
        if (GUILayout.Button("Automate Hide Delays In Children"))
        {
            AiryUIAnimatedElement[] elementsInChildren = Selection.activeGameObject.GetComponentsInChildren<AiryUIAnimatedElement>();

            float step = (AnimatedElement.hideDelay / elementsInChildren.Length);
            float currentValue = 0;

            elementsInChildren[elementsInChildren.Length - 1].hideDelay = 0;
            for (int i = elementsInChildren.Length - 2; i >= 1; i--)
            {
                if (elementsInChildren[i].withDelay)
                    elementsInChildren[i].hideDelay = step + currentValue;

                currentValue += step;
            }
        }

        GUILayout.Space(10);
    }
}