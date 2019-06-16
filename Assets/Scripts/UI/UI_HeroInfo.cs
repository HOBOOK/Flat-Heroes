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

    GameObject PanelHeroInfo;
    Text heroNameText;
    Text heroDescriptionText;
    Text heroLevelText;
    Text heroExpText;
    Slider heroExpSlider;
    HeroData targetHeroData;

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
            GameObject showHeroObj = Instantiate(hero, HeroShowPoint.transform);
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


            //Status 정보
            if(heroStatusInfoPanel!=null)
            {
                heroStatusInfoPanel.transform.GetChild(0).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusAttack(targetHeroData).ToString();
                heroStatusInfoPanel.transform.GetChild(1).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusDefence(targetHeroData).ToString();
                heroStatusInfoPanel.transform.GetChild(2).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusMaxHp(targetHeroData).ToString();
                heroStatusInfoPanel.transform.GetChild(3).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusCriticalPercent(targetHeroData).ToString()+"%";
                heroStatusInfoPanel.transform.GetChild(4).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusSpeedText(HeroSystem.GetHeroStatusAttackSpeed(targetHeroData));
                heroStatusInfoPanel.transform.GetChild(5).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusSpeedText(HeroSystem.GetHeroStatusMoveSpeed(targetHeroData));
                heroStatusInfoPanel.transform.GetChild(6).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusKnockbackResist(targetHeroData).ToString("N1");
                heroStatusInfoPanel.transform.GetChild(7).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusSkillEnergy(targetHeroData).ToString();
            }

            if(heroStatPanel!=null)
            {
                for(int i =0;i<heroStatPanel.transform.childCount;i++)
                {
                    if(heroStatPanel.transform.GetComponentInChildren<Button>()!=null)
                        heroStatPanel.transform.GetComponentInChildren<Button>().gameObject.SetActive(false);
                }
                heroStatPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = targetHeroData.strength.ToString();
                heroStatPanel.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = targetHeroData.intelligent.ToString();
                heroStatPanel.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = targetHeroData.physical.ToString();
                heroStatPanel.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = targetHeroData.agility.ToString();
            }

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
}
