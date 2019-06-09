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
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {
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
            CoinUpdate();

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
    }
    public int GetDPoint()
    {
        return dPoint;
    }
    public void SetDPoint()
    {
        dPoint += 1;
    }

    private void CoinUpdate()
    {
        if (stageInfo.cointGetTime > 1)
        {
            stageInfo.stageCoin += 1;
            stageInfo.cointGetTime = 0;
        }
        else
        {
            stageInfo.cointGetTime += Time.deltaTime;
        }
    }
}

public class StageInfo
{
    public int stageNumber;
    public int mapNumber;
    public int stageCoin;
    public float stageTime;
    public float cointGetTime;
    public StageInfo() { }
    public StageInfo(int mapId)
    {
        this.mapNumber = mapId;
        this.stageNumber = MapSystem.GetMap(mapId).stageNumber;
    }
    public void initStage()
    {
        stageCoin = 300;
        stageTime = 0;
        cointGetTime = 0;

        Debugging.Log("스테이지 씬에 맵로딩 완료. >> " + this.stageNumber + " 스테이지의 " + this.mapNumber + " 맵");
    }
}
