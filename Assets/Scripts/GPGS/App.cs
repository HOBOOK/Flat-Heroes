
////using GooglePlayGames;
////using GooglePlayGames.BasicApi.SavedGame;
//using Newtonsoft.Json;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using UnityEngine;

//public class App : MonoBehaviour
//{
//    public static App Instance;
//    //public GoogleCloudManager gpgsManager;
//    public CloudDataInfo gameInfo;
//    public GoogleSignManager googleManager;

//    private void Awake()
//    {
//        App.Instance = this;
//    }

//    public void StartLogin()
//    {
//        googleManager.OnSignIn();
//    }

//    private void Init(bool isCloudLoad = false)
//    {
//        var path = Application.persistentDataPath + "/CloudDataInfo.bin";


//        Debug.LogFormat("Exists: {0}", File.Exists(path));

//        if (File.Exists(path))
//        {
//            this.googleManager.LoadData();
//        }
//        else
//        {
//            this.gameInfo = new CloudDataInfo();
//            this.gameInfo.SetDataToCloud("", DateTime.Now.ToString());
//            if (!gameInfo.IsNullData())
//            {
//                var json = JsonConvert.SerializeObject(gameInfo);
//                byte[] bytes = Encoding.UTF8.GetBytes(json);
//                File.WriteAllBytes(path, bytes);
//                Debugging.Log(json);
//            }
//            //PlayerPrefs.SetString(FILE_NAME, JsonConvert.SerializeObject(App.Instance.gameInfo));
//        }
//        UI_StartManager.instance.ShowStartUI(isCloudLoad);
//    }
//    public CloudDataInfo SaveData()
//    {
//        this.gameInfo = new CloudDataInfo();
//        this.gameInfo.SetDataToCloud(User.name, DateTime.Now.ToString());
//        return this.gameInfo;
//    }

//    public void SetCloudDataToLocal()
//    {
//        if (this.gameInfo != null)
//        {
//            SaveSystem.SetCloudDataToUser(gameInfo);
//            ItemDatabase.SetCloudDataToItem(gameInfo);
//            HeroDatabase.SetCloudDataToHero(gameInfo);
//            AbilityDatabase.SetCloudDataToAbility(gameInfo);
//            SkillDatabase.SetCloudDataToSkill(gameInfo);
//            MissionDatabase.SetCloudDataToMission(gameInfo);
//            MapDatabase.SetCloudDataToMap(gameInfo);
//        }
//        else
//        {
//            Debugging.LogWarning("클라우드 데이터 로컬저장중 오류 발생");
//        }
//    }
//}
