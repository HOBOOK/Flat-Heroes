using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;

public static class SaveSystem
{
    public static void SavePlayer()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData();
        try
        {
            if(data.level!=0)
                formatter.Serialize(stream, data);
        }
        catch(SerializationException e)
        {
            Debugging.LogError("유저 데이터 저장에 실패 > " + e.Message);
            throw;
        }
        finally
        {
            stream.Close();
        }
        Debugging.LogSystem("File is saved in Successfully.");
    }
    public static void SaveCloudPlayer(string cloudData)
    {
        LoadPlayer();
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = JsonConvert.DeserializeObject<PlayerData>(cloudData);
        try
        {
            if (data.level != 0)
            {
                formatter.Serialize(stream, data);
            }
        }
        catch (SerializationException e)
        {
            Debugging.LogError("유저 클라우드 데이터 저장에 실패 > " + e.Message);
            throw;
        }
        finally
        {
            stream.Close();
            Debugging.LogSystem(data.name + " 의 클라우드 데이터 성공"); ;
        }
    }
    public static string GetUserDataToCloud()
    {
        PlayerData userData = new PlayerData();
        string dataStream = JsonConvert.SerializeObject(userData);
        if(!string.IsNullOrEmpty(dataStream))
        {
            string encrpytData = DataSecurityManager.EncryptData(dataStream);
            return encrpytData;
        }
        return null;
    }
    public static void SetCloudDataToUser(CloudDataInfo data)
    {
        if (!string.IsNullOrEmpty(data.UserData))
        {
            Debug.Log("서버 UserData 로컬 저장 중");
            string decryptData = DataSecurityManager.DecryptData(data.UserData);
            SaveCloudPlayer(decryptData);
        }
    }
    public static void LoadPlayer()
    {
        int loadTryCount = 0;
        string path = Application.persistentDataPath + "/player.fun";
        PlayerData data = null;
        while(loadTryCount<3)
        {
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                data = formatter.Deserialize(stream) as PlayerData;
                stream.Close();
                break;
            }
            else
            {
                loadTryCount++;
            }
        }
        if(data!=null)
        {
            User.isPlaying = false;

            if (!string.IsNullOrEmpty(Common.GoogleUserId))
                User.id = Common.GoogleUserId;
            else
                User.id = data.id;
            User.level = data.level;
            User.exp = data.exp;
            User.coin = data.coin;
            User.blackCrystal = data.blackCrystal;
            if (data.portalEnergy < 0) data.portalEnergy = 0;  User.portalEnergy = data.portalEnergy;
            User.abilityCount = data.abilityCount;
            User.stageNumber = data.stageNumber;
            User.stageDetailNumber = data.stageDetailNumber;
            User.stageHeros = data.stageHeros;
            User.lobbyHeros = data.lobbyHeros;

            User.flatEnergyChargingLevel = data.flatEnergyChargingLevel;
            User.flatEnergyMaxLevel = data.flatEnergyMaxLevel;
            User.addMoneyLevel = data.addMoneyLevel;
            User.addExpLevel = data.addExpLevel;
            User.addAttackLevel = data.addAttackLevel;
            User.addDefenceLevel = data.addDefenceLevel;
            User.addMaxDamageLevel = data.addMaxDamageLevel;
            User.gachaSeed = data.gachaSeed;
            if (data.playerSkill == null) data.playerSkill = new int[2];
            User.playerSkill = data.playerSkill;
            if (data.inventoryCount < 100) data.inventoryCount = 100; else if (data.inventoryCount > 500) data.inventoryCount = 500;
            User.inventoryCount = data.inventoryCount;
            if (data.profileHero < 100) data.profileHero = 101;
            User.profileHero = data.profileHero;
            User.InfinityRankPoint = data.InfinityRankPoint;
            User.magicStone = data.magicStone;
            if (data.magicStoneStack < 0) data.magicStoneStack = 0; User.magicStoneStack = data.magicStoneStack;
            if (data.labLevel < 1) data.labLevel = 1; User.labLevel = data.labLevel;
            User.tutorialSequence = data.tutorialSequence;
            User.isSpeedGame = data.isSpeedGame;
            User.HeroRankPoint = data.HeroRankPoint;
            User.battleRankPoint = data.battleRankPoint;
            User.battleWin = data.battleWin;
            User.battleLose = data.battleLose;
            if (data.battleHeros == null) data.battleHeros = new int[5]; User.battleHeros = data.battleHeros;
            User.transcendenceStone = data.transcendenceStone;
            User.attackRankPoint = data.attackRankPoint;
            User.statsPoint = data.statsPoint;

            User.name = data.name;
            if (string.IsNullOrEmpty(data.language)) data.language = "ko"; User.language = data.language;
            if (string.IsNullOrEmpty(data.eventSaveDate)) data.eventSaveDate = "0";  User.eventSaveDate = data.eventSaveDate;
            if (string.IsNullOrEmpty(data.labProgressDate)) data.labProgressDate = DateTime.Now.Ticks.ToString(); User.labProgressDate = data.labProgressDate;
            if (string.IsNullOrEmpty(data.dailySaveDate)) data.dailySaveDate = "0"; User.dailySaveDate = data.dailySaveDate;
            if (string.IsNullOrEmpty(data.postItems)) data.postItems = ""; User.postItems = data.postItems;
            if (string.IsNullOrEmpty(data.bossClearTime)) data.bossClearTime = InitBossClearTime(); User.bossClearTime = data.bossClearTime;
            if (string.IsNullOrEmpty(data.premiumPassDate)) data.premiumPassDate = InitPremiumPassDate(); User.premiumPassDate = data.premiumPassDate;
            //결제상품변수
            User.isAdsRemove = data.isAdsRemove;
            User.isAdsSkip = data.isAdsSkip;
            if (data.paymentItem == null) data.paymentItem = ""; User.paymentItem = data.paymentItem;
            //Debugging.LogSystem("File is loaded Successfully >> Try : " + loadTryCount + "\r\n" + JsonUtility.ToJson(data));

            User.isAutoCam = data.isAutoCam;
        }
        else
        {
            Debugging.LogSystemWarning("Save file not fount in " + path);
        }
    }
    public static void InitPlayer(string localId, string name)
    {
        string path = Application.persistentDataPath + "/player.fun";
        if (!File.Exists(path)&&string.IsNullOrEmpty(User.id))
        {
            User.id = localId;
            User.level = 1;
            User.exp = 0;
            User.coin = 10000;
            User.blackCrystal = 100;
            User.portalEnergy = 50;
            User.name = name;
            User.abilityCount = 0;
            User.lobbyHeros = new int[5]; User.lobbyHeros[0] = 101;
            User.stageHeros = new int[5];
            User.flatEnergyChargingLevel = 1;
            User.flatEnergyMaxLevel = 1;
            User.addMoneyLevel = 1;
            User.addExpLevel = 1;
            User.addAttackLevel = 1;
            User.addDefenceLevel = 1;
            User.gachaSeed = UnityEngine.Random.Range(0, 100);
            User.playerSkill = new int[2];
            User.playerSkill[0] = 101;
            User.inventoryCount = 100;
            User.magicStone = 0;
            User.magicStoneStack = 0;
            User.labLevel = 1;
            User.labProgressDate = DateTime.Now.Ticks.ToString();
            User.tutorialSequence = 0;
            User.battleRankPoint = 1000;
            User.transcendenceStone = 0;
            User.statsPoint = 1;
            User.isSpeedGame = true;
            User.isAutoCam = false;
            User.bossClearTime = InitBossClearTime();
            User.premiumPassDate = InitPremiumPassDate();
            User.postItems = string.Format("{0}:{1}:{2}","(8003," + 3 + ",환영합니다! "+User.name+" 님!," + DateTime.Now.ToShortDateString() + ")", "(8004," + 2 + ",다운로드 해주셔서 감사합니다.," + DateTime.Now.ToShortDateString() + ")", "(8005," + 1 + ",공식카페도 방문해주세요^^!," + DateTime.Now.ToShortDateString() + ")");
            SavePlayer();
            Debugging.LogSystem("Init Player");
        }
    }
    public static bool IsAlreadyPurchasePackage(int id)
    {
        Debugging.Log("구매목록 >>>>> " + User.paymentItem);
        string[] purchaseItems = User.paymentItem.Split(',');
        foreach(var item in purchaseItems)
        {
            Debugging.Log("구매목록 살피는중 > " + item);
            if(item.Equals(id)||item==id.ToString())
            {
                Debugging.Log(id + "패키지 이미 구매하였음");
                return true;
            }
        }
        return false;
    }
    public static void ExpUp(int exp)
    {
        if(User.exp + exp >=Common.GetUserNeedExp())
        {
            int departExp = (User.exp + exp) - Common.GetUserNeedExp();
            LevelUp(departExp);
        }
        else
        {
            User.exp += exp;
        }
        SavePlayer();
    }
    public static void LevelUp(int departExp)
    {
        int maxLevel = 200;
        if (User.level < maxLevel)
        {
            User.level += 1;
            User.exp = departExp;
            User.statsPoint += 3;
        }
        else
            Debugging.Log("유저레벨이 MAX입니다.");
    }
    public static void AddUserCoin(int amount)
    {
        User.coin += amount;
        SavePlayer();
    }
    public static void AddUserCrystal(int amount)
    {
        User.blackCrystal += amount;
        SavePlayer();
    }
    public static void AddUserEnergy(int amount)
    {
        User.portalEnergy += amount;
        SavePlayer();
    }
    public static void AddUserMagicStone(int amount)
    {
        User.magicStone += amount;
        SavePlayer();
    }
    public static void AddUserTranscendenceStone(int amount)
    {
        User.transcendenceStone += amount;
        SavePlayer();
    }
    public static void ResetUserStatPoint()
    {
        User.statsPoint = (User.level-1)*3;
        SavePlayer();
    }
    public static void UseUserStatPoint(int count=1)
    {
        User.statsPoint = Mathf.Clamp(User.statsPoint - count, 0, 9999);
        SavePlayer();
    }
    public static void ChangeLanguage(LanguageType languageType)
    {
        switch(languageType)
        {
            case LanguageType.ko: User.language = "ko"; break;
            case LanguageType.en: User.language = "en"; break;
        }
        SavePlayer();
        Debugging.Log(string.Format("언어가 변경되었습니다. {0}", User.language));
    }
    public static void ChangeLanguage(string lang)
    {
        foreach(var str in Enum.GetNames(typeof(LanguageType)))
        {
            if(str.Equals(lang)||str==lang)
            {
                User.language = lang;
                break;
            }
        }
        SavePlayer();
        
        Debugging.Log(string.Format("언어가 변경되었습니다. {0}", User.language));
    }
    public static ulong GetBossClearTime(int stage)
    {
        if (string.IsNullOrEmpty(User.bossClearTime))
            User.bossClearTime = InitBossClearTime();

        string[] clearTimes = User.bossClearTime.Split(',');

        return ulong.Parse(clearTimes[Mathf.Clamp(stage-1,0,clearTimes.Length-1)]);
    }
    public static void SetBossClearTime(int stage, string time)
    {
        if (string.IsNullOrEmpty(User.bossClearTime))
            User.bossClearTime = InitBossClearTime();
        string[] clearTimes = User.bossClearTime.Split(',');
        clearTimes[Mathf.Clamp(stage - 1, 0, clearTimes.Length - 1)] = time;
        StringBuilder saveTimes = new StringBuilder();
        foreach(var t in clearTimes)
        {
            saveTimes.Append(t + ",");
        }
        User.bossClearTime = saveTimes.ToString();
        GoogleSignManager.SaveData();
    }
    public static string InitBossClearTime()
    {
        return "0,0,0,0,0,0,0,0,0,0";
    }
    public static string InitPremiumPassDate()
    {
        return DateTime.Now.AddDays(3).ToShortDateString();
    }
    public static bool IsPremiumPassAble()
    {
        if (!string.IsNullOrEmpty(User.premiumPassDate))
        {
            DateTime userPassDate = DateTime.Parse(User.premiumPassDate);
            DateTime curDate = DateTime.Now;
            if (curDate <= userPassDate)
                return true;
        }
        return false;
    }
    public static string getPremiumRemainingPeriodText()
    {
        string returnPeriodText = LocalizationManager.GetText("PremiumPassExpired");
        if (!string.IsNullOrEmpty(User.premiumPassDate))
        {
            DateTime userPassDate = DateTime.Parse(User.premiumPassDate);
            DateTime curDate = DateTime.Now;
            if(curDate <= userPassDate)
            {
                TimeSpan period = userPassDate - curDate;
                if(period.Days>0)
                {
                    returnPeriodText = User.language=="ko"?  string.Format("{0}일 남음", period.Days) : string.Format("{0}days left", period.Days);
                }
                else if(period.Hours>0)
                {
                    returnPeriodText = User.language == "ko" ? string.Format("{0}시간 남음", period.Hours) : string.Format("{0}hours left", period.Hours);
                }
                else
                {
                    returnPeriodText = User.language == "ko" ? string.Format("{0}분 남음", period.Minutes) : string.Format("{0}min left", period.Minutes);
                }
            }
        }
        return returnPeriodText;
    }
    public static string getPremiumBenefitsText()
    {
        return User.language == "ko" ? string.Format("<size='21'>{0}\r\n\r\n{1}\r\n\r\n{2}</size>", "* 자동진행 가능", "* 코인보상 + 20%", "* 보스 모드 대기시간 20% 단축") : string.Format("<size='21'>{0}\r\n\r\n{1}\r\n\r\n{2}</size>", "* Automatic progression possible", "* Coin Reward + 20%", "* 20% faster boss mode wait time");
    }
}
