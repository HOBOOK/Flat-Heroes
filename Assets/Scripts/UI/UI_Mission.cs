using System.Collections;
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

    public GameObject dailymissionAlert;
    public GameObject weeklymissionAlert;
    public GameObject archivementmissionAlert;

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
            MissionSystem.CheckClearMissions();
            ClearView(ScrollContentViewDailyMission.transform);
            ClearView(ScrollContentViewWeekMission.transform);
            ClearView(ScrollContentViewArchivement.transform);
            LoadMissions(MissionSystem.GetDayMissions(), ScrollContentViewDailyMission.transform,0);
            LoadMissions(MissionSystem.GetWeekMissions(), ScrollContentViewWeekMission.transform,1);
            LoadMissions(MissionSystem.GetArchivement(), ScrollContentViewArchivement.transform,2);
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
    void LoadMissions(List<Mission> missions, Transform parent, int type)
    {
        bool isExistEnableMission = false;
        foreach (var mission in missions)
        {
            GameObject missionSlot = Instantiate(slotMissionPrefab, parent);
            missionImage = missionSlot.transform.GetChild(0).GetComponent<Image>();
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
            if (mission.rewardType == 3)
                rewardItemImage.sprite = ItemSystem.GetItemImage(mission.rewardItemId);
            else if(mission.rewardType==4)
                rewardItemImage.sprite = Resources.Load<Sprite>("UI/ui_normalboxFive");
            else if (mission.rewardType == 5)
                rewardItemImage.sprite = Resources.Load<Sprite>("UI/ui_specialboxFive");
            else
                rewardItemImage.sprite = Resources.Load<Sprite>(Common.GetCoinCrystalEnergyImagePath(mission.rewardType));
            rewardItemCountText.text = "x " + Common.GetThousandCommaText(MissionSystem.GetMissionRewardItemCount(mission,true));
            int missionid = mission.id;
            rewardButton.onClick.RemoveAllListeners();
            rewardButton.onClick.AddListener(delegate
            {
                OnClickRewardButton(missionid);
            });
            if (mission.clear)
            {
                rewardButton.GetComponentInChildren<Text>().color = Color.magenta;
                rewardButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("missionClear");
                rewardButton.interactable = false;
                clearPanel.SetActive(true);
            }
            else
            {
                clearPanel.SetActive(false);
                if (mission.enable)
                {
                    rewardButton.GetComponentInChildren<Text>().color = Color.yellow;
                    rewardButton.interactable = true;
                    rewardButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("missionReward");
                    if (!isExistEnableMission)
                        isExistEnableMission = true;
                }
                else
                {
                    rewardButton.GetComponentInChildren<Text>().color = Color.white;
                    rewardButton.interactable = false;
                    rewardButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("missionProgress");
                }
            }
            missionSlot.SetActive(true);
        }
        NoticeEnableMission(type, isExistEnableMission);
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
        }
        else
        {
            Debugging.Log(id + " 의 미션클리어 실패");
        }
        RefreshUI();
    }

    void NoticeEnableMission(int type, bool on)
    {
        if(type==0)
        {
            if (on) dailymissionAlert.SetActive(true);
            else dailymissionAlert.SetActive(false);
        }
        else if(type ==1)
        {
            if (on) weeklymissionAlert.SetActive(true);
            else weeklymissionAlert.SetActive(false);
        }
        else
        {
            if (on) archivementmissionAlert.SetActive(true);
            else archivementmissionAlert.SetActive(false);
        }
    }
}
