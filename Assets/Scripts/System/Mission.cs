using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;

public class Mission
{
    [XmlAttribute("id")]
    public int id;

    [XmlElement("Name")]
    public string name;

    [XmlElement("Description")]
    public string description;

    [XmlElement("MissionType")]
    public int missionType;

    [XmlElement("MissionLevel")]
    public int missionLevel;

    [XmlElement("Image")]
    public string image;

    [XmlElement("Clear")]
    public bool clear;

    [XmlElement("RewardItemId")]
    public int rewardItemId;

    [XmlElement("RewardItemCount")]
    public int rewardItemCount;
}
