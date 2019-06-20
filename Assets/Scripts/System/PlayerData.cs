using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public int exp;
    public int coin;
    public int blackCrystal;
    public int portalEnergy;
    public int abilityCount;
    public int stageNumber;
    public int stageDetailNumber;
    public int[] stageHeros;
    public int[] lobbyHeros;

    public int flatEnergyChargingSpeedLevel;
    public int flatEnergyMaxLevel;
    public int addMoneyLevel;
    public int addExpLevel;
    public int addAttackLevel;
    public int addDefenceLevel;

    public string name;

    public PlayerData()
    {
        // Start integer //
        level = User.level;
        exp = User.exp;
        coin = User.coin;
        blackCrystal = User.blackCrystal;
        portalEnergy = User.portalEnergy;
        abilityCount = User.abilityCount;
        stageNumber = User.stageNumber;
        stageDetailNumber = User.stageDetailNumber;
        stageHeros = User.stageHeros;
        lobbyHeros = User.lobbyHeros;
        // End integer //

        // Start Lab//
        flatEnergyChargingSpeedLevel = User.flatEnergyChargingSpeedLevel;
        flatEnergyMaxLevel = User.flatEnergyMaxLevel;
        addMoneyLevel = User.addMoneyLevel;
        addExpLevel = User.addExpLevel;
        addAttackLevel = User.addAttackLevel;
        addDefenceLevel = User.addDefenceLevel;
        // End Lab//

        name = User.name;

    }
}
