using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class UI_Button : MonoBehaviour
{
    public enum ButtonType { ActiveControll,Trigger,SceneLoad, StageSceneLoad, SceneLoadAddtive, ItemBuy,CharacterBuy,ScenePause};
    public ButtonType buttonType;
    public GameObject targetUI;
    public int targetSceneNumber;
    public enum PaymentType { Coin,BlackCrystal,Cash};
    public PaymentType paymentType;
    public int paymentAmount;
    public int buyItemId;
    public int characterId;
    public GameObject callBackScript;
    public AudioClip audioClip;

    private void OnEnable()
    {
        if (!this.GetComponent<Button>().interactable)
            this.GetComponent<Button>().interactable = true;
    }

    public void OnClick()
    {
        StopPause();
        switch (buttonType)
        {
            case ButtonType.ActiveControll:
                OnButtonEffectSound();
                if (targetUI != null)
                {
                    if (!targetUI.activeSelf)
                    {
                        targetUI.SetActive(true);
                        targetUI.SetActive(true);
                        //if (Camera.main.GetComponent<BlurOptimized>()!=null)
                        //{
                        //    Camera.main.GetComponent<BlurOptimized>().enabled = true;
                        //}
                        showUIanimation();
                    }
                    else
                    {
                        targetUI.SetActive(false);
                        //if (Camera.main.GetComponent<BlurOptimized>() != null&& UI_Manager.instance.GetPopupPanelCount()==0)
                        //{
                        //    Camera.main.GetComponent<BlurOptimized>().enabled = false;
                        //}
                        //hideUIanimation();
                    }
                }
                break;
            case ButtonType.Trigger:
                break;
            case ButtonType.StageSceneLoad:
                int stageHeroCount = CharactersManager.instance.GetStageHeroCount();
                if(stageHeroCount<1)
                {
                    Debugging.Log("영웅을 선택해주세요");
                }
                else
                {
                    if (User.portalEnergy < stageHeroCount)
                        UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.energy, stageHeroCount);
                    else
                    {
                        OnButtonEffectSound();
                        User.portalEnergy -= stageHeroCount;
                        Debugging.Log(stageHeroCount + " 에너지 소모. 전투씬 로드 시작 > " + User.portalEnergy);
                        SaveSystem.SavePlayer();
                        LoadSceneManager.instance.LoadStageScene();
                        this.GetComponent<Button>().interactable = false;
                    }
                }

                break;
            case ButtonType.SceneLoad:
                OnButtonEffectSound();
                LoadSceneManager.instance.LoadScene(targetSceneNumber);
                this.GetComponent<Button>().interactable = false;
                break;
            case ButtonType.SceneLoadAddtive:
                OnButtonEffectSound();
                LoadSceneManager.instance.LoadSceneAddtive(targetSceneNumber);
                this.GetComponent<Button>().interactable = false;
                break;
            case ButtonType.ItemBuy:
                BuyStart();
                break;
            case ButtonType.CharacterBuy:
                BuyStart(1);
                break;
            case ButtonType.ScenePause:
                OnButtonEffectSound();
                if (targetUI != null)
                {
                    if (!targetUI.activeSelf)
                    {
                        targetUI.SetActive(true);
                        targetUI.SetActive(true);
                        if (Camera.main.GetComponent<BlurOptimized>() != null)
                        {
                            Camera.main.GetComponent<BlurOptimized>().enabled = true;
                        }
                        showUIanimation();
                        Invoke("StartPause", 0.75f);
                    }
                    else
                    {
                        targetUI.SetActive(false);
                        if (Camera.main.GetComponent<BlurOptimized>() != null && UI_Manager.instance.GetPopupPanelCount() == 0)
                        {
                            Camera.main.GetComponent<BlurOptimized>().enabled = false;
                        }
                        StopPause();
                        //hideUIanimation();
                    }
                }
                break;
        }
    }

    void StartPause()
    {
        Time.timeScale = 0;
    }
    void StopPause()
    {
        Time.timeScale = 1;
    }

    void OnButtonEffectSound()
    {
        if (SoundManager.instance != null)
        {
            if (audioClip == null)
                SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
            else
                SoundManager.instance.EffectSourcePlay(audioClip);
        }
    }

    void BuyStart(int type=0)
    {
        if(!isCheckAlertOn)
        {
            StartCoroutine(CheckingAlert(type));
        }
    }

    void BuyItemProcessing()
    {
        switch (paymentType)
        {
            case PaymentType.Coin:
                if (PaymentCheck(ref User.coin, paymentAmount))
                {
                    OnButtonEffectSound();
                    if (buyItemId > 9000)
                        ItemSystem.SetObtainMoney(buyItemId);
                    else
                        ItemSystem.SetObtainItem(buyItemId);
                    callBackScript.GetComponent<UI_Shop>().RefreshUI();
                    Item id = ItemSystem.GetItem(buyItemId);
                    UI_Manager.instance.ShowGetAlert(id.image, string.Format("<color='yellow'>{0}</color> 아이템을 획득하였습니다.", id.name));
                }
                else
                {
                    UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.coin, paymentAmount);
                }
                break;
            case PaymentType.BlackCrystal:
                if (PaymentCheck(ref User.blackCrystal, paymentAmount))
                {
                    OnButtonEffectSound();
                    if (buyItemId > 9000)
                        ItemSystem.SetObtainMoney(buyItemId);
                    else
                        ItemSystem.SetObtainItem(buyItemId);
                    callBackScript.GetComponent<UI_Shop>().RefreshUI();
                    Item id = ItemSystem.GetItem(buyItemId);
                    UI_Manager.instance.ShowGetAlert(id.image, string.Format("<color='yellow'>{0}</color> 아이템을 획득하였습니다.", id.name));
                }
                else
                {
                    OnButtonEffectSound();
                    UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.blackCrystal, paymentAmount);
                }
                break;
            case PaymentType.Cash:
                Debugging.Log(buyItemId + " 현금거래 버튼 입니다. >> Cash : " + paymentAmount);
                break;
        }
    }

    void BuyCharProcessing()
    {
        switch (paymentType)
        {
            case PaymentType.Coin:
                if (PaymentCheck(ref User.coin, paymentAmount))
                {
                    OnButtonEffectSound();
                    HeroSystem.SetObtainHero(characterId);
                    callBackScript.GetComponent<UI_ShopCharacter>().RefreshUI();
                    HeroData hd = HeroSystem.GetHero(characterId);
                    UI_Manager.instance.ShowGetAlert("HeroThumbnail/" + hd.image, string.Format("<color='yellow'>{0}</color> 영웅을 획득하였습니다.", hd.name));
                }
                else
                {
                    UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.coin, paymentAmount);
                }
                break;
            case PaymentType.BlackCrystal:
                if (PaymentCheck(ref User.blackCrystal, paymentAmount))
                {
                    OnButtonEffectSound();
                    HeroSystem.SetObtainHero(characterId);
                    callBackScript.GetComponent<UI_ShopCharacter>().RefreshUI();
                    HeroData hd = HeroSystem.GetHero(characterId);
                    UI_Manager.instance.ShowGetAlert("HeroThumbnail/" + hd.image, string.Format("<color='yellow'>{0}</color> 영웅을 획득하였습니다.", hd.name));
                }
                else
                {
                    UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.blackCrystal, paymentAmount);
                }
                break;
            case PaymentType.Cash:
                OnButtonEffectSound();
                Debugging.Log(characterId + " 현금거래 버튼 입니다. >> Cash : " + paymentAmount);
                break;
        }
    }
    IEnumerator CheckingAlert(int type)
    {
        isCheckAlertOn = true;
        var alertPanel = UI_Manager.instance.ShowNeedAlert("Items/" + Enum.GetName(typeof(PaymentType), paymentType), string.Format("<color='yellow'>'{0}' <size='24'>x </size>{1}</color>  사용하여 구매하시겠습니까?", Enum.GetName(typeof(PaymentType), paymentType), paymentAmount));
        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 예를 클릭시
            if (type == 0)
                BuyItemProcessing();
            else
                BuyCharProcessing();
        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 아니오를 클릭시
        }
        isCheckAlertOn = false;
        yield return null;
    }
    bool isCheckAlertOn = false;
    public void StartSelectAbility()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        if (!isCheckAlertOn)
            StartCoroutine("CheckingAlert");
    }

    bool PaymentCheck(ref int target, int payment)
    {
        if (target - payment >= 0)
        {
            target -= payment;
            return true;
        }
        else
        {
            return false;
        }
    }

    void showUIanimation()
    {
        if (targetUI.GetComponentsInChildren<AiryUIAnimatedElement>() != null)
        {
            foreach (var element in targetUI.GetComponentsInChildren<AiryUIAnimatedElement>())
                element.ShowElement();
        }
    }
    void hideUIanimation()
    {
        if (targetUI.GetComponentsInChildren<AiryUIAnimatedElement>() != null)
        {
            foreach (var element in targetUI.GetComponentsInChildren<AiryUIAnimatedElement>())
                element.HideElement();
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(UI_Button))]
public class ButtonInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var enumScript = target as UI_Button;

        enumScript.buttonType = (UI_Button.ButtonType)EditorGUILayout.EnumPopup(enumScript.buttonType);

        switch (enumScript.buttonType)
        {
            case UI_Button.ButtonType.ActiveControll:
                enumScript.targetUI = (GameObject)EditorGUILayout.ObjectField("TargetUI", enumScript.targetUI, typeof(GameObject), true);
                enumScript.audioClip = (AudioClip)EditorGUILayout.ObjectField("ButtonAudioClip", enumScript.audioClip, typeof(AudioClip), true);
                break;
            case UI_Button.ButtonType.Trigger:
                enumScript.audioClip = (AudioClip)EditorGUILayout.ObjectField("ButtonAudioClip", enumScript.audioClip, typeof(AudioClip), true);
                break;
            case UI_Button.ButtonType.SceneLoad:
                enumScript.targetSceneNumber = EditorGUILayout.IntField("TargetNumber", enumScript.targetSceneNumber);
                enumScript.audioClip = (AudioClip)EditorGUILayout.ObjectField("ButtonAudioClip", enumScript.audioClip, typeof(AudioClip), true);
                break;
            case UI_Button.ButtonType.StageSceneLoad:
                enumScript.audioClip = (AudioClip)EditorGUILayout.ObjectField("ButtonAudioClip", enumScript.audioClip, typeof(AudioClip), true);
                break;
            case UI_Button.ButtonType.SceneLoadAddtive:
                enumScript.targetSceneNumber = EditorGUILayout.IntField("TargetNumber", enumScript.targetSceneNumber);
                enumScript.audioClip = (AudioClip)EditorGUILayout.ObjectField("ButtonAudioClip", enumScript.audioClip, typeof(AudioClip), true);
                break;
            case UI_Button.ButtonType.ItemBuy:
                enumScript.buyItemId = EditorGUILayout.IntField("BuyItemId", enumScript.buyItemId);
                enumScript.paymentType = (UI_Button.PaymentType)EditorGUILayout.EnumFlagsField("PaymentType", enumScript.paymentType);
                enumScript.paymentAmount = EditorGUILayout.IntField("Amount", enumScript.paymentAmount);
                enumScript.audioClip = (AudioClip)EditorGUILayout.ObjectField("ButtonAudioClip", enumScript.audioClip, typeof(AudioClip), true);
                break;
            case UI_Button.ButtonType.CharacterBuy:
                enumScript.callBackScript = (GameObject)EditorGUILayout.ObjectField("TargetUI", enumScript.callBackScript, typeof(GameObject), true);
                enumScript.characterId = EditorGUILayout.IntField("BuyItemId", enumScript.characterId);
                enumScript.paymentType = (UI_Button.PaymentType)EditorGUILayout.EnumFlagsField("PaymentType", enumScript.paymentType);
                enumScript.paymentAmount = EditorGUILayout.IntField("Amount", enumScript.paymentAmount);
                enumScript.audioClip = (AudioClip)EditorGUILayout.ObjectField("ButtonAudioClip", enumScript.audioClip, typeof(AudioClip), true);
                break;
            case UI_Button.ButtonType.ScenePause:
                enumScript.targetUI = (GameObject)EditorGUILayout.ObjectField("TargetUI", enumScript.targetUI, typeof(GameObject), true);
                enumScript.audioClip = (AudioClip)EditorGUILayout.ObjectField("ButtonAudioClip", enumScript.audioClip, typeof(AudioClip), true);
                break;
        }
    }
}

#endif

