using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 미션 타입 정의
/// 0:일일임무 1:주요임무 2:업적
/// 0. 일일임무 클리어타입 > 0,1,4,17,18,19
/// 1. 주간임무 클리어타입 > 
/// 2. 업적 클리어타입 > 
/// 
/// 클리어 타입 정의 
/// 0:몬스터 처치 1:스테이지 클리어 2:일일미션 모드 클리어 3:출석 4:가챠뽑기 5:케릭터모으기 6:아이템수집가
///  7: 유저레벨 8:영웅총합레벨 9:연구총합레벨 10:누적코인소모 11:누적수정소모 12:누적에너지소모 13:누적주문서소모 14:플레이어스킬총합레벨
///  15: 영웅스킬총합레벨 16: 총스테이지진행수 17:아이템드랍횟수 18:코인드랍횟수 19:보스킬
///  
/// 보상 타입 정의
/// 0: 코인 1: 수정 2: 에너지 3: 주문서 4: 일반장비상자 5: 특별장비상자
/// </summary>
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

        GetDayMissions();
        if (missions!=null&&userMissions!=null)
        {
            Debugging.LogSystem("MissionDatabase is loaded Succesfully.");
        }

        CheckClearMissions();
    }

    #region 임무성공확인
    public static void ClearMission(int id)
    {
        Mission clearMission = userMissions.Find(m => m.id == id || m.id.Equals(id));
        if (clearMission != null)
        {
            Debugging.Log(id + " 의 미션클리어 했습니다.");
            clearMission.clear = true;
            MissionDatabase.ClearMission(clearMission);
        }
    }
    public static void CheckClearMissions()
    {
        List<Mission> clearMissions = new List<Mission>();
        foreach(var mission in userMissions)
        {
            if (mission.point >= mission.clearPoint&&!mission.enable&&!mission.clear)
            {
                clearMissions.Add(mission);
                mission.enable = true;
            }
        }
        if(clearMissions.Count>0)
        {
            PointSave();
            UI_Manager.instance.ShowAlert("UI/ui_trophy", string.Format("<color='yellow'>'{0}'</color> {1} {2} {3} ", GetMissionName(clearMissions[0].id),LocalizationManager.GetText("missionClearAlertMessage2"), clearMissions.Count-1, LocalizationManager.GetText("missionClearAlertMessage")));
        }
    }
    public static void AddClearPoint(ClearType clearType)
    {
        List<Mission> currentMissions = userMissions.FindAll(x => !x.enable && !x.clear && x.clearType == (int)clearType);
        for (var i = 0; i < currentMissions.Count; i++)
        {
            //업적
            if(currentMissions[i].missionType==2)
            {
                if (currentMissions[i].point < currentMissions[i].clearPoint)
                    currentMissions[i].point += 1;
            }
            //임무
            else
            {
                if (currentMissions[i].point < currentMissions[i].clearPoint)
                    currentMissions[i].point += 1;
            }
        }

    }
    public static void PointSave()
    {
        MissionDatabase.PointSave(userMissions.FindAll(x => !x.clear));
    }
    public static void CoinReward(int coin)
    {

    }
    public static void CrystalReward(int crystal)
    {

    }
    public static void EnergyReward(int energy)
    {

    }
    public static void ItemReward(int itemId)
    {

    }
    public enum ClearType { EnemyKill, StageClear,DayMissionClear,Attendance,Gacha,CollectHero,CollectEquipment,PlayerLevel,TotalHeroLevel,TotalLabLevel,TotalUseCoin,TotalUseCrystal,TotalUseEnergy,TotalUseScroll,TotalPlayerSkillLevel,TotalHeroSkillLevel,TotalStageCount,TotalItemDropCount,TotalCoinDropCount,BossKill };
    public enum RewardType { coin, crystal, energy, scroll, normalGacha, specialGacha };
    #endregion

    #region 임무데이터베이스
    public static Mission GetMission(int id)
    {
        return missions.Find(item => item.id == id || item.id.Equals(id));
    }
    public static List<Mission> GetAllUserMissions()
    {
        return userMissions;
    }
    public static Mission GetUserMission(int id)
    {
        return userMissions.Find(item => item.id==id||item.id.Equals(id));
    }
    public static List<Mission> GetArchivement()
    {
        return missions.FindAll(item => item.missionType == 2 || item.missionType.Equals(2));
    }
    public static List<Mission> GetDayMissions()
    {
        string currentday = DateTime.Now.ToString("dd");
        Debugging.Log("오늘 일자 > " + currentday);
        string saveday = PlayerPrefs.GetString("MissionDay");
        Debugging.Log("저장된 일자 > " + saveday);
        //저장된날짜와 오늘날짜가 다를때. 저녁 12:00가 지났는데
        if (saveday!=currentday||!saveday.Equals(currentday)|| string.IsNullOrEmpty(saveday))
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
        return userMissions.FindAll(m => m.missionType == 0 || m.missionType.Equals(0));
    }
    public static List<Mission> GetWeekMissions()
    {
        string currentWeek = (DateTime.Now.DayOfYear/7).ToString();
        Debugging.Log("오늘 일자 > " + currentWeek);
        string saveWeek = PlayerPrefs.GetString("MissionWeek");
        Debugging.Log("저장된 일자 > " + saveWeek);
        //주가 변경될경우
        if (saveWeek != currentWeek || !saveWeek.Equals(currentWeek)|| string.IsNullOrEmpty(saveWeek))
        {
            PlayerPrefs.SetString("MissionWeek", currentWeek);
            PlayerPrefs.Save();
            // 전체 주간미션을 가져온다.
            List<Mission> weekMissionDatas = missions.FindAll(m => m.missionType == 1 || m.missionType.Equals(1));
            // 랜덤 주간미션 아이디를 5개 뽑는다.
            int[] randomDayMissionID = Common.getRandomId(5, 0, weekMissionDatas.Count);
            // 다시 초기화를 하고.
            List<Mission> resultDayMissionDatas = new List<Mission>();
            for (var i = 0; i < randomDayMissionID.Length; i++)
            {
                // 랜덤으로 뽑은 미션을 추가한다.
                resultDayMissionDatas.Add(GetMission(weekMissionDatas[randomDayMissionID[i]].id));
            }
            // 현재유저주간미션을 삭제하고 새로운 주간미션 5개를 추가저장한다.
            MissionDatabase.RegenerateWeekMission(resultDayMissionDatas);

            // 유저미션을 다시받아온다.
            userMissions.Clear();
            foreach (var mission in MissionDatabase.LoadUser().missions)
            {
                userMissions.Add(mission);
            }
            Debugging.Log("새로운 주간미션 데이터를 가져옴");
        }
        else
        {
            Debugging.Log("기존의 주간미션 데이터를 가져옴");
        }
        return userMissions.FindAll(m => m.missionType == 1 || m.missionType.Equals(1));
    }
    public static int DebugGetUnClearMissionId()
    {
        var missions = userMissions.FindAll(m => m.missionType == 0 || m.missionType.Equals(0) && m.clear == false);
        return missions[UnityEngine.Random.Range(0, missions.Count)].id;
    }
    public static List<Mission> GetAllMissions()
    {
        return missions;
    }
    public static string GetMissionName(int id)
    {
        string name = null;
        Mission mission = missions.Find(x => x.id == id || x.id.Equals(id));
        if (mission != null)
        {
            name = LocalizationManager.GetText("MissionName" + id);
        }
        return name;
    }
    public static string GetMissionDescription(int id)
    {
        string des = null;
        Mission mission = missions.Find(x => x.id == id || x.id.Equals(id));
        if (mission != null)
        {
            des = LocalizationManager.GetText("MissionDescription" + id);
        }
        return des;
    }
    public static Sprite GetMissionImage(Mission mission)
    {
        Sprite sprite = Resources.Load<Sprite>("Mission/mission_" + mission.clearType);
        if (sprite != null)
            return sprite;
        return ItemSystem.GetItemNoneImage();
    }
    #endregion
}
