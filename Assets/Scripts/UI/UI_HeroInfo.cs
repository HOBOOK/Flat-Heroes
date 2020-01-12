using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroInfo : MonoBehaviour
{
    public GameObject HeroShowPoint;
    public Button heroSetLobbyButton;
    public GameObject heroStatusInfoPanel;
    public GameObject heroStatPanel;
    public GameObject heroLevelInfoPanel;
    public GameObject heroEquipmentItemPanel;
    public GameObject heroEquimentItemSlots;
    public GameObject heroSkillSlot;
    public Button leaderButton;
    public Button TranscendenceButton;
    public GameObject getTranscendencePanel;
    public GameObject heroAbilityPanel;
    public Transform heroAbilityInfoTransform;
    public Transform heroTypeTransform;
    public UI_HeroAbilityPanel heroGetAbilityPanel;
    public GameObject informationPanel;

    GameObject showHeroObj;
    GameObject PanelHeroInfo;
    Text heroNameText;
    Text heroDescriptionText;
    Text heroLevelText;
    Text heroExpText;
    Slider heroExpSlider;
    HeroData targetHeroData;
    Button skillLevelUpButton;
    Image skillImage;
    bool isCheckAlert = false;

    private void Awake()
    {
        HeroShowPoint = GameObject.FindGameObjectWithTag("ShowPoint").gameObject;
        PanelHeroInfo = this.transform.GetChild(0).gameObject;
        heroExpSlider = this.transform.GetComponentInChildren<Slider>();
        foreach (var text in PanelHeroInfo.transform.GetComponentsInChildren<Text>())
        {
            if (text.name.Equals("heroNameText"))
            {
                heroNameText = text;
            }
            else if (text.name.Equals("heroDescriptionText"))
            {
                heroDescriptionText = text;
            }
            else if(text.name.Equals("levelText"))
            {
                heroLevelText = text;
            }
            else if(text.name.Equals("expText"))
            {
                heroExpText = text;
            }
        }
    }

    private void OnEnable()
    {
        GetComponentInChildren<AiryUIAnimatedElement>().ShowElement();

    }
    private void OnDisable()
    {
        if (HeroShowPoint.transform.childCount > 0)
            Destroy(HeroShowPoint.transform.GetChild(0).gameObject);
    }

    public void ShowHero(GameObject hero, HeroData heroData)
    {
        if(hero!=null)
        {
            if(leaderButton!=null)
            {
                if (heroData.id == User.profileHero)
                {
                    leaderButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
                else
                    leaderButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
            }
            targetHeroData = heroData;
            showHeroObj = Instantiate(hero, HeroShowPoint.transform);
            showHeroObj.transform.localScale = new Vector3(200, 200, 200);
            showHeroObj.transform.localPosition = Vector3.zero;
            
            if (showHeroObj.GetComponent<Hero>() != null)
                Destroy(showHeroObj.GetComponent<Hero>());
            if (showHeroObj.GetComponent<Rigidbody2D>() != null)
                Destroy(showHeroObj.GetComponent<Rigidbody2D>());
            foreach (var sp in showHeroObj.GetComponentsInChildren<SpriteRenderer>())
            {
                sp.sortingLayerName = "ShowObject";
                sp.gameObject.layer = 16;
            }
            showHeroObj.gameObject.SetActive(true);

            if (heroNameText != null)
                heroNameText.text = HeroSystem.GetHeroName(heroData.id);
            if (heroDescriptionText != null)
                heroDescriptionText.text = HeroSystem.GetHeroDescription(heroData.id);
            if (heroLevelText != null)
                heroLevelText.text = string.Format("LV {0}", heroData.level);
            if (heroExpText != null)
            {
                int exp = targetHeroData.exp;
                int needExp = Common.GetHeroNeedExp(targetHeroData.level);
                float expPercent = ((float)exp / (float)needExp);
                heroExpText.text = string.Format("{0}/{1}({2}%)", exp,needExp , (expPercent*100).ToString("N0"));
                heroExpSlider.value = expPercent;
            }
            if(heroTypeTransform!=null)
            {
                heroTypeTransform.GetComponent<Button>().onClick.RemoveAllListeners();
                heroTypeTransform.GetComponent<Button>().onClick.AddListener(delegate
                {
                    OnClickInfo(heroTypeTransform, LocalizationManager.GetText("HeroTypeDescription" + (HeroSystem.GetHeroType(targetHeroData.id) + 1)));
                });
                heroTypeTransform.GetComponentInChildren<Text>().text = LocalizationManager.GetText("HeroType" + (HeroSystem.GetHeroType(targetHeroData.id) + 1));
            }

            if (CharactersManager.instance.GetLobbyHeros(targetHeroData.id))
            {
                heroSetLobbyButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("heroInfoExceptLobbyButton");
            }
            else
            {
                heroSetLobbyButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("heroInfoToLobbyButton");
            }
            RefreshHeroStatusEquipmentPanel();
        }
    }
    string plusMinusText(int amount)
    {
        if (amount < 0)
            return Common.GetThousandCommaText(amount);
        else
            return "+"+Common.GetThousandCommaText(amount);
    }
    void RefreshHeroAbilityPanel()
    {
        if(heroAbilityInfoTransform!=null)
        {
            if(targetHeroData.ability>0)
            {
                heroAbilityInfoTransform.transform.GetChild(2).gameObject.SetActive(true);
                heroAbilityInfoTransform.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = HeroAbilitySystem.GetHeroAbilityName(targetHeroData.ability);
                heroAbilityInfoTransform.transform.GetChild(2).GetChild(1).GetComponentInChildren<Text>().text = targetHeroData.abilityLevel.ToString();
                heroAbilityInfoTransform.transform.GetChild(3).gameObject.SetActive(false);
                heroAbilityInfoTransform.transform.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
                heroAbilityInfoTransform.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate
                {
                    OnClickInfo(heroAbilityInfoTransform.transform.GetChild(2), HeroAbilitySystem.GetHeroAbilityDetailDescription(targetHeroData.ability, targetHeroData.abilityLevel));
                });
            }
            else
            {
                heroAbilityInfoTransform.transform.GetChild(2).gameObject.SetActive(false);
                heroAbilityInfoTransform.transform.GetChild(3).gameObject.SetActive(true);
            }
        }
    }
    public void RefreshHeroStatusEquipmentPanel()
    {
        RefreshHeroAbilityPanel();
        //Status 정보
        if (heroStatusInfoPanel != null)
        {
            heroStatusInfoPanel.transform.GetChild(0).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusAttack(ref targetHeroData).ToString() + string.Format("<color='yellow'><size='20'>({0})</size></color>", plusMinusText(AbilitySystem.GetAbilityStats(0) + ItemSystem.GetHeroEquipmentItemAttack(ref targetHeroData)+ LabSystem.GetAddAttack(User.addAttackLevel)));
            heroStatusInfoPanel.transform.GetChild(1).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusDefence(ref targetHeroData).ToString() + string.Format("<color='yellow'><size='20'>({0})</size></color>", plusMinusText(AbilitySystem.GetAbilityStats(1) + ItemSystem.GetHeroEquipmentItemDefence(ref targetHeroData) + LabSystem.GetAddDefence(User.addDefenceLevel)));
            heroStatusInfoPanel.transform.GetChild(2).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusMaxHp(ref targetHeroData).ToString() + string.Format("<color='yellow'><size='20'>({0})</size></color>", plusMinusText(AbilitySystem.GetAbilityStats(2) + ItemSystem.GetHeroEquipmentItemHp(ref targetHeroData)));
            heroStatusInfoPanel.transform.GetChild(3).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusCriticalPercent(ref targetHeroData).ToString() + string.Format("<color='yellow'><size='20'>({0})</size></color>%", plusMinusText(AbilitySystem.GetAbilityStats(3) + ItemSystem.GetHeroEquipmentItemCritical(ref targetHeroData)));
            heroStatusInfoPanel.transform.GetChild(4).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusAttackSpeed(ref targetHeroData).ToString() + string.Format("<color='yellow'><size='20'>({0})</size></color>", plusMinusText(AbilitySystem.GetAbilityStats(4) + ItemSystem.GetHeroEquipmentItemAttackSpeed(ref targetHeroData)));
            heroStatusInfoPanel.transform.GetChild(5).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusMoveSpeed(ref targetHeroData).ToString() + string.Format("<color='yellow'><size='20'>({0})</size></color>", plusMinusText(AbilitySystem.GetAbilityStats(5) + ItemSystem.GetHeroEquipmentItemMoveSpeed(ref targetHeroData)));
            heroStatusInfoPanel.transform.GetChild(6).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusKnockbackResist(ref targetHeroData).ToString("N1");
            heroStatusInfoPanel.transform.GetChild(7).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusSkillEnergy(ref targetHeroData).ToString() + string.Format("<color='yellow'><size='20'>({0})</size></color>", plusMinusText(AbilitySystem.GetAbilityStats(6) + ItemSystem.GetHeroEquipmentItemSkillEnergy(ref targetHeroData)));
        }

        if(TranscendenceButton!=null)
        {
            if (targetHeroData.over<3)
            {
                TranscendenceButton.GetComponentInChildren<Text>().text = string.Format("{0} {1}", LocalizationManager.GetText("heroInfoTranscendence"), targetHeroData.over);
                TranscendenceButton.onClick.RemoveAllListeners();
                TranscendenceButton.onClick.AddListener(delegate
                {
                    OnClickTranscendenceHero();
                });
            }
            else
            {
                TranscendenceButton.GetComponentInChildren<Text>().text = string.Format("{0} <color='red'>{1}</color>", LocalizationManager.GetText("heroInfoTranscendence"), targetHeroData.over);
                TranscendenceButton.onClick.RemoveAllListeners();
            }
        }

        // Equipment 장비 정보
        if (heroEquimentItemSlots != null && heroEquimentItemSlots.transform.childCount > 0)
        {
            int[] equipmentItemsId = HeroSystem.GetHeroEquipmentItems(targetHeroData.id);
            for (int i = 0; i < equipmentItemsId.Length; i++)
            {
                if (equipmentItemsId[i] != 0)
                {
                    heroEquimentItemSlots.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemClassImage(equipmentItemsId[i], true);
                    heroEquimentItemSlots.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemImage(equipmentItemsId[i],true);
                    heroEquimentItemSlots.transform.GetChild(i).GetChild(1).GetComponent<Image>().color = ItemColor.GetItemColor(ItemSystem.GetUserEquipmentItem(equipmentItemsId[i]).itemClass);
                    if(ItemSystem.GetUserItemByCustomId(equipmentItemsId[i]).enhancement>0)
                    {
                        heroEquimentItemSlots.transform.GetChild(i).GetChild(2).gameObject.SetActive(true);
                        heroEquimentItemSlots.transform.GetChild(i).GetChild(2).GetComponentInChildren<Text>().text = string.Format("<size='15'>+</size>{0}",ItemSystem.GetUserItemByCustomId(equipmentItemsId[i]).enhancement.ToString());
                    }
                    else
                    {
                        heroEquimentItemSlots.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
                    }
                }
                else
                {
                    heroEquimentItemSlots.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemNoneImage();
                    heroEquimentItemSlots.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/ui_plus2");
                    heroEquimentItemSlots.transform.GetChild(i).GetChild(1).GetComponent<Image>().color = Color.white;
                    heroEquimentItemSlots.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
                }
                int buttonIndex = i;
                heroEquimentItemSlots.transform.GetChild(buttonIndex).GetComponent<Button>().onClick.RemoveAllListeners();
                heroEquimentItemSlots.transform.GetChild(buttonIndex).GetComponent<Button>().onClick.AddListener(delegate
                {
                    OnEquipmentItemClick(buttonIndex, equipmentItemsId[buttonIndex]);
                });
            }
        }

        // 스킬정보
        Skill heroSkill = SkillSystem.GetSkill(targetHeroData.skill);
        if (heroSkillSlot != null && heroSkill != null)
        {
            skillImage = heroSkillSlot.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            skillImage.sprite = SkillSystem.GetSkillImage(heroSkill.id);
            heroSkillSlot.transform.GetComponentInChildren<Text>().text = string.Format("<size='27'>{0}  Lv {1}</size>\r\n{2}", SkillSystem.GetSkillName(heroSkill.id),SkillSystem.GetUserSkillLevel(heroSkill.id), SkillSystem.GetUserSkillDescription(heroSkill,targetHeroData));
            // 스킬강화버튼
            skillLevelUpButton = heroSkillSlot.GetComponentInChildren<Button>();
            int needMoney = SkillSystem.GetUserSkillLevelUpNeedCoin(heroSkill.id);
            skillLevelUpButton.transform.GetChild(2).GetComponent<Text>().text = Common.GetThousandCommaText(needMoney);

            if (Common.PaymentAbleCheck(ref User.coin, needMoney))
            {
                skillLevelUpButton.interactable = true;
            }
            else
            {
                skillLevelUpButton.interactable = false;
            }
            skillLevelUpButton.onClick.RemoveAllListeners();
            skillLevelUpButton.onClick.AddListener(delegate
            {
                OnSkillLevelUpClick(heroSkill.id, needMoney);
            });

            if (SkillSystem.isHeroSkillUpgradeAble(heroSkill.id,targetHeroData))
            {
                skillLevelUpButton.interactable = true;
                skillLevelUpButton.transform.GetChild(3).gameObject.SetActive(false);
            }
            else
            {
                skillLevelUpButton.interactable = false;
                skillLevelUpButton.transform.GetChild(3).GetComponentInChildren<Text>().text = string.Format("! {0} : {1}",LocalizationManager.GetText("HeroLevel"), SkillSystem.GetUserSkillLevel(heroSkill.id)+1);
                skillLevelUpButton.transform.GetChild(3).gameObject.SetActive(true);
            }
        }
    }

    public void OnSkillLevelUpClick(int skill, int needMoney)
    {
        if(Common.PaymentCheck(ref User.coin,needMoney))
         {
            EffectManager.SkillUpgradeEffect(skillImage.transform);
            SkillSystem.SetObtainSkill(skill);
            RefreshHeroStatusEquipmentPanel();
        }
        else
        {
            UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.coin, needMoney);
        }
    }

    public void OnSetLobbyButtonClick()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        if(targetHeroData!=null)
        {
            if(CharactersManager.instance.SetLobbyHeros(targetHeroData.id))
                UI_Manager.instance.CloseAllPopupPanel();
        }
    }

    public void OnEquipmentItemClick(int index, int itemId)
    {
        if(heroEquipmentItemPanel!=null)
        {
            heroEquipmentItemPanel.gameObject.SetActive(true);
            heroEquipmentItemPanel.GetComponent<UI_EquipmentItem>().SetSlotIndex(index,itemId,targetHeroData.id,this.GetComponent<UI_HeroInfo>());
            DisableShowObject();
        }
    }

    public void DisableShowObject()
    {
        if (showHeroObj != null)
        {
            showHeroObj.gameObject.SetActive(false);
        }
    }
    public void EnableShowObject()
    {
        if (showHeroObj != null)
        {
            showHeroObj.gameObject.SetActive(true);
        }
    }

    public void ChangeLeader()
    {
        if(targetHeroData!=null)
        {
            User.profileHero = targetHeroData.id;
            GoogleSignManager.SaveData();
            Common.ChangePlayerProfileImage();
            leaderButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }

    public void OnClickInfo(Transform target, string txt)
    {
        if (informationPanel != null)
        {
            informationPanel.SetActive(true);
            informationPanel.GetComponent<UI_InformationPanel>().ShowInformation(target, txt);
        }
    }


    public void OnClickTranscendenceHero()
    {
        if (!isCheckAlert)
        {
            isCheckAlert = true;
            StartCoroutine(CheckingAlert(targetHeroData.level, targetHeroData.over));
        }
    }

    int NeedTranscendenceCrystal(int over)
    {
        return over * over * 20 + 200;
    }

    IEnumerator CheckingAlert(int level,int over)
    {
        GameObject alertPanel;
        alertPanel = UI_Manager.instance.ShowNeedAlert(Common.GetCoinCrystalEnergyImagePath(6), string.Format("<color='yellow'>'{0}' <size='24'>x </size>{1}</color>  {2}\r\nMax Lv : {3} -> {4}", Common.GetCoinCrystalEnergyText(6), NeedTranscendenceCrystal(over+1), LocalizationManager.GetText("alertTranscendenceMessage"),over*10+100,(over+1)*10+100));

        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            UI_Manager.instance.ClosePopupAlertUI();
            if(level<over*10+100||over>=3)
            {
                UI_Manager.instance.ShowAlert("",LocalizationManager.GetText("alertTranscendenceUnableMessage"));
            }
            else
            {
                if (Common.PaymentCheck(ref User.transcendenceStone, NeedTranscendenceCrystal(over + 1)))
                {
                    targetHeroData.over += 1;
                    HeroSystem.SaveOverHero(targetHeroData);
                    RefreshHeroStatusEquipmentPanel();
                    GoogleSignManager.SaveData();

                    getTranscendencePanel.SetActive(true);
                    getTranscendencePanel.GetComponent<UI_GetTranscendence>().ShowHero(showHeroObj, targetHeroData);
                }
                else
                {
                    UI_Manager.instance.ShowAlert(Common.GetCoinCrystalEnergyImagePath(6), LocalizationManager.GetText("alertTranscendenceUnableMessage2"));
                }
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
    public void OnGetAbilityUIShow()
    {
        if(heroGetAbilityPanel!=null)
        {
            heroGetAbilityPanel.gameObject.SetActive(true);
            foreach(AiryUIAnimatedElement child in heroGetAbilityPanel.GetComponentsInChildren<AiryUIAnimatedElement>())
            {
                child.ShowElement();
            }
            heroGetAbilityPanel.OpenUI(targetHeroData);
        }
    }
}
