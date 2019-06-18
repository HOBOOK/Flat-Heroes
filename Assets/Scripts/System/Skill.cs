using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class Skill
{
    [XmlAttribute("id")]
    public int id;

    [XmlElement("Name")]
    public string name;

    [XmlElement("Description")]
    public string description;

    [XmlElement("Image")]
    public string image;

    [XmlElement("Level")]
    public int level;

    [XmlElement("SkillType")]
    public int skillType;

    [XmlElement("TargetType")]
    public int targetType;

    [XmlElement("Power")]
    public int power;

    [XmlElement("AddPower")]
    public int addPower;

    [XmlElement("Energy")]
    public int energy;

}

