using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;


public class GoogleCloudManager : MonoBehaviour
{
    public Action<SavedGameRequestStatus, byte[]> OnSavedGameDataReadComplete;
    public Action<SavedGameRequestStatus, ISavedGameMetadata> OnSavedGameDataWrittenComplete;

    private bool isSaving;
    private const string FILE_NAME = "player.fun";

    // 초기화
    public void Init()
    {
        PlayGamesClientConfiguration conf = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(conf);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }
    // 로그인
    public void SignIn(System.Action<bool> onComplete)
    {
        Social.localUser.Authenticate(onComplete);

        Social.localUser.Authenticate((result) =>
        {
            if(result)
            {
                Debug.Log("인증성공");
                Debug.LogFormat("id:{0}, username:{1}, underage:{2}", Social.localUser.id, Social.localUser.userName, Social.localUser.underage);
                Debug.LogFormat("image:{0}", Social.localUser.image);
                this.StartCoroutine(this.LoadImage());
            }
            else
            {
                Debug.Log("인증실패");
            }
        });
    }

    private IEnumerator LoadImage()
    {
        yield return ((PlayGamesLocalUser)Social.localUser).LoadImage();
        Debug.LogFormat("image:{0}", Social.localUser.image);
    }

    #region 저장
    public void SaveData()
    {
        if (Social.localUser.authenticated)
        {
            this.isSaving = true;
            ISavedGameClient savedGAmeClient = PlayGamesPlatform.Instance.SavedGame;
            savedGAmeClient.OpenWithAutomaticConflictResolution(FILE_NAME, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
        }
        else
        {
            this.SaveLocal();
        }
    }

    private void SaveLocal()
    {
        var path = Application.persistentDataPath + "/AppSmilejsuGameInfo.bin";
        var json = JsonConvert.SerializeObject(App.Instance.gameInfo);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        File.WriteAllBytes(path, bytes);
        Debugging.Log(json);
    }
    private void SaveGame(ISavedGameMetadata data)
    {
        this.SaveLocal();

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        SavedGameMetadataUpdate updata = new SavedGameMetadataUpdate.Builder().Build();
        var stringToSave = this.GameInfoToString();
        byte[] bytes = Encoding.UTF8.GetBytes(stringToSave);
        savedGameClient.CommitUpdate(data, updata, bytes, OnSavedGameDataWrittenComplete);
        Debugging.Log(stringToSave);
    }
    #endregion

    #region 불러오기

    public void LoadData()
    {
        if(Social.localUser.authenticated)
        {
            this.isSaving = false;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(FILE_NAME, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime,this.OnSavedGameOpened); ;
        }
        else
        {
            this.LoadLocal();
        }
    }

    public void LoadLocal()
    {
        var path = Application.persistentDataPath + "/AppSmilejsuGameInfo.bin";
        byte[] bytes = File.ReadAllBytes(path);
        var json = Encoding.UTF8.GetString(bytes);
        this.StringToGameInfo(json);
        Debugging.Log(json);
    }

    private void LoadGame(ISavedGameMetadata data)
    {
        ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(data, OnSavedGameDataReadComplete);
    }
    #endregion

    private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        Debug.LogFormat("OnSavedGameOpened : {0}, {1}", status, isSaving);

        if(status == SavedGameRequestStatus.Success)
        {
            if(!isSaving)
            {
                this.LoadGame(game);
            }
            else
            {
                this.SaveGame(game);
            }
        }
        else
        {
            if (!isSaving)
            {
                this.LoadLocal();
            }
            else
            {
                this.SaveLocal();
            }
        }
    }

    public void StringToGameInfo(string localData)
    {
        if (localData != string.Empty)
        {
            App.Instance.gameInfo = JsonConvert.DeserializeObject<TestGameInfo>(localData);
        }
    }

    private string GameInfoToString()
    {
        return JsonConvert.SerializeObject(App.Instance.gameInfo);
    }
}
