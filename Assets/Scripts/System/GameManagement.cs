using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using GooglePlayGames;

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

        DontDestroyOnLoad(this);
        SaveSystem.LoadPlayer();
    }
    private void Start()
    {
        HeroSystem.LoadHero();
        SkillSystem.LoadSkill();
        AbilitySystem.LoadAbility();
        ItemSystem.LoadItem();
        MapSystem.LoadMap();
        MissionSystem.LoadMission();
    }
    private void Update()
    {
        TestEventKey();
        UserUpdate();
    }

    private void OnApplicationQuit()
    {
        SaveSystem.SavePlayer();
    }

    #region 유저정보
    private float userUpdateTime=120;
    private void UserUpdate()
    {
        if(userUpdateTime<0)
        {
            if (User.portalEnergy < 30)
                User.portalEnergy += 1;
            userUpdateTime = 120;
        }
        else
        {
            userUpdateTime -= Time.fixedUnscaledDeltaTime;
        }
    }
    public string GetPortalEnergyTime()
    {
        return string.Format("<color='grey'>{0:00}:{1:00}</color>", (int)(userUpdateTime % 3600 / 60), (int)(userUpdateTime % 3600 % 60));
    }
    #endregion

    #region 스테이지 정보
    private StageInfo stageInfo;
    public void SetStageInfo(int mapId)
    {
        stageInfo = new StageInfo(mapId);
    }
    public StageInfo GetStageInfo()
    {
        if (stageInfo != null)
            return stageInfo;
        else
        {
            Debugging.LogError("스테이지 정보를 가져올 수 없습니다.");
            return null;
        }
    }
    #endregion

    #region 디버그용
    void TestEventKey()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            User.portalEnergy += 20;
            User.blackCrystal += 1000;
            User.coin += 100000;
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            float timeScale = 1;
            if (Time.timeScale == 1f)
                timeScale = 2f;
            else if (Time.timeScale == 2f)
                timeScale = 4f;
            else
                timeScale = 1f;
            Time.timeScale = timeScale;
            Debugging.Log("F2 >> " + "게임진행속도 증가 x " + timeScale);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            ItemSystem.SetObtainItem(1,1);
            ItemSystem.SetObtainItem(2, 1);
            ItemSystem.SetObtainItem(3, 1);
            ItemSystem.SetObtainItem(4, 1);
            ItemSystem.SetObtainItem(5, 1);
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            ItemSystem.SetObtainItem(8001, 1);
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
