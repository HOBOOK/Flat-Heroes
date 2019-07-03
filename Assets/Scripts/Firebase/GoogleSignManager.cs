using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Google;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;
using UnityEngine.UI;

public class GoogleSignManager : MonoBehaviour
{
    public Text statusText;
    public string webClientId = "312752000151-4iql3uip7a39ujqejem5st36e03v2gki.apps.googleusercontent.com";

    public static GoogleSignManager Instance;
    public CloudDataInfo gameInfo;
    private GoogleSignInConfiguration configuration;

    // Defer the configuration creation until Awake so the web Client ID
    // Can be set via the property inspector in the Editor.
    void Awake()
    {
        GoogleSignManager.Instance = this;
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestEmail=true,
            RequestIdToken = true
        };
    }

    #region Default 인증
    public void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        Debug.Log("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
    }
    public void OnSignOut()
    {
        Debug.Log("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }
    public void OnDisconnect()
    {
        Debug.LogWarning("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }
    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        try
        {
            if (task.IsFaulted)
            {
                using (IEnumerator<System.Exception> enumerator =
                        task.Exception.InnerExceptions.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        GoogleSignIn.SignInException error =
                                (GoogleSignIn.SignInException)enumerator.Current;
                        Debug.LogWarning("Got Error: " + error.Status + " " + error.Message);
                    }
                    else
                    {
                        Debug.LogWarning("Got Unexpected Exception?!?" + task.Exception);
                    }
                }
                Common.GoogleUserId = null;
            }
            else if (task.IsCanceled)
            {
                Debug.LogWarning("Canceled");
                Common.GoogleUserId = null;
            }
            else
            {
                Common.GoogleUserId = task.Result.UserId;
            }
        }
        catch(Exception e)
        {
            Debug.LogWarning(e.Message);
        }
        Init();
    }
    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        Debug.Log("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently()
              .ContinueWith(OnAuthenticationFinished);
    }
    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        Debug.Log("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
        Common.GoogleUserId = null;
    }
    #endregion

    #region 인증
    private string idToken;
    public static string localId;
    public string AuthKey = "AIzaSyDScfZ8X1nVmJ1XOaKP3EJnAKNdwlSPFvk";
    private string databaseURL = "https://api-project-81117173.firebaseio.com/users/";

    public InputField emailText;
    public InputField usernameText;
    public InputField passwordText;

    public void SignUpUserButton()
    {
        SignUpUser(emailText.text,usernameText.text,passwordText.text);
    }

    public void SignInUserButton()
    {
        SignInUser(emailText.text,passwordText.text);
    }

    public void SignUpUser(string email, string username ,string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key="+ AuthKey, userData).Then(response =>
        {
            idToken = response.idToken;
            localId = response.localId;
            Common.GoogleUserId = username;
            Debug.Log(idToken +"##"+ localId +"##" +username);
            PostToDatabase();
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
    public void SignInUser(string email, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyCustomToken?key=" + AuthKey, userData).Then(response =>
        {
            idToken = response.idToken;
            localId = response.localId;
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }

    private void GetUsername()
    {
        RestClient.Get<CloudDataInfo>(databaseURL + localId + ".json").Then(response =>
        {
            response.name = Common.GoogleUserId;
        });
    }

    private void GetLocalId()
    {
        RestClient.Get<CloudDataInfo>(databaseURL + localId + ".json").Then(response =>
        {
            response.name = Common.GoogleUserId;
        });
    }

    #endregion

    public void Init()
    {
        LoadData();
    }

    // 구글 데이터 전송
    public void PostToDatabase()
    {
        this.gameInfo.SetDataToCloud(localId, DateTime.Now.ToString());
        RestClient.Put(databaseURL + localId + ".json", this.gameInfo);
    }
    public void PutToDatabase()
    {
        if (Common.GoogleUserId != null)
        {
            this.gameInfo.SetDataToCloud(Common.GoogleUserId, DateTime.Now.ToString());
            RestClient.Put(databaseURL + Common.GoogleUserId + ".json", this.gameInfo);
        }
        else
        {
            Debug.LogWarning("구글 로그인 실패");
        }
    }

    public void GetFromDatabase()
    {
        if (Common.GoogleUserId != null)
        {
            RestClient.Get<CloudDataInfo>(databaseURL + Common.GoogleUserId + ".json").Then(response =>
            {
                if(response!=null)
                {
                    this.gameInfo = response;
                    SetCloudDataToLocal();
                    UI_StartManager.instance.ShowStartUI(true);
                }
                else
                {
                    Debug.LogWarning("구글 로그인은 성공했으나 DB가 없음");
                    var path = Application.persistentDataPath + "/CloudDataInfo.bin";
                    if (File.Exists(path))
                    {
                        LoadLocalData();
                    }
                    else
                    {
                        this.gameInfo = new CloudDataInfo();
                        this.gameInfo.SetDataToCloud(Common.GoogleUserId, DateTime.Now.ToString());
                        var json = JsonConvert.SerializeObject(gameInfo);
                        byte[] bytes = Encoding.UTF8.GetBytes(json);
                        File.WriteAllBytes(path, bytes);
                    }
                    //최초생성
                    UI_StartManager.instance.ShowStartUI(true);
                }
            });
        }
        else
        {
            Debug.LogWarning("구글 로그인 실패");
            var path = Application.persistentDataPath + "/CloudDataInfo.bin";
            if (File.Exists(path))
            {
                LoadLocalData();
            }
            else
            {
                this.gameInfo = new CloudDataInfo();
                this.gameInfo.SetDataToCloud("", DateTime.Now.ToString());
                var json = JsonConvert.SerializeObject(gameInfo);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                File.WriteAllBytes(path, bytes);
            }
            UI_StartManager.instance.ShowStartUI(false);
        }
    }
    // 데이터 저장
    public void SaveData()
    {
        this.gameInfo.SetDataToCloud(Common.GoogleUserId, DateTime.Now.ToString());
        SaveLocalData(this.gameInfo);
        SaveCloudData();
    }
    private void SaveCloudData()
    {
        PutToDatabase();
    }
    private void SaveLocalData(CloudDataInfo data)
    {
        var path = Application.persistentDataPath + "/CloudDataInfo.bin";
        var json = JsonConvert.SerializeObject(data);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        File.WriteAllBytes(path, bytes);
        Debug.Log(Application.persistentDataPath + "/CloudDataInfo.bin 저장완료. >> " + DateTime.Now.ToString());
    }
    // 데이터 로드
    public void LoadData()
    {
        GetFromDatabase();
    }
    private void LoadLocalData()
    {
        var path = Application.persistentDataPath + "/CloudDataInfo.bin";
        // 기존 로컬 파일 로드
        if(File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            var json = Encoding.UTF8.GetString(bytes);
            this.StringToGameInfo(json);
        }
    }
    public void StringToGameInfo(string localData)
    {
        if (localData != string.Empty)
        {
            this.gameInfo = JsonConvert.DeserializeObject<CloudDataInfo>(localData);
            SetCloudDataToLocal();
        }
    }
    private string GameInfoToString(CloudDataInfo data)
    {
        return JsonConvert.SerializeObject(data);
    }


    public void SetCloudDataToLocal()
    {
        if (this.gameInfo != null)
        {
            SaveSystem.SetCloudDataToUser(gameInfo);
            ItemDatabase.SetCloudDataToItem(gameInfo);
            HeroDatabase.SetCloudDataToHero(gameInfo);
            AbilityDatabase.SetCloudDataToAbility(gameInfo);
            SkillDatabase.SetCloudDataToSkill(gameInfo);
            MissionDatabase.SetCloudDataToMission(gameInfo);
            MapDatabase.SetCloudDataToMap(gameInfo);
        }
        else
        {
            Debug.LogWarning("클라우드 데이터 로컬저장중 오류 발생");
        }
    }
}