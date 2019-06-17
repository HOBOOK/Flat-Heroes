using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapSystem
{
    public static List<Map> maps = new List<Map>();
    public static List<Map> userMaps = new List<Map>();
    private static string[] mapStageName = { "격전의 숲", "두번째 맵", "세번째 맵" };
    public static void LoadMap()
    {
        maps.Clear();
        userMaps.Clear();
        string path = Application.persistentDataPath + "/Xml/Map.Xml";
        MapDatabase md = null;
        MapDatabase userMd = null;

        if (System.IO.File.Exists(path))
        {
            md = MapDatabase.Load();
            userMd = MapDatabase.LoadUser();
        }
        else
        {
            md = MapDatabase.InitSetting();
            userMd = MapDatabase.LoadUser();
        }
        if (md!=null)
        {
            foreach (Map map in md.maps)
            {
                maps.Add(map);
            }
        }
        if (userMd != null)
        {
            foreach (Map map in userMd.maps)
            {
                userMaps.Add(map);
            }
        }
        if (maps!=null&& userMaps != null)
        {
            Debugging.LogSystem("MapDatabase is loaded Succesfully.");
        }
    }

    #region 유저맵정보
    public static Map GetUserMap(int id)
    {
        return userMaps.Find(item => item.id == id || item.id.Equals(id));
    }
    public static void MapClear(int mapId, int clearPoint = 1)
    {
        Map clearMap = userMaps.Find(map => map.id == (mapId) || map.id.Equals(mapId));
        Map openMap = maps.Find(map => map.id == (mapId+1) || map.id.Equals(mapId+1));
        if(clearMap!=null)
        {
            clearMap.clearPoint = clearPoint;
            if (openMap != null)
            {
                userMaps.Add(openMap);
            }
            MapDatabase.AddMapClear(clearMap.id,openMap.id);
        }
        else
        {
            Debugging.LogWarning("클리어 할 맵을 찾지못함 >> " + mapId);
        }
    }
    public static int GetCurrentMapId(int stageNumber = 0)
    {
        int currentMapId = 5001;
        if (stageNumber > 0)
        {
            foreach (var map in GetMapNode(stageNumber))
            {
                if (isAbleMap(map.id))
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
                if (isAbleMap(map.id))
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
    public static int GetCurrentAllMapId()
    {
        int currentMapId = 5001;
        foreach (var map in maps)
        {
            if (isAbleMap(map.id))
            {
                if (currentMapId < map.id)
                {
                    currentMapId = map.id;
                }
            }
        }
        return currentMapId;
    }
    public static bool isAbleMap(int id)
    {

        Map userMap = userMaps.Find(x => x.id == id || x.id.Equals(id));

        if (userMap != null)
        {

            return true;
        }

        else
            return false;
    }
    #endregion
    #region 전체맵정보
    public static Map GetMap(int id)
    {
        Map map = maps.Find(item => item.id == id || item.id.Equals(id));
        if (map == null)
            map = maps.Find(item => item.id == 5001);
        return map;
    }
    public static List<Map> GetMapNode(int stageNumber)
    {
        return maps.FindAll(item => item.stageNumber.Equals(stageNumber));
    }
    public static int GetMapCount()
    {
        return maps.Count;
    }
    public static List<Map> GetMapNodeAll()
    {
        return maps;
    }
    public static string GetStageName(int index)
    {
        if (index < mapStageName.Length)
            return mapStageName[index];
        else
            return null;
    }
    public static void SetMapSprite(int stageNumber, ref Transform MapTransform)
    {
        Sprite[] mapSprite = Resources.LoadAll<Sprite>("Maps/Stage" + stageNumber);
        foreach (var mapsp in mapSprite)
        {
            if (mapsp.name.Contains("layer1"))
            {
                var foreground = MapTransform.transform.GetChild(1).GetChild(0).gameObject;
                foreach (var foremap in foreground.GetComponentsInChildren<SpriteRenderer>())
                    foremap.sprite = mapsp;
            }
            else if (mapsp.name.Contains("field"))
            {
                var field = MapTransform.transform.GetChild(0).gameObject;
                field.GetComponent<SpriteRenderer>().sprite = mapsp;
            }
            else if (!mapsp.name.Contains("tile"))
            {
                var background = MapTransform.transform.GetChild(1).GetChild(1).gameObject;
                background.GetComponentInChildren<SpriteRenderer>().sprite = mapsp;
            }
        }
        Debugging.Log(stageNumber + " 스테이지 맵이미지 로드 완료");
    }
    #endregion


}
