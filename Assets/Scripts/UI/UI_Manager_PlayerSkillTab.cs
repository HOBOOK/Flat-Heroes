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
                skillNeedCrystalText = slot.transform.GetComponentInChildren<Button>().GetComponentInChildren<Text>();

                skillImage.sprite = SkillSystem.GetSkillImage(playerSkillList[i].id);
                skillDescriptionText.text = string.Format("Lv{0} {1}\r\n\r\n{2}",SkillSystem.GetUserSkillLevel(playerSkillList[i].id),playerSkillList[i].name,playerSkillList[i].description);
                skillNeedCrystalText.text = "100";
            }

        }
    }
}
