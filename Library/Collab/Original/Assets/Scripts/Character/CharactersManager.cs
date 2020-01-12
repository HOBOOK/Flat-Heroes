using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharactersManager : MonoBehaviour
{

    Dictionary<int, GameObject> heroList = new Dictionary<int, GameObject>();
    private GameObject heroInfoUI;
    private Text heroUIname;
    private Text heroUIinfo;
    private Text heroUIlevel;
    private Transform heroUITarget;
    public static CharactersManager instance = null;
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Update()
    {
        if (!Common.GetSceneCompareTo(Common.SCENE.STAGE))
        {
            LobbyUpdate();
        }
    }
    public void SetStagePositionHeros()
    {
        heroList = new Dictionary<int, GameObject>();
        var spawnPoint = GameObject.Find("PlayersHero").transform;
        bool flag = false;
        for (int i = 0; i < User.stageHeros.Length; i++)
        {
            if (User.stageHeros[i]!=0)
            {
                if (PrefabsDatabaseManager.instance.GetHeroPrefab(User.stageHeros[i]) != null)
                {
                    int id = User.stageHeros[i];
                    GameObject hero = Instantiate(PrefabsDatabaseManager.instance.GetHeroPrefab(id)) as GameObject;
                    hero.transform.parent = spawnPoint;
                    hero.transform.localPosition = Vector3.zero - new Vector3(i, 0);
                    hero.GetComponent<Hero>().SetFirstPos(hero.transform.position);
                    flag = !flag;
                    if(!heroList.ContainsKey(id))
                        heroList.Add(id,hero);
                }
            }
        }
        Debugging.Log(User.stageHeros.Length + "의 리스트에서 Stage씬에 영웅소환 완료.");
    }
    public void SetInfinityStagePositionHeros()
    {
        heroList = new Dictionary<int, GameObject>();
        var spawnPoint = GameObject.Find("PlayersHero").transform;
        bool flag = false;
        for (int i = 0; i < User.stageHeros.Length; i++)
        {
            if (User.stageHeros[i] != 0)
            {
                if (PrefabsDatabaseManager.instance.GetHeroPrefab(User.stageHeros[i]) != null)
                {
                    int id = User.stageHeros[i];
                    GameObject hero = Instantiate(PrefabsDatabaseManager.instance.GetHeroPrefab(id)) as GameObject;
                    hero.transform.parent = spawnPoint;
                    hero.transform.localPosition = Vector3.zero - new Vector3(i*0.5f, 0);
                    hero.GetComponent<Hero>().SetFirstPos(hero.transform.position);
                    flag = !flag;
                    if (!heroList.ContainsKey(id))
                        heroList.Add(id, hero);
                }
            }
        }
        Debugging.Log(User.stageHeros.Length + "의 리스트에서 Stage씬에 영웅소환 완료.");
    }


    bool IsAlreayAlive(int id)
    {
        if(Common.stageModeType==Common.StageModeType.Battle)
        {
            return StageBattleManager.instance.IsAlreayAlive(id);
        }
        else
        {
            foreach (var hero in heroList)
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
    }
    public void ResurrectionHero(int id)
    {
        if (PrefabsDatabaseManager.instance.GetHeroPrefab(id) != null&&StageManagement.instance.isStageStart()&&!IsAlreayAlive(id))
        {
            var spawnPoint = GameObject.Find("PlayersHero").transform;
            GameObject hero = Instantiate(PrefabsDatabaseManager.instance.GetHeroPrefab(id)) as GameObject;
            hero.transform.parent = spawnPoint;
            hero.transform.localPosition = Vector3.zero;
            if(Common.stageModeType == Common.StageModeType.Battle)
            {
                StageBattleManager.instance.PvpResurrectionHero(id, hero);
            }
            else
            {
                if (heroList.ContainsKey(id))
                {
                    heroList.Remove(id);
                    heroList.Add(id, hero);
                }
            }
            List<GameObject> enemyList = Common.FindEnemy(true);
            if(enemyList!=null&&enemyList.Count>0)
            {
                foreach(var enemy in enemyList)
                {
                    enemy.GetComponent<Hero>().ResearchingEnemys(hero);
                }
            }
            if(hero!=null)
                HeroSkillManager.instance.ResurrectionHero(hero.GetComponent<Hero>());
        }
    }
    public GameObject GetCurrentInStageHero(int id)
    {
        foreach(var h in heroList)
        {
            if(h.Key.Equals(id)||h.Key==id)
            {
                return h.Value;
            }
        }
        Debugging.Log(id + "의 영웅이 스테이지에 없음");
        return null;
    }
    public int GetStageHeroCount()
    {
        int cnt = 0;
        for(var i = 0; i < User.stageHeros.Length; i++)
        {
            if (User.stageHeros[i] != 0)
                cnt++;
        }
        return cnt;
    }
    public int GetBattleHeroCount()
    {
        int cnt = 0;
        for (var i = 0; i < User.battleHeros.Length; i++)
        {
            if (User.battleHeros[i] != 0)
                cnt++;
        }
        return cnt;
    }
    public void SetLobbyPositionHeros()
    {
        if (User.lobbyHeros!=null)
        {
            for (int i = 0; i < User.lobbyHeros.Length; i++)
            {
                if (User.lobbyHeros[i] != 0)
                {
                    var LobbyPoint = PrefabsDatabaseManager.instance.GetLobbyPoint(i);
                    if (PrefabsDatabaseManager.instance.GetHeroPrefab(User.lobbyHeros[i]) != null)
                    {
                        int id = User.lobbyHeros[i];
                        GameObject hero = Instantiate(PrefabsDatabaseManager.instance.GetHeroPrefab(id)) as GameObject;
                        hero.transform.parent = LobbyPoint;
                        hero.transform.localPosition = Vector3.zero;
                    }
                }
            }
            Debugging.Log(User.lobbyHeros.Length + "의 리스트에서 Main씬에 영웅소환 완료.");
        }

    }

    void LobbyUpdate()
    {
        if (Common.GetSceneCompareTo(Common.SCENE.MAIN))
        {
            CastRay();
            if (heroInfoUI != null && heroInfoUI.activeSelf && heroUITarget != null)
            {
                heroInfoUI.transform.position = heroUITarget.transform.position + new Vector3(0, 2);
            }
        }
    }
    void CastRay()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()&&hit.transform!=null&&hit.transform.GetComponent<Hero>()!=null)
            {
                Debugging.Log(hit.transform.name + " 선택됨");
                Hero hero = hit.transform.GetComponent<Hero>();
                StartCoroutine(OpenHeroInfo(hero));
                if (!hero.animator.GetBool("isAttack"))
                {
                    hero.StartCoroutine("LobbyAttack");
                }
            }
        }
    }
    void SetHeroInfoUI()
    {
        if (heroInfoUI == null)
        {
            heroInfoUI = UI_Manager.instance.PopHeroInfoSummaryUI;
            foreach (var i in heroInfoUI.GetComponentsInChildren<Text>())
            {
                if (i.name.Equals("heroName"))
                    heroUIname = i;
                else if (i.name.Equals("heroInfo"))
                    heroUIinfo = i;
                else if (i.name.Equals("heroLv"))
                    heroUIlevel = i;
            }
        }
    }

    IEnumerator OpenHeroInfo(Hero hero)
    {
        SetHeroInfoUI();
        if (heroInfoUI.activeSelf)
            heroInfoUI.GetComponent<AiryUIAnimatedElement>().HideElement();
        yield return new WaitForSeconds(0.2f);
        heroInfoUI.transform.position = hero.transform.position + new Vector3(0, 2);
        heroInfoUI.GetComponent<AiryUIAnimatedElement>().initialWorldPosition= hero.transform.position + new Vector3(0, 2);
        yield return new WaitForSeconds(0.1f);
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
        heroUITarget = hero.transform;
        heroUIname.text = HeroSystem.GetHeroName(hero.id);
        heroUIinfo.text = string.Format("{0} <size='23'>FP</size>", Common.GetHeroPower(hero).ToString());
        heroUIlevel.text = hero.status.level.ToString();
        heroInfoUI.transform.GetChild(1).GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        heroInfoUI.transform.GetChild(1).GetComponentInChildren<Button>().onClick.AddListener(delegate
        {
            OnClickDetailHeroInfoButton(hero);
        });
        if(hero.heroData.ability>0)
        {
            heroInfoUI.transform.GetChild(3).gameObject.SetActive(true);
            heroInfoUI.transform.GetChild(3).GetComponentInChildren<Text>().text = string.Format("{0} {1}",HeroAbilitySystem.GetHeroAbilityName(hero.heroData.ability),Common.getRomeNumber(hero.heroData.abilityLevel));
        }
        else
        {
            heroInfoUI.transform.GetChild(3).gameObject.SetActive(false);
        }
        heroInfoUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
    }
    void OnClickDetailHeroInfoButton(Hero hero)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        UI_Manager.instance.PopHeroInfoUI.SetActive(true);
        UI_Manager.instance.PopHeroInfoUI.SetActive(true);
        UI_Manager.instance.PopHeroInfoUI.GetComponent<UI_HeroInfo>().ShowHero(PrefabsDatabaseManager.instance.GetHeroPrefab(hero.id), hero.heroData);
    }

    public void SetStageHeros(int index, int id)
    {
        User.stageHeros[index] = id;
    }
    public void SetPvpHeros(int index, int id)
    {
        User.battleHeros[index] = id;
    }
    public bool GetLobbyHeros(int id)
    {
        for(var i = 0; i< User.lobbyHeros.Length; i++)
        {
            if (User.lobbyHeros[i] == id)
                return true;
        }
        return false;
    }

    public bool SetLobbyHeros(int id)
    {
        if(GetLobbyHeros(id))
        {
            for (var i = 0; i < User.lobbyHeros.Length; i++)
            {
                if (User.lobbyHeros[i] == id)
                {
                    User.lobbyHeros[i] = 0;
                    var LobbyPoint = PrefabsDatabaseManager.instance.GetLobbyPoint(i);
                    if (LobbyPoint.transform.childCount > 0 && LobbyPoint.transform.GetChild(0) != null)
                        Destroy(LobbyPoint.transform.GetChild(0).gameObject);
                    break;
                }
            }
            Debugging.Log(id + " 영웅 로비해체");
            return true;
        }
        else
        {
            int ix = -1;
            for (var i = 0; i < User.lobbyHeros.Length; i++)
            {
                if (User.lobbyHeros[i] == 0)
                {
                    ix = i;
                    break;
                }
            }
            if (ix != -1)
            {
                User.lobbyHeros[ix] = id;
                var LobbyPoint = PrefabsDatabaseManager.instance.GetLobbyPoint(ix);
                if (LobbyPoint.transform.childCount > 1 && LobbyPoint.transform.GetChild(1) != null)
                    Destroy(LobbyPoint.transform.GetChild(1).gameObject);
                if (PrefabsDatabaseManager.instance.GetHeroPrefab(id) != null)
                {
                    GameObject hero = Instantiate(PrefabsDatabaseManager.instance.GetHeroPrefab(id)) as GameObject;
                    hero.transform.parent = LobbyPoint;
                    hero.transform.localPosition = Vector3.zero;
                }
                Debugging.Log(id + " 영웅 " + ix + " 열의 로비에 배치완료");
                return true;
            }
            else
            {
                Debugging.Log("로비에 자리없음");
                return false;
            }
        }
    }


    public bool IsExistedStageHero(int id)
    {
        foreach (var i in User.stageHeros)
        {
            if (i == id)
                return true;
        }
        return false;
    }

    public bool IsExistedPvpHero(int id)
    {
        foreach (var i in User.battleHeros)
        {
            if (i == id)
                return true;
        }
        return false;
    }
}
