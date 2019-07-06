using System.Collections;
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
            missionTitleText.text = MissionSystem.GetMissionName(mission.id);
            missionDescriptionText.text = MissionSystem.GetMissionDescription(mission.id) + string.Format(" ({0}/{1})",mission.point,mission.clearPoint);
            if(mission.rewardType==3)
                rewardItemImage.sprite = Resources.Load<Sprite>(ItemSystem.GetItem(mission.rewardItemId).image);
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
                case MissionSystem.RewardType.item:
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
