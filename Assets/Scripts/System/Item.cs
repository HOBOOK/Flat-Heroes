using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System;

public class Item : ICloneable
{
    [XmlAttribute("id")]
    public int id;

    [XmlElement("CustomId")]
    public int customId;

    [XmlElement("EquipCharacterId")]
    public int equipCharacterId;

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

    [XmlElement("Hp")]
    public int hp;

    [XmlElement("Critical")]
    public int critical;

    [XmlElement("AttackSpeed")]
    public int attackSpeed;

    [XmlElement("MoveSpeed")]
    public int moveSpeed;

    [XmlElement("SkillEnergy")]
    public int skillEnergy;

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

    [XmlElement("Class")]
    public int itemClass;

    public object Clone()
    {
        Item newItem = new Item();
        newItem.id = this.id;
        newItem.customId = this.customId;
        newItem.equipCharacterId = this.equipCharacterId;
        newItem.name = this.name;
        newItem.description = this.description;
        newItem.itemtype = this.itemtype;
        newItem.weapontype = this.weapontype;
        newItem.attack = this.attack;
        newItem.defence = this.defence;
        newItem.hp = this.hp;
        newItem.critical = this.critical;
        newItem.attackSpeed = this.attackSpeed;
        newItem.moveSpeed = this.moveSpeed;
        newItem.skillEnergy = this.skillEnergy;
        newItem.value = this.value;
        newItem.image = this.image;
        newItem.enable = this.enable;
        newItem.count = this.count;
        newItem.droprate = this.droprate;
        newItem.itemClass = this.itemClass;
        return newItem;
    }
}
