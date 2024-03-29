﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Button : MonoBehaviour
{
    public enum ButtonType { ActiveControll,Trigger,SceneLoad, StageSceneLoad, SceneLoadAddtive, ItemBuy,CharacterBuy,ScenePause,ItemSell,Gacha,InventoryAdd};
    public ButtonType buttonType;
    public GameObject targetUI;
    public int targetSceneNumber;
    public enum PaymentType { Coin,BlackCrystal,Cash,Advertisement,None};
    public PaymentType paymentType;
    public GachaSystem.GachaType gachaType;
    public int stageType;
    public int paymentAmount;
    public int buyItemId;
    public int characterId;
    public int sellItemId;
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
                        showUIanimation();
                    }
                    else
                    {
                        targetUI.SetActive(false);
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
                    if(ItemSystem.IsGetAbleItem())
                    {
                        if (Common.PaymentCheck(ref User.portalEnergy, stageHeroCount))
                        {
                            OnButtonEffectSound();
                            Debugging.Log(stageHeroCount + " 에너지 소모. 전투씬 로드 시작 > " + User.portalEnergy);
                            SaveSystem.SavePlayer();
                            LoadSceneManager.instance.LoadStageScene(stageType);
                            this.GetComponent<Button>().interactable = false;
                        }
                        else
                        {
                            UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.energy, stageHeroCount);
                        }
                    }
                    else
                    {
                        UI_Manager.instance.ShowAlert("", LocalizationManager.GetText("alertUnableGetItemMessage"));
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
                        showUIanimation();
                        Invoke("StartPause", 0.75f);
                    }
                    else
                    {
                        targetUI.SetActive(false);
                        StopPause();
                        //hideUIanimation();
                    }
                }
                break;
            case ButtonType.ItemSell:
                SellStart();
                break;
            case ButtonType.Gacha:
                GachaStart();
                break;
            case ButtonType.InventoryAdd:
                InventoryAddStart();
                break;
        }
    }
    void InventoryAddStart()
    {
        if(User.inventoryCount<=450)
        {
            if (!isCheckAlertOn)
            {
                StartCoroutine("CheckingInventoryAddAlert");
            }
        }
        else
        {
            UI_Manager.instance.ShowAlert("", LocalizationManager.GetText("alertUnableInventoryAddMessage"));
        }

    }
    void GachaStart()
    {
        if(paymentType!=PaymentType.Advertisement)
        {
            if (!isCheckAlertOn)
            {
                StartCoroutine(CheckingGachaAlert((int)paymentType));
            }
        }
        else
        {
            GachaProcessing();
        }
    }
    void GachaProcessing()
    {
        int getAbleItemCount = 0;
        if(gachaType==GachaSystem.GachaType.NormalFive||gachaType==GachaSystem.GachaType.SpecialFive)
        {
            getAbleItemCount = 5;
        }
        else if(gachaType==GachaSystem.GachaType.NormalOne||gachaType==GachaSystem.GachaType.SpecialOne||gachaType==GachaSystem.GachaType.FreeAd)
        {
            getAbleItemCount = 1;
        }
        if(ItemSystem.IsGetAbleItem(getAbleItemCount))
        {
            switch (paymentType)
            {
                case PaymentType.Coin:
                    if (Common.PaymentCheck(ref User.coin, paymentAmount))
                    {
                        OnButtonEffectSound();
                        UI_Manager.instance.PopupGetGacha(gachaType);
                    }
                    else
                    {
                        UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.coin, paymentAmount);
                    }
                    break;
                case PaymentType.BlackCrystal:
                    if (Common.PaymentCheck(ref User.blackCrystal, paymentAmount))
                    {
                        OnButtonEffectSound();
                        UI_Manager.instance.PopupGetGacha(gachaType);
                    }
                    else
                    {
                        UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.blackCrystal, paymentAmount);
                    }
                    break;
                case PaymentType.Advertisement:
                    OnButtonEffectSound();
                    UnityAdsManager.instance.ShowRewardedAd(UnityAdsManager.RewardItems.SpeicalGachaOne);
                    break;
            }
        }
        else
        {
            UI_Manager.instance.ShowAlert("", LocalizationManager.GetText("alertUnableGetItemMessage"));
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
    public void SetStageType(int type)
    {
        stageType = type;
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
    void CallbackScriptRefresh()
    {
        if (callBackScript.GetComponent<UI_Shop>() != null)
            callBackScript.GetComponent<UI_Shop>().RefreshUI();
        else if (callBackScript.GetComponent<UI_ShopCharacter>() != null)
            callBackScript.GetComponent<UI_ShopCharacter>().RefreshUI();
        else if (callBackScript.GetComponent<UI_ShopETC>() != null)
            callBackScript.GetComponent<UI_ShopETC>().RefreshUI();
        else if (callBackScript.GetComponent<UI_Manager_InventoryTab>() != null)
            callBackScript.GetComponent<UI_Manager_InventoryTab>().RefreshUI();
    }

    void BuyItemProcessing()
    {
        switch (paymentType)
        {
            case PaymentType.Coin:
                if (Common.PaymentCheck(ref User.coin, paymentAmount))
                {
                    OnButtonEffectSound();
                    if (buyItemId > 9000)
                        ItemSystem.SetObtainMoney(buyItemId);
                    else
                        ItemSystem.SetObtainItem(buyItemId);
                    CallbackScriptRefresh();
                    Item id = ItemSystem.GetItem(buyItemId);
                    GoogleSignManager.SaveData();
                    UI_Manager.instance.ShowGetAlert(id.image, string.Format("<color='yellow'>{0}</color> {1}", ItemSystem.GetItemName(id.id),LocalizationManager.GetText("alertGetMessage3")));
                }
                else
                {
                    UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.coin, paymentAmount);
                }
                break;
            case PaymentType.BlackCrystal:
                if (Common.PaymentCheck(ref User.blackCrystal, paymentAmount))
                {
                    OnButtonEffectSound();
                    if (buyItemId > 9000)
                        ItemSystem.SetObtainMoney(buyItemId);
                    else
                        ItemSystem.SetObtainItem(buyItemId);
                    CallbackScriptRefresh();
                    Item id = ItemSystem.GetItem(buyItemId);
                    GoogleSignManager.SaveData();
                    UI_Manager.instance.ShowGetAlert(id.image, string.Format("<color='yellow'>{0}</color> {1}",ItemSystem.GetItemName(id.id),LocalizationManager.GetText("alertGetMessage3")));
                }
                else
                {
                    UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.blackCrystal, paymentAmount);
                }
                break;
            case PaymentType.Cash:
                OnButtonEffectSound();
                IAPManager.instance.OnBtnPurchaseClicked(buyItemId);
                CallbackScriptRefresh();
                Debugging.Log(buyItemId + " 현금거래 버튼 입니다. >> Cash : " + paymentAmount);
                break;
        }
    }
    void InventoryAddProcessing()
    {
        if (Common.PaymentCheck(ref User.blackCrystal, paymentAmount))
        {
            OnButtonEffectSound();
            User.inventoryCount += 50;
            CallbackScriptRefresh();
            GoogleSignManager.SaveData();
            UI_Manager.instance.ShowGetAlert("",string.Format("{0}\r\n{1} -> <color='yellow'>{2}</color>", LocalizationManager.GetText("alertGetMessage7"),(User.inventoryCount-50),User.inventoryCount));
        }
        else
        {
            UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.blackCrystal, paymentAmount);
        }
    }

    void BuyCharProcessing()
    {
        switch (paymentType)
        {
            case PaymentType.Coin:
                if (Common.PaymentCheck(ref User.coin, paymentAmount))
                {
                    OnButtonEffectSound();
                    HeroSystem.SetObtainHero(characterId);
                    CallbackScriptRefresh();
                    HeroData hd = HeroSystem.GetHero(characterId);
                    GoogleSignManager.SaveData();
                    UI_Manager.instance.ShowGetAlert(hd.image, string.Format("<color='yellow'>{0}</color> {1}", HeroSystem.GetHeroName(hd.id), LocalizationManager.GetText("alertGetMessage5")));
                }
                else
                {
                    UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.coin, paymentAmount);
                }
                break;
            case PaymentType.BlackCrystal:
                if (Common.PaymentCheck(ref User.blackCrystal, paymentAmount))
                {
                    OnButtonEffectSound();
                    HeroSystem.SetObtainHero(characterId);
                    CallbackScriptRefresh();
                    HeroData hd = HeroSystem.GetHero(characterId);
                    GoogleSignManager.SaveData();
                    UI_Manager.instance.ShowGetAlert(hd.image, string.Format("<color='yellow'>{0}</color> {1}", HeroSystem.GetHeroName(hd.id), LocalizationManager.GetText("alertGetMessage5")));
                }
                else
                {
                    UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.blackCrystal, paymentAmount);
                }
                break;
            case PaymentType.Cash:
                OnButtonEffectSound();
                OnButtonEffectSound();
                IAPManager.instance.OnBtnPurchaseClicked(buyItemId);
                CallbackScriptRefresh();
                Debugging.Log(characterId + " 현금거래 버튼 입니다. >> Cash : " + paymentAmount);
                break;
        }
    }

    void SellItemProcessing()
    {
        if(SellAbleCheck(ref sellItemId))
        {
            int value = ItemSystem.GetUserItemByCustomId(sellItemId).value;
            OnButtonEffectSound();
            if (ItemSystem.UseItem(sellItemId, 1))
            {
                SaveSystem.AddUserCoin(value);
                GoogleSignManager.SaveData();
                UI_Manager.instance.ShowGetAlert("Items/coin", string.Format("<color='yellow'>{0}</color> {1}{2}", Common.GetThousandCommaText(value),LocalizationManager.GetText("Coin"),LocalizationManager.GetText("alertGetMessage1")));
                if (callBackScript != null)
                {
                    callBackScript.GetComponent<UI_Manager_InventoryTab>().RefreshUI(Common.OrderByType.NAME);
                }
            }
            else
            {
                Item item = ItemSystem.GetUserItemByCustomId(sellItemId);
                if (item != null)
                    UI_Manager.instance.ShowAlert(item.image, string.Format("<color='yellow'>{0}</color> {1} \r\n <color='grey'><size='20'>{2}</size></color>", ItemSystem.GetItemName(item.id),LocalizationManager.GetText("alertUnableSellMessage"),LocalizationManager.GetText("alertSellText")));

            }
        }
        else
        {
            Item item = ItemSystem.GetUserItemByCustomId(sellItemId);
            if(item!=null)
                UI_Manager.instance.ShowAlert(item.image, string.Format("<color='yellow'>{0}</color> {1} \r\n <color='grey'><size='20'>{2}</size></color>",ItemSystem.GetItemName(item.id),LocalizationManager.GetText("alertUnableSellMessage"),LocalizationManager.GetText("alertUnableSellMessage2")));
        }
    }

    void SellStart()
    {
        if(!isCheckAlertOn)
        {
            StartCoroutine("CheckingSellAlert");
        }
    }
    IEnumerator CheckingInventoryAddAlert()
    {
        isCheckAlertOn = true;
        paymentAmount = 30;
        var alertPanel = UI_Manager.instance.ShowNeedAlert("Items/" + Enum.GetName(typeof(PaymentType), paymentType), string.Format("<color='yellow'>'{0}' <size='24'>x </size>{1}</color>  {2}\r\n{3} -> <color='yellow'>{4}</color>", Common.GetCoinCrystalEnergyText(1), paymentAmount, LocalizationManager.GetText("alertNeedMessage7"),User.inventoryCount,(User.inventoryCount+50)));
        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 예를 클릭시
            InventoryAddProcessing();
        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 아니오를 클릭시
        }
        isCheckAlertOn = false;
        yield return null;
    }

    IEnumerator CheckingAlert(int type)
    {
        isCheckAlertOn = true;
        var alertPanel = UI_Manager.instance.ShowNeedAlert("Items/" + Enum.GetName(typeof(PaymentType), paymentType), string.Format("<color='yellow'>'{0}' <size='24'>x </size>{1}</color>  {2}", Common.GetCoinCrystalEnergyText(type), paymentAmount,LocalizationManager.GetText("alertNeedMessage5")));
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

    IEnumerator CheckingSellAlert()
    {
        isCheckAlertOn = true;
        Item sellItem = ItemSystem.GetUserItemByCustomId(sellItemId);
        var alertPanel = UI_Manager.instance.ShowNeedAlert(sellItem.image, string.Format("<color='red'>'{0}'</color> {1}{2} <color='yellow'>{3} {4}</color>  {5}", ItemSystem.GetItemName(sellItem.id), 1, LocalizationManager.GetText("alertNeedMessage3"),Common.GetThousandCommaText(sellItem.value),LocalizationManager.GetText("Coin"), LocalizationManager.GetText("alertNeedMessage4")));
        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            UI_Manager.instance.ClosePopupAlertUI();
            SellItemProcessing();

        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
        }
        isCheckAlertOn = false;
        yield return null;
    }

    IEnumerator CheckingGachaAlert(int type)
    {
        isCheckAlertOn = true;
        var alertPanel = UI_Manager.instance.ShowNeedAlert(Common.GetCoinCrystalEnergyImagePath(type), string.Format("<color='red'>'{0}'</color>{1} <color='yellow'>{2} {3}</color>  {4}", LocalizationManager.GetText("gachaType"+((int)gachaType+1)),LocalizationManager.GetText("alertNeedMessage6"), Common.GetThousandCommaText(paymentAmount),Common.GetCoinCrystalEnergyText(type),LocalizationManager.GetText("alertNeedMessage5")));
        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            UI_Manager.instance.ClosePopupAlertUI();
            GachaProcessing();
        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
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
    bool SellAbleCheck(ref int targetItemId)
    {
        Item item = ItemSystem.GetUserItemByCustomId(targetItemId);
        if(item!=null)
        {
            if(item.equipCharacterId==0&&item.count>0)
            {
                return true;
            }
        }
        return false;
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
                enumScript.stageType = EditorGUILayout.IntField("StageType", enumScript.stageType);
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
            case UI_Button.ButtonType.ItemSell:
                enumScript.callBackScript = (GameObject)EditorGUILayout.ObjectField("InventroyScript", enumScript.callBackScript, typeof(GameObject), true);
                enumScript.sellItemId = EditorGUILayout.IntField("SellItemId", enumScript.sellItemId);
                enumScript.audioClip = (AudioClip)EditorGUILayout.ObjectField("ButtonAudioClip", enumScript.audioClip, typeof(AudioClip), true);
                break;
            case UI_Button.ButtonType.Gacha:
                enumScript.gachaType = (GachaSystem.GachaType)EditorGUILayout.EnumFlagsField("GachaType", enumScript.gachaType);
                enumScript.paymentType = (UI_Button.PaymentType)EditorGUILayout.EnumFlagsField("PaymentType", enumScript.paymentType);
                enumScript.paymentAmount = EditorGUILayout.IntField("Amount", enumScript.paymentAmount);
                enumScript.audioClip = (AudioClip)EditorGUILayout.ObjectField("ButtonAudioClip", enumScript.audioClip, typeof(AudioClip), true);
                break;
            case UI_Button.ButtonType.InventoryAdd:
                enumScript.callBackScript = (GameObject)EditorGUILayout.ObjectField("InventroyScript", enumScript.callBackScript, typeof(GameObject), true);
                enumScript.paymentType = UI_Button.PaymentType.BlackCrystal;
                enumScript.paymentAmount = EditorGUILayout.IntField("Amount", enumScript.paymentAmount);
                enumScript.audioClip = (AudioClip)EditorGUILayout.ObjectField("ButtonAudioClip", enumScript.audioClip, typeof(AudioClip), true);
                break;
        }
    }
}

#endif

