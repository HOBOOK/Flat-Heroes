﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

[XmlRoot("ItemCollection")]
public class ItemDatabase
{
    [XmlArray("Items"), XmlArrayItem("Item")]
    public List<Item> items = new List<Item>();

    public static ItemDatabase InitSetting()
    {
        string path = Application.persistentDataPath + "/Xml/Item.Xml";
        if (!System.IO.File.Exists(path))
        {
            string folderPath;
            folderPath = Application.persistentDataPath + "/Xml";
            DirectoryInfo di = new DirectoryInfo(folderPath);
            if (di.Exists == false)
            {
                di.Create();
            }
            TextAsset _xml = Resources.Load<TextAsset>("XmlData/Item");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_xml.text);
            if (_xml != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ItemDatabase));
                var reader = new StringReader(_xml.text);
                ItemDatabase itemDB = serializer.Deserialize(reader) as ItemDatabase;
                reader.Close();
                CreateXml(path);
                Debugging.Log("ItemDatabase 최초 생성 성공");
                return itemDB;
            }
        }
        Debugging.Log("ItemDatabase 최초 생성 실패");
        return null;
    }
    #region 전체아이템정보
    public static ItemDatabase Load()
    {
        TextAsset _xml = Resources.Load<TextAsset>("XmlData/Item");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_xml.text);
        if (_xml != null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ItemDatabase));
            var reader = new StringReader(_xml.text);
            ItemDatabase itemDB = serializer.Deserialize(reader) as ItemDatabase;
            reader.Close();

            Debugging.Log("ItemDatabase 로드 성공");
            return itemDB;
        }
        Debugging.LogSystemWarning("ItemDatabase wasn't loaded.");
        return null;
    }
    #endregion
    #region 유저아이템정보
    public static ItemDatabase LoadUser()
    {
        string path = Application.persistentDataPath + "/Xml/Item.Xml";
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
                XmlSerializer serializer = new XmlSerializer(typeof(ItemDatabase));
                var reader = new StringReader(_xml);
                ItemDatabase itemDB = serializer.Deserialize(reader) as ItemDatabase;
                reader.Close();
                Debugging.Log("ItemDatabase 기존 파일 로드");
                return itemDB;
            }
        }
        Debugging.LogSystemWarning("ItemDatabase wasn't loaded. >> " + path + " is null. >>");
        return null;
    }
    public static void ItemSave(int id)
    {
        string path = Application.persistentDataPath + "/Xml/Item.Xml";
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(path))
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));

        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////
        ///
        XmlNodeList nodes = xmlDoc.SelectNodes("ItemCollection/Items/Item");
        foreach (XmlNode node in nodes)
        {
            if (node.Attributes.GetNamedItem("id").Value == id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(id.ToString()))
            {
                node.SelectSingleNode("Enable").InnerText = ItemSystem.GetItem(id).enable.ToString().ToLower();
                node.SelectSingleNode("Count").InnerText = ItemSystem.GetItem(id).count.ToString();
                break;
            }
        }
        // 암호화/////
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
        Debugging.Log(id + " 의 아이템 데이터 xml 저장 완료");
    }
    public static void AddItemSave(int id)
    {
        string path = Application.persistentDataPath + "/Xml/Item.Xml";
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(path))
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));

        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////
        CreateNode(ItemSystem.GetItem(id),xmlDoc,path);
        Debugging.Log(id + " 의 아이템 데이터 xml 추가저장 완료");
    }
    public static void DeleteItemSave(int id)
    {
        string path = Application.persistentDataPath + "/Xml/Item.Xml";
        XmlDocument xmlDoc = new XmlDocument();
        if (System.IO.File.Exists(path))
            xmlDoc.LoadXml(System.IO.File.ReadAllText(path));

        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////
        ///
        XmlNodeList nodes = xmlDoc.SelectNodes("ItemCollection/Items/Item");
        foreach (XmlNode node in nodes)
        {
            if (node.Attributes.GetNamedItem("id").Value == id.ToString() || node.Attributes.GetNamedItem("id").Value.Equals(id.ToString()))
            {
                node.RemoveAll();
                break;
            }
        }
        // 암호화/////
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
        Debugging.Log(id + " 의 아이템 데이터 xml 삭제 완료");
    }
    #endregion

    public static void CreateXml(string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        // Xml을 선언한다(xml의 버전과 인코딩 방식을 정해준다.)
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        // 루트 노드 생성
        XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "ItemCollection", string.Empty);
        xmlDoc.AppendChild(root);

        XmlNode root2 = xmlDoc.CreateNode(XmlNodeType.Element, "Items", string.Empty);
        root.AppendChild(root2);

        // 암호화/////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }
    public static void CreateNode(Item data, XmlDocument xmlDoc, string path)
    {
        // 자식 노드 생성
        XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Item", string.Empty);
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
        XmlElement type = xmlDoc.CreateElement("ItemType");
        type.InnerText = data.itemtype.ToString();
        child.AppendChild(type);
        XmlElement wType = xmlDoc.CreateElement("WeaponType");
        wType.InnerText = data.weapontype.ToString();
        child.AppendChild(wType);
        XmlElement attack = xmlDoc.CreateElement("Attack");
        attack.InnerText = data.attack.ToString();
        child.AppendChild(attack);
        XmlElement defence = xmlDoc.CreateElement("Defenece");
        defence.InnerText = data.defence.ToString();
        child.AppendChild(defence);
        XmlElement value = xmlDoc.CreateElement("Value");
        value.InnerText = data.value.ToString();
        child.AppendChild(value);
        XmlElement image = xmlDoc.CreateElement("Image");
        image.InnerText = data.image;
        child.AppendChild(image);
        XmlElement count = xmlDoc.CreateElement("Count");
        count.InnerText = data.count.ToString();
        child.AppendChild(count);
        XmlElement enable = xmlDoc.CreateElement("Enable");
        enable.InnerText = data.enable.ToString().ToLower();
        child.AppendChild(enable);
        XmlElement droprate = xmlDoc.CreateElement("DropRate");
        droprate.InnerText = data.droprate.ToString();
        child.AppendChild(droprate);

        // 암호화/////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
        elmRoot.InnerText = encrpytData;
        ////////////
        xmlDoc.Save(path);
    }
}