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
                // 암호화/////
                XmlElement elmRoot = xmlDoc.DocumentElement;
                var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
                elmRoot.InnerText = encrpytData;
                ////////////
                xmlDoc.Save(path);
                Debugging.Log("MapDatabase 최초 생성 성공");
                return mapDB;
            }
        }
        Debugging.Log("MapDatabase 최초 생성 실패");
        return null;
    }

    public static MapDatabase Load()
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

    public static void MapClearSave(int id)
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
        bool isPointSet = false, isEnableSet= false;

        XmlNodeList nodes = xmlDoc.SelectNodes("MapCollection/Maps/Map");
        foreach (XmlNode node in nodes)
        {
            if (node.Attributes.GetNamedItem("id").Value == id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(id.ToString()))
            {
                node.SelectSingleNode("ClearPoint").InnerText = MapSystem.GetMap(id).clearPoint.ToString();
                isPointSet = true;
            }
            if(node.Attributes.GetNamedItem("id").Value == (id+1).ToString() || node.Attributes.GetNamedItem("id").Value.Equals((id+1).ToString()))
            {
                node.SelectSingleNode("Enable").InnerText = MapSystem.GetMap((id + 1)).enable.ToString().ToLower();
                isEnableSet = true;
            }
            if (isPointSet && isEnableSet)
                break;
        }
        // 암호화/////
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
        Debugging.Log(id + " 의 맵데이터 xml 저장 완료");
    }

}
