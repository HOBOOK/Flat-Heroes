using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class Ability
{
    [XmlAttribute("id")]
    public int id;

    [XmlElement("Name")]
    public string name;

    [XmlElement("Level")]
    public int level;

    [XmlElement("AbilityType")]
    public int abilityType;

    [XmlElement("PowerType")]
    public int powerType;

    [XmlElement("Enable")]
    public bool enable;

    [XmlElement("Power")]
    public int power;

    [XmlElement("Image")]
    public string image;
}

