using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Common : MonoBehaviour
{
    public static int[] EXP_TABLE = { 100, 300, 400, 700, 1100, 1800, 2900, 4700, 7800, 10000 };
    public static int[] USER_EXP_TABLE = { 1000, 2000, 3000, 5000, 8000, 13000, 21000, 34000, 55000, 89000, 144000 };

    public static Common instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    public static GameObject GetPrefabDatabase()
    {
        return GameObject.Find("PrefabDatabase").gameObject;
    }
    public static GameObject GetCameraObject()
    {
        return Camera.main.gameObject;
    }
    public static bool isDataLoadSuccess = false;

    public static List<GameObject> EnemysList;
    public static List<GameObject> AllysList;

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

    public static List<GameObject> FindEnemy()
    {
        List<GameObject> enemys = new List<GameObject>();
        GameObject enemyObjects = GameObject.Find("EnemysHero").gameObject;
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

    public static List<GameObject> FindAlly()
    {
        List<GameObject> allys = new List<GameObject>();
        GameObject userObjects = GameObject.Find("PlayersHero").gameObject;
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
    public enum OrderByType
    {
        NONE,
        NAME,
        VALUE,
    }

    public enum NPC_APPERANCE
    {
        Npc001,
        Npc002,
        Npc003,
        Npc004,
        Npc005,
        Npc006,
        Npc007,
        Npc008,
        Npc009,
        Npc010,
        Npc011,
        Npc012,
        Npc013,
        Monster002,
        Random
    };
    public enum EventType
    {
        CameraShake,
        CameraSlow,
        CameraBlack,
        CameraSizing,
        None
    }
    public static float cameraOrthSize;


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
    public static void RELOAD_GAME()
    {
        Common.isStart = false;
        Common.isAwake = false;
    }

    public static void START_GAME()
    {
        if (!Common.isStart)
        {
            Common.isStart = true;
            Common.isAwake = false;
        }

    }

    public static void AWAKE_GAME()
    {
        if (!Common.isAwake)
        {
            Common.triggerObjectInitialize = true;
            Common.isAwake = true;
        }

    }

    public static bool triggerObjectInitialize;

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

    public static void Chat(string chat, Transform tran = null, int posY = 0)
    {
        if (tran == null)
            return;

        if(tran.GetComponentInChildren<faceOff>()!=null)
        {
            tran.GetComponentInChildren<faceOff>().Face_Mouse();
        }
        GameObject chatObj = ObjectPool.Instance.PopFromPool("chatBox");
        chatObj.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        posY = posY == 0 ? 1 : posY;
        if (tran.rotation.y != 0)
        {
            chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

        }
        else
        {
            chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        }
        chatObj.GetComponent<UI_chatBox>().correctionY = posY;
        chatObj.GetComponent<UI_chatBox>().Target = tran;
        chatObj.GetComponent<UI_chatBox>().chatText = chat;
        chatObj.SetActive(true);
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
        return Vector3.Distance(GetBottomPosition(me), GetBottomPosition(target));
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
        float finalDam = Mathf.Clamp(dam * 100/(100+defence), 1, 9999);
        return (int)finalDam;
    }

  

    public enum SCENE { START,MAIN,STAGE,TRAINING};
    public static SCENE Scene;
    public static bool GetSceneCompareTo(SCENE sCene)
    {
        return (int)sCene == SceneManager.GetActiveScene().buildIndex;
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

}
