using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Roulette : MonoBehaviour
{
    public float multiplier = 1;
    public float reducer;
    bool round1 = false;
    public bool isStoped = false;
    public bool isStarted = false;
    public GameObject Spinner;
    public GameObject Needle;
    public Button RouletteButton;

    private void OnEnable()
    {
        reducer = UnityEngine.Random.Range(0.01f, 0.5f);
        isStarted = false;

        RefreshUI();
    }

    void RefreshUI()
    {
        if (isAbleRoulette())
        {
            RouletteButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("RouletteButton");
            RouletteButton.interactable = true;
        }
        else
        {
            RouletteButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("RouletteUnableButton");
            RouletteButton.interactable = false;
        }
    }

    private bool isAbleRoulette()
    {
        string currentday = DateTime.Now.ToString("dd");
        if (currentday.Equals(User.eventSaveDate) || currentday == User.eventSaveDate)
            return false;
        else
            return true;
    }

    private void FixedUpdate()
    {
        if(isStarted && !isStoped)
            RouletteUpdate();
    }

    public void StartRoulette()
    {
        if(!isStarted)
        {
            isStarted = true;
            isStoped = false;
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_roulette);
            RouletteButton.interactable = false;
        }
    }

    public void GetItem(float rotationZ)
        { 
        // 수정 30개
        if(rotationZ>45 && rotationZ <= 105)
        {
            SaveSystem.AddUserCrystal(30);
            UI_Manager.instance.ShowGetAlert(Common.GetCoinCrystalEnergyImagePath(1), string.Format("<color='yellow'>{0}</color> {1} {2}", Common.GetThousandCommaText(30), LocalizationManager.GetText("Crystal"), LocalizationManager.GetText("alertGetMessage1")));

        }
        // 마석 30개
        else if(rotationZ > 105 && rotationZ <=165)
        {
            SaveSystem.AddUserMagicStone(30);
            UI_Manager.instance.ShowGetAlert(Common.GetCoinCrystalEnergyImagePath(5), string.Format("<color='yellow'>{0}</color> {1} {2}", Common.GetThousandCommaText(30), LocalizationManager.GetText("MagicStone"), LocalizationManager.GetText("alertGetMessage1")));

        }
        // 랜덤 스페셜 상자 1개
        else if(rotationZ >165 && rotationZ <= 225)
        {
            UI_Manager.instance.PopupGetGacha(GachaSystem.GachaType.SpecialOne);
        }
        //  노말 장비 상자 5개
        else if (rotationZ >225 && rotationZ<=285)
        {
            UI_Manager.instance.PopupGetGacha(GachaSystem.GachaType.NormalFive);
        }
        // 코인 30,000
        else if(rotationZ > 285 && rotationZ<=345)
        {
            SaveSystem.AddUserCoin(30000);
            UI_Manager.instance.ShowGetAlert(Common.GetCoinCrystalEnergyImagePath(0), string.Format("<color='yellow'>{0}</color> {1} {2}", Common.GetThousandCommaText(30000), LocalizationManager.GetText("Coin"), LocalizationManager.GetText("alertGetMessage1")));
        }
        // 코인 10,000
        else
        {
            SaveSystem.AddUserCoin(10000);
            UI_Manager.instance.ShowGetAlert(Common.GetCoinCrystalEnergyImagePath(0), string.Format("<color='yellow'>{0}</color> {1} {2}", Common.GetThousandCommaText(10000), LocalizationManager.GetText("Coin"), LocalizationManager.GetText("alertGetMessage1")));
        }
        User.eventSaveDate = DateTime.Now.ToString("dd");
        RefreshUI();
    }

    public void RouletteUpdate()
    {
        if(multiplier > 0)
        {
            Spinner.transform.Rotate(Vector3.forward, 1 * multiplier);
        }
        else
        {
            if(!isStoped)
                GetItem(Spinner.transform.localEulerAngles.z);
            isStoped = true;
        }
        if(multiplier < 20 && !round1)
        {
            multiplier += 0.1f;
        }
        else
        {
            round1 = true;
        }

        if(round1&&multiplier>0)
        {
            multiplier -= reducer;
        }
    }

}
