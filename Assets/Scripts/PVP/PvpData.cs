using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PvpData
{
    public string name;
    public Sprite profileImage;
    public int rankPoint;
    public int[] playerSkill;
    public int[] battleHeros;
    public string heroData;
    public string abilityData;
    public string itemData;
    public LabData labData;

    PvpData() { }

    public PvpData(string uData, string hData, string aData,string iData)
    {
        PlayerData userData = JsonConvert.DeserializeObject<PlayerData>(DataSecurityManager.DecryptData(uData));
        name = userData.name;
        profileImage = HeroSystem.GetHeroThumbnail(userData.profileHero);
        rankPoint = userData.battleRankPoint;
        if (userData.playerSkill == null) userData.playerSkill = new int[2]; playerSkill = userData.playerSkill;
        if (userData.battleHeros == null) userData.battleHeros = userData.stageHeros; battleHeros = userData.stageHeros;
        if(battleHeros==null)
        {
            battleHeros = new int[5];
            battleHeros[0] = 101;
        }
        heroData = string.Format("{0}\n{1}\n{2}\n{3}", "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>", "<HeroCollection>", hData, "</HeroCollection>");
        abilityData = string.Format("{0}\n{1}\n{2}\n{3}", "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>", "<AbilityCollection>", aData, "</AbilityCollection>");
        itemData = string.Format("{0}\n{1}\n{2}\n{3}", "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>", "<ItemCollection>", iData, "</ItemCollection>");
        labData = new LabData(userData.flatEnergyMaxLevel, userData.flatEnergyChargingLevel, userData.addAttackLevel, userData.addDefenceLevel, userData.addMaxDamageLevel);
    }

    public class LabData
    {
        public int flatEnergyData;
        public int flatEnergyChargeData;
        public int addAttack;
        public int addDefence;
        public int addMaxDamage;

        LabData() { }
        public LabData(int nFlatEnergy, int nFlatChargeEnergy, int nAttack, int nDefence, int nMaxDam)
        {
            flatEnergyData = nFlatEnergy;
            flatEnergyChargeData = nFlatChargeEnergy;
            addAttack = nAttack;
            addDefence = nDefence;
            addMaxDamage = nMaxDam;
        }
    }
}
