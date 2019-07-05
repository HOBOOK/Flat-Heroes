using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("SkillCollection")]
public class SkillDatabase
{
    [XmlArray("Skills"), XmlArrayItem("Skill")]
    public List<Skill> skills = new List<Skill>();

    public static SkillDatabase InitSetting()
    {
        string path = Application.persistentDataPath + "/Xml/Skill.Xml";
        if (!System.IO.File.Exists(path))
        {
            string folderPath;
            folderPath = Application.persistentDataPath + "/Xml";
            DirectoryInfo di = new DirectoryInfo(folderPath);
            if (di.Exists == false)
            {
                di.Create();
            }
            TextAsset _xml = Resources.Load<TextAsset>("XmlData/Skill");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_xml.text);
            if (_xml != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SkillDatabase));
                var reader = new StringReader(_xml.text);
                SkillDatabase skillDB = serializer.Deserialize(reader) as SkillDatabase;
                reader.Close();
                CreateXml(skillDB.skills[0], path);
                Debugging.Log("SkillDatabase 최초 생성 성공");
                return skillDB;
            }
        }
        Debugging.Log("SkillDatabase 최초 생성 실패");
        return null;
    }
    #region 전체스킬정보
    public static SkillDatabase Load()
    {
        TextAsset _xml = Resources.Load<TextAsset>("XmlData/Skill");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_xml.text);
        if (_xml != null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SkillDatabase));
            var reader = new StringReader(_xml.text);
            SkillDatabase skillDB = serializer.Deserialize(reader) as SkillDatabase;
            reader.Close();
            Debugging.Log("SkillDatabase 로드 성공");
            return skillDB;
        }
        return null;
    }
    #endregion
    #region 유저스킬정보
    public static string GetSkillDataToCloud()
    {
        string path = Application.persistentDataPath + "/Xml/Skill.Xml";
        if (System.IO.File.Exists(path))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));
            XmlElement elmRoot = xmlDoc.DocumentElement;
            return elmRoot.InnerText;
        }
        return null;
    }
    public static void SetCloudDataToSkill(CloudDataInfo data)
    {
        Debug.Log("서버 SkillData 로컬 저장 중");
        SaveCloudData(data.SkillData);
    }
    public static void SaveCloudData(string data)
    {
        string path = Application.persistentDataPath + "/Xml/Skill.Xml";
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
    public static SkillDatabase LoadUser()
    {
        string path = Application.persistentDataPath + "/Xml/Skill.Xml";
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
                XmlSerializer serializer = new XmlSerializer(typeof(SkillDatabase));
                var reader = new StringReader(_xml);
                SkillDatabase skillDB = serializer.Deserialize(reader) as SkillDatabase;
                reader.Close();
                Debugging.Log("SkillDatabase 기존 파일 로드");
                return skillDB;
            }
        }
        Debugging.LogSystemWarning("SkillDatabase wasn't loaded. >> " + path + " is null. >>");
        return null;
    }
    public static void AddSkill(int id)
    {
        string path = Application.persistentDataPath + "/Xml/Skill.Xml";
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(path))
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));

        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////
        CreateNode(SkillSystem.GetSkill(id), xmlDoc, path);
    }
    public static void SaveSkill(int id)
    {
        string path = Application.persistentDataPath + "/Xml/Skill.Xml";
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(path))
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));

        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////

        XmlNodeList nodes = xmlDoc.SelectNodes("SkillCollection/Skills/Skill");
        foreach (XmlNode node in nodes)
        {
            if (node.Attributes.GetNamedItem("id").Value == id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(id.ToString()))
            {
                Skill skill = SkillSystem.GetUserSkill(id);
                if (skill != null)
                {
                    node.SelectSingleNode("Level").InnerText = skill.level.ToString();
                }
                break;
            }
        }
        // 암호화/////
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
        Debugging.Log(id + " 스킬의 단일 xml 저장 완료");
    }
    #endregion
    public static void CreateXml(Skill data, string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        // Xml을 선언한다(xml의 버전과 인코딩 방식을 정해준다.)
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        // 루트 노드 생성
        XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "SkillCollection", string.Empty);
        xmlDoc.AppendChild(root);

        XmlNode root2 = xmlDoc.CreateNode(XmlNodeType.Element, "Skills", string.Empty);
        root.AppendChild(root2);

        XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Skill", string.Empty);
        xmlDoc.DocumentElement.FirstChild.AppendChild(child);

        XmlAttribute id = xmlDoc.CreateAttribute("id");
        id.Value = data.id.ToString();
        child.Attributes.Append(id);
        // 자식 노드에 들어갈 속성 생성
        XmlElement level = xmlDoc.CreateElement("Level");
        level.InnerText = "1";
        child.AppendChild(level);

        // 암호화/////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }
    public static void CreateNode(Skill data, XmlDocument xmlDoc, string path)
    {
        // 자식 노드 생성
        XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Skill", string.Empty);
        xmlDoc.DocumentElement.FirstChild.AppendChild(child);

        XmlAttribute id = xmlDoc.CreateAttribute("id");
        id.Value = data.id.ToString();
        child.Attributes.Append(id);
        // 자식 노드에 들어갈 속성 생성
        XmlElement level = xmlDoc.CreateElement("Level");
        level.InnerText = "1";
        child.AppendChild(level);
        // 암호화/////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }
}