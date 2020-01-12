using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManagement : MonoBehaviour
{
    private static StageManagement _instance = null;
    public static StageManagement instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(StageManagement)) as StageManagement;

                if (_instance == null)
                {
                    Debug.LogWarning("There's no active StageManagement object");
                }
            }

            return _instance;
        }
    }
    public GameObject AllyCastle;
    public StageInfo stageInfo;
    public static UserInfo userInfo;
    public Transform Map;
    public Transform HeroPoint;
    public Transform EnemyHeroPoint;

    private bool isStartGame;
    private bool isInfinitySpawnProcessing;
    bool isEndGame;
    float checkingHeroAliveTime;
    public Text mapnameText;
    public Text mapInfoText;
    public Text pointText;
    public Image InfinityPointImage;
    private static int kPoint;
    private static int dPoint;
    private static int MonsterCount;
    public UI_InfinityCastleStatsUp UI_InfinityCastleStatsUp;
    public AudioClip audio_ticktock;
    public Common.StageModeType stageModeType;
    List<Hero> stageHeros = new List<Hero>();
    

    public Transform CastlePoint;
    public Transform BossPoint;
    public Transform BossModeHpBarTransform;

    private void OnEnable()
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
            SoundManager.instance.BgmSourceChange(AudioClipManager.instance.StageBgm(map.stageNumber));
            stageInfo.stageType = map.stageType;
            switch (stageInfo.stageType)
            {
                case 0:
                    UI_Manager.instance.Title.GetComponentInChildren<Text>().text = string.Format("<size='30'>{0}.</size> {1}",LocalizationManager.GetText("StageMission"),LocalizationManager.GetText("stageTitleMainType1"));
                    AllyCastle.gameObject.SetActive(true);
                    InitCastle();
                    break;
                case 1:
                    UI_Manager.instance.Title.GetComponentInChildren<Text>().text = string.Format("<size='30'>{0}.</size> {1}", LocalizationManager.GetText("StageMission"), LocalizationManager.GetText("stageTitleMainType2"));
                    AllyCastle.gameObject.SetActive(false);
                    break;
                case 2:
                    UI_Manager.instance.Title.GetComponentInChildren<Text>().text = string.Format("<size='30'>{0}.</size> {1}", LocalizationManager.GetText("StageMission"), LocalizationManager.GetText("stageTitleMainType3"));
                    AllyCastle.gameObject.SetActive(false);
                    InitBoss();
                    break;
            }
            mapnameText.text = string.Format("{0} {1}",MapSystem.GetStageName(map.stageNumber-1) , map.name);
            if (Map != null)
                MapSystem.SetMapSprite(stageInfo.stageNumber, ref Map);
            CharactersManager.instance.SetStagePositionHeros();
        }
        else if (stageModeType == Common.StageModeType.Infinite)
        {
            isInfinitySpawnProcessing = false;
            int RandomStage = UnityEngine.Random.Range(1, 7);
            SoundManager.instance.BgmSourceChange(AudioClipManager.instance.StageInfinityBgm());
            UI_Manager.instance.Title.GetComponentInChildren<Text>().text = string.Format("<size='30'>{0}.</size> {1}", LocalizationManager.GetText("StageMission"), LocalizationManager.GetText("stageTitleInfinity"));
            mapnameText.text = "";
            if (mapInfoText != null)
                mapInfoText.text = "";
            CharactersManager.instance.SetInfinityStagePositionHeros();
        }
        else if(stageModeType == Common.StageModeType.Boss)
        {
            SoundManager.instance.BgmSourceChange(AudioClipManager.instance.StageBgm(BossModeData.mapStage));
            UI_Manager.instance.Title.GetComponentInChildren<Text>().text = string.Format("<size='30'>{0}.</size> {1}", LocalizationManager.GetText("StageMission"), LocalizationManager.GetText("stageTitleBoss"));
            if (mapInfoText != null)
                mapnameText.text = "";
            if (Map != null)
                MapSystem.SetMapSprite(BossModeData.mapStage, ref Map);
            InitBossMode();
            CharactersManager.instance.SetStagePositionHeros();
        }
        else if (stageModeType == Common.StageModeType.Battle)
        {
            SoundManager.instance.BgmSourceChange(AudioClipManager.instance.StageBattleBgm());
            UI_Manager.instance.Title.GetComponentInChildren<Text>().text = string.Format("<size='30'>{0}.</size> {1}", LocalizationManager.GetText("StageMission"), LocalizationManager.GetText("stageTitleBattle"));
        }
        else if(stageModeType==Common.StageModeType.Attack)
        {
            SoundManager.instance.BgmSourceChange(AudioClipManager.instance.StageAttackBgm());
            UI_Manager.instance.Title.GetComponentInChildren<Text>().text = string.Format("<size='30'>{0}.</size> {1}", LocalizationManager.GetText("StageMission"), LocalizationManager.GetText("stageTitleAttack"));
            CharactersManager.instance.SetStagePositionHeros();
        }
        StartCoroutine("StageStartEffect");
    }
    public Hero GetStageHero(int id)
    {
        stageHeros = new List<Hero>();
        foreach (var hero in Common.FindAlly(true))
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
            castle.transform.position = CastlePoint.position+new Vector3(0,castle.transform.localScale.y*0.75f);
            castle.gameObject.SetActive(true);
        }
    }
    private void InitBoss()
    {
        int[] bossesId = { 1003, 1005, 1001, 1002, 1006, 1004, 1007,1008 };
        int bossId = bossesId[stageInfo.stageNumber-1];
        GameObject boss = Instantiate(PrefabsDatabaseManager.instance.GetMonsterPrefab(bossId), BossPoint);
        if (boss != null)
        {
            boss.transform.position = BossPoint.position + new Vector3(0, (boss.transform.localScale.y - 1) * 0.45f, 0);
            boss.gameObject.SetActive(true);
        }
    }
    private void InitBossMode()
    {
        GameObject boss = Instantiate(PrefabsDatabaseManager.instance.GetMonsterPrefab(BossModeData.bossId), BossPoint);
        if (boss != null)
        {
            boss.transform.position = BossPoint.position + new Vector3(0, (boss.transform.localScale.y-1)*0.45f,0);
            boss.gameObject.SetActive(true);
            if (BossModeHpBarTransform!=null)
            {
                BossModeHpBarTransform.GetComponent<UI_BossModeHpBar>().OpenHpUI(boss);
            }
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
        else if(Common.stageModeType==Common.StageModeType.Boss)
        {
            BossModeUpdate();
        }
        else if (Common.stageModeType == Common.StageModeType.Battle)
        {
            BattleModeUpdate();
        }
        else if (Common.stageModeType == Common.StageModeType.Attack)
        {
            AttackModeUpdate();
        }
    }
    private void MainModeUpdate()
    {
        if (stageInfo != null && !isEndGame)
        {
            stageInfo.stageTime += Time.deltaTime;
            if (Common.IsAutoStagePlay||User.isAutoCam)
                AutoEnergyUpdate();
            else
                EnergyUpdate();

            switch(stageInfo.stageType)
            {
                case 0:
                    break;
                case 1:
                    checkingHeroAliveTime += Time.deltaTime;
                    if (checkingHeroAliveTime > 1.0f)
                    {
                        if (CheckHerosEnd())
                        {
                            StageFail();
                        }
                        checkingHeroAliveTime = 0.0f;
                    }
                    break;
                case 2:
                    checkingHeroAliveTime += Time.deltaTime;
                    if (checkingHeroAliveTime > 1.0f)
                    {
                        if (CheckHerosEnd())
                        {
                            if (Common.hitTargetObject != null)
                                Camera.main.GetComponent<FollowCamera>().ChangeTarget(Common.hitTargetObject);
                            StageFail();
                        }
                        checkingHeroAliveTime = 0.0f;
                    }
                    break;
            }
        }
    }
    private void InfinityModeUpdate()
    {
        if (stageInfo != null && !isEndGame && isStageStart())
        {
            if (User.isAutoCam)
                AutoEnergyUpdate();
            else
                EnergyUpdate();
            stageInfo.stageTime -= Time.deltaTime;
            if (stageInfo.stageTime<=0)
            {
                stageInfo.stageTime = 20;
                stageInfo.stageWave += 1;
                StartCoroutine("NextWave");
                if(UI_InfinityCastleStatsUp!=null&&stageInfo.stageWave<20&&stageInfo.stageWave>1)
                {
                    UI_InfinityCastleStatsUp.gameObject.SetActive(true);
                    UI_InfinityCastleStatsUp.ShowCastleStatSelectUI();
                }
            }
            checkingHeroAliveTime += Time.deltaTime;
            if (checkingHeroAliveTime > 1.0f)
            {
                checkingHeroAliveTime = 0.0f;
                if (CheckInfinityModeEnd())
                {
                    StageInfinityEnd();
                }
                else
                {
                     if (GetDPoint() <= 3&&!isInfinitySpawnProcessing)
                    {
                        StartCoroutine("NextWave");
                    }
                    stageInfo.stagePoint += stageInfo.stageWave;
                }
            }
            if (pointText!=null)
            {
                pointText.text = string.Format("{0}pts", Common.GetThousandCommaText(stageInfo.stagePoint));
            }
        }
    }
    IEnumerator NextWave()
    {
        isInfinitySpawnProcessing = true;
        mapnameText.text = string.Format("<size='35'>Wave</size> {0}", stageInfo.stageWave);
        if (mapInfoText != null)
            mapInfoText.text = string.Format("{0} {1}%", LocalizationManager.GetText("stageInfinityPowerupMessage"),stageInfo.stageWave*10);
        var spawnMonsterList = HeroSystem.GetInfinityMonsters(stageInfo.stageWave);
        yield return new WaitForSeconds(1.0f);
        FindObjectOfType<InfiniteSpawn>().Spawn(spawnMonsterList);
        yield return new WaitForSeconds(2.0f);
        foreach(var child in Common.FindEnemy(true))
        {
            child.GetComponent<Hero>().WaveBuff(this.stageInfo.stageWave,true);
        }
        Debugging.Log(this.stageInfo.stageWave + "버프!");
        isInfinitySpawnProcessing = false;
    }
    private void BossModeUpdate()
    {
        if (stageInfo != null && !isEndGame)
        {
            stageInfo.stageTime += Time.deltaTime;
            if (User.isAutoCam)
                AutoEnergyUpdate();
            else
                EnergyUpdate();

            checkingHeroAliveTime += Time.deltaTime;
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

    private void AttackModeUpdate()
    {
        if (stageInfo != null && !isEndGame&&isStartGame)
        {
            stageInfo.stageTime += Time.deltaTime;
            EnergyUpdate();
            if (stageInfo.stageTime >=30)
            {
                SoundManager.instance.StopSingleLoop();
                isStartGame = false;
                isEndGame = true;
                StageAttackEnd();
                return;
            }
            if (pointText != null)
                pointText.text = (30 - stageInfo.stageTime).ToString("N0");
        }
    }
    private void BattleModeUpdate()
    {
        if (stageInfo != null && !isEndGame&&StageBattleManager.instance.IsPlayingGame())
        {
            stageInfo.stageTime += Time.deltaTime;
            AutoEnergyUpdate();
            if(stageInfo.stageTime>=180)
            {
                isStartGame = false;
                isEndGame = true;
                StageBattleManager.instance.BattleEnd(StageBattleManager.instance.IsTimeOverEndWin());
                return;
            }

            checkingHeroAliveTime += Time.deltaTime;
            if (checkingHeroAliveTime > 1.0f)
            {
                if (CheckHerosEnd())
                {
                    isStartGame = false;
                    isEndGame = true;
                    StageBattleManager.instance.BattleEnd(false);
                }
                else if(CheckEnemyHerosEnd())
                {
                    isStartGame = false;
                    isEndGame = true;
                    StageBattleManager.instance.BattleEnd(true);
                }
                else if(CheckPvpDraw())
                {
                    isStartGame = false;
                    isEndGame = true;
                    StageBattleManager.instance.BattleEnd(true,true);
                }
                checkingHeroAliveTime = 0.0f;
            }
        }
    }
    IEnumerator StageStartEffect()
    {
        yield return new WaitForSeconds(5.0f);
        if(FindObjectOfType<CameraEffectHandler>()!=null)
            FindObjectOfType<CameraEffectHandler>().ReActiveOffPanelList();
        if(Common.stageModeType!=Common.StageModeType.Battle)
            HeroSkillManager.instance.ShowUI();
        if (Common.stageModeType == Common.StageModeType.Attack)
            SoundManager.instance.PlaySingleLoop(audio_ticktock);
        ShowGoalTitle();
        isStartGame = true;
        Debugging.Log("스테이지 시작");
        yield return null;
    }
    public bool isStageStart()
    {
        return isStartGame;
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
    private bool CheckInfinityModeEnd()
    {
        if (Common.allyTargetObject == null || (Common.allyTargetObject != null && Common.allyTargetObject.GetComponent<Castle>() != null && Common.allyTargetObject.GetComponent<Castle>().isDead))
        {
            return true;
        }
        return false;
    }
    private bool CheckEnemyHerosEnd()
    {
        int cnt = 0;
        for (int i = 0; i < EnemyHeroPoint.childCount; i++)
        {
            if (EnemyHeroPoint.GetChild(i).gameObject.activeSelf)
                cnt++;
        }
        if (cnt == 0)
            return true;
        else
            return false;
    }
    private bool CheckPvpDraw()
    {
        bool isSupportCharacterOnlySurvive = true;
        foreach(Hero playerH in HeroPoint.GetComponentsInChildren<Hero>())
        {
            if(!playerH.isDead&&!playerH.isFriend)
            {
                isSupportCharacterOnlySurvive = false;
                break;
            }
        }
        if(isSupportCharacterOnlySurvive)
        {
            foreach (Hero enemyH in EnemyHeroPoint.GetComponentsInChildren<Hero>())
            {
                if (!enemyH.isDead && !enemyH.isFriend)
                {
                    isSupportCharacterOnlySurvive = false;
                    break;
                }
            }
        }
        return isSupportCharacterOnlySurvive;
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
    public string GetPvpTime()
    {
        return string.Format("{0:00}:{1:00}", (int)((180-stageInfo.stageTime) % 3600 / 60), (int)((180-stageInfo.stageTime) % 3600 % 60));
    }
    public int GetKPoint()
    {
        return kPoint;
    }
    public void AddAttackPoint(int dam)
    {
        if (stageModeType == Common.StageModeType.Attack)
        {
            stageInfo.stagePoint += dam;
        }
    }
    public void AddInfinityPoint(bool isBoss=false)
    {
        if (stageModeType == Common.StageModeType.Infinite)
        {
            if(isBoss)
            {
                stageInfo.stagePoint += stageInfo.stageWave * 1000;
            }
            else
            {
                stageInfo.stagePoint += stageInfo.stageWave * 50;
            }
            if(InfinityPointImage!=null)
            {
                StartCoroutine("InfinityImageAnimation");
            }
        }
    }
    IEnumerator InfinityImageAnimation()
    {
        float size = 1.1f;
        while(size>1)
        {
            InfinityPointImage.transform.localScale = new Vector3(size, size, size);
            pointText.transform.localScale = new Vector3(size, size, size);
            yield return new WaitForEndOfFrame();
            size -= Time.deltaTime;
        }
        yield return null;
    }
    public void SetKPoint(bool isBoss=false)
    {
        kPoint += 1;
        AddInfinityPoint(isBoss);
        MonsterCount -= 1;
    }
    public int GetDPoint()
    {
        return dPoint;
    }
    public void SetDPoint(int count=0)
    {
        dPoint += count;
    }

    public void StageClear()
    {
        Time.timeScale = 1.0f;
        if (Common.IsAutoStagePlay)
            StartCoroutine("StageAutoClearing");
        else
            StartCoroutine("StageClearing");
    }
    public void StageFail()
    {
        Time.timeScale = 1.0f;
        Common.IsAutoStagePlay = false;
        StartCoroutine("StageFailing");
    }
    public void StageInfinityEnd()
    {
        Time.timeScale = 1.0f;
        StartCoroutine("StageInfinityEnding");
    }
    public void StageBoissEnd()
    {
        Time.timeScale = 1.0f;
        StartCoroutine("StageBossEnding");
    }
    public void StageAttackEnd()
    {
        Time.timeScale = 1.0f;
        SoundManager.instance.StopSingleLoop();
        StartCoroutine("StageAttackEnding");
    }

    public IEnumerator StageFailing()
    {
        HeroSystem.SaveHeros(Common.FindPlayerHero());
        isEndGame = true;
        isStartGame = false;
        GoogleSignManager.SaveData();
        yield return new WaitForSeconds(0.1f);
        UI_Manager.instance.OpenEndGamePanel(false);
        if (UnityEngine.Random.Range(0, 10) < 3)
            GoogleAdMobManager.instance.OnBtnViewAdClicked();
        yield return null;
    }
    public void SetStageClearPoint()
    {
        int point = 1;
        bool isHeroAllAlive= CharactersManager.instance.GetStageHeroCount() == Common.FindAlly(true).Count;
        bool isStageTime = stageInfo.stageTime < 180;
        if (isHeroAllAlive)
            point += 1;
        if (isStageTime)
            point += 1;
        stageInfo.stageClearPoint = point;
    }

    public IEnumerator StageClearing()
    {
        SoundManager.instance.EffectSourcePlayNoPitch(AudioClipManager.instance.victory);
        isStartGame = false;
        isEndGame = true;
        HeroSystem.SaveHeros(Common.FindPlayerHero());
        SetStageClearPoint();
        MapSystem.MapClear(stageInfo.mapNumber, stageInfo.stageClearPoint);
        MissionSystem.AddClearPoint(MissionSystem.ClearType.StageClear);
        MissionSystem.AddClearPoint(MissionSystem.ClearType.TotalStageCount);
        MissionSystem.PointSave();
        SaveSystem.AddUserCoin(stageInfo.stageCoin);
        stageInfo.stageExp = Common.GetUserExp(stageInfo.stageNumber);
        SaveSystem.ExpUp(stageInfo.stageExp);
        var getItems = GetStageItems();
        for (var i = 0; i < getItems.Count; i++)
        {
            ItemSystem.SetObtainItem(getItems[i].id);
        }
        GoogleSignManager.SaveData();
        yield return new WaitForSeconds(0.1f);
        GameObject endPanel = UI_Manager.instance.OpenEndGamePanel(true, stageInfo.stageClearPoint);
        yield return new WaitForSeconds(3.0f);
        endPanel.gameObject.SetActive(false);
        if (UnityEngine.Random.Range(0, 10) < 3)
            GoogleAdMobManager.instance.OnBtnViewAdClicked();
        GameObject resultPanel = UI_Manager.instance.OpenResultPanel();

        yield return null;
    }

    public IEnumerator StageAutoClearing()
    {
        SoundManager.instance.EffectSourcePlayNoPitch(AudioClipManager.instance.victory);
        isStartGame = false;
        isEndGame = true;
        HeroSystem.SaveHeros(Common.FindPlayerHero());
        SetStageClearPoint();
        MapSystem.MapClear(stageInfo.mapNumber, stageInfo.stageClearPoint);
        MissionSystem.AddClearPoint(MissionSystem.ClearType.StageClear);
        MissionSystem.AddClearPoint(MissionSystem.ClearType.TotalStageCount);
        MissionSystem.PointSave();
        SaveSystem.AddUserCoin(stageInfo.stageCoin);
        stageInfo.stageExp = Common.GetUserExp(stageInfo.stageNumber);
        SaveSystem.ExpUp(stageInfo.stageExp);
        var getItems = GetStageItems();
        for (var i = 0; i < getItems.Count; i++)
        {
            ItemSystem.SetObtainItem(getItems[i].id);
        }
        GoogleSignManager.SaveData();
        yield return new WaitForSeconds(0.1f);
        GameObject endPanel = UI_Manager.instance.OpenAutoEndGamePanel(true, stageInfo.stageClearPoint);
        yield return new WaitForSeconds(3.0f);
        endPanel.gameObject.SetActive(false);
        GameObject resultPanel = UI_Manager.instance.OpenResultPanel();
        if (AutoStageManager.Instance.GetIsReadyAutoOff)
        {
            AutoStageManager.Instance.AutoReadyOff();
        }
        else
        {
            yield return new WaitForSeconds(5.0f);
            AutoRestartStage();
        }
        yield return null;
    }

    void AutoRestartStage()
    {
        int stageHeroCount = CharactersManager.instance.GetStageHeroCount();
        if (ItemSystem.IsGetAbleItem())
        {
            if(Common.PaymentAbleCheck(ref User.portalEnergy, stageHeroCount))
            {
                if(SaveSystem.IsPremiumPassAble())
                {
                    Common.PaymentStart(ref User.portalEnergy, stageHeroCount);

                    SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
                    Debugging.Log(stageHeroCount + " 에너지 소모. 전투씬 자동 로드 시작 > " + User.portalEnergy);
                    SaveSystem.SavePlayer();
                    LoadSceneManager.instance.LoadStageScene(0);
                }
                else
                {
                    Common.IsAutoStagePlay = false;
                    UI_Manager.instance.ShowAlert(Common.GetCoinCrystalEnergyImagePath(8), LocalizationManager.GetText("alertAutoUnableTicketMessage"));
                }
            }
            else
            {
                Common.IsAutoStagePlay = false;
                UI_Manager.instance.ShowAlert(Common.GetCoinCrystalEnergyImagePath(2), LocalizationManager.GetText("alertAutoUnableEnergyMessage"));
            }
        }
        else
        {
            Common.IsAutoStagePlay = false;
            UI_Manager.instance.ShowAlert("", LocalizationManager.GetText("alertAutoUnableItemSlotMessage"));
        }
    }


    public IEnumerator StageBossEnding()
    {
        SoundManager.instance.EffectSourcePlayNoPitch(AudioClipManager.instance.victory);
        isStartGame = false;
        isEndGame = true;
        HeroSystem.SaveHeros(Common.FindPlayerHero());
        MissionSystem.AddClearPoint(MissionSystem.ClearType.StageClear);
        MissionSystem.AddClearPoint(MissionSystem.ClearType.TotalStageCount);
        MissionSystem.PointSave();
        stageInfo.stageExp = Common.GetUserExp(stageInfo.stageNumber);
        SaveSystem.ExpUp(stageInfo.stageExp);
        var getItems = GetStageItems();
        for (var i = 0; i < getItems.Count; i++)
        {
            ItemSystem.SetObtainItem(getItems[i].id);
        }
        SaveSystem.AddUserCoin(BossModeData.rewardCoin);
        SaveSystem.AddUserCrystal(BossModeData.rewardCrystal);
        ItemSystem.SetObtainItem(8001, BossModeData.rewardScroll);
        if(BossModeData.rewardTranscendenceStone > 0 ) SaveSystem.AddUserTranscendenceStone(BossModeData.rewardTranscendenceStone);
        if (BossModeData.rewardBoxCount > 0) ItemSystem.SetObtainItem((8003 + BossModeData.rewardBoxType), BossModeData.rewardBoxCount);
        GoogleSignManager.SaveData();
        yield return new WaitForSeconds(0.1f);
        UI_Manager.instance.ShowBossEndingUI(BossModeData.rewardCoin, BossModeData.rewardCrystal, BossModeData.rewardScroll, BossModeData.rewardTranscendenceStone, BossModeData.rewardBoxType, BossModeData.rewardBoxCount);
        BossModeData.ClearBossModeData();
        if (UnityEngine.Random.Range(0, 10) < 4)
            GoogleAdMobManager.instance.OnBtnViewAdClicked();
        yield return null;
    }

    public IEnumerator StageInfinityEnding()
    {
        SoundManager.instance.EffectSourcePlayNoPitch(AudioClipManager.instance.victory);
        isStartGame = false;
        isEndGame = true;
        HeroSystem.SaveHeros(Common.FindPlayerHero());
        MissionSystem.AddClearPoint(MissionSystem.ClearType.StageClear);
        MissionSystem.AddClearPoint(MissionSystem.ClearType.TotalStageCount);
        MissionSystem.PointSave();
        SaveSystem.AddUserCoin(stageInfo.stageCoin);
        stageInfo.stageExp = Common.GetUserExp(stageInfo.stageNumber);
        SaveSystem.ExpUp(stageInfo.stageExp);
        GoogleSignManager.Instance.SetInfinityPoint(stageInfo.stagePoint);
        while (!Common.isLoadCompleted)
            yield return null;
        if (stageInfo.stageWave > 30)
            AddGetStageItem(8005);
        else if (stageInfo.stageWave > 20 && stageInfo.stageWave <= 30)
            AddGetStageItem(8004);
        else if (stageInfo.stageWave > 10 && stageInfo.stageWave <= 20)
            AddGetStageItem(8003);
        var getItems = GetStageItems();
        for (var i = 0; i < getItems.Count; i++)
        {
            ItemSystem.SetObtainItem(getItems[i].id);
        }
        GoogleSignManager.SaveData();
        yield return new WaitForSeconds(0.1f);
        GameObject endPanel = UI_Manager.instance.OpenPopupPanel("EndGame");
        yield return new WaitForSeconds(3.0f);
        endPanel.gameObject.SetActive(false);
        if (UnityEngine.Random.Range(0, 10) < 4)
            GoogleAdMobManager.instance.OnBtnViewAdClicked();
        GameObject resultPanel = UI_Manager.instance.OpenResultPanel();
        yield return null;
    }
    public IEnumerator StageAttackEnding()
    {
        SoundManager.instance.EffectSourcePlayNoPitch(AudioClipManager.instance.victory);
        SoundManager.instance.StopSingleLoop();
        isStartGame = false;
        isEndGame = true;
        HeroSystem.SaveHeros(Common.FindPlayerHero());
        int totalDamage = stageInfo.stagePoint;
        GoogleSignManager.Instance.SetAttackPoint(totalDamage);
        while (!Common.isLoadCompleted)
            yield return null;
        GoogleSignManager.SaveData();
        yield return new WaitForSeconds(0.1f);
        UI_Manager.instance.OpenAttackEndGamePanel(totalDamage);
        if (UnityEngine.Random.Range(0, 10) < 4)
            GoogleAdMobManager.instance.OnBtnViewAdClicked();
        yield return null;
    }
    public UserInfo GetUserInfo()
    {
        return userInfo;
    }
    private void EnergyUpdate()
    {
        if(isStartGame)
        {
            stageInfo.stageEnergy += stageInfo.stageGetEnergy * Time.deltaTime;
            stageInfo.stageEnergy = Mathf.Clamp(stageInfo.stageEnergy, 0, stageInfo.stageMaxEnergy);
        }
    }
    void AutoEnergyUpdate()
    {
        if (isStartGame)
        {
            stageInfo.stageEnergy += stageInfo.stageGetEnergy * Time.deltaTime;
            stageInfo.stageEnergy = Mathf.Clamp(stageInfo.stageEnergy, 0, stageInfo.stageMaxEnergy);
            if (stageInfo.stageEnergy >= HeroSkillManager.instance.MaxNeedEnergy)
            {
                HeroSkillManager.instance.AutoSkillClick();
            }
        }
    }

    public float GetStageEnergy()
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
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        int stageHeroCount = CharactersManager.instance.GetStageHeroCount();
        if (stageHeroCount < 1)
        {
            Debugging.Log("영웅을 선택해주세요");
        }
        else
        {
            if (ItemSystem.IsGetAbleItem())
            {
                if (Common.PaymentCheck(ref User.portalEnergy, stageHeroCount))
                {
                    Debugging.Log(stageHeroCount + " 에너지 소모. 전투씬 로드 시작 > " + User.portalEnergy);
                    GameManagement.instance.SetStageInfo(stageInfo.mapNumber + 1);
                    SaveSystem.SavePlayer();
                    LoadSceneManager.instance.LoadStageScene();
                }
                else
                {
                    UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.energy, stageHeroCount);
                }
            }
            else
            {
                UI_Manager.instance.ShowAlert("", LocalizationManager.GetText("alertUnableGetItemMessage"));
            }
        }
    }

    public void TimeEffect()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("BalloonPopExplosion");
            if (effect != null&&pointText!=null)
            {
                effect.transform.position = pointText.transform.position;
                effect.SetActive(true);
            }
        }
    }
}

public class StageInfo
{
    public int stageNumber;
    public int mapNumber;
    public int stageExp;
    public int stageCoin;
    public int stageType;
    public float stageEnergy;
    public int stageMaxEnergy;
    public float stageTime;
    public float stageGetTime;
    public int stageGetEnergy;
    public int stageClearPoint;
    public int stagePoint;
    public int stageWave;
    public List<int> stageGetItems;
    public int boss;
    public StageInfo() { }
    public StageInfo(int mapId, int stageNum)
    {
        this.mapNumber = mapId;
        this.stageNumber = stageNum;
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
        stagePoint = 0;
        stageWave = 0;

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
        if(exp>= Common.GetUserNeedExp())
        {
            level += 1;
            isLevelUp = true;
            exp = 0;
        }
    }


}
