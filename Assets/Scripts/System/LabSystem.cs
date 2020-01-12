using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabSystem
{
    public static int[] levels;

    public static int GetLapLevel(int type, int plusLevel = 0)
    {
        switch (type)
        {
            case 0:
                return User.flatEnergyMaxLevel + plusLevel;
            case 1:
                return User.flatEnergyChargingLevel + plusLevel;
            case 2:
                return User.addMoneyLevel + plusLevel;
            case 3:
                return User.addExpLevel + plusLevel;
            case 4:
                return User.addAttackLevel + plusLevel;
            case 5:
                return User.addDefenceLevel + plusLevel;
            default:
                return 0;
        }
    }
    public static void SetLapLevelUp(int type)
    {
        switch (type)
        {
            case 0:
                User.flatEnergyMaxLevel += 1;
                break;
            case 1:
                User.flatEnergyChargingLevel += 1;
                break;
            case 2:
                User.addMoneyLevel += 1;
                break;
            case 3:
                User.addExpLevel += 1;
                break;
            case 4:
                User.addAttackLevel += 1;
                break;
            case 5:
                User.addDefenceLevel += 1;
                break;
        }
        SaveSystem.SavePlayer();
    }
    public static int GetNeedMoney(int level)
    {
        return level * level * 100 + 1000;
    }
    public static float GetLapPower(int type, int level)
    {
        switch(type)
        {
            case 0:
                return GetMaxEnergy(level);
            case 1:
                return GetChargeEnergy(level);
            case 2:
                return GetAddMoney(level);
            case 3:
                return GetAddExp(level);
            case 4:
                return GetAddAttack(level);
            case 5:
                return GetAddDefence(level);
            default:
                return 0;
        }
    }

    public static int MaxEnergy
    {
        get { return User.flatEnergyMaxLevel * 10 + 100; }
    }
    public static int ChargeEnergy
    {
        get { return User.flatEnergyChargingLevel*2+5; }
    }
    public static int AddMoney
    {
        get { return User.addMoneyLevel * 10; }
    }
    public static int AddExp
    {
        get { return User.addExpLevel * 10; }
    }
    public static int AddAttack
    {
        get { return User.addAttackLevel * 10; }
    }
    public static int AddDefence
    {
        get { return User.addDefenceLevel * 10; }
    }

    public static int GetMaxEnergy(int level)
    {
        return level * 10 + 100;
    }
    public static float GetChargeEnergy(int level)
    {
        return level*2+5;
    }
    public static int GetAddMoney(int level)
    {
        return level * 5;
    }
    public static int GetAddExp(int level)
    {
        return level * 5;
    }
    public static int GetAddAttack(int level)
    {
        return level * 10;
    }
    public static int GetAddDefence(int level)
    {
        return level * 10;
    }

}
