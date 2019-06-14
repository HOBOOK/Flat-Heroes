﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Mission : MonoBehaviour
{
    #region 변수
    public GameObject slotMissionPrefab;
    protected GameObject ScrollContentViewDailyMission;
    protected GameObject ScrollContentViewRepeatMission;
    protected GameObject ScrollContentViewMainMission;

    Image missionImage;
    Text missionTitleText;
    Text missionDescriptionText;
    Image rewardItemImage;
    Text rewardItemCountText;
    Button rewardButton;
    GameObject clearPanel;
    #endregion
    void Awake()
    {
        if (ScrollContentViewDailyMission == null)
            ScrollContentViewDailyMission = this.GetComponentInChildren<UI_TabManager>().transform.GetChild(0).GetChild(0).GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        if (ScrollContentViewRepeatMission == null)
            ScrollContentViewRepeatMission = this.GetComponentInChildren<UI_TabManager>().transform.GetChild(0).GetChild(1).GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        if (ScrollContentViewMainMission == null)
            ScrollContentViewMainMission = this.GetComponentInChildren<UI_TabManager>().transform.GetChild(0).GetChild(2).GetComponentInChildren<VerticalLayoutGroup>().gameObject;
    }
    void RefreshUI()
    {
        if(slotMissionPrefab!=null&&ScrollContentViewDailyMission!=null&ScrollContentViewMainMission!=null&& ScrollContentViewRepeatMission!=null)
        {
            ClearView(ScrollContentViewDailyMission.transform);
            ClearView(ScrollContentViewRepeatMission.transform);
            ClearView(ScrollContentViewMainMission.transform);
            LoadMissions(MissionSystem.GetDayMissions(), ScrollContentViewDailyMission.transform);
            LoadMissions(MissionSystem.GetRepeatMissions(), ScrollContentViewRepeatMission.transform);
            LoadMissions(MissionSystem.GetMainMissions(), ScrollContentViewMainMission.transform);
        }
    }
    void ClearView(Transform parent)
    {
        if (parent.childCount > 0)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
    }
    void LoadMissions(List<Mission> missions, Transform parent)
    {
        foreach (var mission in missions)
        {
            GameObject missionSlot = Instantiate(slotMissionPrefab, parent);
            missionImage = missionSlot.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            missionTitleText = missionSlot.transform.GetChild(1).GetChild(1).GetComponentInChildren<Text>();
            missionDescriptionText = missionSlot.transform.GetChild(1).GetChild(0).GetComponent<Text>();
            rewardItemImage = missionSlot.transform.GetChild(2).GetChild(1).GetComponent<Image>();
            rewardItemCountText = rewardItemImage.GetComponentInChildren<Text>();
            rewardButton = missionSlot.transform.GetChild(2).GetComponentInChildren<Button>();

            clearPanel = missionSlot.transform.GetChild(3).gameObject;

            missionImage.sprite = Resources.Load<Sprite>(mission.image);
            missionTitleText.text = mission.name;
            missionDescriptionText.text = mission.description;
            rewardItemImage.sprite = Resources.Load<Sprite>(ItemSystem.GetItem(mission.rewardItemId).image);
            rewardItemCountText.text = "x " + Common.GetThousandCommaText(mission.rewardItemCount);
            if (mission.clear)
            {
                rewardButton.GetComponentInChildren<Text>().text = "임무완료";
                clearPanel.SetActive(true);
            }
            else
            {
                rewardButton.GetComponentInChildren<Text>().text = "진행중";
                clearPanel.SetActive(false);
            }
            missionSlot.SetActive(true);
        }
    }

    private void OnEnable()
    {
        RefreshUI();
    }
}
