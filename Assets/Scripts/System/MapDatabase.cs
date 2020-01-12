using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

[XmlRoot("MapCollection")]
public class MapDatabase
{
    [XmlArray("Maps"), XmlArrayItem("Map")]
    public List<Map> maps = new List<Map>();

    public static MapDatabase InitSetting()
    {
        string path = Application.persistentDataPath + "/Xml/Map.Xml";
        if (!System.IO.File.Exists(path))
        {
            string folderPath;
            folderPath = Application.persistentDataPath + "/Xml";
            DirectoryInfo di = new DirectoryInfo(folderPath);
            if (di.Exists == false)
            {
                di.Create();
            }
            TextAsset _xml = Resources.Load<TextAsset>("XmlData/Map");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_xml.text);
            if (_xml != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MapDatabase));
                var reader = new StringReader(_xml.text);
                MapDatabase mapDB = serializer.Deserialize(reader) as MapDatabase;
                reader.Close();
                CreateXml(mapDB.maps[0],path);
                Debugging.Log("MapDatabase 최초 생성 성공");
                return mapDB;
            }
        }
        Debugging.Log("MapDatabase 최초 생성 실패");
        return null;
    }
    #region 전체맵정보
    public static MapDatabase Load()
    {
        TextAsset _xml = Resources.Load<TextAsset>("XmlData/Map");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_xml.text);
        if (_xml != null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MapDatabase));
            var reader = new StringReader(_xml.text);
            MapDatabase mapDB = serializer.Deserialize(reader) as MapDatabase;
            reader.Close();
            Debugging.Log("MapDatabase 로드 성공");
            return mapDB;
        }
        return null;
    }
    #endregion
    #region 유저맵정보
    public static string GetMapDataToCloud()
    {
        string path = Application.persistentDataPath + "/Xml/Map.Xml";
        if (System.IO.File.Exists(path))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));
            XmlElement elmRoot = xmlDoc.DocumentElement;
            return elmRoot.InnerText;
        }
        return null;
    }
    public static void SetCloudDataToMap(CloudDataInfo data)
    {
        Debug.Log("서버 MapData 로컬 저장 중");
        SaveCloudData(data.MapData);
    }
    public static void SaveCloudData(string data)
    {
        string path = Application.persistentDataPath + "/Xml/Map.Xml";
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(path))
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));
        else
        {
            InitSetting();
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));
        }

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
    public static MapDatabase LoadUser()
    {
        string path = Application.persistentDataPath + "/Xml/Map.Xml";
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
                XmlSerializer serializer = new XmlSerializer(typeof(MapDatabase));
                var reader = new StringReader(_xml);
                MapDatabase mapDB = serializer.Deserialize(reader) as MapDatabase;
                reader.Close();
                Debugging.Log("MapDatabase 기존 파일 로드");
                return mapDB;
            }
        }
        Debugging.LogSystemWarning("MapDatabase wasn't loaded. >> " + path + " is null. >>");
        return null;
    }
    public static void AddMapClear(Map clearMap, Map openMap)
    {
        string path = Application.persistentDataPath + "/Xml/Map.Xml";
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(path))
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));

        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////
        
        ChangeNode(clearMap, xmlDoc);
        CreateNode(openMap, xmlDoc, path);
    }
    #endregion
    public static void CreateXml(Map data, string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        // Xml을 선언한다(xml의 버전과 인코딩 방식을 정해준다.)
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        // 루트 노드 생성
        XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "MapCollection", string.Empty);
        xmlDoc.AppendChild(root);

        XmlNode root2 = xmlDoc.CreateNode(XmlNodeType.Element, "Maps", string.Empty);
        root.AppendChild(root2);

        // 자식 노드 생성
        XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Map", string.Empty);
        root2.AppendChild(child);

        XmlAttribute id = xmlDoc.CreateAttribute("id");
        id.Value = data.id.ToString();
        child.Attributes.Append(id);

        // 자식 노드에 들어갈 속성 생성
        XmlElement clearPoint = xmlDoc.CreateElement("ClearPoint");
        clearPoint.InnerText = data.clearPoint.ToString();
        child.AppendChild(clearPoint);
        XmlElement stageNumber = xmlDoc.CreateElement("StageNumber");
        stageNumber.InnerText = data.stageNumber.ToString();
        child.AppendChild(stageNumber);

        // 암호화/////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }
    public static void ChangeNode(Map data, XmlDocument xmlDoc)
    {
        XmlNodeList nodes = xmlDoc.SelectNodes("MapCollection/Maps/Map");
        foreach (XmlNode node in nodes)
        {
            if (node.Attributes.GetNamedItem("id").Value == data.id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(data.id.ToString()))
            {
                node.SelectSingleNode("ClearPoint").InnerText = data.clearPoint.ToString();
                break;
            }
        }
    }
    public static void CreateNode(Map data, XmlDocument xmlDoc, string path)
    {
        if(data!=null)
        {
            // 자식 노드 생성
            XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Map", string.Empty);
            xmlDoc.DocumentElement.FirstChild.AppendChild(child);

            XmlAttribute id = xmlDoc.CreateAttribute("id");
            id.Value = data.id.ToString();
            child.Attributes.Append(id);

            // 자식 노드에 들어갈 속성 생성
            XmlElement clearPoint = xmlDoc.CreateElement("ClearPoint");
            clearPoint.InnerText = data.clearPoint.ToString();
            child.AppendChild(clearPoint);
            XmlElement stageNumber = xmlDoc.CreateElement("StageNumber");
            stageNumber.InnerText = data.stageNumber.ToString();
            child.AppendChild(stageNumber);
        }

        // 암호화/////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }
}
