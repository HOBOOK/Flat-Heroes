using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    private static bool isCheckOk = false;
    private static bool isValidateName = false;
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
        Debug.Log("파이어 베이스 로그인 시작 > " + idToken.Length);
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
            Debug.Log("인증 시작");
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
    private static string databasePostURL = "https://api-project-81117173.firebaseio.com/post/";
    private static string databaseRankURL = "https://api-project-81117173.firebaseio.com/rank/";

    private static void PostToDatabase()
    {
        ShowProgressCircle(10);
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
        ShowProgressCircle(100);
    }

    private void InitToDatabase()
    {
        ShowProgressCircle(10);
        InitUserData(localId, playerName);
        gameInfo = new CloudDataInfo();
        gameInfo.SetDataToCloud(localId, playerName, DateTime.Now.ToString());
        Debug.LogFormat("localID : {0}, playerName : {1} 데이터베이스 저장 \r\n idToken = {2}", gameInfo.localId, gameInfo.name, DBidToken);
        RestClient.Put(databaseURL + "/" + localId + ".json", gameInfo).Progress(p=>
        {
            ShowProgressCircle(p);
            Debug.Log("데이터 DB에 쓰는중 .. " + p);
        }).Done(x =>
        {
            Debug.Log("성공적으로 DB 생성완료");
            SaveLocalData(gameInfo);
            ShowProgressCircle(100);
            //LoadSceneManager.instance.LoadScene(0);
            UI_StartManager.instance.ShowStartUI(true);
        });
        ShowProgressCircle(100);
    }
    public static void GetFromDatabase()
    {
        ShowProgressCircle(10);
        RestClient.Get<CloudDataInfo>(databaseURL + "/" + localId + ".json").Progress(p =>
        {
            ShowProgressCircle(p);
            Debug.Log("DB에서 데이터 받는중 .. " + p);
        }).Then(response =>
        {
            ShowProgressCircle(100);
            Debug.Log(localId + " 의 DB가져오기 성공 > " + response.name);
            gameInfo = response;
            playerName = response.name;
            lastSaveTime = response.lastSavedTime;
            SetCloudDataToLocal();
            UI_StartManager.instance.ShowStartUI(true);
            return;
        }).Catch(err=>
        {
            ShowProgressCircle(100);
            UI_StartManager.instance.ShowErrorUI(LocalizationManager.GetText("alertDBErrorMessage"));
            Debug.Log("localID : "+localId+err.StackTrace);
        });
        ShowProgressCircle(100);
    }

    public void ValidatingName(string name)
    {
        if (!Common.ValidateName(name))
        {
            isCheckOk = true;
            isValidateName = false;
        }
        else
        {
            ShowProgressCircle(10);
            bool isExist = false;
            RestClient.Get(databaseURL + ".json").Done(response =>
            {
                fsData cloudDatas = fsJsonParser.Parse(response.Text);
                Dictionary<string, CloudDataInfo> users = null;
                serializer.TryDeserialize(cloudDatas, ref users);

                foreach (var user in users.Values)
                {
                    if (user.name == name || user.name.Equals(name))
                    {
                        isExist = true;
                        break;
                    }
                }
                isValidateName = !isExist;
                isCheckOk = true;
                Debug.Log(string.Format("{0} 중복체크 완료 > ", isValidateName));
                ShowProgressCircle(100);
            }, err=>
            {
                UI_StartManager.instance.ShowErrorUI(LocalizationManager.GetText("alertDBErrorMessage"));
                Debug.LogError(err.StackTrace);
            });
        }
    }

    private void GetLocalId()
    {
        ShowProgressCircle(10);
        RestClient.Get(databaseURL + ".json").Then(response =>
        {
            fsData cloudDatas = fsJsonParser.Parse(response.Text);

            Dictionary<string, CloudDataInfo> users = null;
            serializer.TryDeserialize(cloudDatas, ref users);

            foreach (var user in users.Values)
            {
                Debugging.Log(user.localId + " 서치중...");
                if (user.localId == localId)
                {
                    GetFromDatabase();
                    Debug.Log(localId + " 가져오기 성공함");
                    return;
                }
            }
            ShowProgressCircle(100);
            NameInputPanel.SetActive(true);
            NameInputPanel.GetComponent<AiryUIAnimatedElement>().ShowElement();
        })
        .Catch(err=> 
        {
            ShowProgressCircle(100);
            Debug.Log(localId + " DB에 없음");
            NameInputPanel.SetActive(true);
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
            UI_StartManager.instance.ShowStartUI(false);
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
            ItemDatabase.SetCloudDataToItem(gameInfo);
            HeroDatabase.SetCloudDataToHero(gameInfo);
            AbilityDatabase.SetCloudDataToAbility(gameInfo);
            SkillDatabase.SetCloudDataToSkill(gameInfo);
            MissionDatabase.SetCloudDataToMission(gameInfo);
            MapDatabase.SetCloudDataToMap(gameInfo);
            SaveSystem.SetCloudDataToUser(gameInfo);
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
            Debugging.Log("프로그래스 ON");
        }
        else
        {
            ProgressCircle.gameObject.SetActive(false);
            Debugging.Log("프로그래스 OFF");
        }
    }

    #region 최초생성
    public GameObject NameInputPanel;
    public void GoogleLoginInitButton()
    {
        SignInWithGoogle();
    }
    IEnumerator CheckingValidateName(Transform parent)
    {
        isCheckOk = false;
        string inputName = parent.GetComponentInChildren<InputField>().text;
        ValidatingName(inputName);
        while (!isCheckOk)
        {
            yield return null;
        }

        ShowProgressCircle(100);
        if (isValidateName)
        {
            Debugging.Log("이름입력 성공");
            playerName = inputName;
            InitToDatabase();
            parent.GetComponent<AiryUIAnimatedElement>().HideElement();
        }
        else
        {
            parent.transform.GetChild(1).gameObject.SetActive(true);
            if (parent.transform.GetChild(1).GetComponent<AiryUIAnimatedElement>() != null)
                parent.transform.GetChild(1).GetComponent<AiryUIAnimatedElement>().ShowElement();
            Debugging.Log("이름입력 실패");
        }
    }
    public void GoogleDatabaseInitbutton(Transform parent)
    {
        Debugging.Log("최초 유저 데이터 생성버튼 시작 " + parent.name);
        StartCoroutine(CheckingValidateName(parent));
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

    #region 랭킹
    public void SetRankData()
    {
        //localId = "LQiyyLOPrJP8g3sMvvKNhQfqZqi2";
        if (!string.IsNullOrEmpty(localId))
        {
            if(!string.IsNullOrEmpty(User.name))
            {
                RestClient.Put(databaseRankURL + "/" + localId + "/Name.json", "\"" + User.name + "\"").Done(x =>
                {

                }, err =>
                {
                    Debug.Log(err.StackTrace);
                });
            }
            RestClient.Put(databaseRankURL + "/" + localId + "/Thumbnail.json", "" + Mathf.Clamp(User.profileHero,101,110) + "").Done(x =>
            {

            }, err =>
            {
                Debug.Log(err.StackTrace);
            });
            RestClient.Put(databaseRankURL + "/" + localId + "/HeroRankPoint.json", "" + User.HeroRankPoint + "").Done(x =>
            {

            }, err =>
            {
                Debug.Log(err.StackTrace);
            });
        }
    }
    public List<KeyValuePair<string, RankDataInfo>> GetRankData(int type)
    {
        if(type==1)
        {
            SetRankData();
        }
        var ReturnRankData = new List<KeyValuePair<string, RankDataInfo>>();
        RestClient.Get(databaseRankURL + ".json").Done(response =>
        {
            ShowProgressCircle(10);
            fsData cloudDatas = fsJsonParser.Parse(response.Text);

            Dictionary<string, RankDataInfo> users = null;
            serializer.TryDeserialize(cloudDatas, ref users);
            
            Dictionary<string, RankDataInfo> rankDatas = new Dictionary<string, RankDataInfo>();
            foreach (var user in users.Values)
            {
                if(type==0)
                {
                    if (user.InfinityRankPoint > 0 && !string.IsNullOrEmpty(user.Name) && !rankDatas.ContainsKey(user.Name))
                    {
                        rankDatas.Add(user.Name, user);
                    }
                }
                else if(type==1)
                {
                    if (user.HeroRankPoint > 0 && !string.IsNullOrEmpty(user.Name) && !rankDatas.ContainsKey(user.Name))
                    {
                        rankDatas.Add(user.Name, user);
                    }
                }
                else if(type == 2)
                {
                    if (user.BattleRankPoint > 0 && !string.IsNullOrEmpty(user.Name) && !rankDatas.ContainsKey(user.Name))
                    {
                        rankDatas.Add(user.Name, user);
                    }
                }
                else 
                {
                    if (user.AttackRankPoint > 0 && !string.IsNullOrEmpty(user.Name) && !rankDatas.ContainsKey(user.Name))
                    {
                        rankDatas.Add(user.Name, user);
                    }
                }
            }
            bool isFindPlayerRank = false;
            int rank = 0;
            if(type==0)
            {
                foreach (var data in rankDatas.OrderByDescending(i => i.Value.InfinityRankPoint))
                {
                    rank++;
                    Debugging.Log(rank + string.Format(" {0} : {1}", data.Key, data.Value.InfinityRankPoint));
                    if (rank > 0 && rank <= 10)
                    {
                        ReturnRankData.Add(data);
                    }
                    if (!string.IsNullOrEmpty(User.name) && (User.name.Equals(data.Key) || User.name == data.Key))
                    {
                        //Debugging.Log("플레이어 랭킹 찾음 > " + rank + " 포인트 > " + data.Value.InfinityRankPoint);
                        isFindPlayerRank = true;
                        Common.RankOfInfinityMode = rank;
                    }
                    if (isFindPlayerRank && rank > 10)
                        break;
                }
            }
            else if(type==1)
            {
                foreach (var data in rankDatas.OrderByDescending(i => i.Value.HeroRankPoint))
                {
                    rank++;
                    if (rank > 0 && rank <= 10)
                    {
                        ReturnRankData.Add(data);
                    }
                    if (!string.IsNullOrEmpty(User.name) && (User.name.Equals(data.Key) || User.name == data.Key))
                    {
                        //Debugging.Log("플레이어 랭킹 찾음 > " + rank + " 포인트 > " + data.Value.InfinityRankPoint);
                        isFindPlayerRank = true;
                        Common.RankOfHeroLevel = rank;
                    }
                    if (isFindPlayerRank && rank > 10)
                        break;
                }
            }
            else if(type==2)
            {
                foreach (var data in rankDatas.OrderByDescending(i => i.Value.BattleRankPoint))
                {
                    rank++;
                    if (rank > 0 && rank <= 10)
                    {
                        ReturnRankData.Add(data);
                    }
                    if (!string.IsNullOrEmpty(User.name) && (User.name.Equals(data.Key) || User.name == data.Key))
                    {
                        //Debugging.Log("플레이어 랭킹 찾음 > " + rank + " 포인트 > " + data.Value.InfinityRankPoint);
                        isFindPlayerRank = true;
                        Common.RankOfPvp = rank;
                    }
                    if (isFindPlayerRank && rank > 10)
                        break;
                }
            }
            else if (type == 3)
            {
                foreach (var data in rankDatas.OrderByDescending(i => i.Value.AttackRankPoint))
                {
                    rank++;
                    if (rank > 0 && rank <= 10)
                    {
                        ReturnRankData.Add(data);
                    }
                    if (!string.IsNullOrEmpty(User.name) && (User.name.Equals(data.Key) || User.name == data.Key))
                    {
                        //Debugging.Log("플레이어 랭킹 찾음 > " + rank + " 포인트 > " + data.Value.InfinityRankPoint);
                        isFindPlayerRank = true;
                        Common.RankOfAttackMode = rank;
                    }
                    if (isFindPlayerRank && rank > 10)
                        break;
                }
            }

            for (var i = ReturnRankData.Count; i < 10; i++)
            {
                Dictionary<string, RankDataInfo> xData = new Dictionary<string, RankDataInfo>();
                xData.Add("N/A", new RankDataInfo());
                ReturnRankData.Add(xData.ToList()[0]);
            }
            ShowProgressCircle(100);
            Debugging.Log("랭킹데이터 성공적으로 받음");
        },err =>
        {
            for (var i = 0; i < 10; i++)
            {
                Dictionary<string, RankDataInfo> xData = new Dictionary<string, RankDataInfo>();
                xData.Add("N/A", new RankDataInfo());
                ReturnRankData.Add(xData.ToList()[0]);
            }
            ShowProgressCircle(100);
            Debugging.Log("랭킹데이터 없음");
        });
        foreach(var d in ReturnRankData)
        {
            Debugging.Log(d.Key + " 리턴");
        }
        return ReturnRankData;
    }
    public List<KeyValuePair<string, RankDataInfo>> GetPvpRankData(int point)
    {
        var ReturnRankData = new List<KeyValuePair<string, RankDataInfo>>();
        RestClient.Get(databaseRankURL + ".json").Done(response =>
        {
            ShowProgressCircle(10);
            fsData cloudDatas = fsJsonParser.Parse(response.Text);

            Dictionary<string, RankDataInfo> users = null;
            serializer.TryDeserialize(cloudDatas, ref users);

            Dictionary<string, RankDataInfo> rankDatas = new Dictionary<string, RankDataInfo>();
            foreach (var user in users.Values)
            {
                if (user.BattleRankPoint > 0 && !string.IsNullOrEmpty(user.Name) && !rankDatas.ContainsKey(user.Name) && Common.GetRank(User.battleRankPoint)==Common.GetRank(user.BattleRankPoint)&&(user.BattleWin+user.BattleLose)>0)
                {
                    rankDatas.Add(user.Name, user);
                }
            }
            bool isFindPlayerRank = false;
            int rank = 0;
            foreach (var data in rankDatas.OrderByDescending(i => i.Value.BattleRankPoint))
            {
                rank++;
                if (rank > 0 && rank <= 20)
                {
                    ReturnRankData.Add(data);
                }
                if (!string.IsNullOrEmpty(User.name) && (User.name.Equals(data.Key) || User.name == data.Key))
                {
                    //Debugging.Log("플레이어 랭킹 찾음 > " + rank + " 포인트 > " + data.Value.InfinityRankPoint);
                    isFindPlayerRank = true;
                    Common.RankOfPvp = rank;
                }
                if (isFindPlayerRank && rank > 20)
                    break;
            }

            for (var i = ReturnRankData.Count; i < 20; i++)
            {
                Dictionary<string, RankDataInfo> xData = new Dictionary<string, RankDataInfo>();
                xData.Add("N/A", new RankDataInfo());
                ReturnRankData.Add(xData.ToList()[0]);
            }
            ShowProgressCircle(100);
            Debugging.Log("랭킹데이터 성공적으로 받음");
        }, err =>
        {
            for (var i = 0; i < 20; i++)
            {
                Dictionary<string, RankDataInfo> xData = new Dictionary<string, RankDataInfo>();
                xData.Add("N/A", new RankDataInfo());
                ReturnRankData.Add(xData.ToList()[0]);
            }
            ShowProgressCircle(100);
            Debugging.Log("랭킹데이터 없음");
        });
        foreach (var d in ReturnRankData)
        {
            Debugging.Log(d.Key + " 리턴");
        }
        return ReturnRankData;
    }
    public void PushPostMessage(int itemId)
    {
        RestClient.Get(databaseURL + ".json").Progress(p=>
        {
            ShowProgressCircle(10);
        }).Done(response =>
        {
            fsData cloudDatas = fsJsonParser.Parse(response.Text);
            Dictionary<string, CloudDataInfo> users = null;
            serializer.TryDeserialize(cloudDatas, ref users);
            List<string> userLocalIds = new List<string>();
            foreach (var user in users.Values)
            {
                userLocalIds.Add(user.localId);
            }
            foreach(var authId in userLocalIds)
            {
                RestClient.Put(databasePostURL + "/" + authId + "/postMessages.json", "\"" + string.Format("({0},{1},{2},{3})", itemId, "1", "개발자의 감사한 마음이 담긴 선물!", DateTime.Now.ToShortDateString()) + "\"")
                .Done(x =>
                {
                    Debugging.Log(authId +" 성공적으로 DB 저장완료");
                    ShowProgressCircle(100);
                }, er =>
                {
                    Debugging.LogError(authId + " 실패");
                    ShowProgressCircle(100);
                });
            }

        }, err =>
        {
            UI_StartManager.instance.ShowErrorUI(LocalizationManager.GetText("alertDBErrorMessage"));
            Debug.LogError(err.StackTrace);
        });
    }
    public void PushPostToUser(string auth, int itemId, int itemCount, string msg)
    {
        RestClient.Get(databasePostURL + "/" + auth + "/postMessages.json").Progress(p =>
        {
            ShowProgressCircle(10);
        }).Done(response =>
        {
            var post = response.Text.Replace("\"", "");
            post = !string.IsNullOrEmpty(post) && post.ToLower().Equals("null") ? post + ":" : "";
            RestClient.Put(databasePostURL + "/" + auth + "/postMessages.json", "\"" + post + string.Format("({0},{1},{2},{3})", itemId, itemCount.ToString(), msg.ToString(), DateTime.Now.ToShortDateString()) + "\"")
            .Done(x =>
            {
                Debugging.Log(auth + " 성공적으로 DB 저장완료");
                ShowProgressCircle(100);
            }, er =>
            {
                Debugging.LogError(auth + " 실패");
                ShowProgressCircle(100);
            });

        }, err =>
        {
            UI_StartManager.instance.ShowErrorUI(LocalizationManager.GetText("alertDBErrorMessage"));
            Debug.LogError(err.StackTrace);
        });
    }
    public void ResetPostMessage()
    {
        //localId = "LQiyyLOPrJP8g3sMvvKNhQfqZqi2";
        if(!string.IsNullOrEmpty(localId))
        {
            Common.postItemDatas = "";
            ShowProgressCircle(10);
            RestClient.Put(databasePostURL + "/" + localId + "/postMessages.json", "\"\"").Progress(p =>
            {
                ShowProgressCircle(p);
                Debug.Log("데이터 DB에 쓰는중 .. " + p);
            }).Done(x =>
            {
                Debug.Log("성공적으로 DB 저장완료");
            });
            ShowProgressCircle(100);
        }
    }
    public void GetPostMessage()
    {
        //localId = "LQiyyLOPrJP8g3sMvvKNhQfqZqi2";
        Common.postItemDatas = "";
        if (!string.IsNullOrEmpty(localId))
        {
            ShowProgressCircle(10);
            RestClient.Get(databasePostURL + "/" + localId + "/postMessages.json").Done(x =>
            {
                if (x != null)
                {
                    Debugging.Log(x.Text);
                    var data = x.Text.Replace("\"", "");
                    if (string.IsNullOrEmpty(data) || data.Equals("null"))
                    {
                        Common.postItemDatas = "-1";
                    }
                    else
                        Common.postItemDatas = data;
                }
                else
                {
                    Common.postItemDatas = "-1";
                }
            }, e =>
            {
                Debug.Log(e.StackTrace);
                Common.postItemDatas = "-1";
            });
        }
        else
        {
            Common.postItemDatas = "-1";
        }
        ShowProgressCircle(100);
    }
    public void SetAttackPoint(int point)
    {
        Common.isLoadCompleted = false;
        ShowProgressCircle(10);
        //localId = "LQiyyLOPrJP8g3sMvvKNhQfqZqi2";
        if (!string.IsNullOrEmpty(localId))
        {
            SetRankData();
            RestClient.Get(databaseRankURL + "/" + localId + "/AttackRankPoint.json").Done(x =>
            {
                if (x != null && !string.IsNullOrEmpty(x.Text) && !x.Text.ToLower().Equals("null"))
                {
                    User.attackRankPoint = int.Parse(x.Text);
                    Debugging.Log("공격모드점수 Get > " + User.attackRankPoint);
                    if (point > User.attackRankPoint)
                    {
                        User.attackRankPoint = point;
                        RestClient.Put(databaseRankURL + "/" + localId + "/AttackRankPoint.json", "" + point + "").Progress(p =>
                        {
                            ShowProgressCircle(p);
                            Debug.Log("데이터 DB에 쓰는중 .. " + p);
                        }).Done(d =>
                        {
                            Debug.Log("성공적으로 DB 저장완료");
                            Common.isLoadCompleted = true;
                        }, err =>
                        {
                            Common.isLoadCompleted = true;
                        });
                        Debugging.Log("공격모드점수 Set > " + User.attackRankPoint);
                    }
                    else
                    {
                        Common.isLoadCompleted = true;
                    }
                }
                else
                {
                    RestClient.Put(databaseRankURL + "/" + localId + "/AttackRankPoint.json", "" + point + "").Progress(p =>
                    {
                        ShowProgressCircle(p);
                        Debug.Log("데이터 DB에 쓰는중 .. " + p);
                    }).Done(d =>
                    {
                        Debug.Log("성공적으로 DB 저장완료");
                        Common.isLoadCompleted = true;
                    });
                }
            }, e =>
            {
                Debugging.Log(e.StackTrace);
                Common.isLoadCompleted = true;
            });
            ShowProgressCircle(100);
        }
        else
        {
            Common.isLoadCompleted = true;
        }
    }

    public void SetInfinityPoint(int point)
    {
        Common.isLoadCompleted = false;
        ShowProgressCircle(10);
        //localId = "LQiyyLOPrJP8g3sMvvKNhQfqZqi2";
        if(!string.IsNullOrEmpty(localId))
        {
            SetRankData();
            RestClient.Get(databaseRankURL + "/" + localId + "/InfinityRankPoint.json").Done(x =>
            {
                if(x!=null&&!string.IsNullOrEmpty(x.Text)&&!x.Text.ToLower().Equals("null"))
                {
                    User.InfinityRankPoint = int.Parse(x.Text);
                    Debugging.Log("무한모드점수 Get > " + User.InfinityRankPoint);
                    if (point > User.InfinityRankPoint)
                    {
                        User.InfinityRankPoint = point;
                        RestClient.Put(databaseRankURL + "/" + localId + "/InfinityRankPoint.json", "" + point + "").Progress(p =>
                        {
                            ShowProgressCircle(p);
                            Debug.Log("데이터 DB에 쓰는중 .. " + p);
                        }).Done(d =>
                        {
                            Debug.Log("성공적으로 DB 저장완료");
                            Common.isLoadCompleted = true;
                        }, err=>
                        {
                            Common.isLoadCompleted = true;
                        });
                        Debugging.Log("무한모드점수 Set > " + User.InfinityRankPoint);
                    }
                    else
                    {
                        Common.isLoadCompleted = true;
                    }
                }
                else
                {
                    RestClient.Put(databaseRankURL + "/" + localId + "/InfinityRankPoint.json", ""+point+"").Progress(p =>
                    {
                        ShowProgressCircle(p);
                        Debug.Log("데이터 DB에 쓰는중 .. " + p);
                    }).Done(d =>
                    {
                        Debug.Log("성공적으로 DB 저장완료");
                        Common.isLoadCompleted = true;
                    });
                }
            }, e =>
            {
                Debugging.Log(e.StackTrace);
                Common.isLoadCompleted = true;
            });
            ShowProgressCircle(100);
        }
        else
        {
            Common.isLoadCompleted = true;
        }
    }
    public void GetInfinityPoint()
    {
        ShowProgressCircle(10);
        //localId = "LQiyyLOPrJP8g3sMvvKNhQfqZqi2";
        RestClient.Get(databaseURL + "/" + localId + "/InfinityRankPoint.json").Done(x =>
        {
            Debug.Log(x.Text);
            if (!string.IsNullOrEmpty(x.Text.Replace("\"", "")))
                User.InfinityRankPoint = int.Parse(x.Text.Replace("\"", ""));
        }, e =>
        {
            Debug.Log(e.StackTrace);
        });
        ShowProgressCircle(100);
    }
    public void FindPvpData()
    {
        ShowProgressCircle(10);
        Common.isLoadCompleted = false;
        //localId = "LQiyyLOPrJP8g3sMvvKNhQfqZqi2";
        if (!string.IsNullOrEmpty(localId))
        {
            RestClient.Get(databaseRankURL + ".json").Done(response =>
            {
                fsData cloudDatas = fsJsonParser.Parse(response.Text);

                Dictionary<string, RankDataInfo> users = null;
                serializer.TryDeserialize(cloudDatas, ref users);
                List<string> enemyList = new List<string>();

                if (users!=null&&users.ContainsKey(localId))
                {
                    users.Remove(localId);
                }
                if (users != null && users.Count > 0)
                {
                    foreach (var user in users)
                    {
                        if (user.Value.BattleRankPoint > User.battleRankPoint - 200 && user.Value.BattleRankPoint < User.battleRankPoint + 200)
                        {
                            enemyList.Add(user.Key);
                        }
                    }
                    if (enemyList != null && enemyList.Count > 5)
                    {
                        Common.pvpEnemyLocalId = enemyList[UnityEngine.Random.Range(0, enemyList.Count - 1)];
                        Common.isLoadCompleted = true;
                        ShowProgressCircle(100);
                    }
                    else
                    {
                        foreach (var user in users)
                        {
                            if (user.Value.BattleRankPoint > User.battleRankPoint - 400 && user.Value.BattleRankPoint < User.battleRankPoint + 400)
                            {
                                enemyList.Add(user.Key);
                            }
                        }
                        if (enemyList != null && enemyList.Count > 5)
                        {
                            Common.pvpEnemyLocalId = enemyList[UnityEngine.Random.Range(0, enemyList.Count - 1)];
                            Common.isLoadCompleted = true;
                            ShowProgressCircle(100);
                        }
                        else
                        {
                            foreach (var user in users)
                            {
                                if (user.Value.BattleRankPoint > User.battleRankPoint - 400 && user.Value.BattleRankPoint < User.battleRankPoint + 400)
                                {
                                    enemyList.Add(user.Key);
                                }
                            }
                            if (enemyList != null && enemyList.Count > 5)
                            {
                                Common.pvpEnemyLocalId = enemyList[UnityEngine.Random.Range(0, enemyList.Count - 1)];
                                Common.isLoadCompleted = true;
                                ShowProgressCircle(100);
                            }
                            else
                            {
                                foreach (var user in users)
                                {
                                    if (user.Value.BattleRankPoint > User.battleRankPoint - 800 && user.Value.BattleRankPoint < User.battleRankPoint + 800)
                                    {
                                        enemyList.Add(user.Key);
                                    }
                                }
                                if (enemyList != null && enemyList.Count > 5)
                                {
                                    Common.pvpEnemyLocalId = enemyList[UnityEngine.Random.Range(0, enemyList.Count - 1)];
                                    Common.isLoadCompleted = true;
                                    ShowProgressCircle(100);
                                }
                                else
                                {
                                    foreach (var user in users)
                                    {
                                        if (user.Value.BattleRankPoint > 0)
                                        {
                                            enemyList.Add(user.Key);
                                        }
                                    }
                                    if (enemyList != null && enemyList.Count > 0)
                                    {
                                        Common.pvpEnemyLocalId = enemyList[UnityEngine.Random.Range(0, enemyList.Count - 1)];
                                        Common.isLoadCompleted = true;
                                        ShowProgressCircle(100);
                                    }
                                    else
                                    {
                                        Common.pvpEnemyLocalId = null;
                                        Common.isLoadCompleted = true;
                                        ShowProgressCircle(100);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Common.pvpEnemyLocalId = null;
                    Common.isLoadCompleted = true;
                    ShowProgressCircle(100);
                }
            }, e =>
            {
                Debug.Log(e.StackTrace);
                Common.pvpEnemyLocalId = null;
                Common.isLoadCompleted = true;
                ShowProgressCircle(100);
            });
        }
        else
        {
            Common.pvpEnemyLocalId = null;
            Common.isLoadCompleted = true;
            ShowProgressCircle(100);
        }
    }
    public void GetPvpData(string playerAuthId)
    {
        ShowProgressCircle(10);
        Common.isLoadCompleted = false;
        //playerAuthId = "LQiyyLOPrJP8g3sMvvKNhQfqZqi2";
        RestClient.Get<CloudDataInfo>(databaseURL + "/" + playerAuthId + ".json").Done(x =>
        {
            if(x!=null&&!string.IsNullOrEmpty(x.UserData)&&!string.IsNullOrEmpty(x.HeroData))
            {
                Common.pvpEnemyData = new PvpData(x.UserData, x.HeroData, x.AbilityData,x.ItemData);
                Debugging.Log(x.name + " 데이터 찾음");
            }
            else
            {
                Common.pvpEnemyData = null;
                Debugging.Log("PVP데이터 null");
            }
            Common.isLoadCompleted = true;
        }, e =>
        {
            Debug.Log(e.StackTrace);
            Common.pvpEnemyData = null;
            Common.isLoadCompleted = true;
        });
        ShowProgressCircle(100);
    }
    public void GetPvpRankPoint()
    {
        Common.isLoadCompleted = false;
        ShowProgressCircle(10);
        //localId = "LQiyyLOPrJP8g3sMvvKNhQfqZqi2";
        if (!string.IsNullOrEmpty(localId))
        {
            RestClient.Get<RankDataInfo>(databaseRankURL + "/" + localId + "/.json").Done(x =>
            {
                if (x != null && x.BattleRankPoint>0)
                {
                    User.battleRankPoint = x.BattleRankPoint;
                    User.battleWin = x.BattleWin;
                    User.battleLose = x.BattleLose;
                    SaveSystem.SavePlayer();
                    Debugging.Log("pvp점수 Get > " + User.battleRankPoint);
                    Common.isLoadCompleted = true;
                    ShowProgressCircle(100);
                }
                else
                {
                    User.battleRankPoint = 1000;
                    User.battleWin = 0;
                    User.battleLose = 0;
                    RestClient.Put(databaseRankURL + "/" + localId + "/BattleRankPoint.json", "" + User.battleRankPoint + "").Done(d =>
                    {

                    }, err =>
                    {
                        Debug.Log(err.StackTrace);
                    });
                    RestClient.Put(databaseRankURL + "/" + localId + "/BattleWin.json", "" + User.battleWin + "").Done(d =>
                    {

                    }, err =>
                    {
                        Debug.Log(err.StackTrace);
                    });
                    RestClient.Put(databaseRankURL + "/" + localId + "/BattleLose.json", "" + User.battleLose + "").Done(d =>
                    {

                    }, err =>
                    {
                        Debug.Log(err.StackTrace);
                    });
                    Common.isLoadCompleted = true;
                    ShowProgressCircle(100);
                }
            }, e =>
            {
                Debugging.Log(e.StackTrace);
                Common.isLoadCompleted = true;
                ShowProgressCircle(100);
            });
        }
        else
        {
            Common.isLoadCompleted = true;
            ShowProgressCircle(100);
        }
    }
    public void SetPvpRankPoint()
    {
        Common.isLoadCompleted = false;
        ShowProgressCircle(10);
        //localId = "LQiyyLOPrJP8g3sMvvKNhQfqZqi2";
        if (!string.IsNullOrEmpty(localId))
        {
            RestClient.Put(databaseRankURL + "/" + localId + "/BattleRankPoint.json", "" + User.battleRankPoint + "").Done(d =>
            {

            }, err =>
            {
                Debug.Log(err.StackTrace);
            });
            RestClient.Put(databaseRankURL + "/" + localId + "/BattleWin.json", "" + User.battleWin + "").Done(d =>
            {

            }, err =>
            {
                Debug.Log(err.StackTrace);
            });
            RestClient.Put(databaseRankURL + "/" + localId + "/BattleLose.json", "" + User.battleLose + "").Done(d =>
            {

            }, err =>
            {
                Debug.Log(err.StackTrace);
            });
            ShowProgressCircle(100);
            Common.isLoadCompleted = true;
        }
        else
        {
            Common.isLoadCompleted = true;
        }
    }
    public void ResetPvpRankAll()
    {
        RestClient.Get(databaseURL + ".json").Progress(p =>
        {
            ShowProgressCircle(10);
        }).Done(response =>
        {
            fsData cloudDatas = fsJsonParser.Parse(response.Text);
            Dictionary<string, CloudDataInfo> users = null;
            serializer.TryDeserialize(cloudDatas, ref users);
            List<CloudDataInfo> userLocalIds = new List<CloudDataInfo>();
            foreach (var user in users.Values)
            {
                if (DateTime.Parse(user.lastSavedTime) > DateTime.Parse("2019-08-30")&&!string.IsNullOrEmpty(user.name)&&!user.name.ToLower().Equals("null"))
                    userLocalIds.Add(user);
            }
            foreach (var authId in userLocalIds)
            {
                RestClient.Put(databaseRankURL + "/" + authId.localId + "/BattleRankPoint.json", "" + 1000 + "").Done(d =>
                {

                }, err =>
                {
                    Debug.Log(err.StackTrace);
                });
                RestClient.Put(databaseRankURL + "/" + authId.localId + "/BattleWin.json", "" + 0 + "").Done(d =>
                {

                }, err =>
                {
                    Debug.Log(err.StackTrace);
                });
                RestClient.Put(databaseRankURL + "/" + authId.localId + "/BattleLose.json", "" + 0 + "").Done(d =>
                {

                }, err =>
                {
                    Debug.Log(err.StackTrace);
                });
                RestClient.Put(databaseRankURL + "/" + authId.localId + "/Name.json", "\"" + authId.name + "\"").Done(d =>
                {

                }, err =>
                {
                    Debug.Log(authId.name);
                    Debug.Log(err.StackTrace);
                });
                RestClient.Put(databaseRankURL + "/" + authId.localId + "/Thumbnail.json", "" + 101 + "").Done(d =>
                {

                }, err =>
                {
                    Debug.Log(err.StackTrace);
                });
            }

        }, err =>
        {
            UI_StartManager.instance.ShowErrorUI(LocalizationManager.GetText("alertDBErrorMessage"));
            Debug.LogError(err.StackTrace);
        });
    }
    // 끝없는 전투 초기화 및 보상지급
    public void ResetEndlessBattleRankAll()
    {
        RestClient.Get(databaseRankURL + ".json").Progress(p =>
        {
            ShowProgressCircle(10);
        }).Done(response =>
        {
            fsData cloudDatas = fsJsonParser.Parse(response.Text);
            Dictionary<string, RankDataInfo> users = null;
            serializer.TryDeserialize(cloudDatas, ref users);
            List<RankDataInfo> userLocalIds = new List<RankDataInfo>();

            var rankResult = from player in users
                             where player.Value.InfinityRankPoint > 0 && !player.Value.Name.ToLower().Equals("null") && !string.IsNullOrEmpty(player.Value.Name) && !player.Key.Equals("PvLAUR9ZhBUfkNnvYicsgPti9vs2")
                             orderby player.Value.InfinityRankPoint descending
                             select player.Key;
            foreach (var authId in rankResult)
            {
                RestClient.Put(databaseRankURL + "/" + authId + "/InfinityRankPoint.json", "" + 0 + "")
                .Done(x =>
                {
                    Debugging.Log(authId + " 성공적으로 DB 저장완료");
                    ShowProgressCircle(100);
                }, er =>
                {
                    Debugging.LogError(authId + " 실패");
                    ShowProgressCircle(100);
                });
            }

        }, err =>
        {
            UI_StartManager.instance.ShowErrorUI(LocalizationManager.GetText("alertDBErrorMessage"));
            Debug.LogError(err.StackTrace);
        });
    }
    public void RewardEndlessBattleRankAll()
    {
        RestClient.Get(databaseRankURL + ".json").Progress(p =>
        {
            ShowProgressCircle(10);
        }).Done(response =>
        {
            fsData cloudDatas = fsJsonParser.Parse(response.Text);
            Dictionary<string, RankDataInfo> users = null;
            serializer.TryDeserialize(cloudDatas, ref users);
            List<RankDataInfo> userLocalIds = new List<RankDataInfo>();

            var rankResult = from player in users
                             where player.Value.InfinityRankPoint > 0 && !player.Value.Name.ToLower().Equals("null") && !string.IsNullOrEmpty(player.Value.Name) && !player.Key.Equals("PvLAUR9ZhBUfkNnvYicsgPti9vs2")
                             orderby player.Value.InfinityRankPoint descending
                             select player.Key;
            int rank = 0;
            int result = 0;
            foreach (var authId in rankResult)
            {
                rank++;
                if (rank == 1)
                    result = 3000;
                else if (rank == 2)
                    result = 1500;
                else if (rank == 3)
                    result = 1000;
                else if (rank >= 4 && rank <= 20)
                    result = 300;
                else 
                    result = 150;
                PushPostToUser(authId, 10002, result, "끝없는 전투 랭킹 보상입니다. !! ^-^");
            }

        }, err =>
        {
            UI_StartManager.instance.ShowErrorUI(LocalizationManager.GetText("alertDBErrorMessage"));
            Debug.LogError(err.StackTrace);
        });
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
            SignInWithGoogleOnFirebase(DBidToken);
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
    #endregion
}