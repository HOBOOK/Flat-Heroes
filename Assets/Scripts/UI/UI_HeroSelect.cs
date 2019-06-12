using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroSelect : MonoBehaviour
{
    #region 변수
    //컨테이너
    GameObject PanelHeroContainer;
    //영웅리스트창
    GameObject ScrollViewContent;
    public GameObject selectHeroLockCover;
    public GameObject heroSlotPrefab;
    Text slotNameText;
    Image slotHeroImage;
    // 영웅선택창
    int index;
    GameObject PanelHeroSelection;
    Text selectedHeroNameText;
    Image selectedHeroImage;

    #endregion

    private void Awake()
    {
        PanelHeroContainer = this.transform.GetChild(0).gameObject;
        ScrollViewContent = PanelHeroContainer.GetComponentInChildren<GridLayoutGroup>().gameObject;
        PanelHeroSelection = PanelHeroContainer.transform.GetChild(1).gameObject;
    }
    private void OnEnable()
    {
        SetImageAndTextSelectHeroPanel();
    }
    void RefreshUI()
    {
        if (heroSlotPrefab != null)
        {
            foreach (Transform child in ScrollViewContent.transform)
            {
                if(!child.name.Equals("none"))
                    Destroy(child.gameObject);
            }
            foreach (var heroSlot in HeroSystem.GetUserHeros())
            {
                if (heroSlot.type == 0&& !CharactersManager.instance.IsExistedStageHero(heroSlot.id))
                {
                    GameObject slotPrefab = Instantiate(heroSlotPrefab, ScrollViewContent.transform);
                    foreach (var i in slotPrefab.GetComponentsInChildren<Text>())
                    {
                        if (i.name.Equals("heroName"))
                            slotNameText = i;
                    }
                    if (slotNameText != null)
                        slotNameText.text = heroSlot.name;
                    slotHeroImage = slotPrefab.transform.GetChild(0).GetComponent<Image>();
                    if (slotHeroImage != null)
                        slotHeroImage.sprite = Resources.Load<Sprite>(heroSlot.image);
                    slotPrefab.GetComponent<Button>().onClick.RemoveAllListeners();
                    slotPrefab.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        OnItemSlotClick(heroSlot);
                    });
                }
            }
        }
        selectHeroLockCover.SetActive(true);
    }

    void Start()
    {
        RefreshUI();
    }
    public void SetImageAndTextSelectHeroPanel()
    {
        for(int i = 0; i<User.stageHeros.Length; i++)
        {
            HeroData data = HeroSystem.GetHero(User.stageHeros[i]);
            if(data!=null)
            {
                selectedHeroNameText = PanelHeroSelection.transform.GetChild(i).GetComponentInChildren<Text>();
                selectedHeroImage = PanelHeroSelection.transform.GetChild(i).GetChild(1).GetComponent<Image>();
                selectedHeroNameText.fontSize = 30;
                selectedHeroNameText.text = string.Format("<size='{0}'>레벨 {1}</size>\r\n{2}", selectedHeroNameText.fontSize - 5, data.level, data.name);
                selectedHeroNameText.color = Color.white;
                selectedHeroImage.sprite = Resources.Load<Sprite>(data.image);
                PanelHeroSelection.transform.GetChild(index).GetComponent<Image>().color = Color.white;
            }
        }
        selectHeroLockCover.SetActive(true);
    }

    public void OnItemSlotClick(HeroData heroData)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        CharactersManager.instance.SetStageHeros(index, heroData.id);
        selectedHeroNameText = PanelHeroSelection.transform.GetChild(index).GetComponentInChildren<Text>();
        selectedHeroImage = PanelHeroSelection.transform.GetChild(index).GetChild(1).GetComponent<Image>();
        selectedHeroNameText.fontSize = 30;
        selectedHeroNameText.text = string.Format("<size='{0}'>레벨 {1}</size>\r\n{2}",selectedHeroNameText.fontSize-5, heroData.level, heroData.name);
        selectedHeroNameText.color = Color.white;
        selectedHeroImage.sprite = Resources.Load<Sprite>(heroData.image);
        PanelHeroSelection.transform.GetChild(index).GetComponent<Image>().color = Color.white;
        RefreshUI();
        selectHeroLockCover.SetActive(true);
    }

    public void OnNoneClick()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        CharactersManager.instance.SetStageHeros(index, 0);
        selectedHeroNameText = PanelHeroSelection.transform.GetChild(index).GetComponentInChildren<Text>();
        selectedHeroImage = PanelHeroSelection.transform.GetChild(index).GetChild(1).GetComponent<Image>();
        selectedHeroNameText.fontSize = 22;
        selectedHeroNameText.text = "영웅을 선택해주세요.";
        selectedHeroNameText.color = new Color(1,1,1,(100f/255f));
        selectedHeroImage.sprite = Resources.Load<Sprite>("Items/coin");
        PanelHeroSelection.transform.GetChild(index).GetComponent<Image>().color = Color.white;
        RefreshUI();
        selectHeroLockCover.SetActive(true);
    }

    public void OnSelectedHeroSlotClick(int ix)
    {
        if(selectHeroLockCover.activeSelf)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
            index = ix;
            PanelHeroSelection.transform.GetChild(index).GetComponent<Image>().color = Color.black;
            selectHeroLockCover.SetActive(false);

        }
        //UI_Manager.instance.PopHeroInfoUI.SetActive(true);
        //UI_Manager.instance.PopHeroInfoUI.SetActive(true);
        //UI_Manager.instance.PopHeroInfoUI.GetComponentInChildren<AiryUIAnimatedElement>().ShowElement();
        //UI_Manager.instance.PopHeroInfoUI.GetComponent<UI_HeroInfo>().ShowHero(PrefabsDatabaseManager.instance.GetHeroPrefab(heroData.id), heroData);
    }
}
