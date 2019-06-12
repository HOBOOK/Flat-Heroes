using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class Map
{
    [XmlAttribute("id")]
    public int id;

    [XmlElement("Name")]
    public string name;

    [XmlElement("Description")]
    public string description;

    [XmlElement("StageNumber")]
    public int stageNumber;

    [XmlElement("Image")]
    public string image;

    [XmlElement("ClearPoint")]
    public int clearPoint;

}
