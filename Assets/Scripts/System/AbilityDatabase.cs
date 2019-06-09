using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("AbilityCollection")]
public class AbilityDatabase
{
    [XmlArray("Abilitys"), XmlArrayItem("Ability")]
    public List<Ability> abilities = new List<Ability>();

    public static AbilityDatabase InitSetting()
    {
        string path = Application.persistentDataPath + "/Xml/Ability.Xml";
        if (!System.IO.File.Exists(path))
        {
            string folderPath;
            folderPath = Application.persistentDataPath + "/Xml";
            DirectoryInfo di = new DirectoryInfo(folderPath);
            if (di.Exists == false)
            {
                di.Create();
            }
            TextAsset _xml = Resources.Load<TextAsset>("XmlData/Ability");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_xml.text);
            if (_xml != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AbilityDatabase));
                var reader = new StringReader(_xml.text);
                AbilityDatabase abilityDB = serializer.Deserialize(reader) as AbilityDatabase;
                reader.Close();

                // 암호화/////
                XmlElement elmRoot = xmlDoc.DocumentElement;
                var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
                elmRoot.InnerText = encrpytData;
                ////////////
                xmlDoc.Save(path);
                Debugging.Log("AbilityDatabase 최초 생성 성공");
                return abilityDB;
            }
        }
        Debugging.Log("AbilityDatabase 최초 생성 실패");
        return null;
    }

    public static AbilityDatabase Load()
    {
        string path = Application.persistentDataPath + "/Xml/Ability.Xml";
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
                XmlSerializer serializer = new XmlSerializer(typeof(AbilityDatabase));
                var reader = new StringReader(_xml);
                AbilityDatabase abilityDB = serializer.Deserialize(reader) as AbilityDatabase;
                reader.Close();
                Debugging.Log("AbilityDatabase 기존 파일 로드");
                return abilityDB;
            }
        }
        Debugging.LogSystemWarning("AbilityDatabase wasn't loaded. >> " + path + " is null. >>");
        return null;
    }

    public static void AbilityObtainSave(int id)
    {
        string path = Application.persistentDataPath + "/Xml/Ability.Xml";
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(path))
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));

        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////

        XmlNodeList nodes = xmlDoc.SelectNodes("AbilityCollection/Abilitys/Ability");
        bool isSuccess = false;
        foreach (XmlNode node in nodes)
        {
            if (node.Attributes.GetNamedItem("id").Value == id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(id.ToString()))
            {
                node.SelectSingleNode("Enable").InnerText = AbilitySystem.GetAbility(id).enable.ToString().ToLower();
                node.SelectSingleNode("Level").InnerText = AbilitySystem.GetAbility(id).level.ToString();
                isSuccess = true;
                break;
            }
        }
        if(isSuccess)
        {
            // 암호화/////
            var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
            elmRoot.InnerText = encrpytData;
            ////////////
            xmlDoc.Save(path);
            Debugging.Log(id + " 의 어빌리티 데이터 " + AbilitySystem.GetAbility(id).level + "레벨 > xml 저장 완료");
        }
        else
        {
            Debugging.LogWarning(id + "의 어빌리티 XML 데이터 저장실패.");
        }

    }
}