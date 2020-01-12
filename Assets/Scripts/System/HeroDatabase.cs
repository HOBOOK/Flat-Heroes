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
    public static string GetHeroDataToCloud()
    {
        string path = Application.persistentDataPath + "/Xml/Heros.Xml";
        if (System.IO.File.Exists(path))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));
            XmlElement elmRoot = xmlDoc.DocumentElement;
            return elmRoot.InnerText;
        }
        return null;
    }
    public static void SetCloudDataToHero(CloudDataInfo data)
    {
        Debug.Log("서버 HeroData 로컬 저장 중");
        SaveCloudData(data.HeroData);
    }
    public static void SaveCloudData(string data)
    {
        string path = Application.persistentDataPath + "/Xml/Heros.Xml";
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
                HeroData hd = HeroSystem.GetUserHero(id);
                if(hd!=null)
                {
                    node.SelectSingleNode("Level").InnerText = hd.level.ToString();
                    node.SelectSingleNode("Exp").InnerText = hd.exp.ToString();
                    node.SelectSingleNode("EquipmentItem").InnerText = hd.equipmentItem.ToString();
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
                    node.SelectSingleNode("EquipmentItem").InnerText = herodatas[i].equipmentItem.ToString();
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
        XmlElement enable = xmlDoc.CreateElement("Enable");
        enable.InnerText = data.enable.ToString().ToLower();
        child.AppendChild(enable);
        XmlElement type = xmlDoc.CreateElement("Type");
        type.InnerText = data.type.ToString();
        child.AppendChild(type);
        XmlElement level = xmlDoc.CreateElement("Level");
        level.InnerText = data.level.ToString();
        child.AppendChild(level);
        XmlElement exp = xmlDoc.CreateElement("Exp");
        exp.InnerText = data.exp.ToString();
        child.AppendChild(exp);
        XmlElement equipmentItem = xmlDoc.CreateElement("EquipmentItem");
        equipmentItem.InnerText = data.equipmentItem.ToString();
        child.AppendChild(equipmentItem);
        XmlElement skill = xmlDoc.CreateElement("Skill");
        skill.InnerText = data.skill.ToString();
        child.AppendChild(skill);
        XmlElement skin = xmlDoc.CreateElement("Skin");
        skin.InnerText = data.skin.ToString();
        child.AppendChild(skin);
        XmlElement over = xmlDoc.CreateElement("Over");
        over.InnerText = data.over.ToString();
        child.AppendChild(over);
        XmlElement ability = xmlDoc.CreateElement("Ability");
        ability.InnerText = data.ability.ToString();
        child.AppendChild(ability);
        XmlElement abilityLevel = xmlDoc.CreateElement("AbilityLevel");
        abilityLevel.InnerText = data.abilityLevel.ToString();
        child.AppendChild(abilityLevel);

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
        XmlElement enable = xmlDoc.CreateElement("Enable");
        enable.InnerText = data.enable.ToString().ToLower();
        child.AppendChild(enable);
        XmlElement type = xmlDoc.CreateElement("Type");
        type.InnerText = data.type.ToString();
        child.AppendChild(type);
        XmlElement level = xmlDoc.CreateElement("Level");
        level.InnerText = data.level.ToString();
        child.AppendChild(level);
        XmlElement exp = xmlDoc.CreateElement("Exp");
        exp.InnerText = data.exp.ToString();
        child.AppendChild(exp);
        XmlElement equipmentItem = xmlDoc.CreateElement("EquipmentItem");
        equipmentItem.InnerText = data.equipmentItem.ToString();
        child.AppendChild(equipmentItem);
        XmlElement skill = xmlDoc.CreateElement("Skill");
        skill.InnerText = data.skill.ToString();
        child.AppendChild(skill);
        XmlElement skin = xmlDoc.CreateElement("Skin");
        skin.InnerText = data.skin.ToString();
        child.AppendChild(skin);
        XmlElement over = xmlDoc.CreateElement("Over");
        over.InnerText = data.over.ToString();
        child.AppendChild(over);
        XmlElement ability = xmlDoc.CreateElement("Ability");
        ability.InnerText = data.ability.ToString();
        child.AppendChild(ability);
        XmlElement abilityLevel = xmlDoc.CreateElement("AbilityLevel");
        abilityLevel.InnerText = data.abilityLevel.ToString();
        child.AppendChild(abilityLevel);

        // 암호화/////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }

    public static void SaveOver(int id)
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
            if (node.Attributes.GetNamedItem("id").Value == id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(id.ToString()))
            {
                HeroData hd = HeroSystem.GetUserHero(id);
                if (hd != null)
                {
                    if (node.SelectSingleNode("Over") != null)
                        node.SelectSingleNode("Over").InnerText = hd.over.ToString();
                    else
                        AddNode(hd,xmlDoc);
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
    public static void SaveHeroSkin(int id)
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
            if (node.Attributes.GetNamedItem("id").Value == id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(id.ToString()))
            {
                HeroData hd = HeroSystem.GetUserHero(id);
                if (hd != null)
                {
                    if (node.SelectSingleNode("Skin") != null)
                        node.SelectSingleNode("Skin").InnerText = hd.skin.ToString();
                    else
                        AddSkinNode(hd, xmlDoc);
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

    public static void SaveHeroAbility(int id)
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
            if (node.Attributes.GetNamedItem("id").Value == id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(id.ToString()))
            {
                HeroData hd = HeroSystem.GetUserHero(id);
                if (hd != null)
                {
                    if (node.SelectSingleNode("Ability") != null)
                        node.SelectSingleNode("Ability").InnerText = hd.ability.ToString();
                    else
                        AddAbilityNode(hd, xmlDoc,0,hd.ability);
                    if(node.SelectSingleNode("AbilityLevel")!=null)
                        node.SelectSingleNode("AbilityLevel").InnerText = hd.abilityLevel.ToString();
                    else
                        AddAbilityNode(hd, xmlDoc,1,hd.abilityLevel);
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

    public static void AddNode(HeroData data,XmlDocument xmlDoc)
    {
        XmlNodeList nodes = xmlDoc.SelectNodes("HeroCollection/Heros/Hero");
        foreach (XmlNode node in nodes)
        {
            if (node.Attributes.GetNamedItem("id").Value == data.id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(data.id.ToString()))
            {
                XmlElement over = xmlDoc.CreateElement("Over");
                over.InnerText = data.over.ToString();
                node.AppendChild(over);
                break;
            }
        }
    }
    public static void AddSkinNode(HeroData data, XmlDocument xmlDoc)
    {
        XmlNodeList nodes = xmlDoc.SelectNodes("HeroCollection/Heros/Hero");
        foreach (XmlNode node in nodes)
        {
            if (node.Attributes.GetNamedItem("id").Value == data.id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(data.id.ToString()))
            {
                XmlElement skin = xmlDoc.CreateElement("Skin");
                skin.InnerText = data.skin.ToString();
                node.AppendChild(skin);
                break;
            }
        }
    }

    public static void AddAbilityNode(HeroData data, XmlDocument xmlDoc, int type, int val)
    {
        XmlNodeList nodes = xmlDoc.SelectNodes("HeroCollection/Heros/Hero");
        foreach (XmlNode node in nodes)
        {
            if (node.Attributes.GetNamedItem("id").Value == data.id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(data.id.ToString()))
            {
                if(type==0)
                {
                    XmlElement ability = xmlDoc.CreateElement("Ability");
                    ability.InnerText = val.ToString();
                    node.AppendChild(ability);
                }
                else
                {
                    XmlElement abilityLevel = xmlDoc.CreateElement("AbilityLevel");
                    abilityLevel.InnerText = val.ToString();
                    node.AppendChild(abilityLevel);
                }
                break;
            }
        }
    }
}
