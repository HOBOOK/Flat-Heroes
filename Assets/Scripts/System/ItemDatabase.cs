using System.Collections;
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

                // 암호화/////
                XmlElement elmRoot = xmlDoc.DocumentElement;
                var encrpytData = DataSecurityManager.EncryptData(elmRoot.InnerXml);
                elmRoot.InnerText = encrpytData;
                ////////////
                xmlDoc.Save(path);
                Debugging.Log("ItemDatabase 최초 생성 성공");
                return itemDB;
            }
        }
        Debugging.Log("ItemDatabase 최초 생성 실패");
        return null;
    }

    public static ItemDatabase Load()
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
}
