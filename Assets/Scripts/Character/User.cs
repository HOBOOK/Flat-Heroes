﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User
{
    [SerializeField]
    public static bool isDead;
    [SerializeField]
    public static bool isPlaying;
    [SerializeField]
    public static int level;
    [SerializeField]
    public static int exp;
    [SerializeField]
    public static int coin;
    [SerializeField]
    public static int blackCrystal;
    [SerializeField]
    public static int portalEnergy;
    [SerializeField]
    public static int abilityCount;
    [SerializeField]
    public static int stageNumber;
    [SerializeField]
    public static int stageDetailNumber;
    [SerializeField]
    public static string name;
    [SerializeField]
    public static int[] stageHeros;
    [SerializeField]
    public static int[] lobbyHeros;
    [SerializeField]
    public static int flatEnergyChargingSpeedLevel;
    [SerializeField]
    public static int flatEnergyMaxLevel;
    [SerializeField]
    public static int addMoneyLevel;
    [SerializeField]
    public static int addExpLevel;
    [SerializeField]
    public static int addAttackLevel;
    [SerializeField]
    public static int addDefenceLevel;
}
