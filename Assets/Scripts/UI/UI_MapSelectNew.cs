﻿using DanielLochner.Assets.SimpleScrollSnap;
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
    int mapIndex;
    bool isNodeLoadCompleted;
    Dictionary<int,Map> mapDatas;
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
    void RefreshUI()
    {
        if (mapSlotNodePrefab != null)
        {
            isNodeLoadCompleted = false;
            mapIndex = 0;
            currentMapIndex = 0;
            mapDatas = new Dictionary<int, Map>();
            mapDatas.Clear();
            currentMapId = MapSystem.GetCurrentAllMapId();
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
                    if (mapNode.id == currentMapId)
                    {
                        currentMapIndex = mapIndex;
                    }

                    mapDatas.Add(mapIndex, mapNode);
                    mapIndex++;
                    this.GetComponentInChildren<SimpleScrollSnap>().AddToBack(mapNodePrefab);
                }
            }
            isNodeLoadCompleted = true;
            StartCoroutine("ShowSelectMap");
        }
    }

    public IEnumerator ShowSelectMap()
    {
        while(!isNodeLoadCompleted)
        {
            Debugging.Log("로딩........");
            yield return null;
        }
        for (int i = 0; i < ContentView.transform.childCount; i++)
        {
            int tempEventIndex = i;
            if (i==currentMapIndex)
            {
                mapNodeImage = ContentView.transform.GetChild(i).GetComponent<Image>();
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
            ContentView.transform.GetChild(i).transform.localPosition = new Vector3((i*100)+2000, ContentView.transform.GetChild(i).transform.localPosition.y);
            ContentView.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            ContentView.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate
            {
                OnMapNodeClick(mapDatas[tempEventIndex].id, tempEventIndex);
            });
            if(mapDatas.ContainsKey(i))
            {
                if (!MapSystem.isAbleMap(mapDatas[i].id))
                {
                    ContentView.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("UI/ui_lock_transparent");
                }
                else
                {
                    if (mapDatas[i].clearPoint < 1)
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
    }
    // 맵노드 클릭 이벤트
    public void OnMapNodeClick(int mapId, int index)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        if (MapSystem.isAbleMap(mapId))
        {
            Debugging.Log(index + "번째 노드클릭");
            mapNodeImage = ContentView.transform.GetChild(index).GetComponent<Image>();
            this.GetComponentInChildren<SimpleScrollSnap>().GoToPanel(index);
            mapNameText.text = MapSystem.GetMap(mapId).name;
            mapDescriptionText.text = MapSystem.GetMap(mapId).description;
            MapInfoPanel.GetComponent<AiryUIAnimatedElement>().ShowElement();
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
