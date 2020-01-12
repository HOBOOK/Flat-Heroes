using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickTutorial : Tutorial
{
    public GameObject targetUI;
    public bool isEffectOn = true;
    public List<GameObject> closePanelList = new List<GameObject>();
    public int isGridLayoutThenIndex;
    Button targetButton;
    bool isClicked = false;
    bool isStart = false;

    public override void CheckIfHappening()
    {
        StartTutorial();
        if (targetButton!=null&&isClicked)
        {
            isClicked = false;
            ClearTutorial();
        }
    }
    private void ClearTutorial()
    {
        foreach (var btn in FindObjectsOfType<Button>())
            btn.interactable = true;
        foreach (var child in closePanelList)
        {
            child.SetActive(false);
        }

        TutorialManager.Instance.pointEffect.SetActive(false);
        TutorialManager.Instance.CompletedTutorial();
    }

    private void StartTutorial()
    {
        if (!isStart)
        {
            foreach (var btn in FindObjectsOfType<Button>())
            {
                if (!btn.name.Equals("SkipButton")&&!btn.CompareTag("AlertUI"))
                    btn.interactable = false;
            }

            TutorialManager.Instance.TutorialPanelParentClear();
            if (targetUI != null)
            {
                if(targetUI.GetComponent<GridLayoutGroup>()!=null)
                    targetButton = targetUI.transform.GetChild(isGridLayoutThenIndex).GetComponent<Button>();
                else
                    targetButton = targetUI.GetComponentInChildren<Button>();
                targetButton.interactable = true;
                TutorialManager.Instance.SetGuidePanelPosition(targetButton.transform);
                TutorialManager.Instance.SetGuidText(Order);
                TutorialManager.Instance.ButtonEffect(targetButton.transform, isEffectOn);
                targetButton.onClick.AddListener(delegate
                {
                    onClick();
                });
            }
            isStart = true;
        }
    }

    public void onClick()
    {
        isClicked = true;
    }
}
