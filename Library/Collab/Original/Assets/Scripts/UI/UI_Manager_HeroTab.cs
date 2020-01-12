using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager_HeroTab : MonoBehaviour
{
    #region 변수
    //영웅리스트창
    GameObject ScrollViewContent;
    public GameObject heroSlotPrefab;
    Text slotNameText;
    Image slotHeroImage;
    //정보창
    #endregion

    private void Awake()
    {
        ScrollViewContent = this.GetComponentInChildren<GridLayoutGroup>().gameObject;
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
                Destroy(child.gameObject);
            }
            foreach (var heroSlot in HeroSystem.GetUserHeros())
            {
                if (heroSlot.type == 0)
                {
                    GameObject slotPrefab = Instantiate(heroSlotPrefab, ScrollViewContent.transform);
                    foreach (var i in slotPrefab.GetComponentsInChildren<Text>())
                    {
                        if (i.name.Equals("heroName"))
                            slotNameText = i;
                    }
                    if (slotNameText != null)
                        slotNameText.text = HeroSystem.GetHeroName(heroSlot.id);
                    slotHeroImage = slotPrefab.transform.GetChild(0).GetChild(0).GetComponent<Image>();
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
    }

    public void OnItemSlotClick(HeroData heroData)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        UI_Manager.instance.PopHeroInfoUI.SetActive(true);
        UI_Manager.instance.PopHeroInfoUI.SetActive(true);
        UI_Manager.instance.PopHeroInfoUI.GetComponent<UI_HeroInfo>().ShowHero(PrefabsDatabaseManager.instance.GetHeroPrefab(heroData.id),heroData);
    }
}
