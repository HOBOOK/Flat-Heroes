using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Mission : MonoBehaviour
{
    public GameObject slotMissionPrefab;
    protected GameObject ScrollContentViewDailyMission;
    protected GameObject ScrollContentViewMainMission;

    Image missionImage;
    Text missionTitleText;
    Text missionDescriptionText;
    Image rewardItemImage;
    Text rewardItemCountText;

     void Awake()
    {
        if (ScrollContentViewDailyMission == null)
            ScrollContentViewDailyMission = this.GetComponentInChildren<UI_TabManager>().transform.GetChild(0).GetChild(0).GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        if (ScrollContentViewMainMission == null)
            ScrollContentViewMainMission = this.GetComponentInChildren<UI_TabManager>().transform.GetChild(0).GetChild(1).GetComponentInChildren<VerticalLayoutGroup>().gameObject;
    }
    void RefreshUI()
    {
        if(slotMissionPrefab!=null&&ScrollContentViewDailyMission!=null&ScrollContentViewMainMission!=null)
        {
            if(ScrollContentViewDailyMission.transform.childCount>0)
            {
                foreach(Transform child in ScrollContentViewDailyMission.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            if(ScrollContentViewMainMission.transform.childCount>0)
            {
                foreach (Transform child in ScrollContentViewMainMission.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            foreach(var mission in MissionSystem.GetAllMissions())
            {
                if(mission.missionType==0) // 일일 미션
                {
                    GameObject missionSlot = Instantiate(slotMissionPrefab, ScrollContentViewDailyMission.transform);
                    missionImage = missionSlot.transform.GetChild(0).GetChild(0).GetComponent<Image>();
                    missionTitleText = missionSlot.transform.GetChild(1).GetChild(1).GetComponentInChildren<Text>();
                    missionDescriptionText = missionSlot.transform.GetChild(1).GetChild(0).GetComponent<Text>();
                    rewardItemImage = missionSlot.transform.GetChild(2).GetChild(1).GetComponent<Image>();
                    rewardItemCountText = rewardItemImage.GetComponentInChildren<Text>();

                    missionImage.sprite = Resources.Load<Sprite>(mission.image);
                    missionTitleText.text = mission.name;
                    missionDescriptionText.text = mission.description;
                    rewardItemImage.sprite = Resources.Load<Sprite>(ItemSystem.GetItem(mission.rewardItemId).image);
                    rewardItemCountText.text = "x "+Common.GetThousandCommaText(mission.rewardItemCount);
                    missionSlot.SetActive(true);
                }
                else if(mission.missionType==1) // 주 미션
                {
                    GameObject missionSlot = Instantiate(slotMissionPrefab, ScrollContentViewMainMission.transform);
                    missionImage = missionSlot.transform.GetChild(0).GetChild(0).GetComponent<Image>();
                    missionTitleText = missionSlot.transform.GetChild(1).GetChild(1).GetComponentInChildren<Text>();
                    missionDescriptionText = missionSlot.transform.GetChild(1).GetChild(0).GetComponent<Text>();
                    rewardItemImage = missionSlot.transform.GetChild(2).GetChild(1).GetComponent<Image>();
                    rewardItemCountText = rewardItemImage.GetComponentInChildren<Text>();

                    missionImage.sprite = Resources.Load<Sprite>(mission.image);
                    missionTitleText.text = mission.name;
                    missionDescriptionText.text = mission.description;
                    rewardItemImage.sprite = Resources.Load<Sprite>(ItemSystem.GetItem(mission.rewardItemId).image);
                    rewardItemCountText.text = "x " + Common.GetThousandCommaText(mission.rewardItemCount);
                    missionSlot.SetActive(true);
                }
            }
        }
    }
    void Start()
    {
        RefreshUI();
    }
}
