using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CloudDataInfo
{
    public string id;
    public string lastSavedTime;
    public string UserData;
    public string ItemData;
    public string HeroData;
    public string AbilityData;
    public string SkillData;
    public string MissionData;
    public string MapData;

    public void SetDataToCloud(string socialUserId,string saveTime)
    {
        id = socialUserId;
        lastSavedTime = saveTime;
        UserData = SaveSystem.GetUserDataToCloud();
        ItemData = ItemDatabase.GetItemDataToCloud();
        HeroData = HeroDatabase.GetHeroDataToCloud();
        AbilityData = AbilityDatabase.GetAbilityDataToCloud();
        SkillData = SkillDatabase.GetSkillDataToCloud();
        MissionData = MissionDatabase.GetMissionDataToCloud();
        MapData = MapDatabase.GetMapDataToCloud();
    }
}
