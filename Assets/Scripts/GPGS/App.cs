
//using GooglePlayGames;
//using GooglePlayGames.BasicApi.SavedGame;
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
//    public GoogleCloudManager gpgsManager;
//    public CloudDataInfo gameInfo;

//    private void Awake()
//    {
//        App.Instance = this;
//    }

//    public void StartLogin()
//    {
//        this.gpgsManager.OnSavedGameDataReadComplete = (status, bytes) => {
//            if (status == SavedGameRequestStatus.Success)
//            {
//                string strCloudData = null;

//                if (bytes.Length == 0)
//                {
//                    strCloudData = string.Empty;
//                    Debugging.Log("로드 성공, 데이터 없음");
//                }
//                else
//                {
//                    strCloudData = Encoding.UTF8.GetString(bytes);
//                    this.gameInfo = JsonConvert.DeserializeObject<CloudDataInfo>(strCloudData);
//                    Debugging.Log("클라우드로부터 데이터를 불러왔습니다.");
//                }
//            }
//            else
//            {
//                Debugging.LogWarning(string.Format("로드 실패 : {0}", status));
//            }
//        };

//        this.gpgsManager.OnSavedGameDataWrittenComplete = (status, game) => {
//            if (status == SavedGameRequestStatus.Success)
//            {
//                Debugging.Log("클라우드에 저장 하였습니다.");
//            }
//        };

//        this.gpgsManager.Init();

//        this.gpgsManager.SignIn((result) =>
//        {
//            //result = true;

//            if (result)
//            {
//                Debugging.Log(Social.localUser.userName);
//                //로그인성공
//                this.Init(true);
//            }
//            else
//            {
//                Debugging.Log("로그인 실패");
//                this.Init();

//            }
//        });
//    }

//    private void Init(bool isCloudLoad=false)
//    {
//        var path = Application.persistentDataPath + "/CloudDataInfo.bin";
 

//        Debug.LogFormat("Exists: {0}", File.Exists(path));

//        if (File.Exists(path))
//        {
//            this.gpgsManager.LoadData();
//        }
//        else
//        {
//            this.gameInfo = new CloudDataInfo();
//            this.gameInfo.SetDataToCloud("", DateTime.Now.ToString());
//            if(!gameInfo.IsNullData())
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
//        if(this.gameInfo!=null)
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
