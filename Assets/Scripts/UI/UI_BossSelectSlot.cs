using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossSelectSlot : MonoBehaviour
{
    public int BossID;
    public int rewardCoin;
    public int rewardCrystal;
    public int rewardScroll;
    public int rewardTranscendenceStone;
    public int rewardBoxType;
    public int rewardBoxCount;
    int skipNeedCrystal;

    public bool isEnableBoss;
    public bool isLevelCheck;
    bool isCheckAlert = false;
    public int ableStage;

    public Button selectButton;
    public Button initationButton;
    public Image bossImage;
    public Text bossNameText;
    public Text timeText;
    public GameObject lockPanel;
    public GameObject ablePanel;

    double[] msToWait = { 3600000, 7200000, 10800000, 14400000, 21600000, 28800000, 43200000, 86400000 };
    int[] bossIds = { 1003, 1005, 1001, 1002, 1006, 1004, 1007, 1008 };
    double initMsToWait;
    public ulong lastBossClear;



    private void Awake()
    {
        selectButton = this.GetComponent<Button>();
        initationButton = this.transform.GetChild(3).GetComponent<Button>();
        BossID = bossIds[Mathf.Clamp(ableStage - 2, 0, bossIds.Length - 1)];
    }
    void InitCheck()
    {
        isEnableBoss = false;
        if (ableStage == 9)
        {
            if (MapSystem.GetUserMap(80) != null)
                isLevelCheck = MapSystem.GetUserMap(80).clearPoint > 1;
            else
                isLevelCheck = false;
        }
        else
            isLevelCheck = MapSystem.GetMap(MapSystem.GetCurrentAllMapId()).stageNumber >= ableStage;
        if (SaveSystem.IsPremiumPassAble())
        {
            initMsToWait = (int)(msToWait[Mathf.Clamp(ableStage-2,0,msToWait.Length-1)] * 0.8);
        }
        else
        {
            initMsToWait = msToWait[Mathf.Clamp(ableStage - 2, 0, msToWait.Length - 1)];
        }

    }

    private void OnEnable()
    {
        if (initationButton != null)
            initationButton.gameObject.SetActive(false);
        InitCheck();
        RefreshUI();
        lastBossClear = SaveSystem.GetBossClearTime(ableStage - 1);
        if (!IsBossReady())
        {
            selectButton.interactable = false;
        }
    }

    void Update()
    {
        if (!selectButton.IsInteractable()&&isLevelCheck)
        {
            if (IsBossReady())
            {
                if(initationButton!=null)
                    initationButton.gameObject.SetActive(false);
                selectButton.interactable = true;
                timeText.text = "";
                return;
            }

            ulong diff = ((ulong)DateTime.Now.Ticks - lastBossClear);
            ulong m = diff / TimeSpan.TicksPerMillisecond;
            double secondsLeft = (double)(initMsToWait - m) / 1000.0f;
            SetSkipNeedCrystal(secondsLeft);
            if (initationButton != null)
            {
                initationButton.gameObject.SetActive(true);
                initationButton.GetComponentInChildren<Text>().text = skipNeedCrystal.ToString("N0");
            }
            string r = "";
            r += ((int)secondsLeft / 3600).ToString() + ": ";
            secondsLeft -= ((int)secondsLeft / 3600) * 3600;
            r += ((int)secondsLeft / 60).ToString("00") + ": ";
            r += (secondsLeft % 60).ToString("00");
            timeText.text = r;
        }
    }
    public void EnableSlot(bool enable)
    {
        isEnableBoss = enable;
        RefreshUI();
    }

    public void RefreshUI()
    {
        bossImage.sprite = HeroSystem.GetHeroThumbnail(BossID);
        if(Common.bossModeDifficulty == 1)
        {
            bossNameText.text = string.Format("{0}<color='yellow'><size='25'>({1})</size></color>",HeroSystem.GetHeroName(BossID),"Normal");
            rewardCoin = 10000 + (ableStage * ableStage * 5000);
            rewardCrystal = (int)(ableStage * 0.5f) + ableStage + 5;
            rewardScroll = 10 + ((ableStage - 1) * (ableStage - 1));
            rewardTranscendenceStone = Mathf.Clamp(10 + ((ableStage - 1) * (ableStage - 1)),0,70);
            rewardBoxCount = 0;
        }
        else if (Common.bossModeDifficulty == 2)
        {
            bossNameText.text = string.Format("{0}<color='red'><size='25'>({1})</size></color>", HeroSystem.GetHeroName(BossID), "Hard");
            rewardCoin = 15000 + (ableStage * ableStage * 6000);
            rewardCrystal = (int)(ableStage * 0.7f) + ableStage + 7;
            rewardScroll = 5 + ((ableStage - 1) * (ableStage - 1));
            rewardTranscendenceStone = 0;
            if (ableStage>7)
            {
                rewardBoxType = 2;
                rewardBoxCount = 1;
            }
            else if(ableStage>4&&ableStage<=7)
            {
                rewardBoxType = 1;
                rewardBoxCount = 1;
            }
            else
            {
                rewardBoxType = 0;
                rewardBoxCount = 1;
            }
        }
        else
        {
            bossNameText.text = string.Format("{0}<color='white'><size='25'>({1})</size></color>", HeroSystem.GetHeroName(BossID), "Easy");
            rewardCoin = 5000 + (ableStage * ableStage * 2000);
            rewardCrystal = (int)(ableStage * 0.5f) + ableStage + 2;
            rewardScroll = 5 + ((ableStage - 1) * (ableStage - 1));
            rewardTranscendenceStone = 0;
            rewardBoxCount = 0;
        }
        if (isEnableBoss)
        {
            lockPanel.SetActive(true);
        }
        else
        {
            lockPanel.SetActive(false);
        }
        if (isLevelCheck)
        {
            ablePanel.SetActive(false);
        }
        else
        {
            lockPanel.SetActive(false);
            ablePanel.GetComponentInChildren<Text>().text = string.Format("(!) {0} {1}",  ableStage-1,LocalizationManager.GetText("bossSelectLockStageAlert"));
            ablePanel.SetActive(true);
        }
    }

    bool IsBossReady()
    {
        ulong diff = ((ulong)DateTime.Now.Ticks - lastBossClear);
        ulong m = diff / TimeSpan.TicksPerMillisecond;
        double secondsLeft = (double)(initMsToWait - m) / 1000.0f;

        if (secondsLeft < 0)
        {
            timeText.text = "";
            return true;
        }
        return false;
    }

    void SetSkipNeedCrystal(double second)
    {
        skipNeedCrystal = Mathf.Clamp((int)((second / 60) + 20), 20, 1000);
    }

    public void SkipTime()
    {
        if (!IsBossReady()&&!isCheckAlert)
        {
            isCheckAlert = true;
            StartCoroutine("CheckingAlert");
        }
    }
    IEnumerator CheckingAlert()
    {
        GameObject alertPanel;
        alertPanel = UI_Manager.instance.ShowNeedAlert(Common.GetCoinCrystalEnergyImagePath(1), string.Format("{0} {1} {2}", skipNeedCrystal, Common.GetCoinCrystalEnergyText(1), LocalizationManager.GetText("alertBossModeSkipMessage")));

        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            UI_Manager.instance.ClosePopupAlertUI();
            if (Common.PaymentCheck(ref User.blackCrystal, skipNeedCrystal))
            {
                lastBossClear = (ulong)0;
                SaveSystem.SetBossClearTime(ableStage - 1, lastBossClear.ToString());
                RefreshUI();
            }
            else
            {
                UI_Manager.instance.ShowAlert(Common.GetCoinCrystalEnergyImagePath(1), LocalizationManager.GetText("alertCrystal"));
            }
        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 아니오를 클릭시
        }
        isCheckAlert = false;
        yield return null;
    }
}
