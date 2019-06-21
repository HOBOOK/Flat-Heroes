﻿using System.Collections;
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

    GameObject showHeroObj;
    GameObject PanelHeroInfo;
    Text heroNameText;
    Text heroDescriptionText;
    Text heroLevelText;
    Text heroExpText;
    Slider heroExpSlider;
    HeroData targetHeroData;
    Button skillLevelUpButton;
    

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
                heroNameText.text = heroData.name;
            if (heroDescriptionText != null)
                heroDescriptionText.text = heroData.description;
            if (heroLevelText != null)
                heroLevelText.text = string.Format("LV {0}", heroData.level);
            if (heroExpText != null)
            {
                int exp = targetHeroData.exp;
                int needExp = Common.EXP_TABLE[targetHeroData.level - 1];
                float expPercent = ((float)exp / (float)needExp);
                heroExpText.text = string.Format("{0}/{1}({2}%)", exp,needExp , (expPercent*100).ToString("N0"));
                heroExpSlider.value = expPercent;
            }

            if (CharactersManager.instance.GetLobbyHeros(targetHeroData.id))
            {
                heroSetLobbyButton.GetComponentInChildren<Text>().text = "로비해체 >";
            }
            else
            {
                heroSetLobbyButton.GetComponentInChildren<Text>().text = "로비배치 >";
            }

            RefreshHeroStatusEquipmentPanel();
        }
    }

    public void RefreshHeroStatusEquipmentPanel()
    {
        //Status 정보
        if (heroStatusInfoPanel != null)
        {
            heroStatusInfoPanel.transform.GetChild(0).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusAttack(ref targetHeroData).ToString() + string.Format("<color='yellow'>(+{0})</color>", AbilitySystem.GetAbilityStats(0) + ItemSystem.GetHeroEquipmentItemAttack(ref targetHeroData)+ LabSystem.GetAddAttack(User.addAttackLevel));
            heroStatusInfoPanel.transform.GetChild(1).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusDefence(ref targetHeroData).ToString() + string.Format("<color='yellow'>(+{0})</color>", AbilitySystem.GetAbilityStats(1) + ItemSystem.GetHeroEquipmentItemDefence(ref targetHeroData) + LabSystem.GetAddDefence(User.addDefenceLevel));
            heroStatusInfoPanel.transform.GetChild(2).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusMaxHp(ref targetHeroData).ToString() + string.Format("<color='yellow'>(+{0})</color>", AbilitySystem.GetAbilityStats(2));
            heroStatusInfoPanel.transform.GetChild(3).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusCriticalPercent(ref targetHeroData).ToString() + string.Format("<color='yellow'>(+{0})</color>%", AbilitySystem.GetAbilityStats(3));
            heroStatusInfoPanel.transform.GetChild(4).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusAttackSpeed(ref targetHeroData).ToString() + string.Format("<color='yellow'>(+{0})</color>", AbilitySystem.GetAbilityStats(4));
            heroStatusInfoPanel.transform.GetChild(5).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusMoveSpeed(ref targetHeroData).ToString() + string.Format("<color='yellow'>(+{0})</color>", AbilitySystem.GetAbilityStats(5));
            heroStatusInfoPanel.transform.GetChild(6).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusKnockbackResist(ref targetHeroData).ToString("N1");
            heroStatusInfoPanel.transform.GetChild(7).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusSkillEnergy(ref targetHeroData).ToString() + string.Format("<color='yellow'>(+{0})</color>", AbilitySystem.GetAbilityStats(6));
        }

        if (heroStatPanel != null)
        {
            for (int i = 0; i < heroStatPanel.transform.childCount; i++)
            {
                if (heroStatPanel.transform.GetComponentInChildren<Button>() != null)
                    heroStatPanel.transform.GetComponentInChildren<Button>().gameObject.SetActive(false);
            }
            heroStatPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = targetHeroData.strength.ToString();
            heroStatPanel.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = targetHeroData.intelligent.ToString();
            heroStatPanel.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = targetHeroData.physical.ToString();
            heroStatPanel.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = targetHeroData.agility.ToString();
        }

        // Equipment 장비 정보
        if (heroEquimentItemSlots != null && heroEquimentItemSlots.transform.childCount > 0)
        {
            int[] equipmentItemsId = HeroSystem.GetHeroEquipmentItems(targetHeroData.id);
            for (int i = 0; i < equipmentItemsId.Length; i++)
            {
                if (equipmentItemsId[i] != 0)
                {
                    heroEquimentItemSlots.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemImage(equipmentItemsId[i],true);
                }
                else
                {
                    heroEquimentItemSlots.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemNoneImage();

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
            heroSkillSlot.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = SkillSystem.GetSkillImage(heroSkill.id);
            heroSkillSlot.transform.GetComponentInChildren<Text>().text = string.Format("<size='27'>기술레벨 : {0}</size>\r\n<color='grey'>{1}</color>", SkillSystem.GetUserSkillLevel(heroSkill.id), SkillSystem.GetUserSkillDescription(heroSkill,targetHeroData));
            // 스킬강화버튼
            skillLevelUpButton = heroSkillSlot.GetComponentInChildren<Button>();
            int needMoney = SkillSystem.GetUserSkillLevelUpNeedCoin(heroSkill.id);
            skillLevelUpButton.transform.GetChild(0).GetComponentInChildren<Text>().text = Common.GetThousandCommaText(needMoney);

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
        }
    }

    public void OnSkillLevelUpClick(int skill, int needMoney)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        if(Common.PaymentCheck(ref User.coin,needMoney))
         {
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
}
