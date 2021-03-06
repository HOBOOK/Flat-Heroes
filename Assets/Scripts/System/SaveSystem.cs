﻿using UnityEngine;
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
            User.isDead = false;
            User.isPlaying = false;

            if (!string.IsNullOrEmpty(Common.GoogleUserId))
                User.id = Common.GoogleUserId;
            else
                User.id = data.id;
            User.level = data.level;
            User.exp = data.exp;
            User.coin = data.coin;
            User.blackCrystal = data.blackCrystal;
            User.portalEnergy = data.portalEnergy;
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
            User.gachaSeed = data.gachaSeed;
            if (data.playerSkill == null) data.playerSkill = new int[2];
            User.playerSkill = data.playerSkill;
            if (data.inventoryCount < 100) data.inventoryCount = 100;
            User.inventoryCount = data.inventoryCount;

            User.name = data.name;
            if (data.language == null) data.language = "ko";
            User.language = data.language;

            //Debugging.LogSystem("File is loaded Successfully >> Try : " + loadTryCount + "\r\n" + JsonUtility.ToJson(data));
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
            User.coin = 1000;
            User.blackCrystal = 100;
            User.portalEnergy = 5;
            User.name = name;
            User.abilityCount = 1;
            User.lobbyHeros = new int[5];
            User.stageHeros = new int[5];
            User.flatEnergyChargingLevel = 0;
            User.flatEnergyMaxLevel = 0;
            User.addMoneyLevel = 0;
            User.addExpLevel = 0;
            User.addAttackLevel = 0;
            User.addDefenceLevel = 0;
            User.gachaSeed = UnityEngine.Random.Range(0, 100);
            User.playerSkill = new int[2];
            User.inventoryCount = 100;
            SavePlayer();
            Debugging.LogSystem("Init Player");
        }
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
        int maxLevel = 10;
        if (User.level < maxLevel)
        {
            User.level += 1;
            User.exp = departExp;
        }
        else
            Debugging.Log("유저레벨이 MAX입니다.");
        SavePlayer();
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
}
