﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Mission : MonoBehaviour
{
    #region 변수
    public GameObject slotMissionPrefab;
    protected GameObject ScrollContentViewDailyMission;
    protected GameObject ScrollContentViewWeekMission;
    protected GameObject ScrollContentViewArchivement;

    Image missionImage;
    Text missionTitleText;
    Text missionDescriptionText;
    Text missionSliderText;
    Slider missionSlider;
    Image rewardItemImage;
    Text rewardItemCountText;
    Button rewardButton;
    GameObject clearPanel;
    #endregion
    void Awake()
    {
        if (ScrollContentViewDailyMission == null)
            ScrollContentViewDailyMission = this.GetComponentInChildren<UI_TabManager>().transform.GetChild(0).GetChild(0).GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        if (ScrollContentViewWeekMission == null)
            ScrollContentViewWeekMission = this.GetComponentInChildren<UI_TabManager>().transform.GetChild(0).GetChild(1).GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        if (ScrollContentViewArchivement == null)
            ScrollContentViewArchivement = this.GetComponentInChildren<UI_TabManager>().transform.GetChild(0).GetChild(2).GetComponentInChildren<VerticalLayoutGroup>().gameObject;
    }
    void RefreshUI()
    {
        if(slotMissionPrefab!=null&&ScrollContentViewDailyMission!=null& ScrollContentViewWeekMission != null&& ScrollContentViewArchivement != null)
        {
            MissionSystem.CheckClearMissions(false);
            ClearView(ScrollContentViewDailyMission.transform);
            ClearView(ScrollContentViewWeekMission.transform);
            ClearView(ScrollContentViewArchivement.transform);
            LoadMissions(MissionSystem.GetDayMissions(), ScrollContentViewDailyMission.transform);
            LoadMissions(MissionSystem.GetWeekMissions(), ScrollContentViewWeekMission.transform);
            LoadMissions(MissionSystem.GetArchivement(), ScrollContentViewArchivement.transform);
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
            missionSlider = missionSlot.transform.GetChild(1).GetComponentInChildren<Slider>();
            missionSliderText = missionSlider.GetComponentInChildren<Text>();
            rewardItemImage = missionSlot.transform.GetChild(2).GetChild(1).GetComponent<Image>();
            rewardItemCountText = rewardItemImage.GetComponentInChildren<Text>();
            rewardButton = missionSlot.transform.GetChild(2).GetComponentInChildren<Button>();
            clearPanel = missionSlot.transform.GetChild(3).gameObject;

            missionImage.sprite = MissionSystem.GetMissionImage(mission);
            if (mission.missionType == 2)
                missionTitleText.text = MissionSystem.GetMissionName(mission.id) + string.Format(" {0} {1}", (mission.missionLevel+1), LocalizationManager.GetText("MissionLevel"));
            else
                missionTitleText.text = MissionSystem.GetMissionName(mission.id);
            missionDescriptionText.text = MissionSystem.GetMissionDescription(mission.id);
            missionSliderText.text = string.Format(" ({0}/{1})", mission.point, mission.clearPoint);
            missionSlider.value = ((float)mission.point / (float)mission.clearPoint);
            if (mission.rewardType==3)
                rewardItemImage.sprite = ItemSystem.GetItemImage(mission.rewardItemId);
            else
                rewardItemImage.sprite = Resources.Load<Sprite>(Common.GetCoinCrystalEnergyImagePath(mission.rewardType));
            rewardItemCountText.text = "x " + Common.GetThousandCommaText(mission.rewardItemCount);
            int missionid = mission.id;
            rewardButton.onClick.RemoveAllListeners();
            rewardButton.onClick.AddListener(delegate
            {
                OnClickRewardButton(missionid);
            });
            if (mission.clear)
            {
                rewardButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("missionClear");
                rewardButton.interactable = false;
                clearPanel.SetActive(true);
            }
            else
            {
                clearPanel.SetActive(false);
                if (mission.enable)
                {
                    rewardButton.interactable = true;
                    rewardButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("missionReward");
                }
                else
                {
                    rewardButton.interactable = false;
                    rewardButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("missionProgress");
                }
            }
            missionSlot.SetActive(true);
        }
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    public void OnClickRewardButton(int id)
    {
        Mission mission = MissionSystem.GetUserMission(id);
        if(mission!=null&&mission.enable&&!mission.clear)
        {
            MissionSystem.ClearMission(id);
            MissionSystem.RewardType rewardType = (MissionSystem.RewardType)mission.rewardType;

            switch(rewardType)
            {
                case MissionSystem.RewardType.coin:
                    SaveSystem.AddUserCoin(mission.rewardItemCount);
                    UI_Manager.instance.ShowGetAlert(Common.GetCoinCrystalEnergyImagePath(0), string.Format("<color='yellow'>{0}</color> {1} {2}", LocalizationManager.GetText("Coin"),mission.rewardItemCount,LocalizationManager.GetText("alertGetMessage4")));
                    break;
                case MissionSystem.RewardType.crystal:
                    SaveSystem.AddUserCrystal(mission.rewardItemCount);
                    UI_Manager.instance.ShowGetAlert(Common.GetCoinCrystalEnergyImagePath(1), string.Format("<color='yellow'>{0}</color> {1} {2}", LocalizationManager.GetText("Crystal"), mission.rewardItemCount, LocalizationManager.GetText("alertGetMessage4")));
                    break;
                case MissionSystem.RewardType.energy:
                    SaveSystem.AddUserEnergy(mission.rewardItemCount);
                    UI_Manager.instance.ShowGetAlert(Common.GetCoinCrystalEnergyImagePath(2), string.Format("<color='yellow'>{0}</color> {1} {2}", LocalizationManager.GetText("Energy"), mission.rewardItemCount, LocalizationManager.GetText("alertGetMessage4")));
                    break;
                case MissionSystem.RewardType.scroll:
                    ItemSystem.SetObtainItem(mission.rewardItemId, mission.rewardItemCount);
                    Item rewardItem = ItemSystem.GetItem(mission.rewardItemId);
                    UI_Manager.instance.ShowGetAlert(rewardItem.image, string.Format("<color='yellow'>{0}</color> {1}", rewardItem.name,LocalizationManager.GetText("alertGetMessage3")));
                    break;
            }
        }
        else
        {
            Debugging.Log(id + " 의 미션클리어 실패");
        }
        RefreshUI();
    }
}
