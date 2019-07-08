using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManagement : MonoBehaviour
{
    public static StageManagement instance = null;
    public StageInfo stageInfo;
    public static UserInfo userInfo;
    public Transform Map;
    public Transform HeroPoint;
    bool isStartGame = false;
    bool isEndGame;
    float checkingHeroAliveTime;
    public Text mapnameText;
    private static int kPoint;
    private static int dPoint;
    private static int MonsterCount;
    public Common.StageModeType stageModeType;
    List<Hero> stageHeros = new List<Hero>();
    

    public Transform CastlePoint;
    public Transform BossPoint;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {
        isStartGame = false;
        stageModeType = Common.stageModeType;
        MonsterCount = 0;
        isEndGame = false;
        kPoint = 0;
        dPoint = 0;
        userInfo = new UserInfo();
        userInfo.initUserInfo();
        stageInfo = GameManagement.instance.GetStageInfo();
        stageInfo.initStage();
        if (stageModeType == Common.StageModeType.Main)
        {
            Map map = MapSystem.GetMap(stageInfo.mapNumber);
            SoundManager.instance.BgmSourceChange(AudioClipManager.instance.StageBgm);
            switch (map.stageType)
            {
                case 0:
                    UI_Manager.instance.Title.GetComponentInChildren<Text>().text = LocalizationManager.GetText("stageTitleMainType1");
                    InitCastle();
                    break;
                case 1:
                    UI_Manager.instance.Title.GetComponentInChildren<Text>().text = LocalizationManager.GetText("stageTitleMainType2");
                    break;
                case 2:
                    UI_Manager.instance.Title.GetComponentInChildren<Text>().text = LocalizationManager.GetText("stageTitleMainType3");
                    InitBoss();
                    break;
            }
            mapnameText.text = map.name;
            if (Map != null)
                MapSystem.SetMapSprite(stageInfo.stageNumber, ref Map);
        }
        else if (stageModeType == Common.StageModeType.Infinite)
        {
            SoundManager.instance.BgmSourceChange(AudioClipManager.instance.StageBgm);
            UI_Manager.instance.Title.GetComponentInChildren<Text>().text = LocalizationManager.GetText("stageTitleInfinity");
        }

        CharactersManager.instance.SetStagePositionHeros();
        StartCoroutine("StageStartEffect");
    }
    public Hero GetStageHero(int id)
    {
        stageHeros = new List<Hero>();
        foreach (var hero in Common.FindAlly())
        {
            stageHeros.Add(hero.GetComponent<Hero>());
        }
        foreach (var hero in stageHeros)
        {
            if(hero.id==id||hero.id.Equals(id))
            {
                return hero;
            }
        }
        return null;
    }
    private void InitCastle()
    {
        GameObject castle = Instantiate(PrefabsDatabaseManager.instance.GetCastlePrefab(1), CastlePoint);
        if(castle!=null)
        {
            castle.transform.position = CastlePoint.position+new Vector3(0,castle.transform.localScale.y);
            castle.gameObject.SetActive(true);
        }
    }
    private void InitBoss()
    {
        int bossId = 1000 + stageInfo.stageNumber;
        GameObject boss = Instantiate(PrefabsDatabaseManager.instance.GetMonsterPrefab(bossId), BossPoint);
        if (boss != null)
        {
            boss.transform.position = BossPoint.position;
            boss.gameObject.SetActive(true);
        }
    }
    private void ShowGoalTitle()
    {
        UI_Manager.instance.ShowTitle();
    }
    private void FixedUpdate()
    {
        if(Common.stageModeType==Common.StageModeType.Main)
        {
            MainModeUpdate();
        }
        else if(Common.stageModeType==Common.StageModeType.Infinite)
        {
            InfinityModeUpdate();
        }
    }
    private void MainModeUpdate()
    {
        if (stageInfo != null && !isEndGame)
        {
            stageInfo.stageTime += Time.fixedUnscaledDeltaTime;
            EnergyUpdate();
        }
    }
    private void InfinityModeUpdate()
    {
        if (stageInfo != null && !isEndGame)
        {
            stageInfo.stageTime += Time.fixedUnscaledDeltaTime;
            EnergyUpdate();

            checkingHeroAliveTime += Time.fixedUnscaledDeltaTime;
            if (checkingHeroAliveTime > 1.0f)
            {
                if (CheckHerosEnd())
                {
                    StageFail();
                }
                checkingHeroAliveTime = 0.0f;
            }
        }
    }
    IEnumerator StageStartEffect()
    {
        yield return new WaitForFixedUpdate();
        Common.isBlackUpDown = true;
        yield return new WaitForSeconds(7.0f);
        Common.isBlackUpDown = false;
        while (!Camera.main.GetComponent<CameraEffectHandler>().isBlackEffectClear)
        {
            yield return null;
        }
        HeroSkillManager.instance.ShowUI();
        ShowGoalTitle();
        isStartGame = true;
    }
    public bool isStageStart
    {
        get
        {
            return isStartGame;
        }
    }
    public void AddMonsterCount()
    {
        MonsterCount += 1;
    }
    public int GetMonsterCount()
    {
        return MonsterCount;
    }
    private bool CheckHerosEnd()
    {
        int cnt = 0;
        for (int i = 0; i < HeroPoint.childCount; i++)
        {
            if (HeroPoint.GetChild(i).gameObject.activeSelf)
                cnt++;
        }
        if (cnt == 0)
            return true;
        else
            return false;
    }
    private bool CheckAllyCastleEnd()
    {
        if (Common.allyTargetObject == null)
            return true;
        else
            return false;
    }
    public string GetStageTime()
    {
        return string.Format("{0:00}:{1:00}", (int)(stageInfo.stageTime % 3600 / 60), (int)(stageInfo.stageTime % 3600 % 60));
    }
    public int GetKPoint()
    {
        return kPoint;
    }
    public void SetKPoint()
    {
        kPoint += 1;
        MonsterCount -= 1;
    }
    public int GetDPoint()
    {
        return dPoint;
    }
    public void SetDPoint()
    {
        dPoint += 1;
    }

    public void StageClear()
    {
        StartCoroutine("StageClearing");
    }
    public void StageFail()
    {
        StartCoroutine("StageFailing");
    }
    public IEnumerator StageFailing()
    {
        HeroSystem.SaveHeros(Common.FindAlly());
        isEndGame = true;
        isStartGame = false;
        GoogleSignManager.SaveData();
        UI_Manager.instance.OpenEndGamePanel(false);
        GoogleAdMobManager.instance.OnBtnViewAdClicked();
        yield return null;
    }

    public IEnumerator StageClearing()
    {
        isStartGame = false;
        isEndGame = true;
        HeroSystem.SaveHeros(Common.FindAlly());
        MapSystem.MapClear(stageInfo.mapNumber, stageInfo.stageClearPoint);
        MissionSystem.AddClearPoint(MissionSystem.ClearType.StageClear);
        MissionSystem.PointSave();
        SaveSystem.AddUserCoin(stageInfo.stageCoin);
        SaveSystem.ExpUp(stageInfo.stageExp);
        var getItems = GetStageItems();
        for (var i = 0; i < getItems.Count; i++)
        {
            ItemSystem.SetObtainItem(getItems[i].id);
        }
        GoogleSignManager.SaveData();
        UI_Manager.instance.OpenEndGamePanel(true);
        GoogleAdMobManager.instance.OnBtnViewAdClicked();
        yield return null;
    }
    public UserInfo GetUserInfo()
    {
        return userInfo;
    }
    private void EnergyUpdate()
    {
        if (stageInfo.stageGetTime > 1)
        {
            stageInfo.stageEnergy += stageInfo.stageGetEnergy;
            stageInfo.stageEnergy = Mathf.Clamp(stageInfo.stageEnergy, 0, stageInfo.stageMaxEnergy);
            stageInfo.stageGetTime = 0;
        }
        else
        {
            stageInfo.stageGetTime += Time.deltaTime;
        }
    }
    public int GetStageEnergy()
    {
        return stageInfo.stageEnergy;
    }
    public void DrainStageEnergy()
    {
        int drainEnergy = stageInfo.mapNumber;
        if (stageInfo.stageEnergy - drainEnergy < 0)
            stageInfo.stageEnergy = 0;
        else
            stageInfo.stageEnergy -= drainEnergy;
    }

    public bool IsSkillAble(int skillenergy)
    {
        if (stageInfo.stageEnergy - skillenergy < 0)
            return false;
        else
        {
            return true;
        }
    }
    public void AddExp(int amount)
    {
        stageInfo.stageExp += amount;
    }

    public void UseSkill(int skillenergy)
    {
        stageInfo.stageEnergy -= skillenergy;
    }
    public void AddGetStageItem(int id)
    {
        stageInfo.stageGetItems.Add(id);
    }
    public List<Item> GetStageItems()
    {
        List<Item> getItemList = new List<Item>();
        if(stageInfo.stageGetItems!=null&stageInfo.stageGetItems.Count>0)
        {
            foreach(var itemId in stageInfo.stageGetItems)
            {
                Item getItem = ItemSystem.GetItem(itemId);
                if(getItem!=null)
                    getItemList.Add(getItem);
            }
        }
        return getItemList;
    }
    public void OnClickNextStage()
    {
        GameManagement.instance.SetStageInfo(stageInfo.mapNumber+1);
        SaveSystem.SavePlayer();
        LoadSceneManager.instance.LoadStageScene();
    }


}

public class StageInfo
{
    public int stageNumber;
    public int mapNumber;
    public int stageExp;
    public int stageCoin;
    public int stageEnergy;
    public int stageMaxEnergy;
    public float stageTime;
    public float stageGetTime;
    public int stageGetEnergy;
    public int stageClearPoint;
    public List<int> stageGetItems;
    public StageInfo() { }
    public StageInfo(int mapId)
    {
        this.mapNumber = mapId;
        this.stageNumber = MapSystem.GetMap(mapId).stageNumber;
    }
    public void initStage()
    {
        stageEnergy = 0;
        stageCoin = 0;
        stageTime = 0;
        stageGetTime = 0;
        stageGetEnergy = LabSystem.ChargeEnergy;
        stageMaxEnergy = LabSystem.MaxEnergy;
        stageExp = 0;
        stageGetItems = new List<int>();
        stageClearPoint = 0;

        Debugging.Log("스테이지 씬에 맵로딩 완료. >> " + this.stageNumber + " 스테이지의 " + this.mapNumber + " 맵");
    }
}
public class UserInfo
{
    public int level;
    public int exp;
    public int departExp;
    public bool isLevelUp;

    public void initUserInfo()
    {
        level = User.level;
        exp = User.exp;
        departExp = 0;
        isLevelUp = false;
        Debugging.Log(level + " > " + exp + " 영웅 레벨 정보 로드 완료.");
    }
    public void LevelUp()
    {
        if(exp>Common.USER_EXP_TABLE[level-1])
        {
            level += 1;
            isLevelUp = true;
            exp = 0;
        }
    }


}
