using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManagement : MonoBehaviour
{
    public static StageManagement instance = null;
    public StageInfo stageInfo;
    public Transform Map;
    public Transform HeroPoint;
    bool isEndGame;
    float checkingHeroAliveTime;
    private static int kPoint;
    private static int dPoint;
    private static int MonsterCount;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {
        MonsterCount = 0;
        isEndGame = false;
        kPoint = 0;
        dPoint = 0;
        stageInfo = GameManagement.instance.GetStageInfo();
        stageInfo.initStage();
        UI_Manager.instance.Title.GetComponentInChildren<Text>().text = MapSystem.GetMap(stageInfo.mapNumber).name;
        if(Map!=null)
            MapSystem.SetMapSprite(stageInfo.stageNumber, Map);
        CharactersManager.instance.SetStagePositionHeros();
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
        User.coin += stageInfo.stageCoin;
        var getItems = GetStageItems();
        for(var i = 0; i < getItems.Count; i++)
        {
            ItemSystem.SetObtainItem(getItems[i].id);
        }

        Debugging.Log(string.Format("스테이지 클리어 코인 획득 >> {0}(+{1}",User.coin,stageInfo.stageCoin));
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
        stageGetSpeed = 100;
        stageMaxEnergy = 300;
        stageGetItems = new List<int>();

        Debugging.Log("스테이지 씬에 맵로딩 완료. >> " + this.stageNumber + " 스테이지의 " + this.mapNumber + " 맵");
    }
}
