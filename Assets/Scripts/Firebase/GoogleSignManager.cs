using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using FullSerializer;
using Google;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;
using UnityEngine.UI;

public class GoogleSignManager : MonoBehaviour
{
    #region 변수
    public string webClientId = "312752000151-4iql3uip7a39ujqejem5st36e03v2gki.apps.googleusercontent.com";
    public static bool isInitLogin = false;

    public static GoogleSignManager Instance;
    public CloudDataInfo gameInfo;
    private GoogleSignInConfiguration configuration;
    private FirebaseAuth auth;
    #endregion
    void Awake()
    {
        GoogleSignManager.Instance = this;
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestEmail=true,
            RequestIdToken = true
        };
        CheckFirebaseDependencies();
    }
    #region 인증
    private void CheckFirebaseDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if(task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                    auth = FirebaseAuth.DefaultInstance;
                else
                    Debug.Log("Could not resolve all Firebase depen debcies : " + task.Result.ToString());
            }
            else
            {
                Debug.Log("Dependency check was not completed. Error : " + task.Exception.Message);
            }
        });
    }
    public void SignInWithGoogle() { OnSignIn(); }
    public void SignOutFromGoogle() { OnSignOut(); }
    // 이거만 수정하면됨.
    private void SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            AggregateException ex = task.Exception;
            if(ex!=null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    Debug.Log("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
                Debug.Log("Sign In Failed.");
                LocalInit();
            }
            else
            {
                Debug.Log("Sign In Successful. >> \r\n" + idToken);
                DBidToken = idToken;
                localId = task.Result.UserId;
                ServerInit();
            }
        });
    }
    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        try
        {
            if (task.IsFaulted)
            {
                using (IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                        Debug.LogWarning("Got Error: " + error.Status + " " + error.Message);
                    }
                    else
                    {
                        Debug.LogWarning("Got Unexpected Exception?!?" + task.Exception);
                    }
                }
            }
            else if (task.IsCanceled)
            {
                Debug.LogWarning("Canceled");
            }
            else
            {
                SignInWithGoogleOnFirebase(task.Result.IdToken);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
    
    public void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        Debug.Log("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
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
    }
    #endregion

    #region 파이어베이스 DB
    private string DBidToken;
    public static string playerName;
    public static string localId;
    private static string getLocalId;
    public static bool isServerLogin;
    public static fsSerializer serializer = new fsSerializer();

    public void PostToDatabase()
    {
        this.gameInfo.SetDataToCloud(localId, playerName, DateTime.Now.ToString());
        Debug.LogFormat("localID : {0}, playerName : {1} 데이터베이스 저장 \r\n idToken = {2}",this.gameInfo.localId, this.gameInfo.name,DBidToken);
        RestClient.Put(databaseURL + "/" + localId + ".json?auth=" + DBidToken, this.gameInfo).Done(x=>
        {
            Debug.Log("성공적으로 DB 저장완료");
        });

    }
    public void GetFromDatabase()
    {
        RestClient.Get<CloudDataInfo>(databaseURL + "/" + getLocalId + ".json?auth=" + DBidToken).Then(response =>
        {
            this.gameInfo = response;
            SetCloudDataToLocal();
            UI_StartManager.instance.ShowStartUI(true);
        });
    }
    private void GetUsername()
    {
        RestClient.Get<CloudDataInfo>(databaseURL + "/" + localId + ".json?auth=" + DBidToken).Then(response =>
        {
            playerName = response.name;
        });
    }
    private void GetLocalId()
    {
        RestClient.Get(databaseURL + ".json?auth=" + DBidToken).Then(response =>
        {
            fsData cloudDatas = fsJsonParser.Parse(response.Text);

            Dictionary<string, CloudDataInfo> users = null;
            serializer.TryDeserialize(cloudDatas, ref users);

            foreach (var user in users.Values)
            {
                if (user.name == playerName)
                {
                    getLocalId = user.localId;
                    GetFromDatabase();
                    Debug.Log(getLocalId + " 가져오기 성공");
                    break;
                }
            }
        });
    }
    #endregion
    // 시작
    public void Init()
    {
        var path = Application.persistentDataPath + "/CloudDataInfo.bin";
        if (File.Exists(path))
        {
            SignInWithGoogle();
        }
        else
        {
            UI_StartManager.instance.ShowStartUI(false);
        }
    }
    private void ServerInit()
    {
        isServerLogin = true;
        if (!isInitLogin)
        {
            GetLocalId();
        }
        else
        {
            NameInputPanel.GetComponent<AiryUIAnimatedElement>().ShowElement();
            isInitLogin = false;
        }
    }
    private void LocalInit()
    {
        isServerLogin = false;
        var path = Application.persistentDataPath + "/CloudDataInfo.bin";
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            var json = Encoding.UTF8.GetString(bytes);
            this.gameInfo = JsonConvert.DeserializeObject<CloudDataInfo>(json);
            SetCloudDataToLocal();
        }
        else
        {
            this.gameInfo = new CloudDataInfo();
            this.gameInfo.SetDataToCloud(localId, Common.GoogleUserId, DateTime.Now.ToString());
            var json = JsonConvert.SerializeObject(gameInfo);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            File.WriteAllBytes(path, bytes);
        }
    }
    // 데이터 저장
    public void SaveData()
    {
        if(isServerLogin)
        {
            this.gameInfo.SetDataToCloud(localId, Common.GoogleUserId, DateTime.Now.ToString());
            SaveLocalData(this.gameInfo);
            SaveCloudData();
        }
    }
    private void SaveCloudData()
    {
        PostToDatabase();
    }
    private void SaveLocalData(CloudDataInfo data)
    {
        var path = Application.persistentDataPath + "/CloudDataInfo.bin";
        var json = JsonConvert.SerializeObject(data);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        File.WriteAllBytes(path, bytes);
        Debug.Log(Application.persistentDataPath + "/CloudDataInfo.bin 저장완료. >> " + DateTime.Now.ToString());
    }
    // 데이터 처리
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

    #region 최초생성
    public GameObject NameInputPanel;
    public void GoogleLoginInitButton()
    {
        isInitLogin = true;
        SignInWithGoogle();
    }
    public void GoogleDatabaseInitbutton(Text text)
    {
        playerName = text.text;
        Debug.Log("생성 이름 > " + playerName);
        PostToDatabase();
    }
    #endregion

    #region 이메일 인증(Deprecated)
    public string AuthKey = "AIzaSyDScfZ8X1nVmJ1XOaKP3EJnAKNdwlSPFvk";
    private string databaseURL = "https://api-project-81117173.firebaseio.com/users/";
    public void SignUpUser(string email, string username, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + AuthKey, userData).Then(response =>
        {
            DBidToken = response.idToken;
            localId = response.localId;
            Common.GoogleUserId = username;
            Debug.Log(DBidToken + "##" + localId + "##" + username);
            PostToDatabase();
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
    public void SignInUser(string email, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + AuthKey, userData).Then(response =>
        {
            DBidToken = response.idToken;
            localId = response.localId;
            Debug.Log(localId + " 로그인 성공");
            GetUsername();
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
    #endregion
}