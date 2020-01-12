using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Common : MonoBehaviour
{
    public static int RankOfInfinityMode;
    public static int RankOfHeroLevel;
    public static int RankOfPvp;
    public static int RankOfAttackMode;
    public static string postItemDatas;
    public static bool isLoadCompleted;
    public static PvpData pvpEnemyData;
    public static string pvpEnemyLocalId;
    public static int bossModeDifficulty;
    public static bool IsAutoStagePlay;
    public static int GetHeroNeedExp(int level)
    {
        return 100 + (int)(100 * level * level * 0.1f);
    }
    public static int GetUserNeedExp()
    {
        return 1000 + (int)(1000 * User.level * User.level * 0.1f);
    }
    public static string GetRankText(int rankPoint)
    {
        if (rankPoint >= 0 && rankPoint <= 800)
            return "D";
        else if (rankPoint > 800 && rankPoint <= 1100)
            return "C";
        else if (rankPoint > 1100 && rankPoint <= 1500)
            return "B";
        else if (rankPoint > 1500 && rankPoint <= 1900)
            return "A";
        else if (rankPoint > 1900 && rankPoint <= 2300)
            return "S";
        else if (rankPoint > 2300 && rankPoint <= 2700)
            return "SS";
        else if (rankPoint > 2700 && rankPoint <= 3000)
            return "SSS";
        else
            return "Lenged";
    }
    public static string getRomeNumber(int n)
    {
        if (n < 10 && n > 0)
        {
            string[] romeNumbers = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
            return romeNumbers[n - 1];
        }
        return "";
    }
    public static int GetRank(int rankPoint)
    {
        if (rankPoint >= 0 && rankPoint <= 800)
            return 1;
        else if (rankPoint > 800 && rankPoint <= 1100)
            return 2;
        else if (rankPoint > 1100 && rankPoint <= 1500)
            return 3;
        else if (rankPoint > 1500 && rankPoint <= 1900)
            return 4;
        else if (rankPoint > 1900 && rankPoint <= 2300)
            return 5;
        else if (rankPoint > 2300 && rankPoint <= 2700)
            return 6;
        else if (rankPoint > 2700 && rankPoint <= 3000)
            return 7;
        else
            return 8;
    }
    public static int GetHeroExp(int level)
    {
        int exp = 10 + (int)(10 * level * 0.5f);
        int bonusExp = (int)(exp * LabSystem.GetAddExp(User.addExpLevel) * 0.01f);
        return exp + bonusExp;
    }
    public static int GetUserExp(int stageNumber)
    {
        int exp = 300 + (int)(300 * User.level * 0.1f*stageNumber);
        return exp;
    }
    public static int GetMonsterCoin(int level)
    {
        int coin = UnityEngine.Random.Range(30,50) + (int)(30 * level * 0.1f);
        int bonusCoin = (int)(coin * LabSystem.GetAddMoney(User.addMoneyLevel) * 0.01f);
        return coin + bonusCoin;
    }
    public static ulong lastLoginTime;
    public static void LastLoginTimeSave()
    {
        lastLoginTime = (ulong)DateTime.Now.Ticks;
        PlayerPrefs.SetString("LastLogInTime", Common.lastLoginTime.ToString());
    }

    public static bool ValidateName(string name)
    {
        Regex engRegex = new Regex(@"[a-zA-Z]");
        bool ismatch = engRegex.IsMatch(name);
        Regex numRegex = new Regex(@"[0-9]");
        bool ismatchNum = numRegex.IsMatch(name);
        Regex korRegex = new Regex(@"[가-힣]");
        bool ismatchKor = korRegex.IsMatch(name);

        if (!ismatch&&!ismatchNum&&!ismatchKor)
        {
            Debugging.Log("올바르지 않은 이름 포맷");
            return false;
        }
        int nameByteCount = Encoding.Default.GetByteCount(name);
        if(nameByteCount<4||nameByteCount>18)
        {
            Debugging.Log(string.Format("{0} > {1}", name.Length, nameByteCount));
            return false;
        }
        return true;
    }

    public static string GoogleUserId;
    public static Common instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    public static List<GameObject> EnemysList;
    public static List<GameObject> AllysList;

    public static string GetCoinCrystalEnergyText(int type)
    {
        string txt = "";
        switch (type)
        {
            case 0:
                txt = LocalizationManager.GetText("Coin");
                break;
            case 1:
                txt = LocalizationManager.GetText("Crystal");
                break;
            case 2:
                txt = LocalizationManager.GetText("Energy");
                break;
            case 3:
                txt = LocalizationManager.GetText("Ad");
                break;
            case 4:
                txt = LocalizationManager.GetText("Cash");
                break;
            case 5:
                txt = LocalizationManager.GetText("MagicStone");
                break;
            case 6:
                txt = LocalizationManager.GetText("TranscendenceStone");
                break;
        }
        return txt;
    }
    public static string GetCoinCrystalEnergyImagePath(int type)
    {
        string path = "";
        switch(type)
        {
            case 0:
                path = "Items/coin";
                break;
            case 1:
                path = "Items/blackCrystal";
                break;
            case 2:
                path = "Items/portalEnergy";
                break;
            case 3:
                path = "UI/ui_ad";
                break;
            case 4:
                path = "UI/won";
                break;
            case 5:
                path = "UI/magicStone";
                break;
            case 6:
                path = "UI/transcendenceStone";
                break;
            case 7:
                path = "Items/abilityScroll";
                break;
            case 8:
                path = "Items/autoPlayTicket";
                break;
        }
        return path;
    }
    public static void ChangePlayerProfileImage()
    {
        FindObjectOfType<UI_UserProfile>().ChangeProfile();
    }
    public static int FindEnemysCount()
    {
        GameObject enemyObjects = GameObject.Find("EnemysHero").gameObject;
        int count = 0;
        if(enemyObjects!=null)
        {
            foreach (var enemy in enemyObjects.GetComponentsInChildren<Hero>())
            {
                if (!enemy.isDead)
                    count += 1;
            }
        }
        return count;
    }
    public static List<GameObject> FindEnemysByDistance(bool isAlly, bool isLeftOrRight,Transform me ,float distance)
    {
        List<GameObject> enemys = new List<GameObject>();
        if(isLeftOrRight)
        {
            GameObject enemyObjects = null;
            if (isAlly)
                enemyObjects = GameObject.Find("EnemysHero").gameObject;
            else
                enemyObjects = GameObject.Find("PlayersHero").gameObject;
            if (enemyObjects.transform.childCount > 0)
            {
                for (var i = 0; i < enemyObjects.transform.childCount; i++)
                {
                    if (enemyObjects.transform.GetChild(i).gameObject.activeSelf && enemyObjects.transform.GetChild(i).GetComponent<Hero>() != null && !enemyObjects.transform.GetChild(i).GetComponent<Hero>().isDead)
                    {
                        if(enemyObjects.transform.GetChild(i).position.x< me.position.x&&GetDistanceBetweenAnother(me, enemyObjects.transform.GetChild(i).transform)<distance)
                        {
                            enemys.Add(enemyObjects.transform.GetChild(i).gameObject);
                        }
                    }
                }
            }
        }
        else
        {
            GameObject enemyObjects = null;
            if (isAlly)
                enemyObjects = GameObject.Find("EnemysHero").gameObject;
            else
                enemyObjects = GameObject.Find("PlayersHero").gameObject;
            if (enemyObjects.transform.childCount > 0)
            {
                for (var i = 0; i < enemyObjects.transform.childCount; i++)
                {
                    if (enemyObjects.transform.GetChild(i).gameObject.activeSelf && enemyObjects.transform.GetChild(i).GetComponent<Hero>() != null && !enemyObjects.transform.GetChild(i).GetComponent<Hero>().isDead)
                    {
                        if (enemyObjects.transform.GetChild(i).position.x > me.position.x && GetDistanceBetweenAnother(me, enemyObjects.transform.GetChild(i).transform) < distance)
                        {
                            enemys.Add(enemyObjects.transform.GetChild(i).gameObject);
                        }
                    }
                }
            }
        }
        return enemys;
    }
    public static List<GameObject> FindEnemysByDistance(bool isAlly, Transform me, float distance)
    {
        List<GameObject> enemys = new List<GameObject>();
        GameObject enemyObjects = null;
        if (isAlly)
            enemyObjects = GameObject.Find("EnemysHero").gameObject;
        else
            enemyObjects = GameObject.Find("PlayersHero").gameObject;
        if (enemyObjects!=null)
        {
            foreach(var h in enemyObjects.GetComponentsInChildren<Hero>())
            {
                if(!h.isDead)
                {
                    enemys.Add(h.gameObject);
                }
            }
        }
        return enemys;
    }
    public static List<GameObject> FindTutorialEnemy()
    {
        List<GameObject> enemys = new List<GameObject>();
        GameObject enemyObjects = GameObject.Find("EnemysHero").gameObject;
        if (enemyObjects.transform.childCount > 0)
        {
            for (var i = 0; i < enemyObjects.transform.childCount; i++)
            {
                if (enemyObjects.transform.GetChild(i).gameObject.activeSelf && enemyObjects.transform.GetChild(i).GetComponent<TutorialHero>() != null && !enemyObjects.transform.GetChild(i).GetComponent<TutorialHero>().isDead)
                {
                    enemys.Add(enemyObjects.transform.GetChild(i).gameObject);
                }
            }
        }
        //Debugging.Log(enemys.Count + " 개의 적 발견");
        return enemys;
    }
    public static List<GameObject> FindEnemy(bool isAlly)
    {
        List<GameObject> enemys = new List<GameObject>();
        GameObject enemyObjects = null;
        if (isAlly)
            enemyObjects = GameObject.Find("EnemysHero").gameObject;
        else
            enemyObjects = GameObject.Find("PlayersHero").gameObject;
        if (enemyObjects.transform.childCount > 0)
        {
            for (var i = 0; i < enemyObjects.transform.childCount; i++)
            {
                if (enemyObjects.transform.GetChild(i).gameObject.activeSelf&&enemyObjects.transform.GetChild(i).GetComponent<Hero>() != null && !enemyObjects.transform.GetChild(i).GetComponent<Hero>().isDead)
                {
                    enemys.Add(enemyObjects.transform.GetChild(i).gameObject);
                }
            }
        }
        //Debugging.Log(enemys.Count + " 개의 적 발견");
        return enemys;
    }
    public static List<GameObject> FindDeadAlly()
    {
        List<GameObject> allys = new List<GameObject>();
        GameObject userObjects = GameObject.Find("PlayersHero").gameObject;
        if (userObjects.transform.childCount > 0)
        {
            for (var i = 0; i < userObjects.transform.childCount; i++)
            {
                if (userObjects.transform.GetChild(i).GetComponent<Hero>() != null && userObjects.transform.GetChild(i).GetComponent<Hero>().isDead)
                {
                    allys.Add(userObjects.transform.GetChild(i).gameObject);
                }
            }
        }
        //Debugging.Log(allys.Count + " 개의 아군 발견");
        return allys;
    }
    public static List<GameObject> FindStageHeros()
    {
        List<GameObject> allys = new List<GameObject>();
        GameObject userObjects = GameObject.Find("PlayersHero").gameObject;
        if (userObjects.transform.childCount > 0)
        {
            for (var i = 0; i < userObjects.transform.childCount; i++)
            {
                if (userObjects.transform.GetChild(i).GetComponent<Hero>() != null)
                {
                    allys.Add(userObjects.transform.GetChild(i).gameObject);
                }
            }
        }
        return allys;
    }
    public static List<GameObject> FindPlayerHero()
    {
        List<GameObject> allys = new List<GameObject>();
        GameObject userObjects = GameObject.Find("PlayersHero").gameObject;
        if (userObjects.transform.childCount > 0)
        {
            for (var i = 0; i < userObjects.transform.childCount; i++)
            {
                if (userObjects.transform.GetChild(i).GetComponent<Hero>() != null)
                {
                    allys.Add(userObjects.transform.GetChild(i).gameObject);
                }
            }
        }
        //Debugging.Log(allys.Count + " 개의 아군 발견");
        return allys;
    }

    public static List<GameObject> FindAlly(bool isAlly)
    {
        List<GameObject> allys = new List<GameObject>();
        GameObject userObjects = null;
        if(isAlly)
            userObjects = GameObject.Find("PlayersHero").gameObject;
        else
            userObjects = GameObject.Find("EnemysHero").gameObject;
        if (userObjects.transform.childCount > 0)
        {
            for (var i = 0; i < userObjects.transform.childCount; i++)
            {
                if (userObjects.transform.GetChild(i).gameObject.activeSelf&&userObjects.transform.GetChild(i).GetComponent<Hero>() != null && !userObjects.transform.GetChild(i).GetComponent<Hero>().isDead)
                {
                    allys.Add(userObjects.transform.GetChild(i).gameObject);
                }
            }
        }
        //Debugging.Log(allys.Count + " 개의 아군 발견");
        return allys;
    }
    public static List<GameObject> FindAll()
    {
        List<GameObject> all = new List<GameObject>();
        GameObject allObjects = GameObject.Find("Characters").gameObject;
        if (allObjects.transform.childCount > 0)
        {
            for (var i = 0; i < allObjects.transform.childCount; i++)
            {
                if (allObjects.transform.GetChild(i).GetComponent<Hero>() != null && !allObjects.transform.GetChild(i).GetComponent<Hero>().isDead)
                {
                    all.Add(allObjects.transform.GetChild(i).gameObject);
                }
            }
        }
        //Debugging.Log(all.Count + " 개의 적 발견");
        return all;
    }
    //정렬타입
    public enum OrderByType{    NONE,NAME,VALUE }
    public enum EventType{  CameraShake,CameraSlow,CameraBlack,CameraSizing,None    }
    public enum StageModeType { Main, Infinite, Boss,Battle,Attack,Raid}

    public static StageModeType stageModeType;

    public static void CameraAllEffectOff()
    {
        isShake = false;
        isBlackUpDown = false;
        isHitShake = false;
    }
    public static bool isAwake = false;

    public static bool isStart;

    public static bool isShake;

    public static bool isHitShake;

    public static bool isBlackUpDown;

    public static GameObject hitTargetObject;
    public static GameObject allyTargetObject;

    public static int looMinus(int hp, int amount)
    {
        if (hp - amount <= 0)
            return 0;
        else
            return hp - amount;
    }
    public static int looHpPlus(int hp, int hpFull, int amount)
    {
        if (hp + amount >= hpFull)
            return hpFull;
        else
            return hp + amount;
    }

    public static void SetLayerRecursively(GameObject obj, int newLayer, int defaultLayer)
    {
        if (null == obj)
        {
            return;
        }
        obj.layer = obj.layer == defaultLayer ? newLayer : obj.layer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer, defaultLayer);
        }
    }

    public static float IncrementOrDecrementTowards(float n, float target, float a, bool isDecrement = false)
    {
        if (n == target)
        {
            return n;
        }
        else
        {
            float dir = Mathf.Sign(target - n);
            n += a * Time.deltaTime * dir;
            if (isDecrement)
                return (dir == Mathf.Sign(target + n)) ? n : target;
            else
                return (dir == Mathf.Sign(target - n)) ? n : target;
        }
    }

    public static void Chat(string chat, Transform tran = null, int posY = 0, int leftOrRight = 0)
    {
        if (tran == null)
            return;

        if(tran.GetComponentInChildren<faceOff>()!=null)
        {
            tran.GetComponentInChildren<faceOff>().Face_Mouse();
        }
        GameObject chatObj = ObjectPool.Instance.PopFromPool("chatBox");
        chatObj.GetComponent<Image>().color = Color.white;
        chatObj.GetComponentInChildren<Text>().color = new Color(0.1f, 0.1f, 0.1f, 1f);
        posY = posY == 0 ? 1 : posY;
        if(leftOrRight == 0)
        {
            if (tran.rotation.y == 0)
            {
                chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

            }
            else
            {
                chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

            }
        }
        else
        {
            if(leftOrRight == 1)
            {
                chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else if(leftOrRight==2)
            {
                chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }

        chatObj.GetComponent<UI_chatBox>().correctionY = posY;
        chatObj.GetComponent<UI_chatBox>().Target = tran;
        chatObj.GetComponent<UI_chatBox>().chatText = chat;
        chatObj.SetActive(true);
    }

    public static void SkillChat(string chat, Transform tran = null, int posY = 0, int leftOrRight = 0)
    {
        if (tran == null)
            return;

        if (tran.GetComponentInChildren<faceOff>() != null)
        {
            tran.GetComponentInChildren<faceOff>().Face_Mouse();
        }
        GameObject chatObj = ObjectPool.Instance.PopFromPool("chatBox");
        chatObj.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 1f);
        chatObj.GetComponentInChildren<Text>().color = Color.white;
        posY = posY == 0 ? 1 : posY;
        if (leftOrRight == 0)
        {
            if (tran.rotation.y == 0)
            {
                chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

            }
            else
            {
                chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

            }
        }
        else
        {
            if (leftOrRight == 1)
            {
                chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else if (leftOrRight == 2)
            {
                chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }

        chatObj.GetComponent<UI_chatBox>().correctionY = posY;
        chatObj.GetComponent<UI_chatBox>().Target = tran;
        chatObj.GetComponent<UI_chatBox>().chatText = chat;
        chatObj.SetActive(true);
    }

    public static void Message(string msg, Transform tran = null)
    {
        GameObject msgObj = ObjectPool.Instance.PopFromPool("messageBox");
        msgObj.SetActive(true);
        msgObj.GetComponent<UI_messageBox>().StartMessage(msg, tran);
    }

    //페이먼트 체크
    public static bool PaymentAbleCheck(ref int target, int payment)
    {
        if (target - payment >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static void PaymentStart(ref int target, int payment)
    {
        if (target == User.coin || target.Equals(User.coin))
        {
            MissionSystem.AddClearPoint(MissionSystem.ClearType.TotalUseCoin, payment);
        }
        else if (target == User.blackCrystal || target.Equals(User.blackCrystal))
        {
            MissionSystem.AddClearPoint(MissionSystem.ClearType.TotalUseCrystal, payment);
        }
        else if (target == User.portalEnergy || target.Equals(User.portalEnergy))
        {
            MissionSystem.AddClearPoint(MissionSystem.ClearType.TotalUseEnergy, payment);
        }
        else if (target == User.transcendenceStone || target.Equals(User.transcendenceStone))
        {

        }
        target -= payment;
        GoogleSignManager.SaveData();
    }

    public static bool PaymentCheck(ref int target, int payment)
    {
        if(PaymentAbleCheck(ref target,payment))
        {
            PaymentStart(ref target, payment);
            GoogleSignManager.SaveData();
            return true;
        }
        else
        {
            return false;
        }
    }

    //배열 중복제거
    public static T[] GetDistinctValues<T>(T[] array)
    {
        List<T> tmp = new List<T>();

        for (int i = 0; i < array.Length; i++)
        {
            if (tmp.Contains(array[i]))
                continue;
            tmp.Add(array[i]);
        }
        return tmp.ToArray();
    }
    //오브젝트들의 가운데 위치
    public static float GetCenterPositionXwithHeros()
    {
        var heros = FindAlly(true);
        if(heros!=null&&heros.Count>0)
        {
            float minX = heros[0].transform.position.x;
            float maxX = heros[0].transform.position.x;
            for(int i = 0; i< heros.Count; i++)
            {
                if(minX > heros[i].transform.position.x)
                {
                    minX = heros[i].transform.position.x;
                }
            }
            for(int i =0; i<heros.Count; i++)
            {
                if(maxX < heros[i].transform.position.x)
                {
                    maxX = heros[i].transform.position.x;
                }
            }
            if (minX < maxX - 7)
                minX = maxX - 7;
            return (minX + maxX) / 2;
        }
        return 0;
    }
    //오브젝트 바닥위치
    public static Vector3 GetBottomPosition(Transform transform)
    {
        Vector3 v3Pos = transform.position;
        v3Pos.y = 0;
        return v3Pos;
    }
    //오브젝트 와의거리
    public static float GetDistanceBetweenAnother(Transform me, Transform target)
    {
        return Vector2.Distance(GetBottomPosition(me), GetBottomPosition(target));
    }

    //1000단위 숫자 , 찍기
    public static string GetThousandCommaText(int data)
    {
        if (data == 0)
            return "0";
        return string.Format("{0:#,###}", data);
    }

    public static int GetHeroPower(Hero hero)
    {
        int attackPower = (hero.status.attack * (int)(hero.status.attackSpeed + 1)*10) * 3;
        int defencePower = hero.status.maxHp * 2;
        int etcPower = (int)(hero.status.moveSpeed * 10);

        int totalPower = attackPower + defencePower + etcPower;
        //Debugging.Log(hero.HeroName + " 의 파워포인트 > " + totalPower);
        return totalPower;
    }

    public static int GetTierHero(Hero hero)
    {
        int tier = 0;
        int heroPower = GetHeroPower(hero);

        if (heroPower >= 0 && heroPower < 1000)
            tier = 1;
        else if (heroPower >= 1000 && heroPower < 2000)
            tier = 2;
        else if (heroPower >= 2000 && heroPower < 4000)
            tier = 3;
        else if (heroPower >= 4000 && heroPower < 8000)
            tier = 4;
        else
            tier = 5;
        return tier;
    }

    public static int GetDamage(int dam, int defence)
    {
        float finalDam = Mathf.Clamp(dam * 100/(100+defence), 1, 9999+LabSystem.GetAddMaxDamage(User.addMaxDamageLevel));
        return (int)finalDam;
    }
    public static int GetPvpDamage(int dam, int defence, int maxDamLevel)
    {
        float finalDam = Mathf.Clamp(dam * 100 / (100 + defence), 1, 9999+LabSystem.GetAddMaxDamage(maxDamLevel));
        return (int)finalDam;
    }

    public enum SCENE { START,MAIN,STAGE,INFINITE,BOSS,TUTORIAL,INTRO,BATTLE,ATTACK};
    public static SCENE Scene;
    public static bool GetSceneCompareTo(SCENE sCene)
    {
        return (int)sCene == SceneManager.GetActiveScene().buildIndex;
    }
    public static int GetSceneNumber()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    public static string GetRandomID(int cnt)
    {
        System.Random rand = new System.Random();
        string input = "abcdefghijklmnopqrstuvwxyz0123456789";
        var chars = Enumerable.Range(0, cnt).Select(x => input[rand.Next(0, input.Length)]);
        return new string(chars.ToArray()).ToUpper();
    }


    public static Transform CanvasUI()
    {
        return GameObject.Find("CanvasUI").transform;
    }

    public static int[] getRandomId(int length, int min, int max)
    {
        int[] randArray = new int[length];
        bool isSame;

        for(int i = 0; i<length; i++)
        {
            while(true)
            {
                randArray[i] = UnityEngine.Random.Range(min, max);
                isSame = false;

                for(int j = 0; j <i; ++j)
                {
                    if(randArray[j]==randArray[i])
                    {
                        isSame = true;
                        break;
                    }
                }
                if (!isSame) break;
            }
        }
        return randArray;
    }

    public static int GetRandomItemId(List<Item> userEquipmentItems)
    {
        int maxId = 0;
        for(var i = 0; i < userEquipmentItems.Count; i++)
        {
            if (userEquipmentItems[i].itemtype < 100)
            {
                if (maxId < userEquipmentItems[i].customId)
                    maxId = userEquipmentItems[i].customId;
            }
            else
                continue;

        }
        maxId += 1;
        return maxId;
    }

    public static void DestroyAllDontDestroyOnLoadObjects()
    {

        var go = new GameObject("Sacrificial Lamb");
        DontDestroyOnLoad(go);

        foreach (var root in go.scene.GetRootGameObjects())
        {
            if(!root.name.Equals("StartManager")&&root.name!="StartManager")
                Destroy(root);
            else
            {
                foreach(Transform child in root.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }


    }
}
