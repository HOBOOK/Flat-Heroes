using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MapSelectNew : MonoBehaviour
{
    #region 변수
    public GameObject ContentView;
    public GameObject MapInfoPanel;
    //선택 이펙트
    public GameObject ShowEffectPrefab;
    private GameObject showEffect;
    // 노드 프리팹
    public GameObject mapSlotNodePrefab;
    // 선택된 맵이름 텍스트
    Text mapNameText;
    // 선택된 맵정보 텍스트
    Text mapDescriptionText;
    // 선택된 스테이지 이미지
    Image mapImage;
    // 선택된 맵노드 이미지
    Image mapNodeImage;
    // 선택된 맵노드에 보여줄 영웅오브젝트
    GameObject hero;
    // 선택된 맵 ID
    int currentMapId;
    // 선택된 맵 INDEX
    int currentMapIndex;
    #endregion

    private void Awake()
    {
        if(ShowEffectPrefab!=null)
        {
            showEffect = Instantiate(ShowEffectPrefab, this.transform);
            showEffect.SetActive(false);
        }
        foreach (var text in MapInfoPanel.GetComponentsInChildren<Text>())
        {
            if (text.name.Equals("mapNameText"))
                mapNameText = text;
            else if (text.name.Equals("mapDescriptionText"))
                mapDescriptionText = text;
        }
    }
    private void OnEnable()
    {
        RefreshUI();
    }
    public void GenerateMapNodes()
    {
        MapSystem.LoadMap();
        CreateMapNodesI();
    }
    void CreateMapNodesI()
    {
        if (mapSlotNodePrefab != null)
        {
            for(int i = 0; i < ContentView.transform.childCount; i++)
            {
                this.GetComponentInChildren<SimpleScrollSnap>().Remove(i);
            }
            foreach (var mapNode in MapSystem.GetMapNodeAll())
            {
                if (mapNode != null)
                {
                    // 맵노드 생성
                    GameObject mapNodePrefab = mapSlotNodePrefab;
                    mapNodePrefab.name = mapNode.name;
                    this.GetComponentInChildren<SimpleScrollSnap>().AddToBack(mapNodePrefab);
                }
            }
        }
    }
    void RefreshUI()
    {
        StartCoroutine("ShowSelectMap");
    }

    public IEnumerator ShowSelectMap()
    {
        while(ContentView.transform.childCount<MapSystem.GetMapCount())
        {
            Debugging.Log("로딩........");
            yield return null;
        }
        currentMapId = MapSystem.GetCurrentAllMapId();
        currentMapIndex = GetMapIndex(currentMapId);
        Debugging.Log(currentMapIndex + "번째 인덱스가 현재 맵입니다.");
        for (int i = 0; i < ContentView.transform.childCount; i++)
        {
            int tempEventIndex = i;
            if (i==currentMapIndex)
            {
                mapNodeImage = ContentView.transform.GetChild(i).GetComponent<Image>();
                ShowMapInfo(GetMapId(i));
                if (hero == null)
                {
                    hero = Instantiate(PrefabsDatabaseManager.instance.GetHeroPrefab(101), mapNodeImage.transform);
                    if (hero.GetComponent<Hero>() != null)
                        DestroyImmediate(hero.GetComponent<Hero>());
                    if (hero.GetComponent<Rigidbody2D>() != null)
                        DestroyImmediate(hero.GetComponent<Rigidbody2D>());
                    foreach (var sp in hero.GetComponentsInChildren<SpriteRenderer>())
                    {
                        sp.sortingLayerName = "ShowObject";
                        sp.gameObject.layer = 16;
                    }
                    hero.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    hero.GetComponent<Animator>().SetBool("isMoving", true);
                    hero.GetComponent<Animator>().SetBool("isRun", true);
                }
                this.GetComponentInChildren<SimpleScrollSnap>().GoToPanel(tempEventIndex);
                ShowHero();
            }
            ContentView.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            ContentView.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate
            {
                OnMapNodeClick(MapSystem.GetMap(GetMapId(tempEventIndex)).id, tempEventIndex);
            });
            if (!MapSystem.isAbleMap(MapSystem.GetMap(GetMapId(i)).id))
            {
                ContentView.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("UI/ui_lock_transparent");
            }
            else
            {
                if (MapSystem.GetMap(GetMapId(i)).clearPoint < 1)
                {
                    ContentView.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                }
                else
                {
                    ContentView.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
            }
        }
    }
    int GetMapIndex(int id)
    {
        return id - 5001;
    }
    int GetMapId(int index)
    {
        return index + 5001;
    }
    // 맵노드 클릭 이벤트
    public void OnMapNodeClick(int mapId, int index)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        if (MapSystem.isAbleMap(mapId))
        {
            currentMapId = GetMapId(index);
            Debugging.Log(index + "번째 노드클릭");
            mapNodeImage = ContentView.transform.GetChild(index).GetComponent<Image>();
            this.GetComponentInChildren<SimpleScrollSnap>().GoToPanel(index);
            ShowMapInfo(mapId);
            ShowHero();
        }
        else
        {
            Debugging.Log(mapId + " 의 맵은 아직 열리지 않은 맵입니다.");
        }
    }
    void ShowMapInfo(int mapId)
    {
        //mapNameText.text = string.Format("{0} {1}", MapSystem.GetStageName(stageNumber - 1), MapSystem.GetMap(currentMapId).name);
        //mapDescriptionText.text = MapSystem.GetStageDescription(stageNumber - 1);
        MapInfoPanel.GetComponent<AiryUIAnimatedElement>().ShowElement();
    }

    public void ShowHero()
    {
        if (hero != null)
        {
            hero.transform.parent = mapNodeImage.transform;
            hero.transform.localScale = new Vector3(30, 30, 30);
            hero.transform.localPosition = Vector3.zero + new Vector3(0,10);
            hero.GetComponent<Animator>().SetBool("isMoving", true);
            hero.GetComponent<Animator>().SetBool("isRun", true);
            hero.gameObject.SetActive(true);
            if (showEffect != null)
            {
                showEffect.transform.parent = mapNodeImage.transform;
                showEffect.transform.localScale = new Vector3(30, 30, 30);
                showEffect.transform.localPosition = Vector3.zero + new Vector3(0, 40);
                showEffect.gameObject.SetActive(true);
            }
        }
    }
    public void OnMapSelectCompletedClick()
    {
        GameManagement.instance.SetStageInfo(currentMapId);
        Debugging.Log(currentMapId + " 의 맵 선택완료.");
    }
}
