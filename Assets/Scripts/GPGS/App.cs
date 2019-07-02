
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class App : MonoBehaviour
{
    public static App Instance;
    public GoogleCloudManager gpgsManager;
    public TestGameInfo gameInfo;

    private void Awake()
    {
        App.Instance = this;
    }

    private void Start()
    {
        this.gpgsManager.OnSavedGameDataReadComplete = (status, bytes) => {
            if (status == SavedGameRequestStatus.Success)
            {
                string strCloudData = null;

                if (bytes.Length == 0)
                {
                    strCloudData = string.Empty;
                    Debugging.Log("로드 성공, 데이터 없음");
                }
                else
                {
                    strCloudData = Encoding.UTF8.GetString(bytes);
                    this.gameInfo = JsonConvert.DeserializeObject<TestGameInfo>(strCloudData);
                    Debugging.Log("클라우드로부터 데이터를 불러왔습니다.");
                }
            }
            else
            {
                Debugging.LogWarning(string.Format("로드 실패 : {0}", status));
            }
        };

        this.gpgsManager.OnSavedGameDataWrittenComplete = (status, game) => {
            if (status == SavedGameRequestStatus.Success)
            {
                Debugging.Log("클라우드에 저장 하였습니다.");
            }
        };

        this.gpgsManager.Init();

        this.gpgsManager.SignIn((result) =>
        {
            //result = true;

            if (result)
            {
                Debugging.Log(Social.localUser.userName);
                //로그인성공
                this.Init();
            }
            else
            {
                Debugging.Log("로그인 실패");

                this.Init();
            }
        });
    }

    private void Init()
    {
        var path = Application.persistentDataPath + "/AppSmilejsuGameInfo.bin";

        Debug.Log(path);

        Debug.LogFormat("Exists: {0}", File.Exists(path));

        if (File.Exists(path))
        {
            this.gpgsManager.LoadData();
        }
        else
        {
            this.gameInfo = new TestGameInfo();
            this.gameInfo.id = Social.localUser.id;
            this.gameInfo.lastSavedTime = DateTime.Now.ToString();

            var json = JsonConvert.SerializeObject(User.name);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            File.WriteAllBytes(path, bytes);

            //PlayerPrefs.SetString(FILE_NAME, JsonConvert.SerializeObject(App.Instance.gameInfo));
        }
    }
}
