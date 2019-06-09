using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class Item
{
    [XmlAttribute("id")]
    public int id;

    [XmlElement("Name")]
    public string name;

    [XmlElement("Description")]
    public string description;

    [XmlElement("ItemType")]
    public int itemtype;

    [XmlElement("WeaponType")]
    public int weapontype;

    [XmlElement("Attack")]
    public int attack;

    [XmlElement("Defence")]
    public int defence;

    [XmlElement("Value")]
    public int value;

    [XmlElement("Image")]
    public string image;

    [XmlElement("Enable")]
    public bool enable;

    [XmlElement("Count")]
    public int count;

    [XmlElement("DropRate")]
    public int droprate;
}
