﻿using System.Collections;
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
            HeroData hd = GetHero(heroPrefab.GetComponent<Hero>().id);
            if (hd != null)
                heroDatas.Add(hd);
        }
        heroDatas = Enumerable.ToList(Enumerable.Distinct(heroDatas));
        string debugStr = "";
        foreach (var hd in heroDatas)
            debugStr += hd.name + "\r\n";

        Debugging.Log(debugStr);
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