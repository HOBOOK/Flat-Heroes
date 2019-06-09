using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

[XmlRoot("HeroCollection")]
public class HeroDatabase
{
    [XmlArray("Heros"), XmlArrayItem("Hero")]
    public List<HeroData> heros = new List<HeroData>();

    public static HeroDatabase InitSetting()
    {
        string path = Application.persistentDataPath + "/Xml/Heros.Xml";
        if (!System.IO.File.Exists(path))
        {
            string folderPath;
            folderPath = Application.persistentDataPath + "/Xml";
            DirectoryInfo di = new DirectoryInfo(folderPath);
            if (di.Exists == false)
            {
                di.Create();
            }
            TextAsset _xml = Resources.Load<TextAsset>("XmlData/Heros");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_xml.text);
            if (_xml != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HeroDatabase));
                var reader = new StringReader(_xml.text);
                HeroDatabase heroDB = serializer.Deserialize(reader) as HeroDatabase;
                reader.Close();

                // 암호화/////
                XmlElement elmRoot = xmlDoc.DocumentElement;
                var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
                elmRoot.InnerText = encrpytData;
                ////////////
                xmlDoc.Save(path);

                Debugging.Log("HeroDatabase 최초 생성 성공");
                return heroDB;
            }
        }
        Debugging.Log("HeroDatabase 최초 생성 실패");
        return null;
    }

    public static HeroDatabase Load()
    {
        string path = Application.persistentDataPath + "/Xml/Heros.Xml";
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
                XmlSerializer serializer = new XmlSerializer(typeof(HeroDatabase));
                var reader = new StringReader(_xml);
                HeroDatabase heroDB = serializer.Deserialize(reader) as HeroDatabase;
                reader.Close();
                Debugging.Log("HeroDatabase 기존 파일 로드");
                return heroDB;
            }
        }
        Debugging.LogSystemWarning("HeroDatabase wasn't loaded. >> " + path + " is null. >>");
        return null;
    }

    public static void Save(int id)
    {
        string path = Application.persistentDataPath + "/Xml/Heros.Xml";
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(path))
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));

        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////

        XmlNodeList nodes = xmlDoc.SelectNodes("HeroCollection/Heros/Hero");
        foreach(XmlNode node in nodes)
        {
            if(node.Attributes.GetNamedItem("id").Value==id.ToString()|| node.Attributes.GetNamedItem("id").Value.Equals(id.ToString()))
            {
                HeroData hd = HeroSystem.GetHero(id);
                if(hd!=null)
                {
                    node.SelectSingleNode("Level").InnerText = hd.level.ToString();
                    if(node.SelectSingleNode("Enable").InnerText.Equals("false"))
                        node.SelectSingleNode("Enable").InnerText = hd.enable.ToString().ToLower();
                }

                break;
            }
        }
        // 암호화/////
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
        Debugging.Log(id + " 영웅의 단일 xml 저장 완료");
    }

    public static void SaveAll(List<HeroData> herodatas)
    {
        string path = Application.persistentDataPath + "/Xml/Heros.Xml";
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(path))
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));
        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////

        XmlNodeList nodes = xmlDoc.SelectNodes("HeroCollection/Heros/Hero");
        foreach (XmlNode node in nodes)
        {
           for(int i = 0; i<herodatas.Count;i++)
            {
                if (node.Attributes.GetNamedItem("id").Value == herodatas[i].id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(herodatas[i].id.ToString()))
                {
                    node.SelectSingleNode("Level").InnerText = herodatas[i].level.ToString();
                    node.SelectSingleNode("Exp").InnerText = herodatas[i].exp.ToString();
                    break;
                }
            }
        }
        // 암호화/////
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
        Debugging.Log(herodatas.Count + " 영웅들의 일괄 xml 저장 완료");
    }
}
