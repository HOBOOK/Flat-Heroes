using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_PvpHeroSelect : MonoBehaviour
{
    #region 변수
    //컨테이너
    GameObject PanelHeroContainer;
    //영웅리스트창
    GameObject ScrollViewContent;
    public GameObject selectHeroLockCover;
    public GameObject heroSlotPrefab;
    public GameObject pinPoint;
    public GameObject playerSkillPanel;
    public Text useEnergyText;
    Text slotNameText;
    Image slotHeroImage;
    // 영웅선택창
    int index;
    GameObject PanelHeroSelection;
    Text selectedHeroNameText;
    GameObject selectPanel;
    Image selectedHeroImage;
    public Button StartButton;

    #endregion

    private void Awake()
    {
        PanelHeroContainer = this.transform.GetChild(0).gameObject;
        ScrollViewContent = PanelHeroContainer.GetComponentInChildren<GridLayoutGroup>().gameObject;
        PanelHeroSelection = PanelHeroContainer.transform.GetChild(1).gameObject;

        pinPoint = Instantiate(pinPoint, this.transform);
        pinPoint.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        RefreshUI();
    }
    void RefreshUI()
    {
        if (heroSlotPrefab != null)
        {
            foreach (Transform child in ScrollViewContent.transform)
            {
                if (!child.name.Equals("none"))
                    Destroy(child.gameObject);
            }
            foreach (var heroSlot in HeroSystem.GetUserHeros())
            {
                if (heroSlot.id < 500)
                {
                    GameObject slotPrefab = Instantiate(heroSlotPrefab, ScrollViewContent.transform);
                    foreach (var i in slotPrefab.GetComponentsInChildren<Text>())
                    {
                        if (i.name.Equals("heroName"))
                            slotNameText = i;
                    }
                    if (slotNameText != null)
                        slotNameText.text = HeroSystem.GetHeroName(heroSlot.id);
                    slotHeroImage = slotPrefab.transform.GetChild(0).GetChild(1).GetComponent<Image>();
                    if (slotHeroImage != null)
                        slotHeroImage.sprite = HeroSystem.GetHeroThumbnail(heroSlot.id);
                    slotPrefab.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = HeroSystem.GetHeroClassImage(heroSlot);
                    if (heroSlot.ability > 0)
                    {
                        slotPrefab.transform.GetChild(3).gameObject.SetActive(true);
                        slotPrefab.transform.GetChild(3).GetComponentInChildren<Text>().text = heroSlot.abilityLevel.ToString();
                    }
                    else
                    {
                        slotPrefab.transform.GetChild(3).gameObject.SetActive(false);
                    }
                    slotPrefab.GetComponent<Button>().onClick.RemoveAllListeners();
                    slotPrefab.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        OnItemSlotClick(heroSlot);
                    });
                    if (!CharactersManager.instance.IsExistedPvpHero(heroSlot.id))
                    {
                        slotPrefab.GetComponent<Button>().enabled = true;
                        slotPrefab.transform.GetChild(2).gameObject.SetActive(false);
                    }
                    else
                    {
                        slotPrefab.GetComponent<Button>().enabled = false;
                        slotPrefab.transform.GetChild(2).gameObject.SetActive(true);
                    }
                }
            }
        }
        int stageHeroCount = CharactersManager.instance.GetBattleHeroCount();
        if (stageHeroCount < 1)
        {
            StartButton.gameObject.SetActive(false);
        }
        else
        {
            StartButton.gameObject.SetActive(true);
            StartButton.gameObject.GetComponent<AiryUIAnimatedElement>().ShowElement();
            useEnergyText.text = stageHeroCount.ToString();
        }

        SetImageAndTextSelectHeroPanel();
        RefreshPlayerSkillUI();
        selectHeroLockCover.SetActive(true);
    }

    public void SetImageAndTextSelectHeroPanel()
    {
        for (int i = 0; i < User.battleHeros.Length; i++)
        {
            HeroData data = HeroSystem.GetUserHero(User.battleHeros[i]);
            selectPanel = PanelHeroSelection.transform.GetChild(i).GetChild(2).gameObject;
            selectedHeroNameText = PanelHeroSelection.transform.GetChild(i).GetChild(0).GetComponent<Text>();
            selectedHeroImage = PanelHeroSelection.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Image>();
            ShowHero(selectedHeroImage.transform, data);
            if (data != null)
            {
                selectPanel.gameObject.SetActive(false);
                selectedHeroNameText.text = string.Format("<size='{0}'>{1} {2}</size>  {3}", selectedHeroNameText.fontSize - 5, LocalizationManager.GetText("Level"), data.level, HeroSystem.GetHeroName(data.id));
                selectedHeroImage.enabled = true;
            }
            else
            {
                selectedHeroImage.enabled = false;
                selectedHeroNameText.text = "";
                selectPanel.gameObject.SetActive(true);
            }

        }
        selectHeroLockCover.SetActive(true);
    }
    public void RefreshPlayerSkillUI()
    {
        List<Skill> playerSkillList = SkillSystem.GetSelectSkillList();
        for (int i = 0; i < playerSkillList.Count; i++)
        {
            if (playerSkillList[i] != null)
            {
                playerSkillPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().enabled = true;
                playerSkillPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = SkillSystem.GetSkillImage(playerSkillList[i].id);
            }
        }
        for (int i = playerSkillList.Count; i < 2; i++)
        {
            playerSkillPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().enabled = false;
        }
    }

    public void OnItemSlotClick(HeroData heroData)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        CharactersManager.instance.SetPvpHeros(index, heroData.id);
        RefreshUI();
        pinPoint.SetActive(false);
        selectHeroLockCover.SetActive(true);
    }

    public void OnNoneClick()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        CharactersManager.instance.SetPvpHeros(index, 0);
        RefreshUI();
        pinPoint.SetActive(false);
        selectHeroLockCover.SetActive(true);
    }

    public void OnSelectedHeroSlotClick(int ix)
    {
        if (selectHeroLockCover.activeSelf)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
            index = ix;
            pinPoint.transform.SetParent(PanelHeroSelection.transform.GetChild(ix).transform);
            pinPoint.transform.localPosition = Vector3.zero;
            pinPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 1f);
            pinPoint.SetActive(true);
            selectHeroLockCover.SetActive(false);
        }
    }

    public void ShowHero(Transform parent, HeroData data)
    {
        if (parent.transform.childCount > 0)
        {
            foreach (Transform child in parent.transform)
                Destroy(child.gameObject);
        }
        if (data != null)
        {
            GameObject hero = PrefabsDatabaseManager.instance.GetHeroPrefab(data.id);
            if (hero != null)
            {
                GameObject showHeroObj = Instantiate(hero, parent);
                showHeroObj.transform.localScale = new Vector3(150, 150, 150);
                showHeroObj.transform.localPosition = Vector3.zero;

                if (showHeroObj.GetComponent<Hero>() != null)
                    Destroy(showHeroObj.GetComponent<Hero>());
                if (showHeroObj.GetComponent<Rigidbody2D>() != null)
                    Destroy(showHeroObj.GetComponent<Rigidbody2D>());
                foreach (var sp in showHeroObj.GetComponentsInChildren<SpriteRenderer>())
                {
                    sp.sortingLayerName = "ShowObject";
                    sp.gameObject.layer = 16;
                }
                showHeroObj.gameObject.SetActive(true);
            }
        }
    }
}
