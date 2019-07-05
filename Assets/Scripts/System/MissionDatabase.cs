using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Linq;

[XmlRoot("MissionCollection")]
public class MissionDatabase
{
    [XmlArray("Missions"), XmlArrayItem("Mission")]
    public List<Mission> missions = new List<Mission>();
    public static MissionDatabase InitSetting()
    {
        string path = Application.persistentDataPath + "/Xml/Mission.Xml";
        if (!System.IO.File.Exists(path))
        {
            string folderPath;
            folderPath = Application.persistentDataPath + "/Xml";
            DirectoryInfo di = new DirectoryInfo(folderPath);
            if (di.Exists == false)
            {
                di.Create();
            }
            TextAsset _xml = Resources.Load<TextAsset>("XmlData/Mission");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_xml.text);
            if (_xml != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MissionDatabase));
                var reader = new StringReader(_xml.text);
                MissionDatabase missionDB = serializer.Deserialize(reader) as MissionDatabase;
                reader.Close();
                CreateXml(path);
                Debugging.Log("MissionDatabase 최초 생성 성공");
                return missionDB;
            }
        }
        Debugging.Log("MissionDatabase 최초 생성 실패");
        return null;
    }

    #region 전체임무정보
    public static MissionDatabase Load()
    {
        TextAsset _xml = Resources.Load<TextAsset>("XmlData/Mission");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_xml.text);
        if (_xml != null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MissionDatabase));
            var reader = new StringReader(_xml.text);
            MissionDatabase missionDB = serializer.Deserialize(reader) as MissionDatabase;
            reader.Close();
            Debugging.Log("MissionDatabase 로드 성공");
            return missionDB;
        }
        return null;
    }
    #endregion

    #region 유저임무정보
    public static string GetMissionDataToCloud()
    {
        string path = Application.persistentDataPath + "/Xml/Mission.Xml";
        if (System.IO.File.Exists(path))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));
            XmlElement elmRoot = xmlDoc.DocumentElement;
            return elmRoot.InnerText;
        }
        return null;
    }
    public static void SetCloudDataToMission(CloudDataInfo data)
    {
        SaveCloudData(data.MissionData);
    }
    public static void SaveCloudData(string data)
    {
        string path = Application.persistentDataPath + "/Xml/Mission.Xml";
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(path))
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));

        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        elmRoot.RemoveAll();
        var decrpytData = DataSecurityManager.DecryptData(data);
        elmRoot.InnerXml = decrpytData;
        // 암호화/////
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }
    public static MissionDatabase LoadUser()
    {
        string path = Application.persistentDataPath + "/Xml/Mission.Xml";
        if (System.IO.File.Exists(path))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));
            //복호화////
            XmlElement elmRoot = xmlDoc.DocumentElement;
            var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);

            elmRoot.InnerXml = decrpytData;
            //////////
            string _xml;
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                xmlDoc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                _xml = stringWriter.GetStringBuilder().ToString();
            }
            if (_xml != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MissionDatabase));
                var reader = new StringReader(_xml);
                MissionDatabase missionDB = serializer.Deserialize(reader) as MissionDatabase;
                reader.Close();
                Debugging.Log("MissionDatabase 유저 파일 로드");
                return missionDB;
            }
        }
        Debugging.LogSystemWarning("MissionDatabase wasn't loaded. >> " + path + " is null. >>");
        return null;
    }
    public static void RegenerateDayMission(List<Mission> dayMissions)
    {
        string path = Application.persistentDataPath + "/Xml/Mission.Xml";
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(path))
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));

        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;

        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////

        XmlNodeList nodes = xmlDoc.SelectNodes("MissionCollection/Missions/Mission");
        
        foreach (XmlNode node in nodes)
        {
            if(node.SelectSingleNode("MissionType").InnerText.Equals("0"))
            {
                node.ParentNode.RemoveChild(node);
            }
        }
        CreateNodes(dayMissions, xmlDoc, path);
        Debugging.Log("일일 미션 초기화 완료");
    }
    public static void ClearMission(Mission mission)
    {
        string path = Application.persistentDataPath + "/Xml/Mission.Xml";
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(path))
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));

        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;

        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////

        XmlNodeList nodes = xmlDoc.SelectNodes("MissionCollection/Missions/Mission");

        foreach (XmlNode node in nodes)
        {
            if (node.Attributes.GetNamedItem("id").Value == mission.id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(mission.id.ToString()))
            {
                node.SelectSingleNode("Clear").InnerText = mission.clear.ToString().ToLower();
                break;
            }
        }
        // 암호화/////
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
        Debugging.Log(mission.name + " 일일미션클리어 xml 저장 완료");
    }
    public static void PointSave(List<Mission> missons)
    {
        string path = Application.persistentDataPath + "/Xml/Mission.Xml";
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(path))
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));

        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////

        XmlNodeList nodes = xmlDoc.SelectNodes("MissionCollection/Missions/Mission");

        foreach (XmlNode node in nodes)
        {
            foreach(var mission in missons)
            {
                if (node.Attributes.GetNamedItem("id").Value.Equals(mission.id.ToString()))
                {
                    node.SelectSingleNode("Point").InnerText = mission.point.ToString();
                    node.SelectSingleNode("Enable").InnerText = mission.enable.ToString().ToLower();
                    break;
                }
            }
        }
        // 암호화/////
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
        Debugging.Log("미션포인트 xml 저장 완료");
    }
    #endregion


    public static void CreateXml(string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        // Xml을 선언한다(xml의 버전과 인코딩 방식을 정해준다.)
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        // 루트 노드 생성
        XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "MissionCollection", string.Empty);
        xmlDoc.AppendChild(root);

        XmlNode root2 = xmlDoc.CreateNode(XmlNodeType.Element, "Missions", string.Empty);
        root.AppendChild(root2);

        // 암호화/////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }
    public static void CreateNode(Mission data, XmlDocument xmlDoc, string path)
    {
        // 자식 노드 생성
        XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Mission", string.Empty);
        xmlDoc.DocumentElement.FirstChild.AppendChild(child);

        XmlAttribute id = xmlDoc.CreateAttribute("id");
        id.Value = data.id.ToString();
        child.Attributes.Append(id);

        // 자식 노드에 들어갈 속성 생성
        XmlElement name = xmlDoc.CreateElement("Name");
        name.InnerText = data.name;
        child.AppendChild(name);
        XmlElement description = xmlDoc.CreateElement("Description");
        description.InnerText = data.description;
        child.AppendChild(description);
        XmlElement type = xmlDoc.CreateElement("MissionType");
        type.InnerText = data.missionType.ToString();
        child.AppendChild(type);
        XmlElement level = xmlDoc.CreateElement("MissionLevel");
        level.InnerText = data.missionLevel.ToString();
        child.AppendChild(level);
        XmlElement image = xmlDoc.CreateElement("Image");
        image.InnerText = data.image;
        child.AppendChild(image);
        XmlElement point = xmlDoc.CreateElement("Point");
        point.InnerText = data.point.ToString();
        child.AppendChild(point);
        XmlElement clearPoint = xmlDoc.CreateElement("ClearPoint");
        clearPoint.InnerText = data.clearPoint.ToString();
        child.AppendChild(clearPoint);
        XmlElement clearType = xmlDoc.CreateElement("ClearType");
        clearType.InnerText = data.clearType.ToString();
        child.AppendChild(clearType);
        XmlElement clear = xmlDoc.CreateElement("Clear");
        clear.InnerText = data.clear.ToString().ToLower();
        child.AppendChild(clear);
        XmlElement enable = xmlDoc.CreateElement("Enable");
        enable.InnerText = data.clear.ToString().ToLower();
        child.AppendChild(enable);
        XmlElement rewardType = xmlDoc.CreateElement("RewardType");
        rewardType.InnerText = data.rewardType.ToString();
        child.AppendChild(rewardType);
        XmlElement rewardItem = xmlDoc.CreateElement("RewardItemId");
        rewardItem.InnerText = data.rewardItemId.ToString();
        child.AppendChild(rewardItem);
        XmlElement rewardItemCount = xmlDoc.CreateElement("RewardItemCount");
        rewardItemCount.InnerText = data.rewardItemCount.ToString();
        rewardItemCount.AppendChild(rewardItemCount);

        // 암호화/////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }

    public static void CreateNodes(List<Mission> data, XmlDocument xmlDoc, string path)
    {
        for(var i = 0; i < data.Count; i++)
        {
            // 자식 노드 생성
            XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Mission", string.Empty);
            xmlDoc.DocumentElement.FirstChild.AppendChild(child);

            XmlAttribute id = xmlDoc.CreateAttribute("id");
            id.Value = data[i].id.ToString();
            child.Attributes.Append(id);

            // 자식 노드에 들어갈 속성 생성
            XmlElement name = xmlDoc.CreateElement("Name");
            name.InnerText = data[i].name;
            child.AppendChild(name);
            XmlElement description = xmlDoc.CreateElement("Description");
            description.InnerText = data[i].description;
            child.AppendChild(description);
            XmlElement type = xmlDoc.CreateElement("MissionType");
            type.InnerText = data[i].missionType.ToString();
            child.AppendChild(type);
            XmlElement level = xmlDoc.CreateElement("MissionLevel");
            level.InnerText = data[i].missionLevel.ToString();
            child.AppendChild(level);
            XmlElement image = xmlDoc.CreateElement("Image");
            image.InnerText = data[i].image;
            child.AppendChild(image);
            XmlElement point = xmlDoc.CreateElement("Point");
            point.InnerText = data[i].point.ToString();
            child.AppendChild(point);
            XmlElement clearPoint = xmlDoc.CreateElement("ClearPoint");
            clearPoint.InnerText = data[i].clearPoint.ToString();
            child.AppendChild(clearPoint);
            XmlElement clearType = xmlDoc.CreateElement("ClearType");
            clearType.InnerText = data[i].clearType.ToString();
            child.AppendChild(clearType);
            XmlElement clear = xmlDoc.CreateElement("Clear");
            clear.InnerText = data[i].clear.ToString().ToLower();
            child.AppendChild(clear);
            XmlElement enable = xmlDoc.CreateElement("Enable");
            enable.InnerText = data[i].enable.ToString().ToLower();
            child.AppendChild(enable);
            XmlElement rewardType = xmlDoc.CreateElement("RewardType");
            rewardType.InnerText = data[i].rewardType.ToString();
            child.AppendChild(rewardType);
            XmlElement rewardItem = xmlDoc.CreateElement("RewardItemId");
            rewardItem.InnerText = data[i].rewardItemId.ToString();
            child.AppendChild(rewardItem);
            XmlElement rewardItemCount = xmlDoc.CreateElement("RewardItemCount");
            rewardItemCount.InnerText = data[i].rewardItemCount.ToString();
            child.AppendChild(rewardItemCount);
        }
        // 암호화/////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }
}