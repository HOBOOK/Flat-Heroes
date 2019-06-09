using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiryUIBackButtonManager : MonoBehaviour
{
    public static AiryUIBackButtonManager Instance;

    private List<AiryUIBackButton> ActiveButtons = new List<AiryUIBackButton>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DontDestroyOnLoad(gameObject);
    }

    public void AddButtonToList(AiryUIBackButton backButton)
    {
        if (!ActiveButtons.Contains(backButton))
        {
            ActiveButtons.Add(backButton);
        }
    }

    public void RemoveButtonFromList(AiryUIBackButton backButton)
    {
        if (ActiveButtons.Contains(backButton))
        {
            ActiveButtons.Remove(backButton);
        }
    }

    public void DoBack(AiryUIBackButton backButton)
    {
        if (ActiveButtons.Contains(backButton))
        {
            int index = ActiveButtons.IndexOf(backButton);

            if (index == ActiveButtons.Count - 1)
            {
                backButton.GetComponentInParent<AiryUIAnimationManager>().HideMenu();
                backButton.GetComponent<AiryUIAnimatedElement>().HideElement();
            }
        }
    }

    public void DoBackOnCurrentObject(AiryUIBackButton backButton)
    {
        backButton.GetComponentInParent<AiryUIAnimationManager>().HideMenu();
        backButton.backButton.GetComponent<AiryUIAnimatedElement>().HideElement();
    }
}