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
    private static GameObject ProgressCircle;
    public GameObject ProgressCirclePrefab;
    public static GoogleSignManager Instance;
    private static CloudDataInfo gameInfo;
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
        if(ProgressCircle==null&&ProgressCirclePrefab!=null)
        {
            ProgressCircle = Instantiate(ProgressCirclePrefab, this.transform);
            ProgressCircle.transform.localPosition = Vector3.zero;
            ProgressCircle.gameObject.SetActive(false);
        }
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
                isServerLogin = true;
                GetLocalId();
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
    private static string DBidToken;
    public static string playerName;
    public static string localId;
    public static string lastSaveTime;

    public static bool isServerLogin;
    public static fsSerializer serializer = new fsSerializer();

    private static string AuthKey = "AIzaSyDScfZ8X1nVmJ1XOaKP3EJnAKNdwlSPFvk";
    private static string databaseURL = "https://api-project-81117173.firebaseio.com/users/";

    private static void PostToDatabase()
    {
        gameInfo.SetDataToCloud(localId, playerName, DateTime.Now.ToString());
        Debug.LogFormat("localID : {0}, playerName : {1} 데이터베이스 저장 \r\n idToken = {2}",gameInfo.localId, gameInfo.name,DBidToken);
        RestClient.Put(databaseURL + "/" + localId + ".json", gameInfo).Progress(p=>
        {
            ShowProgressCircle(p);
            Debug.Log("데이터 DB에 쓰는중 .. " + p);
        }).Done(x=>
        {
            Debug.Log("성공적으로 DB 저장완료");
        });

    }
    private static void InitToDatabase()
    {
        InitUserData(localId, playerName);
        gameInfo.SetDataToCloud(localId, playerName, DateTime.Now.ToString());
        Debug.LogFormat("localID : {0}, playerName : {1} 데이터베이스 저장 \r\n idToken = {2}", gameInfo.localId, gameInfo.name, DBidToken);
        RestClient.Put(databaseURL + "/" + localId + ".json", gameInfo).Progress(p=>
        {
            ShowProgressCircle(p);
            Debug.Log("데이터 DB에 쓰는중 .. " + p);
        }).Done(x =>
        {
            Debug.Log("성공적으로 DB 생성완료");
            SaveData();
            LoadSceneManager.instance.LoadScene(0);
        });
    }
    public static void GetFromDatabase()
    {
        RestClient.Get<CloudDataInfo>(databaseURL + "/" + localId + ".json").Progress(p=>
        {
            ShowProgressCircle(p);
            Debug.Log("DB에서 데이터 받는중 .. " + p);
        }).Done(response =>
        {

            Debugging.Log(localId + " 의 DB가져오기 성공 > " + response.name);
            gameInfo = response;
            lastSaveTime = response.lastSavedTime;
            SetCloudDataToLocal();
            UI_StartManager.instance.ShowStartUI(true);
        });
    }

    private void GetLocalId()
    {
        RestClient.Get(databaseURL + ".json").Then(response =>
        {
            fsData cloudDatas = fsJsonParser.Parse(response.Text);

            Dictionary<string, CloudDataInfo> users = null;
            serializer.TryDeserialize(cloudDatas, ref users);

            foreach (var user in users.Values)
            {
                if (user.localId == localId)
                {
                    GetFromDatabase();
                    Debug.Log(localId + " 가져오기 성공함");
                    return;
                }
            }
            NameInputPanel.GetComponent<AiryUIAnimatedElement>().ShowElement();
        });
    }
    #endregion
    // 시작
    public void Init()
    {
        if(isServerLocalDataExist())
        {
            // 정상진행중
            if(isLocalDataExist())
                SignInWithGoogle();
            else
                Debug.LogWarning("Error 팝업");
        }
        else
        {
            if(isLocalDataExist())
            {
                //게스트시작
                UI_StartManager.instance.ShowStartUI(false);
            }
            else
            {
                //최초시작
                UI_StartManager.instance.ShowStartUI(false);
            }
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
            gameInfo = JsonConvert.DeserializeObject<CloudDataInfo>(json);
            SetCloudDataToLocal();
        }
        else
        {
            gameInfo = new CloudDataInfo();
            gameInfo.SetDataToCloud(localId, playerName, DateTime.Now.ToString());
            var json = JsonConvert.SerializeObject(gameInfo);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            File.WriteAllBytes(path, bytes);
        }
    }
    // 데이터 저장
    public static void SaveData()
    {
        if (isServerLogin)
        {
            SaveSystem.SavePlayer();
            gameInfo.SetDataToCloud(localId, playerName, DateTime.Now.ToString());
            lastSaveTime = DateTime.Now.ToString();
            SaveLocalData(gameInfo);
            PostToDatabase();
        }
        else
        {
            ShowProgressCircle(10);
            SaveSystem.SavePlayer();
            ShowProgressCircle(100);
        }
    }

    private static void SaveLocalData(CloudDataInfo data)
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
    private static void SetCloudDataToLocal()
    {
        if (gameInfo != null)
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
    private static void InitUserData(string localId, string name)
    {
        SaveSystem.InitPlayer(localId, name);
        ItemDatabase.InitSetting();
        HeroDatabase.InitSetting();
        AbilityDatabase.InitSetting();
        SkillDatabase.InitSetting();
        MissionDatabase.InitSetting();
        MapDatabase.InitSetting();
        SaveData();
    }
    //private bool isServerDataExist()
    //{
    //    bool isExist = false;
    //    RestClient.Get(databaseURL + ".json").Then(response =>
    //    {
    //        fsData cloudDatas = fsJsonParser.Parse(response.Text);

    //        Dictionary<string, CloudDataInfo> users = null;
    //        serializer.TryDeserialize(cloudDatas, ref users);

    //        foreach (var user in users.Values)
    //        {
    //            if (user.localId == localId)
    //            {
    //                isExist = true;
    //                break;
    //            }
    //        }
    //    });
    //    return isExist;
    //}
    private bool isLocalDataExist()
    {
        string[] localUserDataPaths = {"/player.fun"
                                            , "/Xml/Ability.Xml"
                                            , "/Xml/Item.Xml"
                                            , "/Xml/Map.Xml"
                                            , "/Xml/Mission.Xml"
                                            , "/Xml/Skill.Xml"
        };
        for (var i = 0; i < localUserDataPaths.Length; i++)
        {
            var path = Application.persistentDataPath + localUserDataPaths[i];
            if (!File.Exists(path))
            {
                Debug.Log(path + " 의 파일이 존재하지않음");
                return false;
            }
        }
        return true;
    }
    private bool isServerLocalDataExist()
    {
        var serverlocalDataPath = Application.persistentDataPath + "/CloudDataInfo.bin";
        if (File.Exists(serverlocalDataPath))
        {
            return true;
        }
        Debug.Log(Application.persistentDataPath + "/CloudDataInfo.bin 파일이 로컬에 없음");
        return false;
    }

    public static void ShowProgressCircle(float p)
    {

        if(p>0 && p<100)
        {
            ProgressCircle.gameObject.SetActive(true);
        }
        else
        {
            ProgressCircle.gameObject.SetActive(false);
        }
    }

    #region 최초생성
    public GameObject NameInputPanel;
    public void GoogleLoginInitButton()
    {
        SignInWithGoogle();
    }
    public void GoogleDatabaseInitbutton(Text text)
    {
        playerName = text.text;
        InitToDatabase();
    }
    public void GuestInitbutton()
    {
        if (!isLocalDataExist())
        {
            var geustId = "guest_" + Common.GetRandomID(6);
            InitUserData(geustId, geustId);
            LoadSceneManager.instance.LoadScene(1);
        }
        else
        {
            LoadSceneManager.instance.LoadScene(1);
        }
    }
    #endregion

    #region 이메일 인증(Deprecated)

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
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
    #endregion
}