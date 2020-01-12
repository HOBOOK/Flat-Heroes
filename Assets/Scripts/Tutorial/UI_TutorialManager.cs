using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TutorialManager : MonoBehaviour
{
    public GameObject IntroPanel;

    public GameObject TutorialPanel;
    public GameObject BottomPanel;
    public GameObject CompletedPanel;

    private GameObject energyPanel;
    private GameObject heroSkillPanel;
    private GameObject playerSkillPanel;

    public GameObject PopupAlertUI;
    public GameObject canvasOverlay;

    public static UI_TutorialManager instance = null;

    public Text energyText;
    public Text tutorialEnergyText;
    public Text alertText;

    void SetText()
    {
        energyText.text = string.Format("{0}<color='white'>/<size='36'>100</size></color>", (int)TutorialStageManager.instance.GetFlatEnergy());
        tutorialEnergyText.text = string.Format("{0}<color='white'>/<size='36'>100</size></color>", (int)TutorialStageManager.instance.GetFlatEnergy());
    }

    public GameObject ShowNeedAlert(string spritePath, string alertText)
    {
        if (PopupAlertUI != null)
        {
            GameObject popup;
            if (canvasOverlay != null)
            {
                popup = Instantiate(PopupAlertUI, canvasOverlay.transform);
            }
            else
            {
                popup = Instantiate(PopupAlertUI, this.transform);
            }
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
            foreach (Transform otherUI in popup.transform.GetChild(0).transform)
            {
                otherUI.gameObject.SetActive(false);
            }
            GameObject getAlertPanel = popup.transform.GetChild(0).GetChild(2).gameObject;
            getAlertPanel.gameObject.SetActive(true);
            Image getImage = getAlertPanel.transform.GetChild(0).GetComponent<Image>();
            Text getText = getAlertPanel.transform.GetChild(1).GetComponent<Text>();
            getImage.sprite = Resources.Load<Sprite>(spritePath);
            if (getImage.sprite == null)
                getImage.enabled = false;
            else
                getImage.enabled = true;
            getText.text = alertText;
            getAlertPanel.GetComponentInChildren<UI_CheckButton>().isChecking = false;
            popup.GetComponent<AiryUIAnimatedElement>().ShowElement();
            return getAlertPanel;
        }
        return null;
    }


    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private void Start()
    {
        if(BottomPanel!=null)
        {
            energyPanel = BottomPanel.transform.GetChild(0).gameObject;
            heroSkillPanel = BottomPanel.transform.GetChild(1).gameObject;
            playerSkillPanel = BottomPanel.transform.GetChild(2).gameObject;
        }
    }

    private void Update()
    {
        SetText();
    }

    #region 튜토리얼
    public void OpenCompletedPanel()
    {
        CompletedPanel.gameObject.SetActive(true);
        CompletedPanel.GetComponent<AiryUIAnimatedElement>().ShowElement();
    }
    public void OpenPanel(int type)
    {
        switch (type)
        {
            case 0:
                energyPanel.gameObject.SetActive(true);
                heroSkillPanel.gameObject.SetActive(false);
                playerSkillPanel.gameObject.SetActive(false);
                break;
            case 1:
                energyPanel.gameObject.SetActive(false);
                heroSkillPanel.gameObject.SetActive(true);
                playerSkillPanel.gameObject.SetActive(false);
                break;
            case 2:
                energyPanel.gameObject.SetActive(false);
                heroSkillPanel.gameObject.SetActive(false);
                playerSkillPanel.gameObject.SetActive(true);
                break;
        }
        TutorialPanel.gameObject.SetActive(true);
        InActiveTutorialPanelAll();
        TutorialPanel.transform.GetChild(1).gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    public void OpenAlert(string text)
    {
        StartCoroutine(ShowingAlert(text));
    }
    void InActiveTutorialPanelAll()
    {
        for(var i  =0; i <TutorialPanel.transform.childCount; i++)
        {
            TutorialPanel.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void OnClickTutorialPanel()
    {
        if (energyPanel.gameObject.activeSelf)
        {
            Time.timeScale = 1;
            OpenAlert("몬스터들을 처치하세요!");
            StartCoroutine("CloseTutorialPanel");
        }

    }
    public void OnClickTutorialButton()
    {
        Time.timeScale = 1;
        if (heroSkillPanel.gameObject.activeSelf)
        {
            FindObjectOfType<TutorialHeroSkillManager>().OnSkillButtonClick(0);
        }
        else if(playerSkillPanel.gameObject.activeSelf)
        {
            FindObjectOfType<UI_UserSkillButton>().OnClick();
        }
        StartCoroutine("CloseTutorialPanel");
    }
    IEnumerator CloseTutorialPanel()
    {
        foreach(var child in TutorialPanel.transform.GetChild(1).GetComponentsInChildren<AiryUIAnimatedElement>())
        {
            child.HideElement();
        }
        yield return new WaitForSeconds(0.5f);
        TutorialPanel.transform.GetChild(1).gameObject.SetActive(false);
    }
    IEnumerator ShowingAlert(string text)
    {
        TutorialPanel.gameObject.SetActive(true);
        TutorialPanel.transform.GetChild(0).gameObject.SetActive(true);
        TutorialPanel.transform.GetChild(0).GetComponent<AiryUIAnimatedElement>().ShowElement();
        alertText.text = text;
        yield return new WaitForSeconds(2.0f);
        TutorialPanel.transform.GetChild(0).GetComponent<AiryUIAnimatedElement>().HideElement();
        yield return new WaitForSeconds(1.0f);
        TutorialPanel.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    #endregion
}
