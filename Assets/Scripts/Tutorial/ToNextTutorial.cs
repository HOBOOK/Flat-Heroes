using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToNextTutorial : Tutorial
{
    bool isClicked = false;
    bool isStart = false;

    public override void CheckIfHappening()
    {
        StartTutorial();
        if (Input.GetMouseButtonDown(0))
            isClicked = true;

        if (isClicked)
        {
            ClearTutorial();
            isClicked = false;
        }
    }
    private void ClearTutorial()
    {
        TutorialManager.Instance.pointEffect.SetActive(false);
        TutorialManager.Instance.CompletedTutorial();
    }

    private void StartTutorial()
    {
        if (!isStart)
        {
            foreach (var btn in FindObjectsOfType<Button>())
            {
                if (!btn.name.Equals("SkipButton") && !btn.CompareTag("AlertUI"))
                    btn.interactable = false;
            }

            TutorialManager.Instance.pointEffect.SetActive(false);
            TutorialManager.Instance.SetGuidePanelPosition();
            TutorialManager.Instance.SetGuidText(Order);
            isStart = true;
        }
    }
}
