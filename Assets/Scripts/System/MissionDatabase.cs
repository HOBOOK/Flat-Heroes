using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using System.Xml;
using System.IO;

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
                // 암호화/////
                XmlElement elmRoot = xmlDoc.DocumentElement;
                var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
                elmRoot.InnerText = encrpytData;
                ////////////
                xmlDoc.Save(path);
                Debugging.Log("MissionDatabase 최초 생성 성공");
                return missionDB;
            }
        }
        Debugging.Log("MissionDatabase 최초 생성 실패");
        return null;
    }

    public static MissionDatabase Load()
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
                Debugging.Log("MissionDatabase 기존 파일 로드");
                return missionDB;
            }
        }
        Debugging.LogSystemWarning("MissionDatabase wasn't loaded. >> " + path + " is null. >>");
        return null;
    }
}