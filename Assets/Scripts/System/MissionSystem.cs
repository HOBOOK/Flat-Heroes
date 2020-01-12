using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 미션 타입 정의
/// 0:일일임무 1:주요임무 2:업적
/// 0. 일일임무 클리어타입 > 0,1,4,17,18,19,20,15
/// 1. 주간임무 클리어타입 > 0,1,2,3,10,19,20,21,12,13,22
/// 2. 업적 클리어타입 > 3,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20
/// 
/// 클리어 타입 정의 
/// 0:몬스터 처치 1:스테이지 클리어 2:일일미션 모두 클리어 3:출석 4:가챠뽑기 5:케릭터모으기 6:아이템수집가
///  7: 유저레벨 8:영웅총합레벨 9:연구총합레벨 10:누적코인소모 11:누적수정소모 12:누적에너지소모 13:누적주문서소모 14:플레이어스킬총합레벨
///  15: 영웅스킬총합레벨 16: 총스테이지진행수 17:아이템드랍횟수 18:코인드랍횟수 19:보스킬 20:장비조합하기 21:무료가챠뽑기 22:주간미션 모두 클리어
///  
/// >> 업적종합확인타입 7,8,9,14,15
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
        if (missions != null && userMissions != null)
        {
            Debugging.LogSystem("MissionDatabase is loaded Succesfully.");
        }
        GetDayMissions();
        GetWeekMissions();
        GetArchivement();
        CheckClearMissions();
    }

    #region 임무성공확인
    public static void ClearMission(int id)
    {
        Mission clearMission = userMissions.Find(m => m.id == id || m.id.Equals(id));
        if (clearMission != null)
        {
            Debugging.Log(id + " 의 미션클리어 했습니다.");
            if(clearMission.missionType==2)
            {
                clearMission.enable = false;
                clearMission.missionLevel += 1;
                clearMission.clearPoint = GetArchivementClearPoint(clearMission);
            }
            else
            {
                if(clearMission.missionType==0)
                    AddClearPoint(ClearType.DayMissionClear);
                else if(clearMission.missionType==1)
                    AddClearPoint(ClearType.WeeklyClear);

                clearMission.clear = true;
                MissionDatabase.ClearMission(clearMission);
            }
            RewardMission(clearMission);
        }
    }
    public static void RewardMission(Mission mission)
    {
        RewardType rewardType = (RewardType)mission.rewardType;
        switch (rewardType)
        {
            case RewardType.coin:
                SaveSystem.AddUserCoin(GetMissionRewardItemCount(mission));
                UI_Manager.instance.ShowGetAlert(Common.GetCoinCrystalEnergyImagePath(0), string.Format("<color='yellow'>{0}</color> {1} {2}", LocalizationManager.GetText("Coin"), Common.GetThousandCommaText(GetMissionRewardItemCount(mission)), LocalizationManager.GetText("alertGetMessage4")));
                break;
            case RewardType.crystal:
                SaveSystem.AddUserCrystal(GetMissionRewardItemCount(mission));
                UI_Manager.instance.ShowGetAlert(Common.GetCoinCrystalEnergyImagePath(1), string.Format("<color='yellow'>{0}</color> {1} {2}", LocalizationManager.GetText("Crystal"), Common.GetThousandCommaText(GetMissionRewardItemCount(mission)), LocalizationManager.GetText("alertGetMessage4")));
                break;
            case RewardType.energy:
                SaveSystem.AddUserEnergy(GetMissionRewardItemCount(mission));
                UI_Manager.instance.ShowGetAlert(Common.GetCoinCrystalEnergyImagePath(2), string.Format("<color='yellow'>{0}</color> {1} {2}", LocalizationManager.GetText("Energy"), Common.GetThousandCommaText(GetMissionRewardItemCount(mission)), LocalizationManager.GetText("alertGetMessage4")));
                break;
            case RewardType.scroll:
                ItemSystem.SetObtainItem(mission.rewardItemId, GetMissionRewardItemCount(mission));
                Item rewardItem = ItemSystem.GetItem(mission.rewardItemId);
                UI_Manager.instance.ShowGetAlert(rewardItem.image, string.Format("<color='yellow'>{0}</color> {1}", ItemSystem.GetItemName(rewardItem.id), LocalizationManager.GetText("alertGetMessage3")));
                break;
            case RewardType.specialGacha:
                UI_Manager.instance.PopupGetGacha(GachaSystem.GachaType.SpecialFive);
                break;
        }
    }
    public static int GetMissionRewardItemCount(Mission mission, bool isUI=false)
    {
        RewardType rewardType = (RewardType)mission.rewardType;
        Mission refMission = missions.Find(x => x.id == mission.id || x.id.Equals(mission.id));
        if(refMission!=null)
        {
            if (mission.missionType == 2)
            {
                if (rewardType == RewardType.coin || rewardType == RewardType.crystal || rewardType == RewardType.energy || rewardType == RewardType.scroll)
                {
                    int rewardCount = isUI? refMission.rewardItemCount + (int)(refMission.rewardItemCount * (mission.missionLevel + 1) * (mission.missionLevel + 1) * 0.03f) : refMission.rewardItemCount + (int)(refMission.rewardItemCount * mission.missionLevel * mission.missionLevel * 0.03f);
                    return rewardCount;
                }

                else
                    return refMission.rewardItemCount;
            }
            else
            {
                return refMission.rewardItemCount;
            }
        }
        return 0;
    }
    public static void CheckClearMissions()
    {
        SetArchivementClearPoint();
        List<Mission> clearMissions = new List<Mission>();
        bool isUnClearEnabledMission = false;
        foreach(var mission in userMissions)
        {
            if (mission.point >= mission.clearPoint && !mission.enable && !mission.clear)
            {
                clearMissions.Add(mission);
                mission.enable = true;
            }
            if (!isUnClearEnabledMission && mission.enable&&!mission.clear)
            {
                Debugging.LogWarning("=============>>" + GetMissionName(mission.id));
                isUnClearEnabledMission = true;
            }
        }
        PointSave();
        UI_Manager.instance.MissionUIAlertOn(isUnClearEnabledMission);
    }
    public static void AddClearPoint(ClearType clearType)
    {
        List<Mission> currentMissions = userMissions.FindAll(x => !x.enable && !x.clear && x.clearType == (int)clearType);
        for (var i = 0; i < currentMissions.Count; i++)
        {
            //업적
            if(currentMissions[i].missionType==2)
            {
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
    public static void AddClearPoint(ClearType clearType, int count)
    {
        List<Mission> currentMissions = userMissions.FindAll(x => !x.enable && !x.clear && x.clearType == (int)clearType);
        for (var i = 0; i < currentMissions.Count; i++)
        {
            //업적
            if (currentMissions[i].missionType == 2)
            {
                currentMissions[i].point += count;
            }
            //임무
            else
            {
                if (currentMissions[i].point < currentMissions[i].clearPoint)
                    currentMissions[i].point += count;
            }
        }
    }
    public static void SetArchivementClearPoint()
    {
        int[] setCheckTypes = {7, 8, 9, 14, 15 };
        int point = 0;
        for(var j = 0; j < setCheckTypes.Length; j++)
        {
            List<Mission> currentMissions = userMissions.FindAll(x => !x.enable && !x.clear && x.clearType == setCheckTypes[j]);
            for (var i = 0; i < currentMissions.Count; i++)
            {
                if (currentMissions[i].missionType == 2)
                {
                    switch(currentMissions[i].clearType)
                    {
                        case 7:
                            point = User.level;
                            break;
                        case 8:
                            point = 0;
                            foreach(var hero in HeroSystem.GetUserHeros())
                            {
                                point += hero.level;
                            }
                            User.HeroRankPoint = point;
                            break;
                        case 9:
                            point = User.flatEnergyMaxLevel + User.flatEnergyChargingLevel + User.addMoneyLevel + User.addExpLevel + User.addAttackLevel + User.addDefenceLevel;
                            break;
                        case 14:
                            point = 0;
                            foreach(var skill in SkillSystem.GetPlayerSkillList())
                            {
                                point += SkillSystem.GetUserSkillLevel(skill.id);
                            }
                            break;
                        case 15:
                            point = 0;
                            foreach(var skill in SkillSystem.GetUserHerosSkills())
                            {
                                point += SkillSystem.GetUserSkillLevel(skill.id);
                            }
                            break;
                    }
                    currentMissions[i].point = point;
                }
            }
        }
        Debugging.Log("업적 세팅타입 설정완료");
    }
    public static void PointSave()
    {
        MissionDatabase.PointSave(userMissions.FindAll(x => !x.clear));
    }
    public enum ClearType { EnemyKill, StageClear,DayMissionClear,Attendance,Gacha,CollectHero,CollectEquipment,PlayerLevel,TotalHeroLevel,TotalLabLevel,TotalUseCoin,TotalUseCrystal,TotalUseEnergy,TotalUseScroll,TotalPlayerSkillLevel,TotalHeroSkillLevel,TotalStageCount,TotalItemDropCount,TotalCoinDropCount,BossKill,EquipUpgrade,FreeGacha,WeeklyClear };
    public enum RewardType { coin, crystal, energy, scroll, normalGacha, specialGacha };
    public static int GetArchivementClearPoint(Mission mission)
    {
        int point = 0;
        if(mission.missionType==2)
        {
            var refMission = missions.Find(x => x.id == mission.id || x.id.Equals(mission.id));
            int refPoint = refMission.clearPoint;
            if(refMission!=null)
                point = refPoint + (int)(refPoint * mission.missionLevel*mission.missionLevel);
        }
        return point;
    }
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
        List<Mission> archivementList = missions.FindAll(x => x.missionType == 2 || x.missionType.Equals(2));
        List<Mission> newArchivementList = new List<Mission>();
        foreach(var achivement in archivementList)
        {
            Mission mission = userMissions.Find(x => x.id == achivement.id || x.id.Equals(achivement));
            if(mission==null)
            {
                newArchivementList.Add(achivement);
            }
        }
        if(newArchivementList.Count>0)
        {
            MissionDatabase.GenerateMission(newArchivementList);
            foreach (var mission in newArchivementList)
            {
                userMissions.Add(mission);
            }
        }
        return userMissions.FindAll(item => item.missionType == 2 || item.missionType.Equals(2));
    }
    public static List<Mission> GetDayMissions()
    {
        string currentday = DateTime.Now.ToString("dd");
        Debugging.Log("오늘 일자 > " + currentday);
        string saveday = PlayerPrefs.GetString("MissionDay");
        Debugging.Log("저장된 일자 > " + saveday);
        //저장된날짜와 오늘날짜가 다를때. 저녁 12:00가 지났는데
        if (saveday!=currentday||!saveday.Equals(currentday)|| string.IsNullOrEmpty(saveday) || userMissions.FindAll(m => m.missionType == 0 || m.missionType.Equals(0)).Count < 1)
        {
            PlayerPrefs.SetString("MissionDay", currentday);
            PlayerPrefs.Save();
            // 전체 일일미션을 가져온다.
            List<Mission> dayMissionDatas = missions.FindAll(m => (m.missionType == 0 || m.missionType.Equals(0))&&m.id!=0);
            // 랜덤 일일미션 아이디를 5개 뽑는다.
            int[] randomDayMissionID = Common.getRandomId(4, 0, dayMissionDatas.Count);
            // 다시 초기화를 하고.
            List<Mission> resultDayMissionDatas = new List<Mission>();
            resultDayMissionDatas.Add(GetMission(0));
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
        string currentWeek = (int.Parse(DateTime.Now.ToString("dd"))/7).ToString();
        Debugging.Log("이번 주 > " + currentWeek);
        string saveWeek = PlayerPrefs.GetString("MissionWeek");
        Debugging.Log("저장된 주 > " + saveWeek);
        //주가 변경될경우
        if (saveWeek != currentWeek || !saveWeek.Equals(currentWeek)|| string.IsNullOrEmpty(saveWeek)|| userMissions.FindAll(m => m.missionType == 1 || m.missionType.Equals(1)).Count<1)
        {
            PlayerPrefs.SetString("MissionWeek", currentWeek);
            PlayerPrefs.Save();
            // 전체 주간미션을 가져온다.
            List<Mission> weekMissionDatas = missions.FindAll(m => (m.missionType == 1 || m.missionType.Equals(1))&&m.id!=100);
            // 랜덤 주간미션 아이디를 5개 뽑는다.
            int[] randomDayMissionID = Common.getRandomId(5, 0, weekMissionDatas.Count);
            // 다시 초기화를 하고.
            List<Mission> resultDayMissionDatas = new List<Mission>();
            resultDayMissionDatas.Add(GetMission(100));
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
            des = LocalizationManager.GetText("MissionClearMessage" + mission.clearType);
        }
        return des;
    }
    public static Sprite GetMissionImage(Mission mission)
    {
        switch(mission.missionType)
        {
            case 0:
                return Resources.Load<Sprite>("Mission/mission_Day");
            case 1:
                return Resources.Load<Sprite>("Mission/mission_Week");
            case 2:
                return Resources.Load<Sprite>("Mission/mission_Archivement1");
        }
        return ItemSystem.GetItemNoneImage();
    }
    #endregion
}
