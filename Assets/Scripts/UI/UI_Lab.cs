using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lab : MonoBehaviour
{
    public Text LaboratoryLevelText;
    public Image LabImage;
    public Text LabMagicStoneText;
    public Slider LabProgressSlider;
    public Button LabProgressButton;
    public Button LabLevelUpButton;

    GameObject ScrollContentView;

    Text labLevelText;
    Text labPowerText;
    Text labNeedCoinText;
    Button labLevelButton;


    private void Awake()
    {
        if (ScrollContentView == null)
            ScrollContentView = this.GetComponentInChildren<ContentSizeFitter>().gameObject;
    }

    private void OnEnable()
    {
        RefreshUI();
    }
    private void FixedUpdate()
    {
        LabUpdate();
    }

    float LabWaitTime
    {
        get { return 3600000 / User.labLevel; }
    }

    string GetLabWaitTime(int level)
    {
        return (60 / level).ToString("N0") + LocalizationManager.GetText("Minute");
    }

    void LabUpdate()
    {
        ulong diff = ((ulong)DateTime.Now.Ticks - ulong.Parse(User.labProgressDate));
        ulong m = diff / TimeSpan.TicksPerMillisecond;
        double secondsLeft = (double)(LabWaitTime - m) / 1000.0f;
        ResetLabProgress((float)secondsLeft * 1000,LabWaitTime);
        float progressValue = 1 - ((float)(secondsLeft * 1000 / LabWaitTime));
        LabImage.fillAmount = progressValue;
        LabProgressSlider.value = progressValue;
        string r = "";
        r += ((int)secondsLeft / 3600).ToString() + ": ";
        secondsLeft -= ((int)secondsLeft / 3600) * 3600;
        r += ((int)secondsLeft / 60).ToString("00") + ": ";
        r += (secondsLeft % 60).ToString("00");
        LabProgressSlider.GetComponentInChildren<Text>().text = r;
    }
    public void OnClcikLaboratoryLevelUp()
    {
        if(!isCheckAlertOn)
        {
            StartCoroutine(CheckingAlert(User.labLevel * User.labLevel * User.labLevel * 15000));
        }
    }
    public void OnClickGetMagicStoneStack()
    {
        if(User.magicStoneStack>0)
        {
            Debugging.Log(User.magicStoneStack + " 획득");
            int addAmount = User.magicStoneStack;
            SaveSystem.AddUserMagicStone(addAmount);
            User.magicStoneStack = 0;
            GoogleSignManager.SaveData();
            UI_Manager.instance.ShowGetAlert(Common.GetCoinCrystalEnergyImagePath(5), string.Format("<color='yellow'>{0}</color> {1} {2}", Common.GetThousandCommaText(addAmount), LocalizationManager.GetText("MagicStone"), LocalizationManager.GetText("alertGetMessage1")));
            RefreshUI();
        }
    }
    public void OnClickLabProgressButton()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        if(Common.PaymentCheck(ref User.coin, User.labLevel * 500))
        {
            ulong oneMinute = 60000 * TimeSpan.TicksPerMillisecond;
            User.labProgressDate = (ulong.Parse(User.labProgressDate) - oneMinute).ToString();
        }
        else
        {
            UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.coin, User.labLevel * 500);
        }
    }

    void SetLabPanelUI()
    {
        LabMagicStoneText.text = string.Format("x {0} {1}", User.magicStoneStack, LocalizationManager.GetText("LabGetButtonText"));
        LaboratoryLevelText.text = string.Format("{0} {1} {2}", LocalizationManager.GetText("LabTitle"), User.labLevel, LocalizationManager.GetText("Phase"));
        LabProgressButton.GetComponentInChildren<Text>().text = Common.GetThousandCommaText(User.labLevel * 500);
    }
    void ResetLabProgress(float currentTime,float standardTime)
    {
        if(currentTime < 0)
        {
            if (User.magicStoneStack < 0)
                User.magicStoneStack = 0;
            if(User.magicStoneStack<2000)
                User.magicStoneStack += Mathf.Clamp((Mathf.Abs((int)(currentTime / standardTime)) + 1),0,2000)*User.labLevel;
            User.magicStoneStack = Mathf.Clamp(User.magicStoneStack, 0, 2000);
            User.labProgressDate = DateTime.Now.Ticks.ToString();
            SetLabPanelUI();
            SaveSystem.SavePlayer();
        }
    }

    void RefreshUI()
    {
        //LabPanel
        SetLabPanelUI();
        //ScrollContent
        for (int i = 0; i < ScrollContentView.transform.childCount; i++)
        {
            labLevelText = ScrollContentView.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>();
            labPowerText = ScrollContentView.transform.GetChild(i).GetChild(1).GetChild(1).GetComponentInChildren<Text>();
            labLevelButton = ScrollContentView.transform.GetChild(i).GetComponentInChildren<Button>();
            labNeedCoinText = labLevelButton.GetComponentInChildren<Text>();

            int labTypeLevel = LabSystem.GetLapLevel(i);
            int levelupNeedMagicStone = LabSystem.GetNeedStone(labTypeLevel);
            int maxLevel = (User.labLevel - 1) * 10 + 10;

            labLevelText.text = string.Format("Lv <color='yellow'>{0}</color>/{1}", labTypeLevel,maxLevel);
            if(i==2||i==3)// 뒤에 %가 붙는경우
                labPowerText.text = string.Format("{0}% > <color='yellow'>{1}%</color>", LabSystem.GetLapPower(i, labTypeLevel), LabSystem.GetLapPower(i, labTypeLevel + 1));
            else if(i==6)
                labPowerText.text = string.Format("{0} > <color='yellow'>{1}</color>", 9999+LabSystem.GetLapPower(i, labTypeLevel), 9999+LabSystem.GetLapPower(i, labTypeLevel + 1));
            else
                labPowerText.text = string.Format("{0} > <color='yellow'>{1}</color>",LabSystem.GetLapPower(i, labTypeLevel), LabSystem.GetLapPower(i, labTypeLevel+1));


            if (labTypeLevel >= maxLevel)
            { 
                labNeedCoinText.color = Color.red;
                labNeedCoinText.text =  string.Format("(!){0} {1} {2}",LocalizationManager.GetText("LabTitle"), LocalizationManager.GetText("Phase"), User.labLevel+1);
            }
            else
            {
                labNeedCoinText.color = Color.white;
                labNeedCoinText.text = Common.GetThousandCommaText(levelupNeedMagicStone);
            }

            int slotIndex = i;
            labLevelButton.onClick.RemoveAllListeners();
            labLevelButton.onClick.AddListener(delegate
            {
                OnClickLabLevelUp(slotIndex, levelupNeedMagicStone);
            });
            if(Common.PaymentAbleCheck(ref User.magicStone, levelupNeedMagicStone) &&labTypeLevel< maxLevel)
            {
                labLevelButton.interactable = true;
            }
            else
            {
                labLevelButton.interactable = false;
            }

        }
    }

    void OnClickLabLevelUp(int type, int needStone)
    {
        if(Common.PaymentCheck(ref User.magicStone, needStone))
        {
            LabSystem.SetLapLevelUp(type);
            RefreshUI();
        }
        else
        {
            //
        }
    }
    bool isCheckAlertOn = false;
    IEnumerator CheckingAlert(int paymentAmount)
    {
        isCheckAlertOn = true;
        var alertPanel = UI_Manager.instance.ShowNeedAlert("UI/ui_laboratoryFill", string.Format("{0} \r\n<size='24'>{1} : {2} -> {3}   {4} : {5} -> {6}</size>",LocalizationManager.GetText("LabPhaseUpQuestion"),LocalizationManager.GetText("LabPhaseTimeInformation"),GetLabWaitTime(User.labLevel), GetLabWaitTime(User.labLevel+1),LocalizationManager.GetText("LabPhaseGetInformation"),User.labLevel, User.labLevel+1) +string.Format("\r\n\r\n<color='yellow'>'{0}' <size='24'>x </size>{1}</color>  {2}", Common.GetCoinCrystalEnergyText(0), Common.GetThousandCommaText(paymentAmount), LocalizationManager.GetText("alertNeedText")));
        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            UI_Manager.instance.ClosePopupAlertUI();

            if(Common.PaymentCheck(ref User.coin, paymentAmount))
            {
                Transform LabaratoryImage = LabImage.transform.parent;
                SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
                GameObject effect = EffectPool.Instance.PopFromPool("SkillUpgradeEffect", LabaratoryImage);
                effect.transform.localScale = new Vector3(1, 1, 1);
                effect.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f);
                Vector3 pos = LabaratoryImage.position;
                pos.z = 0;
                effect.transform.localPosition = pos;
                effect.gameObject.SetActive(true);
                LabaratoryImage.GetComponent<AiryUIAnimatedElement>().ShowElement();
                User.labLevel += 1;
                GoogleSignManager.SaveData();
                RefreshUI();
            }
            else
            {
                UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.coin, paymentAmount);
            }
        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 아니오를 클릭시
        }
        isCheckAlertOn = false;
        yield return null;
    }
}
