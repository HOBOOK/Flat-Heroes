using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSystem
{
    public static List<Mission> missions = new List<Mission>();

    public static void LoadMission()
    {
        missions.Clear();
        string path = Application.persistentDataPath + "/Xml/Mission.Xml";
        MissionDatabase md = null;

        if (System.IO.File.Exists(path))
            md = MissionDatabase.Load();
        else
            md = MissionDatabase.InitSetting();

        if (md != null)
        {
            foreach (Mission mission in md.missions)
            {
                missions.Add(mission);
            }
            Debugging.LogSystem("MissionDatabase is loaded Succesfully.");
        }
    }

    public static Mission GetMission(int id)
    {
        return missions.Find(item => item.id == id || item.id.Equals(id));
    }

    public static List<Mission> GetAllMissions()
    {
        return missions;
    }
}
