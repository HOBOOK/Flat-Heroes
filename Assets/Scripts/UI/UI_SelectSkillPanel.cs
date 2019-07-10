using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectSkillPanel : MonoBehaviour
{
    public GameObject ScrollContentView;
    public GameObject slotPrefab;
    public GameObject SelectSkillPanel;
    public GameObject SelectSkillInformationPanel;

    public List<Skill> playerSkillList = new List<Skill>();
    public List<Skill> selectedSkillList = new List<Skill>();

    Image skillImage;
    Button skillButton;

    private void OnEnable()
    {
        EnableUI();
    }

    void EnableUI()
    {
        if (ScrollContentView != null)
        {
            playerSkillList.Clear();
            selectedSkillList.Clear();
            playerSkillList = SkillSystem.GetAblePlayerSkillList();
            selectedSkillList = SkillSystem.GetSelectSkillList();
            RefreshUI();
        }
    }

    void RefreshUI()
    {
        foreach (Transform child in ScrollContentView.transform)
        {
            Destroy(child.gameObject);
        }
        for(int i =0; i<2; i++)
        {
            if(SelectSkillPanel.transform.GetChild(i).childCount>0)
            {
                Destroy(SelectSkillPanel.transform.GetChild(i).GetChild(0).gameObject);
            }
        }

        for (var i = 0; i < playerSkillList.Count; i++)
        {
            GameObject slot = Instantiate(slotPrefab, ScrollContentView.transform);
            skillImage = slot.transform.GetChild(0).GetComponent<Image>();
            skillButton = slot.GetComponent<Button>();

            skillImage.sprite = SkillSystem.GetSkillImage(playerSkillList[i].id);
            skillButton.onClick.RemoveAllListeners();
            int index = i;
            skillButton.onClick.AddListener(delegate
            {
                OnSelectSkill(playerSkillList[index]);
            });
            if(SkillSystem.isPlayerSkillAble(playerSkillList[i].id))
            {
                skillButton.enabled = true;
                slot.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                skillButton.enabled = false;
                slot.transform.GetChild(2).gameObject.SetActive(true);
            }
        }

        for (var i = 0; i < selectedSkillList.Count && i < 2; i++)
        {
            if (selectedSkillList[i] != null)
            {
                GameObject slot = Instantiate(slotPrefab, SelectSkillPanel.transform.GetChild(i).transform);
                slot.transform.localPosition = Vector3.zero;
                slot.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f);
                skillImage = slot.transform.GetChild(0).GetComponent<Image>();
                skillButton = slot.GetComponent<Button>();

                skillImage.sprite = SkillSystem.GetSkillImage(selectedSkillList[i].id);
                skillButton.onClick.RemoveAllListeners();
                int index = i;
                skillButton.onClick.AddListener(delegate
                {
                    OnCancleSelectedSkill(selectedSkillList[index]);
                });

                SelectSkillInformationPanel.transform.GetChild(i).gameObject.SetActive(true);
                SelectSkillInformationPanel.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = SkillSystem.GetSkillName(selectedSkillList[i].id);
                SelectSkillInformationPanel.transform.GetChild(i).GetChild(1).GetComponent<Text>().text = SkillSystem.GetSkillDescription(selectedSkillList[i].id);
            }
        }
        for(var i = selectedSkillList.Count; i <SelectSkillInformationPanel.transform.childCount; i++)
        {
            SelectSkillInformationPanel.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void OnSelectSkill(Skill skill)
    {
        if(selectedSkillList.Count<2)
        {
            selectedSkillList.Add(skill);
            playerSkillList.Remove(skill);
            RefreshUI();
        }
    }

    void OnCancleSelectedSkill(Skill skill)
    {
        selectedSkillList.Remove(skill);
        playerSkillList.Add(skill);
        RefreshUI();
    }

    public void CompletedSelectSkill()
    {
        SkillSystem.SetPlayerSkill(selectedSkillList);
        if (this.transform.parent.GetComponent<UI_HeroSelect>()!=null)
        {
            this.transform.parent.GetComponent<UI_HeroSelect>().RefreshPlayerSkillUI();
        }
        this.gameObject.SetActive(false);
    }
}
