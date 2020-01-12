using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance = null;
    public GameObject canvasOverlay;
    public GameObject canvasPopup;
    public GameObject Title;
    public GameObject CoverUI;
    public GameObject PopHeroInfoSummaryUI;
    public GameObject PopHeroInfoUI;
    public GameObject PopGetAbilityUI;
    public GameObject PopupInterActiveCover;
    public GameObject PopupAlertUI;
    public GameObject PopupGetGachaUI;
    public GameObject MissionButton;
    public GameObject ShopUI;
    public GameObject LoginRewardUI;
    public GameObject BossEndingPanel;
    public GameObject DailyCheckUI;

    public enum PopupAlertTYPE { energy,scroll,coin,blackCrystal}

    GameObject popupPanel;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
        if (popupPanel == null)
            popupPanel = GameObject.FindWithTag("PopupPanel") as GameObject;
    }
    private void OnEnable()
    {
        //if (Camera.main.GetComponent<BlurOptimized>()!=null)
        //    Camera.main.GetComponent<BlurOptimized>().enabled = false;
        if (Common.GetSceneCompareTo(Common.SCENE.MAIN))
            ShowTitle();
    }
    public void ShowTitle()
    {
        if (Title != null && PlayerPrefs.GetInt("isTutorialEnd")==2)
        {
            Title.gameObject.SetActive(true);

            if (Title.GetComponent<AiryUIAnimatedElement>() != null)
                Title.GetComponent<AiryUIAnimatedElement>().ShowElement();
        }
    }
    public void ShowDailyCheckUI(int day)
    {
        if(DailyCheckUI!=null)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
            DailyCheckUI.gameObject.SetActive(true);
            DailyCheckUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
            DailyCheckUI.GetComponent<UI_DailyCheck>().StartDailyUI(day);
        }
    }
    public void ShowBossEndingUI(int rewardCoin, int rewardCrystal, int rewardScroll,int rewardTranscendenceStone)
    {
        if(BossEndingPanel!=null)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
            Transform rewardParent = BossEndingPanel.GetComponentInChildren<HorizontalLayoutGroup>().transform;
            rewardParent.transform.GetChild(0).GetComponentInChildren<Text>().text = string.Format("+ {0}", Common.GetThousandCommaText(rewardCoin));
            rewardParent.transform.GetChild(1).GetComponentInChildren<Text>().text = string.Format("+ {0}", Common.GetThousandCommaText(rewardCrystal));
            rewardParent.transform.GetChild(2).GetComponentInChildren<Text>().text = string.Format("+ {0}", Common.GetThousandCommaText(rewardScroll));
            if(rewardTranscendenceStone>0)
            {
                rewardParent.transform.GetChild(3).gameObject.SetActive(true);
                rewardParent.transform.GetChild(3).GetComponentInChildren<Text>().text = string.Format("+ {0}", Common.GetThousandCommaText(rewardTranscendenceStone));
            }
            else
            {
                rewardParent.transform.GetChild(3).gameObject.SetActive(false);
            }
            BossEndingPanel.gameObject.SetActive(true);
        }
    }
    public void MissionUIAlertOn(bool onoff)
    {
        if(MissionButton!=null)
        {
            if(onoff)
                MissionButton.transform.GetChild(2).gameObject.SetActive(true);
            else
                MissionButton.transform.GetChild(2).gameObject.SetActive(false);
        }
    }
    public void ShowLoginRewardUI(int rewardCoin)
    {
        if (LoginRewardUI != null)
        {
            LoginRewardUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponentInChildren<Text>().text = string.Format("{0} {1}", Common.GetThousandCommaText(rewardCoin), LocalizationManager.GetText("Coin"));
            Transform buttonParent = LoginRewardUI.GetComponentInChildren<HorizontalLayoutGroup>().transform;
            buttonParent.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            buttonParent.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate {
                OnLoginRewardClick(false, rewardCoin);
            });
            buttonParent.transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            buttonParent.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate {
                OnLoginRewardClick(true, rewardCoin);
            });
            LoginRewardUI.SetActive(true);
            LoginRewardUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
        }
    }
    public void OnLoginRewardClick(bool isAdvertisement, int rewardCoin)
    {
        if (isAdvertisement)
            UnityAdsManager.instance.ShowRewardedAd(UnityAdsManager.RewardItems.Coin, rewardCoin*2);
        else
        {
            SaveSystem.AddUserCoin(rewardCoin);
            UI_Manager.instance.ShowGetAlert("Items/coin", string.Format("<color='yellow'>{0}</color> {1} {2}", Common.GetThousandCommaText(rewardCoin), LocalizationManager.GetText("Coin"), LocalizationManager.GetText("alertGetMessage1")));
        }
        //TODO 시간저장
        LoginRewardUI.SetActive(false);

    }

    public void ShowShopUI(int type)
    {
        if(ShopUI!=null)
        {
            CloseAllPopupPanel();
            ShopUI.SetActive(true);
            SoundManager.instance.EffectOnOff(AudioClipManager.instance.ui_shop);
            UI_TabManager shopTabManager = ShopUI.GetComponentInChildren<UI_TabManager>();
            switch (type)
            {
                case 0://coin
                    shopTabManager.OnTabButtonClick(1);
                    break;
                case 1://crystal
                    shopTabManager.OnTabButtonClick(0);
                    break;
                case 2://energy
                    shopTabManager.OnTabButtonClick(2);
                    break;
                case 4://패키지
                    shopTabManager.OnTabButtonClick(4);
                    break;
            }
        }
    }
    public GameObject GetPopupPanel(string popupName)
    {
        GameObject popupPanel = GameObject.FindWithTag("PopupPanel") as GameObject;
        if (popupPanel != null && popupPanel.transform.childCount > 0)
        {
            for(var i = 0; i < popupPanel.transform.childCount; i++)
            {
                if (popupPanel.transform.GetChild(i).name.Replace("PanelPop", "").Equals(popupName))
                {
                    Debugging.Log("Get Panel POP Succeed. ==> " + popupPanel.transform.GetChild(i).name);
                    return popupPanel.transform.GetChild(i).gameObject;
                }
            }
        }
        Debugging.Log("Panel POP! ==> " + popupName + " is NULL ");
        return null;
    }

    public void OpenPopupPanel(string popupName)
    {
        if(popupPanel==null)
            popupPanel = GameObject.FindWithTag("PopupPanel") as GameObject;
        GameObject popupObject = null;
        if (popupPanel != null && popupPanel.transform.childCount > 0)
        {
            for (var i = 0; i < popupPanel.transform.childCount; i++)
            {
                if (popupPanel.transform.GetChild(i).name.Replace("PanelPop", "").Equals(popupName))
                {
                    popupObject = popupPanel.transform.GetChild(i).gameObject;
                    break;
                }
            }
        }
        if(popupObject!=null&&popupObject.GetComponentInChildren<AiryUIAnimatedElement>()!=null)
        {
            popupObject.gameObject.SetActive(true);
            showUIanimation(popupObject);
        }
    }
    public void OpenEndGamePanel(bool isWin, int clearPoint = 1)
    {
        if (popupPanel == null)
            popupPanel = GameObject.FindWithTag("PopupPanel") as GameObject;
        GameObject popupObject = null;
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
        if (popupPanel != null && popupPanel.transform.childCount > 0)
        {
            for (var i = 0; i < popupPanel.transform.childCount; i++)
            {
                if(isWin)
                {
                    if (popupPanel.transform.GetChild(i).name.Equals("PanelPopEndGame"))
                    {
                        popupObject = popupPanel.transform.GetChild(i).gameObject;
                        break;
                    }
                }
                else
                {
                    if (popupPanel.transform.GetChild(i).name.Equals("PanelPopDefeat"))
                    {
                        popupObject = popupPanel.transform.GetChild(i).gameObject;
                        break;
                    }
                }
            }
        }
        if (popupObject != null && popupObject.GetComponentInChildren<AiryUIAnimatedElement>() != null)
        {
            Text resultText = popupObject.transform.GetChild(2).GetComponentInChildren<Text>();
            if (clearPoint == 2)
                resultText.text = string.Format("{0}", LocalizationManager.GetText("stageVictoryClear2"));
            else if (clearPoint==3)
                resultText.text = string.Format("{0}", LocalizationManager.GetText("stageVictoryClear3"));
            else
                resultText.text = string.Format("{0}",LocalizationManager.GetText("stageVictoryClear1"));
            popupObject.gameObject.SetActive(true);
            showUIanimation(popupObject);
            this.gameObject.SetActive(false);
        }
    }
    public void OpenAttackEndGamePanel(int dam)
    {
        if (popupPanel == null)
            popupPanel = GameObject.FindWithTag("PopupPanel") as GameObject;
        GameObject popupObject = null;
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
        if (popupPanel != null && popupPanel.transform.childCount > 0)
        {
            for (var i = 0; i < popupPanel.transform.childCount; i++)
            {
                if (popupPanel.transform.GetChild(i).name.Equals("PanelPopEndGame"))
                {
                    popupObject = popupPanel.transform.GetChild(i).gameObject;
                    break;
                }
            }
        }
        if (popupObject != null && popupObject.GetComponentInChildren<AiryUIAnimatedElement>() != null)
        {
            Text resultText = popupObject.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>();
            resultText.text = string.Format("최고 데미지 >   <size='60'>{0}</size>\r\n\r\n현재 데미지 >   <size='60'>{1}</size>", User.attackRankPoint.ToString("N0"),dam.ToString("N0"));
            popupObject.gameObject.SetActive(true);
            showUIanimation(popupObject);
            this.gameObject.SetActive(false);
        }
    }
    public void OpenBattleEndGamePanel(bool isWin)
    {
        if (popupPanel == null)
            popupPanel = GameObject.FindWithTag("PopupPanel") as GameObject;
        GameObject popupObject = null;
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
        if (popupPanel != null && popupPanel.transform.childCount > 0)
        {
            for (var i = 0; i < popupPanel.transform.childCount; i++)
            {
                if (popupPanel.transform.GetChild(i).name.Equals("PanelPopEndGame"))
                {
                    popupObject = popupPanel.transform.GetChild(i).gameObject;
                    break;
                }
            }
        }
        if (popupObject != null && popupObject.GetComponentInChildren<AiryUIAnimatedElement>() != null)
        {
            Text resultText = popupObject.transform.GetChild(1).GetChild(1).GetComponentInChildren<Text>();
            if (isWin)
                resultText.text = string.Format("{0}", "Win");
            else
                resultText.text = string.Format("{0}", "Lose");
            popupObject.gameObject.SetActive(true);
            showUIanimation(popupObject);
            this.gameObject.SetActive(false);
        }
    }
    public void OpenBattleEndGameErrorPanel()
    {
        if (popupPanel == null)
            popupPanel = GameObject.FindWithTag("PopupPanel") as GameObject;
        GameObject popupObject = null;
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
        if (popupPanel != null && popupPanel.transform.childCount > 0)
        {
            for (var i = 0; i < popupPanel.transform.childCount; i++)
            {
                if (popupPanel.transform.GetChild(i).name.Equals("PanelPopEndGame"))
                {
                    popupObject = popupPanel.transform.GetChild(i).gameObject;
                    break;
                }
            }
        }
        if (popupObject != null && popupObject.GetComponentInChildren<AiryUIAnimatedElement>() != null)
        {
            Text resultText = popupObject.transform.GetChild(1).GetChild(1).GetComponentInChildren<Text>();
            resultText.text = string.Format("{0}", "Error");
            popupObject.gameObject.SetActive(true);
            showUIanimation(popupObject);
            this.gameObject.SetActive(false);
        }
    }
    public void CloseAllPopupPanel()
    {
        GameObject popupPanel = GameObject.FindWithTag("PopupPanel") as GameObject;

        if (popupPanel != null && popupPanel.transform.childCount > 0)
        {
            for (var i = 0; i < popupPanel.transform.childCount; i++)
            {
                if (popupPanel.transform.GetChild(i).gameObject.activeSelf)
                {
                    if (popupPanel.transform.GetChild(i).GetComponent<AiryUIAnimatedElement>() != null)
                        popupPanel.transform.GetChild(i).GetComponent<AiryUIAnimatedElement>().HideElement();
                    else
                        popupPanel.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    public void CoverFadeIn()
    {
        if(CoverUI!=null)
            StartCoroutine(FadeIn(CoverUI.GetComponent<Image>()));
    }
    IEnumerator FadeIn(Image image)
    {
        if (image == null)
            yield return null;
        float cnt = 0;
        Color color = image.color;
        while(cnt<1)
        {
            image.color = new Color(color.r, color.g, color.b, cnt);
            yield return new WaitForEndOfFrame();
            cnt += 0.03f;
        }
        yield return null;
    }
    public int GetPopupPanelCount()
    {
        int cnt = 0;
        if (popupPanel!=null)
        {
            for(int i = 0; i < popupPanel.transform.childCount; i++)
            {
                if (popupPanel.transform.GetChild(i).gameObject.activeSelf)
                    cnt += 1;
            }
        }
        return cnt;
    }
    public void PopupGetGacha(GachaSystem.GachaType gachaType)
    {
        if (PopupGetGachaUI != null&&!PopupGetGachaUI.activeSelf)
        {
            showUIanimation(PopupGetGachaUI);
            PopupGetGachaUI.GetComponent<UI_GetGacha>().GachaStart(gachaType);
        }
        else
            Debugging.Log("가챠 팝업창이 없습니다.");
    }

    public void PopupGetAbility(Ability ability)
    {
        if (PopGetAbilityUI != null)
        {
            PopGetAbilityUI.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(ability.image);
            PopGetAbilityUI.transform.GetChild(1).GetComponent<Text>().text = AbilitySystem.GetAbilityName(ability.id);
            showUIanimation(PopGetAbilityUI);
        }
        else
            Debugging.Log("어빌리티 팝업창이 없습니다.");
    }

    void showUIanimation(GameObject obj)
    {
        if (obj.GetComponentsInChildren<AiryUIAnimatedElement>() != null)
        {
            foreach (var element in obj.GetComponentsInChildren<AiryUIAnimatedElement>())
                element.ShowElement();
        }
    }
    void hideUIanimation(GameObject obj)
    {
        if (obj.GetComponentsInChildren<AiryUIAnimatedElement>() != null)
        {
            foreach (var element in obj.GetComponentsInChildren<AiryUIAnimatedElement>())
                element.HideElement();
        }
    }
    public void ShowGetAlert(string spritePath, string txt)
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
            GameObject getAlertPanel = popup.transform.GetChild(0).GetChild(1).gameObject;
            getAlertPanel.gameObject.SetActive(true);
            Image getImage = getAlertPanel.transform.GetChild(0).GetComponent<Image>();
            Text getText = getAlertPanel.transform.GetChild(1).GetComponent<Text>();
            getImage.sprite = Resources.Load<Sprite>(spritePath);
            if (getImage.sprite == null)
                getImage.enabled = false;
            else
                getImage.enabled = true;
            getText.text = txt;
            popup.GetComponent<AiryUIAnimatedElement>().ShowElement();
        }
    }
    public void ShowGetAlert(Sprite sprite, string txt)
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
            GameObject getAlertPanel = popup.transform.GetChild(0).GetChild(1).gameObject;
            getAlertPanel.gameObject.SetActive(true);
            Image getImage = getAlertPanel.transform.GetChild(0).GetComponent<Image>();
            Text getText = getAlertPanel.transform.GetChild(1).GetComponent<Text>();
            getImage.sprite = sprite;
            if (getImage.sprite == null)
                getImage.enabled = false;
            else
                getImage.enabled = true;
            getText.text = txt;
            popup.GetComponent<AiryUIAnimatedElement>().ShowElement();
        }
    }
    public void ClosePopupAlertUI()
    {
        foreach(var p in GameObject.FindGameObjectsWithTag("AlertUI"))
        {
            p.gameObject.SetActive(false);
        }
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

    public void ShowAlert(PopupAlertTYPE alertType, int needAmount)
    {
        if(PopupAlertUI!=null)
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
            GameObject ShowtageAlertPanel = popup.transform.GetChild(0).GetChild(0).gameObject;
            ShowtageAlertPanel.gameObject.SetActive(true);
            Image shortageImage = ShowtageAlertPanel.transform.GetChild(0).GetComponent<Image>();
            Text shortageText = ShowtageAlertPanel.transform.GetChild(1).GetComponent<Text>();
            switch (alertType)
            {
                case PopupAlertTYPE.energy:
                    shortageImage.sprite = Resources.Load<Sprite>("Items/portalEnergy");
                    shortageText.text = string.Format("{0} \r\n <color='red;>{1} : {2}</color>", LocalizationManager.GetText("alertEnergy"),LocalizationManager.GetText("alertNeedText"),Common.GetThousandCommaText(needAmount));
                    break;
                case PopupAlertTYPE.scroll:
                    shortageImage.sprite = Resources.Load<Sprite>("Items/abilityScroll");
                    shortageText.text = string.Format("{0} \r\n <color='red;>{1} : {2}</color>", LocalizationManager.GetText("alertScroll"), LocalizationManager.GetText("alertNeedText"), Common.GetThousandCommaText(needAmount));
                    break;
                case PopupAlertTYPE.coin:
                    shortageImage.sprite = Resources.Load<Sprite>("Items/coin");
                    shortageText.text = string.Format("{0} \r\n <color='red;>{1} : {2}</color>", LocalizationManager.GetText("alertCoin"), LocalizationManager.GetText("alertNeedText"), Common.GetThousandCommaText(needAmount));
                    break;
                case PopupAlertTYPE.blackCrystal:
                    shortageImage.sprite = Resources.Load<Sprite>("Items/blackCrystal");
                    shortageText.text = string.Format("{0} \r\n <color='red;>{1} : {2}</color>", LocalizationManager.GetText("alertCrystal"), LocalizationManager.GetText("alertNeedText"), Common.GetThousandCommaText(needAmount));
                    break;
            }
            popup.GetComponent<AiryUIAnimatedElement>().ShowElement();
        }
    }

    public void ShowAlert(string imagePath, string alertText)
    {
        if (PopupAlertUI != null)
        {
            GameObject popup;
            if(canvasOverlay != null)
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
            GameObject ShowtageAlertPanel = popup.transform.GetChild(0).GetChild(0).gameObject;
            ShowtageAlertPanel.gameObject.SetActive(true);
            Image shortageImage = ShowtageAlertPanel.transform.GetChild(0).GetComponent<Image>();
            Text shortageText = ShowtageAlertPanel.transform.GetChild(1).GetComponent<Text>();
            shortageImage.sprite = Resources.Load<Sprite>(imagePath);
            if (shortageImage.sprite == null)
                shortageImage.enabled = false;
            else
                shortageImage.enabled = true;
            shortageText.text = alertText;
            popup.GetComponent<AiryUIAnimatedElement>().ShowElement();
        }
    }


}
