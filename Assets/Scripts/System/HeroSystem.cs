using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public static class HeroSystem
{
    // 히어로 데이터베이스
    private static List<HeroData> heros = new List<HeroData>();
    // 유저가 가진 히어로 데이터베이스
    private static List<HeroData> userHeros = new List<HeroData>();
    public static void LoadHero()
    {
        heros.Clear();
        userHeros.Clear();
        string path = Application.persistentDataPath + "/Xml/Heros.Xml";
        HeroDatabase hd = null;
        HeroDatabase userHd = null;

        // 유저 데이터가 있을경우 로드
        if (System.IO.File.Exists(path))
        {
            hd = HeroDatabase.Load();
            userHd = HeroDatabase.LoadUser();
        }
        // 최초생성
        else
        {
            hd = HeroDatabase.InitSetting();
            userHd = HeroDatabase.LoadUser();
        }
        if (hd!=null)
        {
            foreach (HeroData hero in hd.heros)
            {
                heros.Add(hero);
            }
        }
        if(userHd != null)
        {
            foreach (HeroData hero in userHd.heros)
            {
                userHeros.Add(hero);
            }
        }
        if(heros!=null&userHeros !=null)
        {
            Debugging.LogSystem("HeroDatabase is loaded Succesfully.");
        }
    }

    #region 유저히어로정보
    public static List<HeroData> GetUserHeros()
    {
        List<HeroData> heroDatas = userHeros;
        heroDatas.Sort((i1, i2) => i1.name.CompareTo(i2.name));
        return heroDatas;
    }
    public static HeroData GetUserHero(int id)
    {
        return userHeros.Find(hero => hero.id == id || hero.id.Equals(id));
    }
    public static void LevelUpStatusSet(int id,Hero heroPrefab)
    {
        HeroData userData = userHeros.Find(x => x.id == id || x.id.Equals(id));

        if(userData!=null)
        {
            userData.strength += (int)(userData.strength * 0.1f);
            userData.intelligent += (int)(userData.intelligent * 0.1f);
            userData.physical += (int)(userData.physical * 0.1f);
            userData.agility += (int)(userData.agility * 0.1f);
            userData.level += 1;
            userData.exp = 0;
            heroPrefab.status.SetHeroStatus(userData);
        }
    }
    public static void SetHero(Hero heroPrefab)
    {
        HeroData heroData = userHeros.Find(hero => hero.id == heroPrefab.id || hero.id.Equals(heroPrefab.id));
        if (heroData != null)
        {
            heroData.level = heroPrefab.status.level;
            heroData.exp = heroPrefab.status.exp;
        }
        else
        {
            Debugging.LogWarning("세팅할 영웅을 찾지못함 >> " + heroPrefab.id);
        }
    }
    public static void SetObtainHero(int id)
    {
        HeroData obtainHero = heros.Find(h => h.id == id || h.id.Equals(id));

        if (obtainHero != null)
        {
            HeroDatabase.AddUser(id);
            userHeros.Add(obtainHero);
            Debugging.Log(id + " 영웅 획득 성공!");
        }
        else
        {
            Debugging.LogError("획득할 영웅을 찾지못함 >> " + id);
        }
    }
    public static void SaveHero(Hero heroPrefab)
    {
        HeroData heroData = userHeros.Find(hero => hero.id == heroPrefab.id || hero.id.Equals(heroPrefab.id));
        if (heroData != null)
        {
            heroData.level = heroPrefab.status.level;
            heroData.exp = heroPrefab.status.exp;
            HeroDatabase.SaveUser(heroPrefab.id);
        }
        else
        {
            Debugging.LogError("세팅할 영웅을 찾지못함 >> " + heroPrefab.id);
        }
    }
    public static void SaveHeros(List<GameObject> heros)
    {
        if (heros != null)
        {
            HeroDatabase.SaveAllUser(GetHerosData(heros));
        }
        else
        {
            Debugging.LogError("저장할 영웅들을 찾지못함");
        }
    }
    public static int GetHeroNeedEnergy(int id)
    {
        HeroData data = userHeros.Find(x => x.id == id || x.id.Equals(id));
        if(data!=null)
        {
            return (50 - (data.intelligent));
        }
        return 1000;
    }
    public static int GetHeroStatusAttack(HeroData data)
    {
        if (data.type == 0)
            return (10) + (data.strength * 5) + (data.intelligent * 4) + (data.physical) + (data.agility * 2) + AbilitySystem.GetAbilityStats(0);
        else
            return (10) + (data.strength * 5) + (data.intelligent * 4) + (data.physical) + (data.agility * 2);
    }
    public static int GetHeroStatusDefence(HeroData data)
    {
        return (data.physical * 5) + (data.agility) + AbilitySystem.GetAbilityStats(2);
    }
    public static int GetHeroStatusMaxHp(HeroData data)
    {
        return (200)+(data.strength * 2) + (data.intelligent) + (data.physical * 10) + (data.agility * 3);
    }
    public static int GetHeroStatusCriticalPercent(HeroData data)
    {
        return (int)((data.intelligent * 0.05f) + (data.agility * 0.2f));
    }
    public static float GetHeroStatusAttackSpeed(HeroData data)
    {
        return (data.strength * 0.001f) + (data.agility * 0.005f);
    }
    public static float GetHeroStatusMoveSpeed(HeroData data)
    {
        return (data.agility * 0.005f) + 0.5f;
    }
    public static float GetHeroStatusKnockbackResist(HeroData data)
    {
        return (data.physical * 0.2f) + (data.agility * 0.05f);
    }
    public static int GetHeroStatusSkillEnergy(HeroData data)
    {
        return data.intelligent;
    }
    public static int GetRecoveryHp(HeroData data)
    {
        return Mathf.Clamp((data.physical + 100) / 100, 1, 100);
    }
    public static string GetHeroStatusSpeedText(float data)
    {
        string txt = "";
        if (data > 2.0f)
            txt = "전설적";
        else if (data > 1.5f)
            txt = "특별함";
        else if (data > 1.0f)
            txt = "뛰어남";
        else if (data > 0.5f)
            txt = "일반적";
        else
            txt = "느림";
        return txt;
    }
    public static List<string> GetHeroChats(int id)
    {
        List<string> chatList = new List<string>();
        HeroData data = GetHero(id);
        if(data!=null)
        {
            string[] str = data.chat.Split(',');

            for (int i = 0; i < str.Length; i++)
            {
                chatList.Add(str[i]);
            }
        }
        return chatList;
    }
    #endregion

    #region 전체히어로정보
    public static HeroData GetHero(int id)
    {
        return heros.Find(hero => hero.id == id || hero.id.Equals(id));
    }
    public static HeroData GetHero(string name)
    {
        return heros.Find(hero => hero.name.Equals(name));
    }
    public static List<HeroData> GetUnableHeros()
    {
        List<HeroData> heroDatas = heros.FindAll(x => x.type!=1);
        heroDatas = heroDatas.Where(f => !userHeros.Any(t => t.id == f.id)).ToList();
        heroDatas.Sort((i1, i2) => i1.name.CompareTo(i2.name));
        return heroDatas;
    }
    public static List<HeroData> GetHerosData(List<GameObject> heroPrefabList)
    {
        List<HeroData> heroDatas = new List<HeroData>();
        foreach (var heroPrefab in heroPrefabList)
        {
            HeroData hd = GetUserHero(heroPrefab.GetComponent<Hero>().id);
            if (hd != null)
                heroDatas.Add(hd);
        }
        heroDatas = Enumerable.ToList(Enumerable.Distinct(heroDatas));
        string debugStr = "";
        foreach (var hd in heroDatas)
            debugStr += hd.name + "\r\n";
        return heroDatas;
    }
    public static Sprite GetHeroThumbnail(int id)
    {
        HeroData data = heros.Find(hero => hero.id == id || hero.id.Equals(id));
        if (data != null)
            return Resources.Load<Sprite>(data.image);
        else
            return null;
    }
    #endregion


}
