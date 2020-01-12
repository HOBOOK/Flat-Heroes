using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public List<Tutorial> Tutorials = new List<Tutorial>();
    public GameObject tutorialTitle;
    public GameObject pointEffect;

    public GameObject tutoralGuidePanel;
    Text guideText;

    public GameObject TutorialPanel;

    public Transform tutorialPanelParent;

    public void TutorialPanelParentClear()
    {
        if(tutorialPanelParent.transform.GetChild(0).childCount >0)
        {
            foreach(Transform child in tutorialPanelParent.transform.GetChild(0))
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private static TutorialManager instance;

    public static TutorialManager Instance
    {
        get
        {
            if(instance==null)
                instance = FindObjectOfType<TutorialManager>() as TutorialManager;

            if (instance == null)
                Debugging.LogWarning("튜토리얼 매니저가 존재하지 않음");
            return instance;
        }
    }

    private Tutorial currentTutorial;

    void Start()
    {
        if (User.tutorialSequence==1)
        {
            TutorialPanel.SetActive(true);
            guideText = tutoralGuidePanel.GetComponentInChildren<Text>();
            StartCoroutine("StartTutorial");
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
    IEnumerator StartTutorial()
    {
        tutorialTitle.GetComponent<AiryUIAnimatedElement>().ShowElement();
        yield return new WaitForSeconds(3.0f);
        tutoralGuidePanel.SetActive(true);
        SetNextTutorial(0);
    }
    public void ButtonEffect(Transform target, bool isOnOff)
    {
        if(isOnOff)
        {
            StartCoroutine(ButtonEffectOn(target, isOnOff));
        }
        else
        {
            pointEffect.SetActive(false);
        }
    }
    IEnumerator ButtonEffectOn(Transform target, bool isOnOff)
    {
        yield return new WaitForSeconds(0.1f);
        float delayTime = 1.0f;
        while(delayTime>0&&target==null)
        {
            delayTime -= Time.deltaTime;
            yield return null;
        }
        pointEffect.SetActive(true);
        pointEffect.transform.position = target.position;
    }

    public void SetGuidePanelPosition(Transform target=null)
    {
        if (target == null)
            target = tutorialPanelParent;

        RectTransform targetRect = target.GetComponent<RectTransform>();
        tutoralGuidePanel.transform.position = target.transform.position + new Vector3(0.5f, 0.5f);
        if (tutoralGuidePanel.transform.position.x > 0.8f)
        {
            tutoralGuidePanel.transform.position = target.transform.position - new Vector3(1, 0);
            if (tutoralGuidePanel.transform.position.y < 3f)
            {
                tutoralGuidePanel.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                tutoralGuidePanel.transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                tutoralGuidePanel.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else
            {
                tutoralGuidePanel.transform.position = tutoralGuidePanel.transform.position - new Vector3(0, 0.5f);
                tutoralGuidePanel.transform.rotation = Quaternion.Euler(new Vector3(180, 180, 0));
                tutoralGuidePanel.transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
                tutoralGuidePanel.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(180, 180, 0));
            }

        }
        else
        {
            if (tutoralGuidePanel.transform.position.y < 3f)
            {
                tutoralGuidePanel.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                tutoralGuidePanel.transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                tutoralGuidePanel.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else
            {
                tutoralGuidePanel.transform.position = tutoralGuidePanel.transform.position - new Vector3(0, 0.5f);
                tutoralGuidePanel.transform.rotation = Quaternion.Euler(new Vector3(180, 0, 0));
                tutoralGuidePanel.transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
                tutoralGuidePanel.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
            }
        }


        tutoralGuidePanel.transform.GetChild(1).GetChild(0).GetComponent<AiryUIAnimatedElement>().ShowElement();
        tutoralGuidePanel.transform.GetChild(1).GetChild(0).transform.localPosition = Vector3.zero;
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
    }

    void Update()
    {
        if (currentTutorial)
            currentTutorial.CheckIfHappening();
    }
    public void SetNextTutorial(int currentOrder)
    {
        currentTutorial = GetTutorialByOrder(currentOrder);

        if(!currentTutorial)
        {
            CompletedAllTutorials();
            return;
        }
    }
    public void SetGuidText(int Order)
    {
        guideText.text = LocalizationManager.GetText("TutorialExplanation" + Order);
    }
    public void CompletedTutorial()
    {
        SetNextTutorial(currentTutorial.Order + 1);
    }

    public void CompletedAllTutorials()
    {
        foreach (var btn in FindObjectsOfType<Button>())
            btn.interactable = true;

        TutorialPanel.SetActive(false);
        tutoralGuidePanel.SetActive(false);
        TutorialReward();
        User.tutorialSequence = 2;
        GoogleSignManager.SaveData();
        this.gameObject.SetActive(false);
    }

    bool isCheckAlertOn = false;
    IEnumerator CheckingAlert()
    {
        isCheckAlertOn = true;
        var alertPanel = UI_Manager.instance.ShowNeedAlert("", string.Format("{0}", LocalizationManager.GetText("alertTutorialSkipMessage")));
        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            Time.timeScale = 1.0f;
            UI_Manager.instance.ClosePopupAlertUI();
            CompletedAllTutorials();
        }
        else
        {
            
            UI_Manager.instance.ClosePopupAlertUI();
            // 아니오를 클릭시
        }
        isCheckAlertOn = false;
        yield return null;
    }

    public void SkipButton()
    {
        if (!isCheckAlertOn)
        {
            StartCoroutine("CheckingAlert");
        }
    }

    public Tutorial GetTutorialByOrder(int Order)
    {
        for(var i = 0; i < Tutorials.Count; i++)
        {
            if (Tutorials[i].Order == Order)
                return Tutorials[i];
        }
        return null;
    }

    public void TutorialReward()
    {
        if (HeroSystem.GetUserHero(103) == null)
        {
            HeroSystem.SetObtainHero(103);
            HeroData hd = HeroSystem.GetHero(103);
            GoogleSignManager.SaveData();
            UI_Manager.instance.ShowGetAlert(hd.image, string.Format("{0}\r\n<color='yellow'>{1}</color> {2}\r\n{3}", LocalizationManager.GetText("alertGetMessage8"), HeroSystem.GetHeroName(hd.id), LocalizationManager.GetText("alertGetMessage5"), LocalizationManager.GetText("alertGetMessage9")));
        }
    }
}
