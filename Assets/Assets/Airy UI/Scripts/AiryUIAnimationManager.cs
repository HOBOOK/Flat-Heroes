using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AiryUIAnimatedElement))]
public class AiryUIAnimationManager : MonoBehaviour
{
    [HideInInspector] public AiryUIAnimatedElement[] childrenElements;
    [Tooltip("Wheather or not to show the animation when the menu is enabled")] public bool showMenuOnEnable = true;

    private bool elementsUpdated = false;

    private void Awake()
    {
        elementsUpdated = false;
        if (!elementsUpdated)
        {
            UpdateElementsInChildren();
        }
    }

    private void Start()
    {
        if (showMenuOnEnable && elementsUpdated)
        {
            ShowMenu();
        }
    }

    public void ShowMenu()
    {
        gameObject.SetActive(true);

        if (elementsUpdated)
        {
            foreach (var element in childrenElements)
            {
                if (element.showItemOnMenuEnable)
                    element.ShowElement();
            }
        }
    }

    public void HideMenu()
    {
        foreach (var element in childrenElements)
        {
            element.HideElement();
        }
    }

    public void UpdateElementsInChildren()
    {
        childrenElements = GetComponentsInChildren<AiryUIAnimatedElement>();
        elementsUpdated = true;
    }
}