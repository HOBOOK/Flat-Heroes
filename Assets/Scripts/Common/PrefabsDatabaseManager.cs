using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Destroy를 사용할 객체관리클래스
public class PrefabsDatabaseManager : MonoBehaviour
{
    public static PrefabsDatabaseManager instance = null;
    public GameObject heroLobbyPoint;
    public static List<GameObject> heroPrefabList = new List<GameObject>();
    public static List<GameObject> monsterPrefabList = new List<GameObject>();
    public static List<GameObject> castlePrefabList = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        //var heroPrefabs = Resources.LoadAll<GameObject>("Prefabs/Heros");
        //foreach (var heroPrefab in heroPrefabs)
        //{
        //    heroPrefabList.Add(heroPrefab);
        //}
        //var monsterPrefabs = Resources.LoadAll<GameObject>("Prefabs/Monsters");
        //foreach (var monsterPrefab in monsterPrefabs)
        //{
        //    heroPrefabList.Add(monsterPrefab);
        //}
    }

    public void AddPrefabToHeroList(GameObject prefab)
    {
        if (prefab != null)
            heroPrefabList.Add(prefab);
        else
        {
            UI_StartManager.instance.ShowErrorUI("영웅 데이터가 정상적으로 추가되지 않았습니다.");
        }

    }
    public void AddPrefabToMonsterList(GameObject prefab)
    {
        if(prefab!=null)
            monsterPrefabList.Add(prefab);
    }
    public void AddPrefabToCastleList(GameObject prefab)
    {
        if(prefab!=null)
            castlePrefabList.Add(prefab);
    }

    public Transform GetLobbyPoint(int index)
    {
        if (heroLobbyPoint == null)
        {
            heroLobbyPoint = GameObject.Find("HeroPoints");
        }
        if(heroPrefabList!=null)
        {
            Transform point = null;
            int childcnt = heroLobbyPoint.transform.childCount;
            if (index < childcnt)
            {
                point = heroLobbyPoint.transform.GetChild(index).transform;
            }
            else
            {
                Debugging.Log("index초과 > " + index);
            }
            if (point != null)
                return point;
            else
                Debugging.Log("point가 null");
        }
        else
        {
            Debugging.Log("heroPrefabList가 Null임");
        }
;       return null;
    }

    public GameObject GetHeroPrefab(int id)
    {
        if(heroPrefabList!=null)
        {
            foreach(var hero in heroPrefabList)
            {

                if(hero.GetComponent<Hero>().id==id)
                {
                    return hero;
                }
            }
            Debugging.LogWarning(heroPrefabList.Count + " 의 영웅 프리팹 준비됨. >> " + id + " 의 영웅 프리팹을 찾을 수 없음.");
            return null;
        }
        else
        {

            Debugging.LogWarning("heroPrefabList가 없음.");
            return null;
        }
    }
    public GameObject GetCastlePrefab(int id)
    {
        if (castlePrefabList != null)
        {
            foreach (var castle in castlePrefabList)
            {

                if (castle.GetComponent<Castle>().id == id)
                {
                    return castle;
                }
            }
            Debugging.LogWarning(castlePrefabList.Count + " 의 캐슬 프리팹 준비됨. >> " + id + " 의 캐슬 프리팹을 찾을 수 없음.");
            return null;
        }
        else
        {

            Debugging.LogWarning("castlePrefabList 없음.");
            return null;
        }
    }
    public GameObject GetMonsterPrefab(int id)
    {
        if (monsterPrefabList != null)
        {
            foreach (var monster in monsterPrefabList)
            {
                if (monster.GetComponent<Hero>().id == id)
                {
                    return monster;
                }
            }
            Debugging.LogWarning(monsterPrefabList.Count + " 의 몬스터 프리팹 준비됨. >> " + id + " 의 몬스터 프리팹을 찾을 수 없음.");
            return null;
        }
        else
        {

            Debugging.LogWarning("monsterPrefabList 가 없음.");
            return null;
        }
    }

    public void GetPrefabList()
    {
        string str = "";
        foreach (var i in heroPrefabList)
        {
            str += i.name + "\r\n";
        }
        foreach (var i in monsterPrefabList)
        {
            str += i.name + "\r\n";
        }
        foreach (var i in castlePrefabList)
        {
            str += i.name + "\r\n";
        }
        Debugging.Log(str);
    }

}