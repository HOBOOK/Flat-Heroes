using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class StageBattleManager : MonoBehaviour
{
    #region UI변수
    public GameObject StartUI;
    public GameObject ResultUI;
    public GameObject InformationUI;
    public Text pvpTimeText;
    public Transform PlayerUITransform;
    public Transform EnemyPlayerSkillTransform;

    Text playerNameText;
    Text playerPointText;
    Text playerHpText;
    Slider playerFlatEnergySlider;
    Text playerFlatEnergySliderText;
    public Image playerThumbnail;
    public Transform EnemyUITransform;
    Text enemyNameText;
    Text enemyPointText;
    Text enemyHpText;
    Slider enemyFlatEnergySlider;
    Text enemyFlatEnergySliderText;
    public Image enemyThumbnail;

    int playerTotalHp;
    int playerCurrentHp;
    float playerHpPercent;
    int enemyTotalHp;
    int enemyCurrentHp;
    float enemyHpPercent;

    #endregion

    #region 시스템 변수
    private static StageBattleManager _instance = null;
    public static StageBattleManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(StageBattleManager)) as StageBattleManager;

                if (_instance == null)
                {
                    Debug.LogError("There's no active StageManagement object");
                }
            }

            return _instance;
        }
    }
    Dictionary<int, GameObject> playerHeroList = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> heroList = new Dictionary<int, GameObject>();
    List<Hero> enemyHeroes = new List<Hero>();
    List<HeroData> enemyHeroDataList = new List<HeroData>();
    List<Ability> enemyAbilityDataList = new List<Ability>();
    List<Item> enemyItemDataList = new List<Item>();
    UnityEngine.Random rand;
    int[] enemyAbilitiesStats = new int[7]; // 0:attack 1:defence 2:hp 3:critical 4:attackSpeed 5: moveSpeed 6: energy
    int getRankPoint = 0;
    bool getPvpResult = false;
    PvpData enemyData;
    bool isDataReceive = false;
    public bool isStartGame = false;
    public bool isPlayGame = false;
    Hero currentSkillHero;
    bool isSkillCasting = false;

    float enemyFlatEnergy;
    float enemyFlatChargeEnergy;
    float enemyFlatMaxEnergy;
    float pvpTime;
    #endregion

    private void Awake()
    {
        if(PlayerUITransform!=null)
        {
            foreach(var txt in PlayerUITransform.GetComponentsInChildren<Text>())
            {
                if(txt.name.Equals("nameText"))
                {
                    playerNameText = txt;
                }
                else if(txt.name.Equals("pointText"))
                {
                    playerPointText = txt;
                }
                else if(txt.name.Equals("hpText"))
                {
                    playerHpText = txt;
                }
            }
            playerFlatEnergySlider = PlayerUITransform.GetChild(0).GetComponent<Slider>();
            playerFlatEnergySliderText = playerFlatEnergySlider.GetComponentInChildren<Text>();
        }
        if(EnemyUITransform!=null)
        {
            foreach (var txt in EnemyUITransform.GetComponentsInChildren<Text>())
            {
                if (txt.name.Equals("nameText"))
                {
                    enemyNameText = txt;
                }
                else if (txt.name.Equals("pointText"))
                {
                    enemyPointText = txt;
                }
                else if (txt.name.Equals("hpText"))
                {
                    enemyHpText = txt;
                }
            }
            enemyFlatEnergySlider = EnemyUITransform.GetChild(0).GetComponent<Slider>();
            enemyFlatEnergySliderText = enemyFlatEnergySlider.GetComponentInChildren<Text>();
        }
    }

    void Start()
    {
        pvpTime = 180f;
        Common.stageModeType = Common.StageModeType.Battle;
        isStartGame = false;
        isPlayGame = false;
        GetEnemyData();
    }
    void FixedUpdate()
    {
        if(isStartGame)
        {
            UpdatePlayerUI();
            UpdateEnemyUI();
            EnemyFlatEnergyUpdate();
            PvpInformationUpdate();
        }

    }

    private void OnApplicationQuit()
    {
        ExitPenalty();
    }

    public bool IsTimeOverEndWin()
    {
        if (playerTotalHp < enemyTotalHp)
            return false;
        else
            return true;
    }

    void PvpInformationUpdate()
    {
        if(IsPlayingGame())
        {
            pvpTimeText.text = StageManagement.instance.GetPvpTime();
        }
    }

    void EnemyFlatEnergyUpdate()
    {
        if(IsPlayingGame() && StageManagement.instance.isStageStart())
        {
            enemyFlatEnergy += enemyData.labData.flatEnergyChargeData * Time.deltaTime;
            enemyFlatEnergy = Mathf.Clamp(enemyFlatEnergy, 0, enemyFlatMaxEnergy);
            if (enemyFlatMaxEnergy > 0 && enemyFlatMaxEnergy <= 150)
            {
                if (enemyFlatEnergy >= 100)
                {
                    EnemySkillAttack(100);
                }
            }
            else if (enemyFlatMaxEnergy > 150 && enemyFlatMaxEnergy <= 300)
            {
                if (enemyFlatEnergy >= 150)
                {
                    EnemySkillAttack(130);
                }
            }
            else if(enemyFlatMaxEnergy > 300 && enemyFlatMaxEnergy <= 500)
            {
                if (enemyFlatEnergy > 300)
                {
                    EnemySkillAttack(150);
                }
            }
            else
            {
                if (enemyFlatEnergy > 300)
                {
                    EnemySkillAttack(180);
                }
            }
        }
    }

    public void EnemySkillAttack(float energy)
    {
        var heroes = new List<Hero>();
        foreach(var h in enemyHeroes)
        {
            if (h != null && h.gameObject.activeSelf && !h.isDead)
                heroes.Add(h);
        }

        if(heroes!=null&&heroes.Count>0)
        {
            currentSkillHero = heroes[UnityEngine.Random.Range(0, heroes.Count - 1)];
        }
        if(currentSkillHero!=null&&!isSkillCasting)
        {
            StartCoroutine(SkillCast(energy));
        }
    }

    IEnumerator SkillCast(float energy)
    {
        isSkillCasting = true;
        float maxDelayTime = 3.0f;
        while(!currentSkillHero.IsReached()&&maxDelayTime>0)
        {
            maxDelayTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        isSkillCasting = false;
        currentSkillHero.SkillAttack();
        enemyFlatEnergy -= energy;
    }


    public void GetEnemyData()
    {
        if(!isDataReceive&&!string.IsNullOrEmpty(Common.pvpEnemyLocalId))
        {
            StartCoroutine(GettingEnemyData(Common.pvpEnemyLocalId));
            isDataReceive = true;
        }
    }

    IEnumerator GettingEnemyData(string id)
    {
        GoogleSignManager.Instance.GetPvpData(id);
        while(!Common.isLoadCompleted)
        {
            yield return null;
        }
        if(Common.pvpEnemyData!=null)
        {
            enemyData = Common.pvpEnemyData;
            int[] enemyHeroDatas = enemyData.battleHeros;
            if (GetStageHeroCount(enemyHeroDatas) > 0)
            {
                string name = enemyData.name;
                Sprite profileImage = enemyData.profileImage;
                if (enemyData.rankPoint == 0) enemyData.rankPoint = 1000;
                enemyHeroDataList = GetEnemyHeroDatas(enemyData.battleHeros, enemyData.heroData);
                enemyAbilityDataList = GetEnemyAbilityData(enemyData.abilityData);
                enemyItemDataList = GetEnemyItemData(enemyData.itemData);
                SetAbilityStats();
                HideInformationUI();
                ShowStartUI(enemyData);
                yield return new WaitForSeconds(3.0f);
                CloseStartUI();
                yield return new WaitForSeconds(1.0f);
                InitBattleMode();
                SetStagePositionPlayerHeros();
                yield return new WaitForFixedUpdate();
                HeroSkillManager.instance.ShowUI(true);
                SetPlayerUI();
                SetStagePositionEnemyHeros(enemyData.battleHeros);
                SetEnemy(enemyData);
                SetEnemyUI(enemyData);
                ShowInformationUI();
                isStartGame = true;
                yield return new WaitForSeconds(3.0f);
                isPlayGame = true;
            }
            else
            {
                StageBattleErrorEnd();
                User.portalEnergy += CharactersManager.instance.GetStageHeroCount();
                Debugging.Log("무효처리");
            }
        }
        else
        {
            StageBattleErrorEnd();
            User.portalEnergy += CharactersManager.instance.GetStageHeroCount();
            Debugging.Log("무효처리");
        }

        yield return null;
    }
    void HideInformationUI()
    {
        if (InformationUI != null)
            InformationUI.gameObject.SetActive(false);
    }
    void ShowInformationUI()
    {
        if (InformationUI != null)
        {
            InformationUI.gameObject.SetActive(true);
            if (InformationUI.GetComponent<AiryUIAnimatedElement>() != null)
                InformationUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
        }

    }

    void ShowStartUI(PvpData data)
    {
        if(StartUI!=null)
        {
            Transform playerProfileTransform = StartUI.transform.GetChild(1).transform;
            Transform enemyProfileTransform = StartUI.transform.GetChild(0).transform;

            playerProfileTransform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = HeroSystem.GetHeroThumbnail(User.profileHero);
            playerProfileTransform.GetChild(0).GetChild(1).GetComponent<Text>().text = User.battleRankPoint.ToString();
            playerProfileTransform.GetChild(0).GetChild(2).GetComponent<Text>().text = User.name;
            playerProfileTransform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = Common.GetRankText(User.battleRankPoint);
            playerProfileTransform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<Text>().color = ItemColor.GetItemColor(Common.GetRank(User.battleRankPoint));

            enemyProfileTransform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = data.profileImage;
            enemyProfileTransform.GetChild(0).GetChild(1).GetComponent<Text>().text = data.rankPoint.ToString();
            enemyProfileTransform.GetChild(0).GetChild(2).GetComponent<Text>().text = data.name;
            enemyProfileTransform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = Common.GetRankText(data.rankPoint);
            enemyProfileTransform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<Text>().color = ItemColor.GetItemColor(Common.GetRank(data.rankPoint));

            StartUI.SetActive(true);
        }
    }
    void CloseStartUI()
    {
        if(StartUI!=null)
        {
            StartUI.GetComponent<AiryUIAnimatedElement>().HideElement();
        }
    }
    void SetEnemyUI(PvpData data)
    {
        enemyNameText.text = data.name;
        enemyPointText.text = data.rankPoint.ToString();
        enemyThumbnail.sprite = data.profileImage;
    }
    void SetPlayerUI()
    {
        playerNameText.text = User.name;
        playerPointText.text = User.battleRankPoint.ToString();
        playerThumbnail.sprite = HeroSystem.GetHeroThumbnail(User.profileHero);
    }
    void UpdatePlayerUI()
    {
        UpdatePlayerHp();
        playerHpPercent = (float)playerCurrentHp / (float)playerTotalHp;
        PlayerUITransform.GetComponent<Slider>().value = playerHpPercent;
        playerHpText.text = string.Format("{0}/{1}({2}%)", playerCurrentHp, playerTotalHp, (playerHpPercent * 100).ToString("N0"));
        playerFlatEnergySlider.value = (float)StageManagement.instance.stageInfo.stageEnergy / (float)StageManagement.instance.stageInfo.stageMaxEnergy;
        playerFlatEnergySliderText.text = string.Format("{0}/{1}", (int)StageManagement.instance.stageInfo.stageEnergy, (int)StageManagement.instance.stageInfo.stageMaxEnergy);
    }

    void UpdatePlayerHp()
    {
        int hp = 0;
        foreach (var hero in playerHeroList)
        {
            hp += hero.Value.GetComponent<Hero>().status.maxHp;
        }
        playerTotalHp = hp;
        hp = 0;
        foreach (var hero in playerHeroList)
        {
            hp += hero.Value.GetComponent<Hero>().status.hp;
        }
        playerCurrentHp = hp;
    }

    void UpdateEnemyUI()
    {
        UpdateEnemyHp();
        enemyHpPercent = (float)enemyCurrentHp / (float)enemyTotalHp;
        EnemyUITransform.GetComponent<Slider>().value = enemyHpPercent;
        enemyHpText.text = string.Format("{0}/{1}({2}%)", enemyCurrentHp, enemyTotalHp, (enemyHpPercent*100).ToString("N0"));
        enemyFlatEnergySlider.value = (float)enemyFlatEnergy / (float)enemyFlatMaxEnergy;
        enemyFlatEnergySliderText.text = string.Format("{0}/{1}", (int)enemyFlatEnergy, (int)enemyFlatMaxEnergy);
    }

    void UpdateEnemyHp()
    {
        int hp = 0;
        foreach (var hero in heroList)
        {
            hp += hero.Value.GetComponent<Hero>().status.maxHp;
        }
        enemyTotalHp = hp;
        hp = 0;
        foreach (var hero in heroList)
        {
            hp += hero.Value.GetComponent<Hero>().status.hp;
        }
        enemyCurrentHp = hp;
    }
    public void PvpResurrectionHero(int id, GameObject hero)
    {
        if (playerHeroList.ContainsKey(id))
        {
            playerHeroList.Remove(id);
            playerHeroList.Add(id, hero);
        }
    }

   public bool IsAlreayAlive(int id)
    {
        foreach (var hero in playerHeroList)
        {
            if (hero.Value.GetComponent<Hero>() != null)
            {
                if (hero.Value.GetComponent<Hero>().id == id)
                {
                    if (hero.Value.GetComponent<Hero>().isDead)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public void SetStagePositionPlayerHeros()
    {
        playerHeroList = new Dictionary<int, GameObject>();
        var spawnPoint = GameObject.Find("PlayersHero").transform;
        bool flag = false;
        for (int i = 0; i < User.battleHeros.Length; i++)
        {
            if (User.battleHeros[i] != 0)
            {
                if (PrefabsDatabaseManager.instance.GetHeroPrefab(User.battleHeros[i]) != null)
                {
                    int id = User.battleHeros[i];
                    GameObject hero = Instantiate(PrefabsDatabaseManager.instance.GetHeroPrefab(id)) as GameObject;
                    hero.transform.parent = spawnPoint;
                    hero.transform.localPosition = Vector3.zero - new Vector3(i, 0);
                    flag = !flag;
                    if (!playerHeroList.ContainsKey(id))
                        playerHeroList.Add(id, hero);
                    else
                        playerHeroList[id] = hero;
                    hero.SetActive(true);
                }
            }
        }
        Debugging.Log(User.battleHeros.Length + "의 리스트에서 Stage씬에 영웅소환 완료.");
    }

    public void SetStagePositionEnemyHeros(int[] heroDatas)
    {
        heroList = new Dictionary<int, GameObject>();
        var spawnPoint = GameObject.Find("EnemysHero").transform;
        bool flag = false;
        for (int i = 0; i < heroDatas.Length; i++)
        {
            if (heroDatas[i] != 0)
            {
                if (PrefabsDatabaseManager.instance.GetHeroPrefab(heroDatas[i]) != null)
                {
                    int id = heroDatas[i];
                    GameObject hero = Instantiate(PrefabsDatabaseManager.instance.GetHeroPrefab(id)) as GameObject;
                    hero.GetComponent<Hero>().isPlayerHero = false;
                    hero.GetComponent<Hero>().heroData = GetEnemyHero(id);
                    hero.gameObject.SetActive(true);
                    hero.transform.parent = spawnPoint;
                    hero.transform.localPosition = Vector3.zero + new Vector3(i, 0);
                    flag = !flag;
                    if (!heroList.ContainsKey(id))
                    {
                        heroList.Add(id, hero);
                        enemyHeroes.Add(hero.GetComponent<Hero>());
                    }
                    else
                        playerHeroList[id] = hero;

                }
            }
        }
        Debugging.Log(User.stageHeros.Length + "의 리스트에서 Stage씬에 영웅소환 완료.");
    }

    void SetEnemy(PvpData data)
    {
        enemyFlatMaxEnergy = data.labData.flatEnergyData * 10 + 100;
        enemyFlatChargeEnergy = data.labData.flatEnergyChargeData * 2 + 5;
    }

    public void ExitStageBattleScene()
    {
        ExitPenalty();
        Time.timeScale = 1.0f;
        LoadSceneManager.instance.LoadScene(1);
    }

    public int GetStageHeroCount(int[] heroDatas)
    {
        int cnt = 0;
        for (var i = 0; i < heroDatas.Length; i++)
        {
            if (heroDatas[i] != 0)
                cnt++;
        }
        return cnt;
    }

    public List<HeroData> GetEnemyHeroDatas(int[] battleHeros, string pvpHeroData)
    {
        var enemyHeroDatas = new List<HeroData>();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(pvpHeroData);
        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////
        string _xml;
        using (var stringWriter = new StringWriter())
        using (var xmlTextWriter = XmlWriter.Create(stringWriter))
        {
            xmlDoc.WriteTo(xmlTextWriter);
            xmlTextWriter.Flush();
            _xml = stringWriter.GetStringBuilder().ToString();
        }
        if (_xml != null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HeroDatabase));
            var reader = new StringReader(_xml);
            HeroDatabase heroDB = serializer.Deserialize(reader) as HeroDatabase;
            reader.Close();

            foreach(var hero in heroDB.heros)
            {
                if(IsExistEntry(battleHeros,hero.id))
                {
                    enemyHeroDatas.Add(hero);
                }
            }
        }
        return enemyHeroDatas;
    }

    public List<Ability> GetEnemyAbilityData(string pvpAbilityData)
    {
        var enemyAbilityDatas = new List<Ability>();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(pvpAbilityData);
        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////
        string _xml;
        using (var stringWriter = new StringWriter())
        using (var xmlTextWriter = XmlWriter.Create(stringWriter))
        {
            xmlDoc.WriteTo(xmlTextWriter);
            xmlTextWriter.Flush();
            _xml = stringWriter.GetStringBuilder().ToString();
        }
        if (_xml != null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AbilityDatabase));
            var reader = new StringReader(_xml);
            AbilityDatabase abiltyDB = serializer.Deserialize(reader) as AbilityDatabase;
            reader.Close();

            foreach (var ability in abiltyDB.abilities)
            {
                enemyAbilityDatas.Add(ability);
            }
        }
        return enemyAbilityDatas;
    }
    public List<Item> GetEnemyItemData(string pvpItemData)
    {
        var enemyItemDatas = new List<Item>();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(pvpItemData);
        //복호화////
        XmlElement elmRoot = xmlDoc.DocumentElement;
        var decrpytData = DataSecurityManager.DecryptData(elmRoot.InnerText);
        elmRoot.InnerXml = decrpytData;
        //////////
        string _xml;
        using (var stringWriter = new StringWriter())
        using (var xmlTextWriter = XmlWriter.Create(stringWriter))
        {
            xmlDoc.WriteTo(xmlTextWriter);
            xmlTextWriter.Flush();
            _xml = stringWriter.GetStringBuilder().ToString();
        }
        if (_xml != null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ItemDatabase));
            var reader = new StringReader(_xml);
            ItemDatabase itemDB = serializer.Deserialize(reader) as ItemDatabase;
            reader.Close();

            foreach (var pvpItem in itemDB.items)
            {
                enemyItemDatas.Add(pvpItem);
            }
        }
        return enemyItemDatas;
    }

    HeroData GetEnemyHero(int id)
    {
        if(enemyHeroDataList!=null&&enemyHeroDataList.Count>0)
        {
            return enemyHeroDataList.Find(x => x.id.Equals(id) || x.id == id);
        }
        else
        {
            return HeroSystem.GetHero(id);
        }
    }

    public bool IsExistEntry(int[] datas, int data)
    {
        for(var i = 0; i < datas.Length; i++)
        {
            if (datas[i].Equals(data))
                return true;
        }
        return false;
    }

    public GameObject GetCurrentInBattleHero(int id)
    {
        foreach (var h in playerHeroList)
        {
            if (h.Key.Equals(id) || h.Key == id)
            {
                return h.Value;
            }
        }
        Debugging.Log(id + "의 영웅이 스테이지에 없음");
        return null;
    }

    public bool IsPlayingGame()
    {
        if (isPlayGame && isStartGame)
            return true;
        return false;
    }

    private void InitBattleMode()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.teleport);
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("SoftPortalRed");
            var spawnPoint = GameObject.Find("PlayersHero").transform;
            effect.transform.position = spawnPoint.position + new Vector3(-2, 0);
            effect.transform.localScale = Vector3.one * 3;
            effect.SetActive(true);
            GameObject effect2 = EffectPool.Instance.PopFromPool("SoftPortalRed");
            spawnPoint = GameObject.Find("EnemysHero").transform;
            effect2.transform.position = spawnPoint.position + new Vector3(2, 0);
            effect2.transform.localScale = Vector3.one * 3;
            effect2.SetActive(true);
        }
    }

    public void BattleEnd(bool isWin, bool isDraw=false)
    {
        Time.timeScale = 1.0f;
        if(isDraw)
        {
            isWin = enemyTotalHp > playerTotalHp ? false : true;
        }
        StartCoroutine(StageBattleEnding(isWin));
    }
    public void SetObtainMedal(bool isWin)
    {
        if (isWin)
        {
            ItemSystem.SetObtainItem(8002, 3);
            Debugging.Log("승리하여 메달 3개 획득");
        }
        else
        {
            ItemSystem.SetObtainItem(8002, 1);
            Debugging.Log("패배하여 메달 1개 획득");
        }

    }

    public void SetRankPoint(bool isWin)
    {
        int initPoint = 30;
        int refPoint = Mathf.Abs(User.battleRankPoint - enemyData.rankPoint);
        bool isUp = User.battleRankPoint > enemyData.rankPoint;
        float estimatedWinrate = 0.5f;
        float addEstimatedWinrate = Mathf.Clamp((refPoint/10) * 0.01f,0,0.5f);
        estimatedWinrate = isUp ? 0.5f + addEstimatedWinrate : 0.5f - addEstimatedWinrate;

        if (isWin)
        {
            User.battleWin += 1;
            getRankPoint = Mathf.Clamp((int)(initPoint * (1 - estimatedWinrate)),1,30);
        }
        else
        {
            User.battleLose += 1;
            getRankPoint = Mathf.Clamp((int)(initPoint * (0 - estimatedWinrate)), -30, -1);
        }

        User.battleRankPoint += getRankPoint;
        User.battleRankPoint = Mathf.Clamp(User.battleRankPoint, 1, 5000);
    }

    public void ExitPenalty()
    {
        SetRankPoint(false);
        GoogleSignManager.SaveData();
        GoogleSignManager.Instance.SetPvpRankPoint();
    }

    public IEnumerator StageBattleEnding(bool isWin)
    {
        SoundManager.instance.BgmSourceChange(null);
        if (isWin)
            SoundManager.instance.EffectSourcePlayNoPitch(AudioClipManager.instance.victory);
        isStartGame = false;
        //SetObtainMedal(isWin);
        SetRankPoint(isWin);
        GoogleSignManager.SaveData();
        GoogleSignManager.Instance.SetPvpRankPoint();
        while (!Common.isLoadCompleted)
            yield return null;
        GameObject endPanel = UI_Manager.instance.OpenBattleEndGamePanel(isWin);
        yield return new WaitForSeconds(3.0f);
        endPanel.gameObject.SetActive(false);
        if (UnityEngine.Random.Range(0, 10) < 4)
            GoogleAdMobManager.instance.OnBtnViewAdClicked();
        ShowResultUI();

        yield return null;
    }
    public void StageBattleErrorEnd()
    {
        isStartGame = false;
        UI_Manager.instance.OpenBattleEndGameErrorPanel();
    }

    public void ShowResultUI()
    {
        ResultUI.SetActive(true);
        ResultUI.GetComponent<UI_StagePvpResult>().ShowUI(getPvpResult, getRankPoint);
    }

    #region 영웅스탯
    public int GetHeroStatusAttack(ref HeroData data)
    {
        return (10) + (HeroSystem.GetHeroStr(data) * 5) + (HeroSystem.GetHeroInt(data) * 4) + (HeroSystem.GetHeroAgl(data) * 2) + GetAbilityStats(0) + GetHeroEquipmentItemAttack(ref data) + LabSystem.GetAddAttack(enemyData.labData.addAttack);

    }
    public int GetHeroStatusDefence(ref HeroData data)
    {
        return (HeroSystem.GetHeroPhy(data) * 5) + (HeroSystem.GetHeroAgl(data)) + GetAbilityStats(1) + GetHeroEquipmentItemDefence(ref data) + LabSystem.GetAddDefence(enemyData.labData.addAttack);
    }
    public int GetHeroStatusMaxHp(ref HeroData data)
    {
        return (200) + (data.strength * 2) + (HeroSystem.GetHeroInt(data)) + (HeroSystem.GetHeroPhy(data) * 15) + (HeroSystem.GetHeroAgl(data) * 3) + GetAbilityStats(2) + GetHeroEquipmentItemHp(ref data);
    }
    public int GetHeroStatusCriticalPercent(ref HeroData data)
    {
        return (int)((HeroSystem.GetHeroAgl(data) * 0.15f)) + GetAbilityStats(3) + GetHeroEquipmentItemCritical(ref data);
    }
    public int GetHeroStatusAttackSpeed(ref HeroData data)
    {
        return data.strength + (HeroSystem.GetHeroAgl(data) * 5) + GetAbilityStats(4) + GetHeroEquipmentItemAttackSpeed(ref data);
    }
    public int GetHeroStatusMoveSpeed(ref HeroData data)
    {
        return HeroSystem.GetHeroAgl(data) + 50 + GetAbilityStats(5) + GetHeroEquipmentItemMoveSpeed(ref data);
    }
    public  float GetHeroStatusKnockbackResist(ref HeroData data)
    {
        return (HeroSystem.GetHeroPhy(data) * 0.2f) + (HeroSystem.GetHeroAgl(data) * 0.05f);
    }
    public int GetHeroStatusSkillEnergy(ref HeroData data)
    {
        return HeroSystem.GetHeroInt(data) + GetAbilityStats(6) + GetHeroEquipmentItemSkillEnergy(ref data);
    }
    public int GetHeroStatusPenetration(ref HeroData data)
    {
        return GetHeroEquipmentItemSkillEnergy(ref data);
    }

    public int GetAbilityStats(int powerType)
    {
        powerType = Mathf.Clamp(powerType, 0, enemyAbilitiesStats.Length);
        return enemyAbilitiesStats[powerType];
    }
    public void SetAbilityStats()
    {
        if (enemyAbilityDataList != null)
        {
            int abilityStat;
            for (int i = 0; i < enemyAbilitiesStats.Length; i++)
            {
                abilityStat = 0;
                foreach (var ad in enemyAbilityDataList.FindAll(x => x.powerType == i))
                {
                    if (ad.level > 0)
                        abilityStat += ad.power * ad.level;
                }
                enemyAbilitiesStats[i] = abilityStat;
            }
            Debugging.Log("pvp 어빌리티 세팅 완료");
        }
    }

    public int[] GetHeroEquipmentItems(int id)
    {
        int[] itemIds = new int[5];
        HeroData data = enemyHeroDataList.Find(x => x.id == id || x.id.Equals(id));
        if (data != null)
        {
            string[] items = data.equipmentItem.Split(',');
            for (int i = 0; i < 5; i++)
            {
                int itemid = System.Convert.ToInt32(items[i]);
                if (itemid != 0)
                {
                    itemIds[i] = itemid;
                }
            }
        }
        return itemIds;
    }

    public int GetHeroEquipmentItemAttack(ref HeroData heroData)
    {
        int attack = 0;
        int[] heroItems = GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = enemyItemDataList.Find(x => (x.customId == heroItems[i] || x.customId.Equals(heroItems[i])) && x.itemtype == 0);
                if(item!=null)
                    attack += item != null ? ItemSystem.ItemAttack(item) : 0;
            }
        }
        return attack;
    }
    public int GetHeroEquipmentItemDefence(ref HeroData heroData)
    {
        int defence = 0;
        int[] heroItems = GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = enemyItemDataList.Find(x => (x.customId == heroItems[i] || x.customId.Equals(heroItems[i])) && x.itemtype == 0);
                defence += item != null ? ItemSystem.ItemDefence(item) : 0;
            }
        }
        return defence;
    }
    public int GetHeroEquipmentItemHp(ref HeroData heroData)
    {
        int hp = 0;
        int[] heroItems = GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = enemyItemDataList.Find(x => (x.customId == heroItems[i] || x.customId.Equals(heroItems[i])) && x.itemtype == 0);

                hp += item != null ? ItemSystem.ItemHp(item) : 0;
            }
        }
        return hp;
    }
    public int GetHeroEquipmentItemCritical(ref HeroData heroData)
    {
        int cri = 0;
        int[] heroItems = GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = enemyItemDataList.Find(x => (x.customId == heroItems[i] || x.customId.Equals(heroItems[i])) && x.itemtype == 0);
                cri += item != null ? ItemSystem.ItemCritical(item) : 0;
            }
        }
        return cri;
    }
    public int GetHeroEquipmentItemAttackSpeed(ref HeroData heroData)
    {
        int aSpeed = 0;
        int[] heroItems = GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = enemyItemDataList.Find(x => (x.customId == heroItems[i] || x.customId.Equals(heroItems[i])) && x.itemtype == 0);
                aSpeed += item != null ? ItemSystem.ItemAttackSpeed(item) : 0;
            }
        }
        return aSpeed;
    }
    public int GetHeroEquipmentItemMoveSpeed(ref HeroData heroData)
    {
        int mSpeed = 0;
        int[] heroItems = GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = enemyItemDataList.Find(x => (x.customId == heroItems[i] || x.customId.Equals(heroItems[i])) && x.itemtype == 0);
                mSpeed += item != null ? ItemSystem.ItemMoveSpeed(item) : 0;
            }
        }
        return mSpeed;
    }
    public int GetHeroEquipmentItemSkillEnergy(ref HeroData heroData)
    {
        int energy = 0;
        int[] heroItems = GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = enemyItemDataList.Find(x => (x.customId == heroItems[i] || x.customId.Equals(heroItems[i])) && x.itemtype == 0);
                energy += item != null ? ItemSystem.ItemSkillEnergy(item) : 0;
            }
        }
        return energy;
    }
    public int GetHeroEquipmentItemPenetration(ref HeroData heroData)
    {
        int pent = 0;
        int[] heroItems = GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = ItemSystem.GetUserEquipmentItem(heroItems[i]);
                pent += item != null ? ItemSystem.ItemPenetration(item) : 0;
            }
        }
        return pent;
    }
    public int GetHeroMaxDamageLevel()
    {
        return enemyData.labData.addMaxDamage;
    }



    #endregion
}