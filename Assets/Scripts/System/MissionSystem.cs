using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionSystem
{
    public static List<Mission> missions = new List<Mission>();
    public static List<Mission> userMissions = new List<Mission>();

    public static void LoadMission()
    {
        missions.Clear();
        userMissions.Clear();
        string path = Application.persistentDataPath + "/Xml/Mission.Xml";
        MissionDatabase md = null;
        MissionDatabase userMd = null;

        if (System.IO.File.Exists(path))
        {
            md = MissionDatabase.Load();
            userMd = MissionDatabase.LoadUser();
        }
        else
        {
            md = MissionDatabase.InitSetting();
            userMd = MissionDatabase.LoadUser();
        }

        if (md != null)
        {
            foreach (Mission mission in md.missions)
            {
                missions.Add(mission);
            }
        }
        if(userMd!=null)
        {
            foreach(Mission mission in userMd.missions)
            {
                userMissions.Add(mission);
            }
        }

        if(missions!=null&&userMissions!=null)
        {
            Debugging.LogSystem("MissionDatabase is loaded Succesfully.");
        }
    }

    public static Mission GetMission(int id)
    {
        return missions.Find(item => item.id == id || item.id.Equals(id));
    }
    public static List<Mission> GetAllUserMissions()
    {
        return userMissions;
    }
    public static List<Mission> GetUnClearMissions()
    {
        List<Mission> missionDatas = missions.FindAll(m=>m.missionType==1||m.missionType.Equals(1));
        missionDatas = missionDatas.Where(f => !userMissions.Any(t => t.id == f.id)).ToList();
        missionDatas.Sort((i1, i2) => i1.id.CompareTo(i2.id));
        return missionDatas;
    }

    public static List<Mission> GetDayMissions()
    {
        string currentday = DateTime.Now.ToString("dd");
        Debugging.Log("오늘 일자 > " + currentday);
        string saveday = PlayerPrefs.GetString("MissionDay");
        if(string.IsNullOrEmpty(saveday))
        {
            PlayerPrefs.SetString("MissionDay", "00");
            PlayerPrefs.Save();
            saveday = "00";
        }
        Debugging.Log("저장된 일자 > " + saveday);
        //저장된날짜와 오늘날짜가 다를때. 저녁 12:00가 지났는데
        if (saveday!=currentday||!saveday.Equals(currentday))
        {
            PlayerPrefs.SetString("MissionDay", currentday);
            PlayerPrefs.Save();
            // 전체 일일미션을 가져온다.
            List<Mission> dayMissionDatas = missions.FindAll(m => m.missionType == 0 || m.missionType.Equals(0));
            // 랜덤 일일미션 아이디를 5개 뽑는다.
            int[] randomDayMissionID = Common.getRandomId(5, 0, dayMissionDatas.Count);
            // 다시 초기화를 하고.
            List<Mission> resultDayMissionDatas = new List<Mission>();
            for (var i = 0; i < randomDayMissionID.Length; i++)
            {
                // 랜덤으로 뽑은 미션을 추가한다.
                resultDayMissionDatas.Add(GetMission(dayMissionDatas[randomDayMissionID[i]].id));
            }
            // 현재유저일일미션을 삭제하고 새로운 일일미션 5개를 추가저장한다.
            MissionDatabase.RegenerateDayMission(resultDayMissionDatas);

            // 유저미션을 다시받아온다.
            userMissions.Clear();
            foreach(var mission in MissionDatabase.LoadUser().missions)
            {
                userMissions.Add(mission);
            }
            Debugging.Log("새로운 일일미션 데이터를 가져옴");
        }
        else
        {
            Debugging.Log("기존의 일일미션 데이터를 가져옴");
        }
        return userMissions.FindAll(m => m.missionType == 0 || m.missionType.Equals(0) && m.clear==false);
    }
    public static int DebugGetUnClearMissionId()
    {
        var missions = userMissions.FindAll(m => m.missionType == 0 || m.missionType.Equals(0) && m.clear == false);
        return missions[UnityEngine.Random.Range(0, missions.Count)].id;
    }

    public static void ClearMission(int id)
    {
        Mission clearMission = userMissions.Find(m => m.id == id || m.id.Equals(id));
        if(clearMission!=null)
        {
            Debugging.Log(id + " 의 미션클리어 했습니다.");
            clearMission.clear = true;
            MissionDatabase.ClearMission(clearMission);
        }
    }

    public static List<Mission> GetAllMissions()
    {
        return missions;
    }
}
