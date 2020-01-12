using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAbilitySystem : MonoBehaviour
{
    public static List<HeroAbility> heroAbilityList = new List<HeroAbility>();
    public static List<HeroAbility> supportHeroAbilityList = new List<HeroAbility>();

    public static void LoadHeroAbility()
    {
        for (var i = 0; i < 9; i++)
        {
            heroAbilityList.Add(new HeroAbility(i + 1, LocalizationManager.GetText("HeroAbilityName" + (i + 1)), LocalizationManager.GetText("HeroAbilityDescription" + (i + 1)), 0));
            if (i == 3 || i == 4 || i == 7||i==8)
            {
                supportHeroAbilityList.Add(new HeroAbility(i + 1, LocalizationManager.GetText("HeroAbilityName" + (i + 1)), LocalizationManager.GetText("HeroAbilityDescription" + (i + 1)), 0));
            }
        }
    }

    public static string GetHeroAbilityName(int id)
    {
        return heroAbilityList.Find(x => x.id == id || x.id.Equals(id)).name;
    }
    public static string GetHeroAbilityDescription(int id)
    {
        return heroAbilityList.Find(x => x.id == id || x.id.Equals(id)).description;
    }

    public static int GetAbilityPower(int type, int level)
    {
        switch(type)
        {
            case 1: return level;
            case 2: return 5 + level * 5;
            case 3: return level * 10;
            case 4: return level * 70;
            case 5: return level * 100;
            case 6: return level * 3;
            case 7: return (level * level * 1000)/5;
            case 8: return level * 30;
            case 9: return level * 300;
        }
        return 0;
    }

    public static string GetAbilityPowerDescription(int type, int level)
    {
        switch (type)
        {
            case 1: return string.Format("{0}{1}",GetAbilityPower(type,level),"%");
            case 2: return string.Format("{0}{1}",GetAbilityPower(type,level),"");
            case 3: return string.Format("{0}{1}",GetAbilityPower(type,level),"%");
            case 4: return string.Format("{0}{1}",GetAbilityPower(type,level),"");
            case 5: return string.Format("{0}{1}",GetAbilityPower(type,level),"");
            case 6: return string.Format("{0}{1}",GetAbilityPower(type,level),"%");
            case 7: return string.Format("{0}{1}",GetAbilityPower(type,level),"");
            case 8: return string.Format("{0}{1}",GetAbilityPower(type,level)/10,"%");
            case 9: return string.Format("{0}{1}",GetAbilityPower(type,level),"");
        }
        return "";
    }

    public static string GetHeroAbilityDetailDescription(int type, int level)
    {
        return string.Format("{0} <color='red'>{1}</color>", GetHeroAbilityDescription(type), GetAbilityPowerDescription(type, level));
    }
}
