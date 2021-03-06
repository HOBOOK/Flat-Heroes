﻿using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;

public class Mission
{
    [XmlAttribute("id")]
    public int id;

    [XmlElement("MissionType")]
    public int missionType;

    [XmlElement("MissionLevel")]
    public int missionLevel;

    [XmlElement("Point")]
    public int point;

    [XmlElement("ClearPoint")]
    public int clearPoint;

    [XmlElement("ClearType")]
    public int clearType;

    [XmlElement("Clear")]
    public bool clear;

    [XmlElement("Enable")]
    public bool enable;

    [XmlElement("RewardType")]
    public int rewardType;

    [XmlElement("RewardItemId")]
    public int rewardItemId;

    [XmlElement("RewardItemCount")]
    public int rewardItemCount;
}
