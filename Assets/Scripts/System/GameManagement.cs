using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public static GameManagement instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        this.transform.hideFlags = HideFlags.HideInInspector;
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        DontDestroyOnLoad(this);
        SaveSystem.LoadPlayer();
    }
    private void Start()
    {
        HeroSystem.LoadHero();
        HeroAbilitySystem.LoadHeroAbility();
        SkillSystem.LoadSkill();
        AbilitySystem.LoadAbility();
        ItemSystem.LoadItem();
        MapSystem.LoadMap();
        MissionSystem.LoadMission();
        LocalizationManager.LoadLanguage(User.language);
        UserStart();
    }
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isCheckAlertOn)
                StartCoroutine("CheckingAlert");
        }
        TestEventKey();
        UserUpdate();
    }
    bool isCheckAlertOn = false;
    IEnumerator CheckingAlert()
    {
        isCheckAlertOn = true;
        var alertPanel = UI_Manager.instance.ShowNeedAlert("", LocalizationManager.GetText("alertExitMessage"));
        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 예를 클릭
            Application.Quit();
        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 아니오를 클릭
        }
        isCheckAlertOn = false;
        yield return null;
    }

    bool bPaused;
    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            bPaused = true;
            Common.LastLoginTimeSave();
            GoogleSignManager.SaveData();
            Debugging.Log("일시정지");
        }
        else
        {
            if(bPaused)
            {
                bPaused = false;
                GetPortalEnergy();
            }
        }
    }

    private void OnApplicationQuit()
    {
        Common.LastLoginTimeSave();
        GoogleSignManager.SaveData();
    }

    public void TutorialStageStart()
    {
        if (Common.GetSceneCompareTo(Common.SCENE.MAIN))
        {
            if(PlayerPrefs.HasKey("isTutorialEnd")&&PlayerPrefs.GetInt("isTutorialEnd")>0&&User.tutorialSequence==0)
            {
                User.tutorialSequence = PlayerPrefs.GetInt("isTutorialEnd");
            }
            if (User.tutorialSequence == 0)
            {
                Debugging.Log("튜토리얼 시작");
                LoadSceneManager.instance.LoadScene(6);
            }
            else if(User.tutorialSequence == 1)
            {
                ItemSystem.SetObtainItem(1,4);
                ItemSystem.SetObtainItem(8001);
            }
        }
        else
        {
            Debugging.Log("Main씬이 아님");
        }

    }

    #region 유저정보
    private void UserStart()
    {
        if (User.tutorialSequence>1)
        {
            GetPortalEnergy();
            GetAttendanceDate();
        }
        SetResetPlayerAbility();
    }
    private float userUpdateTime=45;
    private void UserUpdate()
    {
        if(userUpdateTime<0)
        {
            if (User.portalEnergy < 0)
                User.portalEnergy = 0;
            if (User.portalEnergy < 40)
                User.portalEnergy += 1;
            userUpdateTime = 45;
        }
        else
        {
            userUpdateTime -= Time.fixedUnscaledDeltaTime;
        }
    }
    public string GetPortalEnergyTime()
    {
        return string.Format("<size='32'><color='grey'>{0:0}:{1:00}</color></size>", (int)(userUpdateTime % 3600 / 60), (int)(userUpdateTime % 3600 % 60));
    }
    public void GetPortalEnergy()
    {
        Debugging.Log("겟 포탈 에너지");
        if (PlayerPrefs.HasKey("LastLogInTime"))
        {
            Common.lastLoginTime = ulong.Parse(PlayerPrefs.GetString("LastLogInTime"));
        }
        else
        {
            Common.LastLoginTimeSave();
        }

        if (Common.lastLoginTime != 0)
        {
            ulong diff = ((ulong)DateTime.Now.Ticks - Common.lastLoginTime);
            ulong m = diff / TimeSpan.TicksPerMillisecond;
            double secondsLeft = (double)(m) / 1000.0f;
            if (secondsLeft > 60)
            {
                int rewardCoin = Mathf.Clamp((int)secondsLeft*2, 120, 72000);
                UI_Manager.instance.ShowLoginRewardUI(rewardCoin);

                if (User.portalEnergy < 40)
                {
                    if (User.portalEnergy + (int)(secondsLeft / 45) >= 40)
                        User.portalEnergy = 40;
                    else
                        User.portalEnergy += Mathf.Clamp((int)(secondsLeft / 45), 0, 40);
                }
                Common.LastLoginTimeSave();
                GoogleSignManager.SaveData();
            }
        }
    }
    public void GetAttendanceDate()
    {
        string userDate = null;
        if (!PlayerPrefs.HasKey("DailyCheck"))
            PlayerPrefs.SetString("DailyCheck", DateTime.Now.ToShortDateString());
        //최초시작
        Debugging.Log(User.dailySaveDate + "\r\n" + PlayerPrefs.GetString("DailyCheck"));
        if (string.IsNullOrEmpty(User.dailySaveDate) ||User.dailySaveDate.Equals("0"))
        {
            User.dailySaveDate = DateTime.Now.ToShortDateString();
            PlayerPrefs.SetString("DailyCheck", User.dailySaveDate);
        }
        userDate = PlayerPrefs.GetString("DailyCheck");
        string currentDate = DateTime.Now.ToShortDateString();

        DateTime T1 = DateTime.Parse(userDate); //저장된출석
        DateTime T2 = DateTime.Parse(User.dailySaveDate); //최초시작
        DateTime T3 = DateTime.Parse(currentDate); //현재

        DateTime StartDate = DateTime.Parse("2018-08-18");
        DateTime FinalDate = DateTime.Parse("2020-09-18");
        if (T3 < FinalDate && T3 > StartDate)
        {
            TimeSpan TS = T3 - T1;
            TimeSpan TS2 = T1 - T2;

            int diffDay = TS.Days;  //날짜의 차이 구하기
            int diffDay2 = TS2.Days;

            // 연속체크 초기화
            if (diffDay > 2 || diffDay2 > 5)
            {
                User.dailySaveDate = currentDate;
                PlayerPrefs.SetString("DailyCheck", currentDate);
                //UI_Manager.instance.ShowDailyCheckUI(0);
            }
            else if (diffDay == 1)
            {
                MissionSystem.AddClearPoint(MissionSystem.ClearType.Attendance);
                PlayerPrefs.SetString("DailyCheck", currentDate);
                //UI_Manager.instance.ShowDailyCheckUI(diffDay2 + 1);
            }
        }
    }
    int GetInitationScrollCount(int abilityCount)
    {
        if (abilityCount == 0)
            return 0;
        else
            return abilityCount + GetInitationScrollCount(abilityCount - 1);
    }
    public void SetResetPlayerAbility()
    {
        if (User.abilityCount > 0)
        {
            string date = DateTime.Now > DateTime.Parse("2019-10-28") ? DateTime.Now.ToShortDateString() : "2019-10-28";
            int returnScrollCount = GetInitationScrollCount(Mathf.Clamp(User.abilityCount - 1, 0, 5000));
            User.abilityCount = 0;
            User.statsPoint = ((User.level - 1) * 3)+1;
            AbilitySystem.ResetAbility();
            PostMessageManager.Instance.AddPostMessage("(8001," + returnScrollCount + ",플레이어 능력 개편에 따른 반환된 스크롤," + date + ")");
            PostMessageManager.Instance.AddPostMessage("(10002," + 300 + ",개편 보상입니다. ^_^," + date + ")");
            PostMessageManager.Instance.AddPostMessage("(10001," + 500000 + ",개편 보상입니다. ^_^," + date + ")");
            GoogleSignManager.SaveData();
            UI_Manager.instance.ShowAlert(Common.GetCoinCrystalEnergyImagePath(7), string.Format("<color='yellow'>+{0} {1}</color>\r\n\r\n{2}", LocalizationManager.GetText("UserStatPoint"), (User.level - 1 * 3), "플레이어 능력이 개편되어 사용된 능력개방주문서(플랫 스크롤)이 반환되었습니다. \r\n우편함을 확인해주세요. "));
        }
    }
    #endregion

    #region 스테이지 정보
    private StageInfo stageInfo;
    public void SetStageInfo(int mapId)
    {
        Map map = MapSystem.GetMap(mapId);
        if(map!=null)
        {
            if (Common.stageModeType == Common.StageModeType.Main)
                stageInfo = new StageInfo(map.id, map.stageNumber);
            else if (Common.stageModeType == Common.StageModeType.Infinite)
                stageInfo = new StageInfo(map.id, UnityEngine.Random.Range(0, 7));
            else if (Common.stageModeType == Common.StageModeType.Boss)
                stageInfo = new StageInfo(map.id, 7);
            else if (Common.stageModeType == Common.StageModeType.Battle)
                stageInfo = new StageInfo();
            else if (Common.stageModeType == Common.StageModeType.Attack)
                stageInfo = new StageInfo();
        }
    }
    public StageInfo GetStageInfo()
    {
        if (stageInfo != null)
            return stageInfo;
        else
        {
            
            Debugging.LogError("스테이지 정보를 가져올 수 없습니다.");
            return new StageInfo();
        }
    }
    #endregion

    #region 디버그용
    void TestEventKey()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debugging.Log("F1");
        }
    }
    void DebugInitXmlData()
    {
        string path = "XmlData/Map";
        TextAsset _xml = Resources.Load<TextAsset>(path);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_xml.text);

        XmlNodeList nodes = xmlDoc.SelectNodes("MapCollection/Maps/Map"); 
        foreach (XmlNode node in nodes)
        {
            if (node.SelectSingleNode("ClearPoint").InnerText.Equals("1"))
                node.SelectSingleNode("ClearPoint").InnerText = "0";
            if (node.SelectSingleNode("Enable").InnerText.Equals("true"))
                node.SelectSingleNode("Enable").InnerText = "false";
        }
        xmlDoc.Save("./Assets/Resources/XmlData/Map.xml");
        Debugging.Log(path + " 데이터 쓰기 완료");
    }
    void DebugWriteXmlData(string path, string selectNodes, string savePath, string createElementName)
    {
        //string path = "XmlData/Map";
        TextAsset _xml = Resources.Load<TextAsset>(path);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_xml.text);

        XmlNodeList nodes = xmlDoc.SelectNodes(selectNodes); // ex) MapCollection/Maps/Map
        foreach (XmlNode node in nodes)
        {
            XmlElement name = xmlDoc.CreateElement(createElementName);
            name.InnerText = "0";
            node.AppendChild(name);
        }
        xmlDoc.Save(savePath); // ex) "./Assets/Resources/XmlData/Map.xml"
        Debugging.Log(path +" 데이터 쓰기 완료");
    }

    #endregion
}
