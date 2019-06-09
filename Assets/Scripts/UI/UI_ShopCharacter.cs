using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopCharacter : MonoBehaviour
{
    #region 변수
    //아이템리스트창
    GameObject ScrollViewContent;
    public GameObject ItemSlotPrefab;
    Text slotNameText;
    Text slotValueText;
    Image slotItemImage;
    Image slotMoneyImage;
    GameObject BuyButton;

    //정보창
    public GameObject ItemInfoView;
    Image infoImage;
    Text infoItemNameText;
    Text infoItemDescriptionText;
    Text infoItemValueText;
    #endregion
    private void Awake()
    {
        //아이템리스트창
        ScrollViewContent = this.GetComponentInChildren<GridLayoutGroup>().transform.gameObject;

        //정보창
        if(ItemInfoView!=null)
        {
            infoImage = ItemInfoView.transform.GetChild(0).GetComponent<Image>();
            foreach (var txt in ItemInfoView.GetComponentsInChildren<Text>())
            {
                if (txt.name.Equals("ItemName"))
                    infoItemNameText = txt;
                else if (txt.name.Equals("ItemDescription"))
                    infoItemDescriptionText = txt;
                else if (txt.name.Equals("ItemValue"))
                    infoItemValueText = txt;
            }
            BuyButton = ItemInfoView.GetComponentInChildren<Button>().gameObject;
        }
    }
    private void OnEnable()
    {
        infoImage.enabled = false;
        infoItemDescriptionText.text = "";
        infoItemNameText.text = "";
        infoItemValueText.text = "";
        ItemInfoView.SetActive(false);
        BuyButton.GetComponent<Button>().enabled = false;
    }
    void Start()
    {
        RefreshUI();
    }
    
    public void RefreshUI()
    {
        if (ItemSlotPrefab != null && HeroSystem.GetUnableHeros().Count != ScrollViewContent.transform.childCount)
        {
            foreach (Transform child in ScrollViewContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var slot in HeroSystem.GetUnableHeros())
            {
                GameObject slotPrefab = Instantiate(ItemSlotPrefab, ScrollViewContent.transform);
                foreach (var i in slotPrefab.GetComponentsInChildren<Text>())
                {
                    if (i.name.Equals("ItemName"))
                        slotNameText = i;
                    else if (i.name.Equals("ItemValue"))
                        slotValueText = i;
                }
                if (slotNameText != null)
                    slotNameText.text = slot.name;
                if (slotValueText != null)
                    slotValueText.text = Common.GetThousandCommaText(slot.id);
                slotItemImage = slotPrefab.transform.GetChild(0).GetChild(0).GetComponent<Image>();
                slotMoneyImage = slotPrefab.transform.GetChild(2).GetChild(0).GetComponent<Image>();
                if (slotItemImage != null)
                    slotItemImage.sprite = Resources.Load<Sprite>(slot.image);
                if (slotMoneyImage != null)
                {
                    slotMoneyImage.sprite = Resources.Load<Sprite>("Items/blackCrystal");
                }
                slotPrefab.GetComponent<Button>().onClick.RemoveAllListeners();
                slotPrefab.GetComponent<Button>().onClick.AddListener(delegate
                {
                    OnItemSlotClick(slotPrefab.transform, slot);
                });
            }
        }
        infoImage.enabled = false;
        infoItemDescriptionText.text = "";
        infoItemNameText.text = "";
        infoItemValueText.text = "";
        ItemInfoView.SetActive(false);
        BuyButton.GetComponent<Button>().enabled = false;
    }

    public void OnItemSlotClick(Transform trans, HeroData heroData)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        ItemInfoView.SetActive(true);
        foreach (var i in trans.GetComponentsInChildren<Text>())
        {
            if (i.name.Equals("ItemName"))
                slotNameText = i;
            else if (i.name.Equals("ItemValue"))
                slotValueText = i;
        }
        slotItemImage = trans.GetChild(0).GetChild(0).GetComponent<Image>();
        infoImage.sprite = slotItemImage.sprite;
        infoImage.enabled = true;
        infoImage.GetComponent<AiryUIAnimatedElement>().ShowElement();
        infoItemDescriptionText.text = heroData.description;
        infoItemNameText.text = slotNameText.text;
        infoItemValueText.text = slotValueText.text;
        BuyButton.GetComponent<Button>().enabled = true;
        BuyButton.GetComponent<UI_Button>().callBackScript = this.gameObject;
        BuyButton.GetComponent<UI_Button>().buttonType = UI_Button.ButtonType.CharacterBuy;
        BuyButton.GetComponent<UI_Button>().characterId = heroData.id;
        BuyButton.GetComponent<UI_Button>().paymentType = UI_Button.PaymentType.BlackCrystal;
        BuyButton.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Items/blackCrystal");
        BuyButton.GetComponent<UI_Button>().paymentAmount = heroData.id;
        BuyButton.GetComponentInChildren<Text>().text = Common.GetThousandCommaText(heroData.id);
    }
}
