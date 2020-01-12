using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MapSelect : MonoBehaviour
{
    #region 변수
    //스크롤 스냅뷰
    GameObject ScrollSnapMapView;
    //콘텐츠 뷰
    GameObject ContentView;
    //선택 이펙트
    public GameObject ShowEffectPrefab;
    private GameObject showEffect;
    // 스테이지 슬롯
    List<GameObject> mapSlotList;
    // 노드 슬롯
    Dictionary<int, GameObject> mapNodeList;
    // 노드 프리팹
    public GameObject mapSlotNodePrefab;

    // 선택된 맵정보
    GameObject mapInfoPanel;
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
    

    #endregion

    private void Awake()
    {
        ScrollSnapMapView = this.transform.GetComponentInChildren<DynamicContentController>().gameObject;
        ContentView = ScrollSnapMapView.transform.GetChild(0).GetChild(0).gameObject;
        mapInfoPanel = this.transform.GetChild(0).GetChild(2).gameObject;
        if(ShowEffectPrefab!=null)
        {
            showEffect = Instantiate(ShowEffectPrefab, this.transform);
            showEffect.SetActive(false);
        }
        foreach (var text in mapInfoPanel.GetComponentsInChildren<Text>())
        {
            if (text.name.Equals("mapNameText"))
                mapNameText = text;
            else if (text.name.Equals("mapDescriptionText"))
                mapDescriptionText = text;
        }
    }
    private void OnEnable()
    {
        ShowHero();
    }
    
    void Start()
    {
        if (mapSlotNodePrefab!=null)
        {
            int stageCount = (int)((MapSystem.maps.Count-1)/10)+1;
            int currentStageIndex = 0;
            // 스테이지 변경이벤트 추가
            ScrollSnapMapView.GetComponent<SimpleScrollSnap>().onPanelChanged.RemoveAllListeners();
           ScrollSnapMapView.GetComponent<SimpleScrollSnap>().onPanelChanged.AddListener(delegate
            {
                OnStagePanelChanged();
            });
            // Stage 카운트만큼 스크롤스냅뷰 추가
            ScrollSnapMapView.GetComponent<DynamicContentController>().AddToBack(stageCount);
            // 추가된 스테이지 슬롯을 담을 리스트 추가
            mapSlotList = new List<GameObject>();
            // 맵 노드의 아이디와 슬롯을 담을 리스트 추가
            mapNodeList = new Dictionary<int,GameObject>();
            // 추가된 스테이지 슬롯에맞는 맵 노드 추가
            for (var j = 0; j < ContentView.transform.childCount; j++)
            {
                ContentView.transform.GetChild(j).GetChild(1).GetChild(0).GetComponent<Image>().sprite = MapSystem.GetStageThumbnailImage(j + 1);

                mapSlotList.Add(ContentView.transform.GetChild(j).gameObject);
                foreach(var mapNode in MapSystem.GetMapNode(j+1))
                {
                    if(mapNode!=null)
                    {
                        GameObject mapNodePrefab = Instantiate(mapSlotNodePrefab, mapSlotList[j].transform.GetChild(0).transform);
                        // 맵노드 버튼 이벤트 추가
                        mapNodePrefab.GetComponent<Button>().onClick.AddListener(delegate
                        {
                            OnMapNodeClick(mapNode.id);
                        });
                        mapNodePrefab.SetActive(true);
                        // 맵노드리스트에 아이디와 슬롯프리팹을 추가
                        mapNodeList.Add(mapNode.id, mapNodePrefab);
                        // 아직 갈 수 없는 맵노드에 잠금 아이콘 추가
                        if(!MapSystem.isAbleMap(mapNode.id))
                        {
                            mapNodePrefab.transform.GetChild(0).GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("UI/ui_lock_transparent");
                            mapNodePrefab.transform.GetChild(1).gameObject.SetActive(false);
                        }
                        else
                        {
                            if (mapNode.clearPoint > 2)
                            {
                                mapNodePrefab.transform.GetChild(0).gameObject.SetActive(true);
                                mapNodePrefab.transform.GetChild(1).gameObject.SetActive(true);
                            }
                            else if(mapNode.clearPoint==2)
                            {
                                mapNodePrefab.transform.GetChild(0).gameObject.SetActive(true);
                                mapNodePrefab.transform.GetChild(1).gameObject.SetActive(false);
                            }
                            else
                            {
                                mapNodePrefab.transform.GetChild(0).gameObject.SetActive(false);
                                mapNodePrefab.transform.GetChild(1).gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }

            // 현재 유저가 갈 수있는 최상단 스테이지를 찾음
            currentMapId = MapSystem.GetCurrentMapId();
            // 현재 스테이지 넘버인덱스
            currentStageIndex = MapSystem.GetMap(currentMapId).stageNumber-1;
            for(var i = 0; i < mapSlotList.Count; i++)
            {
                if(i> currentStageIndex)
                {
                    mapSlotList[i].transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    mapSlotList[i].transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
                }
            }
            // 찾은 스테이지 정보를 뷰로 보여줌
            mapNameText.text = string.Format("{0} <size='40'>{1}</size>", MapSystem.GetStageName(currentStageIndex), MapSystem.GetMap(currentMapId).name);
            mapDescriptionText.text = MapSystem.GetStageDescription(currentStageIndex);
            foreach (var node in mapNodeList)
            {
                if(node.Key==currentMapId)
                {
                    mapNodeImage = node.Value.GetComponent<Image>();
                    break;
                }
            }
            //mapNodeImage.overrideSprite = Resources.Load<Sprite>("UI/ui_container2");
            ScrollSnapMapView.GetComponent<SimpleScrollSnap>().GoToPanel(currentStageIndex);
            if (hero == null)
            {
                hero = Instantiate(PrefabsDatabaseManager.instance.GetHeroPrefab(User.profileHero), mapNodeImage.transform);
                if (hero.GetComponent<Hero>() != null)
                    Destroy(hero.GetComponent<Hero>());
                if (hero.GetComponent<Rigidbody2D>() != null)
                    Destroy(hero.GetComponent<Rigidbody2D>());
                foreach (var sp in hero.GetComponentsInChildren<SpriteRenderer>())
                {
                    sp.sortingLayerName = "ShowObject";
                    sp.gameObject.layer = 16;
                }
                hero.transform.localRotation = Quaternion.Euler(0, 180, 0);
                hero.GetComponent<Animator>().SetBool("isMoving", true);
                hero.GetComponent<Animator>().SetBool("isRun", true);
            }
            ShowHero();
        }
    }

    // 스테이지 변경 이벤트
    public void OnStagePanelChanged()
    {
        int stageNumber = ScrollSnapMapView.GetComponent<SimpleScrollSnap>().TargetPanel+1;
        currentMapId = MapSystem.GetCurrentMapId(stageNumber);
        // 현재 패널의 스테이지가 자신의 스테이지 보다 같거나 작을때
        if (stageNumber <= MapSystem.GetMap(currentMapId).stageNumber)
        {
            mapNameText.text =string.Format("{0} <size='40'>{1}</size>",MapSystem.GetStageName(stageNumber-1) , MapSystem.GetMap(currentMapId).name);
            mapDescriptionText.text = MapSystem.GetStageDescription(stageNumber - 1);
        }
        else
        {
            mapNameText.text = MapSystem.GetStageName(stageNumber - 1);
            mapDescriptionText.text = string.Format("<color='red'>{0}</color>", LocalizationManager.GetText("MapWarningMessage"));
        }
        foreach (var node in mapNodeList)
        {
            if (node.Key == currentMapId)
            {
                mapNodeImage = node.Value.GetComponent<Image>();
                break;
            }
        }
        mapInfoPanel.GetComponent<AiryUIAnimatedElement>().ShowElement();
        ShowHero();
    }

    // 맵노드 클릭 이벤트
    public void OnMapNodeClick(int mapId)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        if (MapSystem.isAbleMap(mapId))
        {
            currentMapId = mapId;
            mapNameText.text = string.Format("{0} <size='40'>{1}</size>", MapSystem.GetStageName(MapSystem.GetMap(currentMapId).stageNumber - 1), MapSystem.GetMap(currentMapId).name);
            foreach (var node in mapNodeList)
            {
                if (node.Key == mapId)
                {
                    mapNodeImage = node.Value.GetComponent<Image>();
                    break;
                }
            }
            mapInfoPanel.GetComponent<AiryUIAnimatedElement>().ShowElement();
            ShowHero();
        }
        else
        {
            Debugging.Log(mapId + " 의 맵은 아직 열리지 않은 맵입니다.");
        }
    }

    public void ShowHero()
    {
        if (hero != null)
        {
            hero.transform.parent = mapNodeImage.transform;
            hero.transform.localScale = new Vector3(100, 100, 100);
            hero.transform.localPosition = Vector3.zero + new Vector3(0,50);
            hero.GetComponent<Animator>().SetBool("isMoving", true);
            hero.GetComponent<Animator>().SetBool("isRun", true);
            hero.gameObject.SetActive(true);
            if (showEffect != null)
            {
                showEffect.transform.parent = mapNodeImage.transform;
                showEffect.transform.localScale = new Vector3(100, 100, 100);
                showEffect.transform.localPosition = Vector3.zero + new Vector3(0, 150);
                showEffect.gameObject.SetActive(true);
            }
        }
    }
    public void OnMapSelectCompletedClick()
    {
        Common.stageModeType = Common.StageModeType.Main;
        GameManagement.instance.SetStageInfo(currentMapId);
        Debugging.Log(currentMapId + " 의 맵 선택완료.");
    }
}
