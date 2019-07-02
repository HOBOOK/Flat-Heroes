using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class AbilitySystem
{
    private static int[] abilitiesStats = new int[7]; // 0:attack 1:defence 2:hp 3:critical 4:attackSpeed 5: moveSpeed 6: energy
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
            SetAbilityStats();
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
                        abilityStat += ad.power*ad.level;
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
    public static string GetAllAbilityStatToString()
    {
        string[] statStrings = {LocalizationManager.GetText("heroInfoAttack")+" + ",
            LocalizationManager.GetText("heroInfoDefence")+" + ",
            LocalizationManager.GetText("heroInfoHp")+" + ",
            LocalizationManager.GetText("heroInfoCritical")+" + ",
            LocalizationManager.GetText("heroInfoAttackSpeed")+" + ",
            LocalizationManager.GetText("heroInfoMoveSpeed")+" + ",
            LocalizationManager.GetText("heroInfoSkillEnergy")+" + "};

        string statString = "";
        StringBuilder sb = new StringBuilder(statString);
        int cacheStat = 0;
        for(var i = 0; i<abilitiesStats.Length; i++)
        {
            cacheStat = GetAbilityStats(i);
            if(cacheStat!=0)
            {
                sb.Append(statStrings[i] + cacheStat+"\r\n");
            }
        }
        statString = sb.ToString();
        return statString;
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
        string des = string.Format("{0} (<color='red'>+ {1}</color>)", LocalizationManager.GetText("AbilityDescription" + (powerType + 1)), power);
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
    public static string GetAbilityName(int id)
    {
        string name = null;
        Ability ab = abilities.Find(x => x.id == id || x.id.Equals(id));
        if(ab!=null)
        {
            name = LocalizationManager.GetText("AbilityName" + id);
        }
        return name;
    }
    #endregion
}

