using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager_PlayerSkillTab : MonoBehaviour
{
    public GameObject ScrollContentView;
    public GameObject slotPrefab;

    List<Skill> playerSkillList = new List<Skill>();
    Image skillImage;
    Text skillDescriptionText;
    Text skillNeedCrystalText;
    Button skillUpgradeButton;

    private void OnEnable()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        if(ScrollContentView!=null)
        {
            foreach(Transform child in ScrollContentView.transform)
            {
                Destroy(child.gameObject);
            }

            playerSkillList.Clear();
            playerSkillList = SkillSystem.GetPlayerSkillList();

            for(var i = 0; i < playerSkillList.Count;i++)
            {
                GameObject slot = Instantiate(slotPrefab, ScrollContentView.transform);
                skillImage = slot.transform.GetChild(0).GetChild(0).GetComponent<Image>();
                skillDescriptionText = slot.transform.GetChild(1).GetComponent<Text>();
                skillUpgradeButton = slot.transform.GetComponentInChildren<Button>();
                skillNeedCrystalText = skillUpgradeButton.GetComponentInChildren<Text>();


                skillImage.sprite = SkillSystem.GetSkillImage(playerSkillList[i].id);
                skillDescriptionText.text = SkillSystem.GetPlayerSkillDescription(playerSkillList[i]);

                int upgradePayment = 100;
                skillNeedCrystalText.text = Common.GetThousandCommaText(upgradePayment);
                skillUpgradeButton.onClick.RemoveAllListeners();
                int index = i;
                skillUpgradeButton.onClick.AddListener(delegate
                {
                    OnClickSkillUpgrate(index, playerSkillList[index].id, upgradePayment);
                });
                if (Common.PaymentAbleCheck(ref User.blackCrystal, upgradePayment))
                {
                    skillUpgradeButton.enabled = true;
                }
                else
                {
                    skillUpgradeButton.enabled = false;
                }

                if (SkillSystem.isPlayerSkillAble(playerSkillList[i].id))
                {
                    slot.transform.GetChild(3).gameObject.SetActive(false);
                    if (SkillSystem.isPlayerSkillUpgradeAble(playerSkillList[i].id))
                    {
                        skillUpgradeButton.enabled = true;
                        skillUpgradeButton.transform.GetChild(2).gameObject.SetActive(false);
                    }
                    else
                    {
                        skillUpgradeButton.enabled = false;
                        skillUpgradeButton.transform.GetChild(2).gameObject.SetActive(true);
                        skillUpgradeButton.transform.GetChild(2).GetComponentInChildren<Text>().text = string.Format("! 유저레벨 : {0}", SkillSystem.GetUserSkillLevel(playerSkillList[i].id)+1);
                    }
                }
                else
                {
                    slot.transform.GetChild(3).gameObject.SetActive(true);
                    slot.transform.GetChild(3).GetComponentInChildren<Text>().text = string.Format("! 해제레벨 : {0}",playerSkillList[i].level);
                }
            }
        }
    }

    public void OnClickSkillUpgrate(int index, int skillId, int payment)
    {
        if(Common.PaymentCheck(ref User.blackCrystal, payment))
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
            SkillSystem.SetObtainSkill(skillId);
            EffectManager.SkillUpgradeEffect(ScrollContentView.transform.GetChild(index).GetChild(0).transform);
            RefreshUI();
        }
        else
        {
            UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.blackCrystal, payment);
        }


    }
}
