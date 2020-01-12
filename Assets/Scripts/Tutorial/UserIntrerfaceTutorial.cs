using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserIntrerfaceTutorial : Tutorial
{
    public List<GameObject> closePanelList = new List<GameObject>();
    public GameObject targetButton;
    public GameObject targetUI;
    public int isGridLayoutThenIndex;
    Button button;
    bool isStart = false;

    public override void CheckIfHappening()
    {
        StartTutorial();
        if (targetUI != null && targetUI.activeSelf&&isStart)
        {
            foreach (var btn in FindObjectsOfType<Button>())
            {
                btn.interactable = true;
            }

            foreach (var child in closePanelList)
            {
                child.SetActive(false);
            }
            TutorialManager.Instance.pointEffect.SetActive(false);
            TutorialManager.Instance.CompletedTutorial();
        }
    }

    private void StartTutorial()
    {
        if(!isStart)
        {
            TutorialManager.Instance.TutorialPanelParentClear();
            foreach (var btn in FindObjectsOfType<Button>())
            {
                if (!btn.name.Equals("SkipButton") && !btn.CompareTag("AlertUI"))
                    btn.interactable = false;
            }
            if (targetButton != null)
            {
                targetButton.SetActive(true);
                if (targetButton.GetComponent<GridLayoutGroup>() != null)
                    button = targetButton.transform.GetChild(isGridLayoutThenIndex).GetComponent<Button>();
                else
                    button = targetButton.GetComponent<Button>();
                if(button!=null)
                    button.interactable = true;
                //TutorialManager.Instance.pointEffect.transform.position = button.transform.position;
                //TutorialManager.Instance.pointEffect.SetActive(true);
                TutorialManager.Instance.ButtonEffect(button.transform, true);
                TutorialManager.Instance.SetGuidePanelPosition(button.transform);
                TutorialManager.Instance.SetGuidText(Order);
            }
            isStart = true;
        }
    }
}
