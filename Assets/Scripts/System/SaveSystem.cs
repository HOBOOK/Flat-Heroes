﻿using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;
using System.Text;

public static class SaveSystem
{
    public static void SavePlayer()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData();

        formatter.Serialize(stream, data);
        stream.Close();

        Debugging.LogSystem("File is saved in Successfully.");
    }

    public static void LoadPlayer()
    {
        Common.isDataLoadSuccess = false;
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

            User.level = data.level;
            User.coin = data.coin;
            User.blackCrystal = data.blackCrystal;
            User.portalEnergy = data.portalEnergy;
            User.abilityCount = data.abilityCount;
            User.stageNumber = data.stageNumber;
            User.stageDetailNumber = data.stageDetailNumber;
            User.stageHeros = data.stageHeros;
            User.lobbyHeros = data.lobbyHeros;

            User.name = data.name;
            //Debugging.LogSystem("File is loaded Successfully >> Try : " + loadTryCount + "\r\n" + JsonUtility.ToJson(data));
        }
        else
        {
            Debugging.LogSystemWarning("Save file not fount in " + path);
            InitPlayer();
        }
        Common.isDataLoadSuccess = true;
    }

    public static void InitPlayer()
    {
        User.level = 1;
        User.exp = 0;
        User.coin = 1000;
        User.blackCrystal = 100;
        User.portalEnergy = 5;
        User.name = Common.GetRandomID(6);
        User.abilityCount = 1;
        User.lobbyHeros = new int[5];
        User.stageHeros = new int[5];
        Debugging.LogSystem("Init Player");
    }

    public static void SetUserCrystal(int amount)
    {
        User.blackCrystal += amount;
    }
    public static void SetUserEnergy(int amount)
    {
        User.portalEnergy += amount;
    }
}
