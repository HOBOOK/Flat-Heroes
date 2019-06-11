using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HeroSystem
{
    private static List<HeroData> heros = new List<HeroData>();
    public static void LoadHero()
    {
        heros.Clear();
        string path = Application.persistentDataPath + "/Xml/Heros.Xml";
        HeroDatabase hd = null;

        if (System.IO.File.Exists(path))
            hd = HeroDatabase.Load();
        else
            hd = HeroDatabase.InitSetting();

        if(hd!=null)
        {
            foreach (HeroData hero in hd.heros)
            {
                heros.Add(hero);
            }
            Debugging.LogSystem("HeroDatabase is loaded Succesfully.");
        }
    }

    public static HeroData GetHero(int id)
    {
        return heros.Find(hero => hero.id == id || hero.id.Equals(id));
    }

    public static List<HeroData> GetUserHeros()
    {
        List<HeroData> heroDatas = heros.FindAll(h => h.enable == true);
        heroDatas.Sort((i1, i2) => i1.name.CompareTo(i2.name));
        return heroDatas;
    }

    public static List<HeroData> GetUnableHeros()
    {
        List<HeroData> heroDatas = heros.FindAll(h => h.enable == false&&h.type==0);
        heroDatas.Sort((i1, i2) => i1.name.CompareTo(i2.name));
        return heroDatas;
    }

    public static HeroData GetHero(string name)
    {
        return heros.Find(hero => hero.name.Equals(name));
    }
    public static void SetHero(Hero heroPrefab)
    {
        HeroData heroData = heros.Find(hero => hero.id == heroPrefab.id || hero.id.Equals(heroPrefab.id));
        if (heroData != null)
        {
            heroData.level = heroPrefab.status.level;
            heroData.exp = heroPrefab.status.exp;
        }
        else
        {
            Debugging.LogError("세팅할 영웅을 찾지못함 >> " + heroPrefab.id);
        }
    }
    public static void SetObtainHero(int id)
    {
        HeroData obtainHero = heros.Find(h => h.id == id || h.id.Equals(id));

        if (obtainHero != null)
        {
            obtainHero.enable = true;
            HeroDatabase.Save(id);
            Debugging.Log(id + " 영웅 획득 성공!");
        }
        else
        {
            Debugging.LogError("획득할 맵을 찾지못함 >> " + id);
        }
    }

    public static void SaveHero(Hero heroPrefab)
    {
        HeroData heroData = heros.Find(hero => hero.id == heroPrefab.id || hero.id.Equals(heroPrefab.id));
        if(heroData!=null)
        {
            heroData.level = heroPrefab.status.level;
            heroData.exp = heroPrefab.status.exp;
            HeroDatabase.Save(heroPrefab.id);
        }
        else
        {
            Debugging.LogError("세팅할 영웅을 찾지못함 >> " + heroPrefab.id);
        }
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

    public static void SaveHeros(List<GameObject> heros)
    {
        if(heros!=null)
        {
            HeroDatabase.SaveAll(GetHerosData(heros));
        }
        else
        {
            Debugging.LogError("저장할 영웅들을 찾지못함");
        }
    }
    public static Sprite GetHeroThumbnail(int id)
    {
        HeroData data = heros.Find(hero => hero.id == id || hero.id.Equals(id));
        if (data != null)
            return Resources.Load<Sprite>(data.image);
        else
            return null;
    }

}
