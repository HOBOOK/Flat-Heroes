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
                    flag = !flag;
                    if(!heroList.ContainsKey(id))
                        heroList.Add(id,hero);
                }
            }
        }
        Debugging.Log(User.stageHeros.Length + "의 리스트에서 Stage씬에 영웅소환 완료.");
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
            if(heroInfoUI!=null&&heroInfoUI.activeSelf&& heroUITarget!=null)
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
        heroUIname.text = hero.HeroName;
        heroUIinfo.text = string.Format("레벨:{0}\r\n전투력:{1}\r\n", hero.status.level, Common.GetHeroPower(hero));
        heroInfoUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
    }

    public void SetStageHeros(int index, int id)
    {
        User.stageHeros[index] = id;
        Debugging.Log(index + " 열에 " + id + "의 영웅 추가됨 >> " + User.stageHeros[index]);
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
}
