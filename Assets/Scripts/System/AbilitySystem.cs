﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AbilitySystem
{
    private static int[] abilitiesStats = new int[3]; // 0:attack 1:attackSpeed 2:defence
    // 어빌리티 전체 데이터베이스
    private static List<Ability> abilities = new List<Ability>();
    // 유저가 가진 어빌리티 데이터베이스
    private static List<Ability> userAbilities = new List<Ability>();
    public static void LoadAbility()
    {
        abilities.Clear();
        userAbilities.Clear();
        string path = Application.persistentDataPath + "/Xml/Ability.Xml";
        AbilityDatabase ad = null;
        AbilityDatabase userAd = null;

        if (System.IO.File.Exists(path))
        {
            ad = AbilityDatabase.Load();
            userAd = AbilityDatabase.LoadUser();
        }
        else
        {
            ad = AbilityDatabase.InitSetting();
            userAd = AbilityDatabase.LoadUser();
        }

        if(ad!=null)
        {
            foreach (Ability ability in ad.abilities)
            {
                abilities.Add(ability);
            }
        }
        if(userAd!=null)
        {
            foreach (Ability ability in userAd.abilities)
            {
                userAbilities.Add(ability);
            }
        }
        if(ad!=null&&userAd!=null)
        {
            Debugging.LogSystem("AbilityDatabase is loaded Succesfully.");
        }
    }

    #region 유저어빌리티정보
    public static Ability GetUserAbility(int id)
    {
        return userAbilities.Find(ability => ability.id == id || ability.id.Equals(id));
    }
    public static List<Ability> GetUserAbilities()
    {
        return userAbilities;
    }
    public static int GetAbilityStats(int powerType)
    {
        powerType = Mathf.Clamp(powerType, 0, abilitiesStats.Length);
        return abilitiesStats[powerType];
    }
    public static void SetAbilityStats()
    {
        if (userAbilities != null)
        {
            int abilityStat;
            for (int i = 0; i < abilitiesStats.Length; i++)
            {
                abilityStat = 0;
                foreach (var ad in userAbilities.FindAll(x => x.powerType == i))
                {
                    if (ad.level>0)
                        abilityStat += ad.power;
                }
                abilitiesStats[i] = abilityStat;
            }
        }
    }
    public static void SetObtainAbility(int id)
    {
        Ability userAb = userAbilities.Find(x => x.id == id || x.id.Equals(id));
        if(userAb != null)
        {
            userAb.level += 1;
            User.abilityCount += 1;
            SaveSystem.SavePlayer();
            AbilityDatabase.SaveAbility(id);
        }
        else
        {
            Ability ab = abilities.Find(ability => ability.id == id || ability.id.Equals(id));
            if (ab != null)
            {
                User.abilityCount += 1;
                SaveSystem.SavePlayer();
                ab.level = 1;
                userAbilities.Add(ab);
                AbilityDatabase.AddAbility(id);
            }
        }
    }
    #endregion

    #region 전체어빌리티정보
    public static Ability GetAbility(int id)
    {
        return abilities.Find(ability => ability.id == id || ability.id.Equals(id));
    }
    public static List<Ability> GetAllAbilities()
    {
        return abilities;
    }
    public static string GetAbilityDescription(int powerType, int power)
    {
        string des = "";
        switch (powerType)
        {
            case 0:
                des = string.Format("영웅들의 공격력이 <color='red'>{0}</color> 증가합니다.", power);
                break;
            case 1:
                des = string.Format("영웅들의 공격속도가 <color='red'>{0}</color> 증가합니다.", power);
                break;
            case 2:
                des = string.Format("영웅들의 방어력이 <color='red'>{0}</color> 증가합니다.", power);
                break;
            case 3:
                des = string.Format("영웅들의 방어력이 <color='red'>{0}</color> 증가합니다.", power);
                break;
        }
        return des;
    }
    public static bool isAbleAbility(int id)
    {
        Ability abilityDatas = userAbilities.Find(x => x.id == id || x.id.Equals(id));
        if (abilityDatas != null&&abilityDatas.level>0)
            return true;
        else
            return false;
    }
    #endregion
}
