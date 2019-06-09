using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapSystem
{
    public static List<Map> maps = new List<Map>();
    private static string[] mapStageName = { "격전의 숲", "두번째 맵", "세번째 맵" };
    public static void LoadMap()
    {
        maps.Clear();
        string path = Application.persistentDataPath + "/Xml/Map.Xml";
        MapDatabase md = null;

        if (System.IO.File.Exists(path))
            md = MapDatabase.Load();
        else
            md = MapDatabase.InitSetting();

        if(md!=null)
        {
            foreach (Map map in md.maps)
            {
                maps.Add(map);
            }
            Debugging.LogSystem("MapDatabase is loaded Succesfully.");
        }
    }

    public static Map GetMap(int id)
    {
        return maps.Find(item => item.id == id || item.id.Equals(id));
    }

    public static Map GetMap(string name)
    {
        return maps.Find(item => item.name.Equals(name));
    }

    public static List<Map> GetMapNode(int stageNumber)
    {
        return maps.FindAll(item => item.stageNumber.Equals(stageNumber));
    }

    public static void MapClear(int mapId, int clearPoint = 1)
    {
        Map clearMap = maps.Find(map => map.id == mapId || map.id.Equals(mapId));
        Map openMap = maps.Find(map => map.id == (mapId + 1) || map.id.Equals(mapId + 1));
        if (clearMap != null)
        {
            clearMap.clearPoint = clearPoint;
            if (openMap != null)
            {
                openMap.enable = true;
                MapDatabase.MapClearSave(mapId);
            }
            else
                Debugging.LogWarning("오픈 할 맵을 찾지못함 >> " + mapId);
        }
        else
        {
            Debugging.LogWarning("클리어 할 맵을 찾지못함 >> " + mapId);
        }
    }

    public static int GetCurrentMapId(int stageNumber = 0)
    {
        int currentMapId = 5001;
        if(stageNumber>0)
        {
            foreach (var map in GetMapNode(stageNumber))
            {
                if (map.enable)
                {
                    if (currentMapId < map.id)
                    {
                        currentMapId = map.id;
                    }
                }
            }
        }
        else
        {
            foreach (var map in maps)
            {
                if (map.enable)
                {
                    if (currentMapId < map.id)
                    {
                        currentMapId = map.id;
                    }
                }
            }
        }
        return currentMapId;
    }

    public static string GetStageName(int index)
    {
        if (index < mapStageName.Length)
            return mapStageName[index];
        else
            return null;
    }

    public static void SetMapSprite(int stageNumber, Transform MapTransform)
    {
        Sprite[] mapSprite = Resources.LoadAll<Sprite>("Maps/Stage"+stageNumber);
        foreach(var mapsp in mapSprite)
        {
            if(mapsp.name.Contains("layer1"))
            {
                var foreground = MapTransform.transform.GetChild(1).GetChild(0).gameObject;
                foreach (var foremap in foreground.GetComponentsInChildren<SpriteRenderer>())
                    foremap.sprite = mapsp;
            }
            else if(mapsp.name.Contains("field"))
            {
                var field = MapTransform.transform.GetChild(0).gameObject;
                field.GetComponent<SpriteRenderer>().sprite = mapsp;
            }
            else if(!mapsp.name.Contains("thumbnail"))
            {
                var background = MapTransform.transform.GetChild(1).GetChild(1).gameObject;
                background.GetComponentInChildren<SpriteRenderer>().sprite = mapsp;
            }
        }
        Debugging.Log(stageNumber + " 스테이지 맵이미지 로드 완료");
    }
}
