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
                CreateXml(heroDB.heros[0],path);
                Debugging.Log("HeroDatabase 최초 생성 성공");
                return heroDB;
            }
        }
        Debugging.Log("HeroDatabase 최초 생성 실패");
        return null;
    }
    #region 전체히어로정보
    public static HeroDatabase Load()
    {
        TextAsset _xml = Resources.Load<TextAsset>("XmlData/Heros");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_xml.text);
        if (_xml != null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HeroDatabase));
            var reader = new StringReader(_xml.text);
            HeroDatabase heroDB = serializer.Deserialize(reader) as HeroDatabase;
            reader.Close();

            Debugging.Log("HeroDatabase 로드 성공");
            return heroDB;
        }
        return null;
    }
    #endregion
    #region 유저히어로정보
    public static HeroDatabase LoadUser()
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
                Debugging.Log(decrpytData);
                Debugging.Log("HeroDatabase 유저 파일 로드");
                return heroDB;
            }
        }
        Debugging.LogSystemWarning("HeroDatabase wasn't loaded. >> " + path + " is null. >>");
        return null;
    }
    public static void AddUser(int id)
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
        CreateNode(HeroSystem.GetHero(id), xmlDoc, path);
    }
    public static void SaveUser(int id)
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
                    node.SelectSingleNode("Exp").InnerText = hd.exp.ToString();
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
    public static void SaveAllUser(List<HeroData> herodatas)
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
    #endregion

    public static void CreateXml(HeroData data, string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        // Xml을 선언한다(xml의 버전과 인코딩 방식을 정해준다.)
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        // 루트 노드 생성
        XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "HeroCollection", string.Empty);
        xmlDoc.AppendChild(root);

        XmlNode root2 = xmlDoc.CreateNode(XmlNodeType.Element, "Heros", string.Empty);
        root.AppendChild(root2);

        // 자식 노드 생성
        XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Hero", string.Empty);
        root2.AppendChild(child);

        XmlAttribute id = xmlDoc.CreateAttribute("id");
        id.Value = data.id.ToString();
        child.Attributes.Append(id);

        // 자식 노드에 들어갈 속성 생성
        XmlElement name = xmlDoc.CreateElement("Name");
        name.InnerText = data.name;
        child.AppendChild(name);

        XmlElement image = xmlDoc.CreateElement("Image");
        image.InnerText = data.image;
        child.AppendChild(image);
        XmlElement enable = xmlDoc.CreateElement("Enable");
        enable.InnerText = data.enable.ToString().ToLower();
        child.AppendChild(enable);
        XmlElement type = xmlDoc.CreateElement("Type");
        type.InnerText = data.type.ToString();
        child.AppendChild(type);
        XmlElement description = xmlDoc.CreateElement("Description");
        description.InnerText = data.description;
        child.AppendChild(description);
        XmlElement level = xmlDoc.CreateElement("Level");
        level.InnerText = data.level.ToString();
        child.AppendChild(level);
        XmlElement exp = xmlDoc.CreateElement("Exp");
        exp.InnerText = data.exp.ToString();
        child.AppendChild(exp);

        // 암호화/////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }
    public static void CreateNode(HeroData data, XmlDocument xmlDoc, string path)
    {
        // 자식 노드 생성
        XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Hero", string.Empty);
        xmlDoc.DocumentElement.FirstChild.AppendChild(child);

        XmlAttribute id = xmlDoc.CreateAttribute("id");
        id.Value = data.id.ToString();
        child.Attributes.Append(id);

        // 자식 노드에 들어갈 속성 생성
        XmlElement name = xmlDoc.CreateElement("Name");
        name.InnerText = data.name;
        child.AppendChild(name);

        XmlElement image = xmlDoc.CreateElement("Image");
        image.InnerText = data.image;
        child.AppendChild(image);
        XmlElement enable = xmlDoc.CreateElement("Enable");
        enable.InnerText = data.enable.ToString().ToLower();
        child.AppendChild(enable);
        XmlElement type = xmlDoc.CreateElement("Type");
        type.InnerText = data.type.ToString();
        child.AppendChild(type);
        XmlElement description = xmlDoc.CreateElement("Description");
        description.InnerText = data.description;
        child.AppendChild(description);
        XmlElement level = xmlDoc.CreateElement("Level");
        level.InnerText = data.level.ToString();
        child.AppendChild(level);
        XmlElement exp = xmlDoc.CreateElement("Exp");
        exp.InnerText = data.exp.ToString();
        child.AppendChild(exp);

        // 암호화/////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }
}
