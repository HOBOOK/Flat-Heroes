using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

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
}
