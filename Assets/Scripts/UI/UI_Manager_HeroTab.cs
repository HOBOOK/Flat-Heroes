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
    Text slotLevelText;
    Image slotHeroImage;
    Image container1;
    Image container2;
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
                if (heroSlot.id < 500)
                {
                    GameObject slotPrefab = Instantiate(heroSlotPrefab, ScrollViewContent.transform);
                    foreach (var i in slotPrefab.GetComponentsInChildren<Text>())
                    {
                        if (i.name.Equals("heroName"))
                            slotNameText = i;
                        else if (i.name.Equals("heroLevel"))
                            slotLevelText = i;
                    }
                    if (slotNameText != null)
                        slotNameText.text = HeroSystem.GetHeroName(heroSlot.id);
                    if (slotLevelText != null)
                        slotLevelText.text = string.Format("Lv. {0}", heroSlot.level.ToString());
                    slotHeroImage = slotPrefab.transform.GetChild(0).GetChild(1).GetComponent<Image>();
                    if (slotHeroImage != null)
                        slotHeroImage.sprite = HeroSystem.GetHeroThumbnail(heroSlot.id);
                    slotPrefab.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = HeroSystem.GetHeroClassImage(heroSlot);

                    //테두리
                    slotPrefab.transform.GetChild(4).GetComponent<Image>().color = HeroClassColor.GetHeroColor(heroSlot.over);

                    if (heroSlot.ability>0)
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
