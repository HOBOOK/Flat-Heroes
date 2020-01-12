using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System;

public class HeroData
{
    [XmlAttribute("id")]
    public int id;

    [XmlElement("Name")]
    public string name;

    [XmlElement("Image")]
    public string image;

    [XmlElement("Enable")]
    public bool enable;

    [XmlElement("Type")]
    public int type;

    [XmlElement("Description")]
    public string description;

    [XmlElement("Level")]
    public int level;

    [XmlElement("Exp")]
    public int exp;

    [XmlElement("Value")]
    public int value;

    [XmlElement("Strength")]
    public int strength;

    [XmlElement("Intelligent")]
    public int intelligent;

    [XmlElement("Physical")]
    public int physical;

    [XmlElement("Agility")]
    public int agility;

    [XmlElement("Chat")]
    public string chat;

    [XmlElement("EquipmentItem")]
    public string equipmentItem;

    [XmlElement("Skill")]
    public int skill;

    [XmlElement("AttackType")]
    public int attackType;

    [XmlElement("Skin")]
    public int skin;

    [XmlElement("Over")]
    public int over;

    [XmlElement("Ability")]
    public int ability;

    [XmlElement("AbilityLevel")]
    public int abilityLevel;
}
