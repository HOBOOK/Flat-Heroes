using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AbilitySystem
{
    private static List<Ability> abilities = new List<Ability>();
    private static int[] abilitiesStats = new int[3]; // 0:attack 1:attackSpeed 2:defence
    public static void LoadAbility()
    {
        abilities.Clear();
        string path = Application.persistentDataPath + "/Xml/Ability.Xml";
        AbilityDatabase ad = null;

        if (System.IO.File.Exists(path))
            ad = AbilityDatabase.Load();
        else
            ad = AbilityDatabase.InitSetting();

        if(ad!=null)
        {
            foreach (Ability ability in ad.abilities)
            {
                abilities.Add(ability);
            }
            SetAbilityStats();
            Debugging.LogSystem("AbilityDatabase is loaded Succesfully.");
        }

    }

    public static Ability GetAbility(int id)
    {
        return abilities.Find(ability => ability.id == id || ability.id.Equals(id));
    }

    public static List<Ability> GetAllAbilities()
    {
        return abilities;
    }

    public static void ObtainAbility(int id)
    {
        Ability ab = abilities.Find(ability => ability.id == id || ability.id.Equals(id));
        if(ab!=null)
        {
            if (!ab.enable)
                ab.enable = true;
            ab.level += 1;
            User.abilityCount += 1;
            SaveSystem.SavePlayer();
            AbilityDatabase.AbilityObtainSave(id);
        }
    }

    public static string GetAbilityDescription(int powerType, int power)
    {
        string des = "";
        switch(powerType)
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

    public static void SetAbilityStats()
    {
        int abilityStat;
        for (int i = 0; i<abilitiesStats.Length; i++)
        {
            abilityStat = 0;
            foreach (var ad in abilities.FindAll(x => x.powerType == i))
            {
                if(ad.enable)
                    abilityStat += ad.power;
            }
            abilitiesStats[i] = abilityStat;
        }
    }

    public static int GetAbilityStats(int powerType)
    {
        powerType = Mathf.Clamp(powerType, 0, abilitiesStats.Length);
        return abilitiesStats[powerType];
    }
}

