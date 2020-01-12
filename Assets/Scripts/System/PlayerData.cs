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

    public int flatEnergyChargingLevel;
    public int flatEnergyMaxLevel;
    public int addMoneyLevel;
    public int addExpLevel;
    public int addAttackLevel;
    public int addDefenceLevel;
    public int gachaSeed;
    public int[] playerSkill;
    public int inventoryCount;
    public int profileHero;
    public int InfinityRankPoint;
    public int magicStone;
    public int magicStoneStack;
    public int labLevel;
    public int tutorialSequence;
    public int HeroRankPoint;
    public int battleRankPoint;
    public int battleWin;
    public int battleLose;
    public int[] battleHeros;
    public int transcendenceStone;
    public int attackRankPoint;
    public int addMaxDamageLevel;
    public int statsPoint;

    public string id;
    public string name;
    public string language;
    public string eventSaveDate;
    public string dailySaveDate;
    public string premiumPassDate;

    public bool isAdsRemove;
    public bool isAdsSkip;
    public string paymentItem;
    public string labProgressDate;
    public string postItems;
    public string bossClearTime;

    public bool isAutoCam;
    public bool isSpeedGame;

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
        gachaSeed = User.gachaSeed;
        playerSkill = User.playerSkill;
        inventoryCount = User.inventoryCount;
        profileHero = User.profileHero;
        InfinityRankPoint = User.InfinityRankPoint;
        magicStone = User.magicStone;
        magicStoneStack = User.magicStoneStack;
        labLevel = User.labLevel;
        tutorialSequence = User.tutorialSequence;
        HeroRankPoint = User.HeroRankPoint;
        battleRankPoint = User.battleRankPoint;
        battleWin = User.battleWin;
        battleLose = User.battleLose;
        battleHeros = User.battleHeros;
        transcendenceStone = User.transcendenceStone;
        attackRankPoint = User.attackRankPoint;
        statsPoint = User.statsPoint;
        // End integer //

        // Start Lab//
        flatEnergyChargingLevel = User.flatEnergyChargingLevel;
        flatEnergyMaxLevel = User.flatEnergyMaxLevel;
        addMoneyLevel = User.addMoneyLevel;
        addExpLevel = User.addExpLevel;
        addAttackLevel = User.addAttackLevel;
        addDefenceLevel = User.addDefenceLevel;
        addMaxDamageLevel = User.addMaxDamageLevel;
        // End Lab//

        id = User.id;
        name = User.name;
        language = User.language;
        eventSaveDate = User.eventSaveDate;
        labProgressDate = User.labProgressDate;
        dailySaveDate = User.dailySaveDate;
        postItems = User.postItems;
        bossClearTime = User.bossClearTime;
        premiumPassDate = User.premiumPassDate;

        // IAP
        isAdsRemove = User.isAdsRemove;
        isAdsSkip = User.isAdsSkip;
        paymentItem = User.paymentItem;
        //

        // Boolean//
        isAutoCam = User.isAutoCam;
        isSpeedGame = User.isSpeedGame;
    }
    public override string ToString()
    {
        return string.Format("Level : {0} Exp : {1} Coin : {2} Crystal : {3} Energy : {4} AbCount : {5} StageNumber : {6} StageDetailNumber : {7} StageHero : {8} LobbyHero : {8} GachaSeed : {9} PlayerSkill : {10} FlatEnergyChargetLevel : {11} FlatEnergyMaxLevel : {12} AddMoneyLevel : {13} AddExpLevel : {14} AddAttackLevel : {15} AddDefenceLevel : {16} Name : {17} Language : {18}",level ,exp,coin,blackCrystal,portalEnergy,abilityCount,stageNumber,stageDetailNumber,stageHeros,lobbyHeros,gachaSeed,playerSkill,flatEnergyChargingLevel,flatEnergyMaxLevel,addMoneyLevel,addExpLevel,addAttackLevel,addDefenceLevel,name,language);
    }
}
