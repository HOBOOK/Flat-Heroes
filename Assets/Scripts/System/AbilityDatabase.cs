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
                CreateXml(path);
                Debugging.Log("AbilityDatabase 최초 생성 성공");
                return abilityDB;
            }
        }
        Debugging.Log("AbilityDatabase 최초 생성 실패");
        return null;
    }
    #region 전체어빌리티정보
    public static AbilityDatabase Load()
    {
        TextAsset _xml = Resources.Load<TextAsset>("XmlData/Ability");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_xml.text);
        if (_xml != null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AbilityDatabase));
            var reader = new StringReader(_xml.text);
            AbilityDatabase abilityDB = serializer.Deserialize(reader) as AbilityDatabase;
            reader.Close();
            Debugging.Log("AbilityDatabase 로드 성공");
            return abilityDB;
        }
        return null;
    }
    #endregion
    #region 유저 어빌리티정보
    public static AbilityDatabase LoadUser()
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
                Debugging.Log(decrpytData);
                Debugging.Log("AbilityDatabase 기존 파일 로드");
                return abilityDB;
            }
        }
        Debugging.LogSystemWarning("AbilityDatabase wasn't loaded. >> " + path + " is null. >>");
        return null;
    }
    public static void AddAbility(int id)
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
        CreateNode(AbilitySystem.GetAbility(id), xmlDoc, path);
    }
    public static void SaveAbility(int id)
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
        foreach (XmlNode node in nodes)
        {
            if (node.Attributes.GetNamedItem("id").Value == id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(id.ToString()))
            {
                Ability ability = AbilitySystem.GetAbility(id);
                if (ability != null)
                {
                    node.SelectSingleNode("Level").InnerText = ability.level.ToString();
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
    #endregion
    public static void CreateXml(string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        // Xml을 선언한다(xml의 버전과 인코딩 방식을 정해준다.)
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        // 루트 노드 생성
        XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "AbilityCollection", string.Empty);
        xmlDoc.AppendChild(root);

        XmlNode root2 = xmlDoc.CreateNode(XmlNodeType.Element, "Abilitys", string.Empty);
        root.AppendChild(root2);

        // 암호화/////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }
    public static void CreateNode(Ability data, XmlDocument xmlDoc, string path)
    {
        // 자식 노드 생성
        XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Ability", string.Empty);
        xmlDoc.DocumentElement.FirstChild.AppendChild(child);

        XmlAttribute id = xmlDoc.CreateAttribute("id");
        id.Value = data.id.ToString();
        child.Attributes.Append(id);

        // 자식 노드에 들어갈 속성 생성
        XmlElement name = xmlDoc.CreateElement("Name");
        name.InnerText = data.name;
        child.AppendChild(name);
        XmlElement level = xmlDoc.CreateElement("Level");
        level.InnerText = "1";
        child.AppendChild(level);
        XmlElement type = xmlDoc.CreateElement("AbilityType");
        type.InnerText = data.abilityType.ToString();
        child.AppendChild(type);
        XmlElement powerType = xmlDoc.CreateElement("PowerType");
        powerType.InnerText = data.powerType.ToString();
        child.AppendChild(powerType);
        XmlElement power = xmlDoc.CreateElement("Power");
        power.InnerText = data.power.ToString();
        child.AppendChild(power);
        XmlElement image = xmlDoc.CreateElement("Image");
        image.InnerText = data.image;
        child.AppendChild(image);

        // 암호화/////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }
}