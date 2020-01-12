using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossSelect : MonoBehaviour
{
    GameObject slotContentView;
    UI_BossSelectSlot[] bossSlotList;
    UI_BossSelectSlot selectedBossSlot;
    GameObject bossPrefab;
    int selectedBossId;
    public Text bossInfoText;
    public Text bossNameText;
    public GameObject CompletedButton;
    public Transform bossPointTransform;
    public Transform rewardTransform;
    public Dropdown orderByDropdown;

    int attackInfo;
    int defenceInfo;
    int hpInfo;

    private void Awake()
    {
        if (slotContentView == null)
            slotContentView = this.GetComponentInChildren<ContentSizeFitter>().gameObject;

        orderByDropdown.options.Clear();
        orderByDropdown.options.Add(new Dropdown.OptionData(LocalizationManager.GetText("Easy")));
        orderByDropdown.options.Add(new Dropdown.OptionData(LocalizationManager.GetText("Normal")));
        orderByDropdown.options.Add(new Dropdown.OptionData(LocalizationManager.GetText("Hard")));
        orderByDropdown.RefreshShownValue();

        bossSlotList = slotContentView.GetComponentsInChildren<UI_BossSelectSlot>();
    }
    private void OnEnable()
    {
        selectedBossSlot = null;
        selectedBossId = 0;
        HideBoss();
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (selectedBossSlot != null)
        {
            CompletedButton.SetActive(true);
            CompletedButton.GetComponent<AiryUIAnimatedElement>().ShowElement();

            if(selectedBossId>0)
            {
                RefreshRewardUI();
                SetBossModeDifficultyBuff(selectedBossId);
                bossInfoText.gameObject.SetActive(true);
                bossNameText.gameObject.SetActive(true);
                bossInfoText.text = string.Format("{0} >  {1}\r\n{2} >  {3}\r\n{4} >  {5}", LocalizationManager.GetText("heroInfoAttack"), attackInfo.ToString("N0"), LocalizationManager.GetText("heroInfoDefence"), defenceInfo.ToString("N0"), LocalizationManager.GetText("heroInfoHp"), hpInfo.ToString("N0"));
                bossNameText.text = HeroSystem.GetHeroName(selectedBossId);
                bossInfoText.GetComponent<AiryUIAnimatedElement>().ShowElement();
                bossNameText.GetComponent<AiryUIAnimatedElement>().ShowElement();
            }
        }
        else
        {
            CompletedButton.SetActive(false);
            if (rewardTransform != null)
                rewardTransform.gameObject.SetActive(false);
            if (bossInfoText != null)
                bossInfoText.gameObject.SetActive(false);
            bossNameText.text = "";
            bossInfoText.gameObject.SetActive(false);
        }
        if(orderByDropdown!=null)
        {
            orderByDropdown.value = Common.bossModeDifficulty;
        }
    }
    void SetBossModeDifficultyBuff(int id)
    {
        HeroData data = HeroSystem.GetHero(id);
        if (data != null)
        {
            if (Common.bossModeDifficulty == 1)
            {
                attackInfo = HeroSystem.GetHeroStatusAttack(ref data) * 3;
                defenceInfo = HeroSystem.GetHeroStatusDefence(ref data) *2;
                hpInfo = HeroSystem.GetHeroStatusMaxHp(ref data) * 10;
            }
            else if (Common.bossModeDifficulty == 2)
            {
                attackInfo = HeroSystem.GetHeroStatusAttack(ref data) * 4;
                defenceInfo = HeroSystem.GetHeroStatusDefence(ref data) * 4;
                hpInfo = HeroSystem.GetHeroStatusMaxHp(ref data) * 50;
            }
            else
            {
                attackInfo = HeroSystem.GetHeroStatusAttack(ref data);
                defenceInfo = HeroSystem.GetHeroStatusDefence(ref data);
                hpInfo = HeroSystem.GetHeroStatusMaxHp(ref data);
            }
        }
    }

    void RefreshRewardUI()
    {
        if(rewardTransform!=null)
        {
            rewardTransform.gameObject.SetActive(true);
            rewardTransform.parent.GetComponent<AiryUIAnimatedElement>().ShowElement();
            for (var i = 0; i < rewardTransform.childCount; i++)
            {
                switch(i)
                {
                    case 0:
                        rewardTransform.GetChild(i).GetComponentInChildren<Text>().text = selectedBossSlot.rewardCoin.ToString("N0");
                        break;
                    case 1:
                        rewardTransform.GetChild(i).GetComponentInChildren<Text>().text = selectedBossSlot.rewardCrystal.ToString("N0");
                        break;
                    case 2:
                        rewardTransform.GetChild(i).GetComponentInChildren<Text>().text = selectedBossSlot.rewardScroll.ToString("N0");
                        break;
                    case 3:
                        if(selectedBossSlot.rewardTranscendenceStone>0)
                        {
                            rewardTransform.GetChild(i).gameObject.SetActive(true);
                            rewardTransform.GetChild(i).GetComponentInChildren<Text>().text = selectedBossSlot.rewardTranscendenceStone.ToString("N0");
                        }
                        else
                        {
                            rewardTransform.GetChild(i).gameObject.SetActive(false);
                        }
                        break;
                    case 4:
                        if(Common.bossModeDifficulty<2)
                        {
                            rewardTransform.GetChild(i).gameObject.SetActive(false);
                        }
                        else
                        {
                            rewardTransform.GetChild(i).gameObject.SetActive(true);
                            rewardTransform.GetChild(i).GetComponent<Image>().sprite = ItemSystem.GetItemImage(8003 + selectedBossSlot.rewardBoxType);
                            rewardTransform.GetChild(i).GetComponentInChildren<Text>().text = selectedBossSlot.rewardBoxCount.ToString("N0");
                        }
                        break;
                }
            }
        }
    }

    public void OnClickSetDifficulty(int difficulty)
    {
        Common.bossModeDifficulty = difficulty;
        foreach(var slot in FindObjectsOfType<UI_BossSelectSlot>())
        {
            slot.RefreshUI();
        }
        RefreshUI();
    }

    public void ChooseBoss(Button button)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        if(button.GetComponent<UI_BossSelectSlot>()!=null)
        {
            if (!button.GetComponent<UI_BossSelectSlot>().isLevelCheck)
                return;

            if(selectedBossSlot== button.GetComponent<UI_BossSelectSlot>())
            { 
                selectedBossSlot.GetComponent<UI_BossSelectSlot>().EnableSlot(false);
                selectedBossSlot = null;
                HideBoss();
            }
            else
            {
                if(selectedBossSlot!=null)
                {
                    selectedBossSlot.GetComponent<UI_BossSelectSlot>().EnableSlot(false);
                    selectedBossSlot = null;
                }
                selectedBossSlot = button.GetComponent<UI_BossSelectSlot>();
                selectedBossSlot.GetComponent<UI_BossSelectSlot>().EnableSlot(true);
                ShowBoss(selectedBossSlot.GetComponent<UI_BossSelectSlot>().BossID);
            }
            RefreshUI();
        }
    }

    public void CompleteBossSelect()
    {
        if(selectedBossSlot!=null)
        {
            BossModeData.SetBossModeData(selectedBossSlot.BossID, selectedBossSlot.rewardCoin, selectedBossSlot.rewardCrystal, selectedBossSlot.rewardScroll,selectedBossSlot.rewardTranscendenceStone,(selectedBossSlot.ableStage-1),selectedBossSlot.rewardBoxType,selectedBossSlot.rewardBoxCount);
            CompletedButton.GetComponent<UI_Button>().OnClick();
            this.gameObject.SetActive(false);
        }
        else
        {
            Debugging.LogWarning("보스모드선택 오류");
        }
    }

    void ShowBoss(int bossId)
    {
        if (bossPointTransform != null)
        {
            if(bossPointTransform.childCount>0)
            {
                foreach(Transform child in bossPointTransform.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            bossPrefab = Instantiate(PrefabsDatabaseManager.instance.GetMonsterPrefab(bossId), bossPointTransform);
            bossPrefab.transform.localScale = new Vector3(200, 200, 200);
            if (bossId == 1004) bossPrefab.transform.localScale = new Vector3(150, 150, 150);
            if(bossId==1001||bossId==1002||bossId==1005)
                bossPrefab.transform.localPosition = new Vector3(0, 100, 0);
            else
                bossPrefab.transform.localPosition = new Vector3(0, 0, 0);

            if (bossPrefab.GetComponent<Hero>() != null)
            {
                selectedBossId = bossPrefab.GetComponent<Hero>().id;
                Destroy(bossPrefab.GetComponent<Hero>());
            }
            if (bossPrefab.GetComponent<Rigidbody2D>() != null)
                Destroy(bossPrefab.GetComponent<Rigidbody2D>());
            foreach (var sp in bossPrefab.GetComponentsInChildren<SpriteRenderer>())
            {
                sp.sortingLayerName = "ShowObject";
                sp.gameObject.layer = 16;
                sp.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
            bossPrefab.gameObject.SetActive(true);
        }
    }
    void HideBoss()
    {
        if(bossPrefab!=null)
        {
            bossPrefab.SetActive(false);
            selectedBossId = 0;
        }
    }

    public void OnOrderByButtonClick(Dropdown dropdown)
    {
        OnClickSetDifficulty(dropdown.value);
    }
}
