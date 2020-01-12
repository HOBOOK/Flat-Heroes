using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CloudDataInfo
{
    public string localId;
    public string name;
    public string lastSavedTime;
    public string UserData;
    public string ItemData;
    public string HeroData;
    public string AbilityData;
    public string SkillData;
    public string MissionData;
    public string MapData;

    public CloudDataInfo()
    {
        lastSavedTime = DateTime.Now.ToString();
    }

    public CloudDataInfo(string socialId, string socialName, string saveTime)
    {
        localId = socialId;
        name = socialName;
        lastSavedTime = saveTime;
    }

    public void SetDataToCloud(string socialUserId,string socialName, string saveTime)
    {
        localId = socialUserId;
        name = socialName;
        lastSavedTime = saveTime;
        try
        {
            UserData = SaveSystem.GetUserDataToCloud();
            ItemData = ItemDatabase.GetItemDataToCloud();
            HeroData = HeroDatabase.GetHeroDataToCloud();
            AbilityData = AbilityDatabase.GetAbilityDataToCloud();
            SkillData = SkillDatabase.GetSkillDataToCloud();
            MissionData = MissionDatabase.GetMissionDataToCloud();
            MapData = MapDatabase.GetMapDataToCloud();
        }
        catch(Exception e)
        {
            Debugging.Log(e.StackTrace);
        }

    }
    public bool IsNullData()
    {
        if (UserData != null && ItemData != null && HeroData != null && AbilityData != null && SkillData != null && MissionData != null && MapData != null)
            return false;
        else
            return true;
    }
}
