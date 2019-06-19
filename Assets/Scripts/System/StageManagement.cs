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
    bool isEndGame;
    float checkingHeroAliveTime;
    public Text mapnameText;
    private static int kPoint;
    private static int dPoint;
    private static int MonsterCount;
    public Common.StageModeType stageModeType;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {
        stageModeType = Common.stageModeType;
        MonsterCount = 0;
        isEndGame = false;
        kPoint = 0;
        dPoint = 0;
        userInfo = new UserInfo();
        userInfo.initUserInfo();
        stageInfo = GameManagement.instance.GetStageInfo();
        stageInfo.initStage();
        if(stageModeType==Common.StageModeType.Main)
        {
            Map map = MapSystem.GetMap(stageInfo.mapNumber);
            SoundManager.instance.BgmSourceChange(AudioClipManager.instance.Bgm2);
            switch (map.stageType)
            {
                case 0:
                    UI_Manager.instance.Title.GetComponentInChildren<Text>().text = "플랫 에너지를 흡수하는 구조물 파괴.";
                    break;
                case 1:
                    UI_Manager.instance.Title.GetComponentInChildren<Text>().text = "모든 적을 섬멸.";
                    break;
                case 2:
                    UI_Manager.instance.Title.GetComponentInChildren<Text>().text = "보스 격퇴.";
                    break;
            }
            mapnameText.text = map.name;
            if (Map != null)
                MapSystem.SetMapSprite(stageInfo.stageNumber, ref Map);
        }
        else if(stageModeType==Common.StageModeType.Infinite)
        {
            SoundManager.instance.BgmSourceChange(AudioClipManager.instance.Bgm2);
            UI_Manager.instance.Title.GetComponentInChildren<Text>().text = "가능할 때까지 모든적을 섬멸.";
        }

        CharactersManager.instance.SetStagePositionHeros();
        StartCoroutine("StageStartEffect");
    }
    private void ShowGoalTitle()
    {
        UI_Manager.instance.ShowTitle();
    }
    private void FixedUpdate()
    {
        if(stageInfo!=null&&!isEndGame)
        {
            stageInfo.stageTime += Time.fixedUnscaledDeltaTime;
            EnergyUpdate();

            checkingHeroAliveTime += Time.fixedUnscaledDeltaTime;
            if(checkingHeroAliveTime>1.0f)
            {
                if(CheckHerosEnd())
                {
                    HeroSystem.SaveHeros(Common.FindAlly());
                    UI_Manager.instance.OpenEndGamePanel(false);
                    isEndGame = true;
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
        while(!Camera.main.GetComponent<CameraEffectHandler>().isBlackEffectClear)
        {
            yield return null;
        }
        HeroSkillManager.instance.ShowUI();
        ShowGoalTitle();
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
        for(int i = 0; i < HeroPoint.childCount; i++)
        {
            if (HeroPoint.GetChild(i).gameObject.activeSelf)
                cnt++;
        }
        if (cnt == 0)
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
        SaveSystem.AddUserCoin(stageInfo.stageCoin);
        SaveSystem.ExpUp(stageInfo.stageExp);
        SaveSystem.SavePlayer();
        var getItems = GetStageItems();
        for(var i = 0; i < getItems.Count; i++)
        {
            ItemSystem.SetObtainItem(getItems[i].id);
        }
    }
    public UserInfo GetUserInfo()
    {
        return userInfo;
    }
    private void EnergyUpdate()
    {
        if (stageInfo.stageGetTime > 1)
        {
            stageInfo.stageEnergy += 1;
            stageInfo.stageEnergy = Mathf.Clamp(stageInfo.stageEnergy, 0, stageInfo.stageMaxEnergy);
            stageInfo.stageGetTime = 0;
        }
        else
        {
            stageInfo.stageGetTime += Time.deltaTime* stageInfo.stageGetSpeed;
        }
    }
    public int GetStageEnergy()
    {
        return stageInfo.stageEnergy;
    }
    public void DrainStageEnergy()
    {
        stageInfo.stageEnergy -= MonsterCount*3;
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
    public float stageGetSpeed;
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
        stageGetSpeed = 10;
        stageMaxEnergy = 100;
        stageExp = 0;
        stageGetItems = new List<int>();

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
