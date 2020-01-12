using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopPackage : MonoBehaviour
{
    #region 변수
    //아이템리스트창
    GameObject ScrollViewContent;
    public GameObject ItemSlotPrefab;
    Text slotNameText;
    Text slotValueText;
    Image slotItemImage;
    Image slotMoneyImage;
    Image discountImage;
    GameObject BuyButton;
    public GameObject detailButton;

    //정보창
    public GameObject ItemInfoView;
    Image infoImage;
    Text infoItemNameText;
    Text infoItemDescriptionText;
    Text infoItemValueText;

    //
    public List<int> discountItems = new List<int>();
    public float discountRate;
    #endregion
    private void Awake()
    {
        //아이템리스트창
        ScrollViewContent = this.GetComponentInChildren<GridLayoutGroup>().transform.gameObject;

        //정보창
        if (ItemInfoView != null)
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
        detailButton.gameObject.SetActive(false);
        RefreshUI();
    }
    public void RefreshUI()
    {
        if (ItemSlotPrefab != null && ItemSystem.GetItemCount() != ScrollViewContent.transform.childCount)
        {
            foreach (Transform child in ScrollViewContent.transform)
            {
                if (child.GetComponent<ItemSlot>() != null)
                    Destroy(child.gameObject);
            }
            foreach (var slot in ItemSystem.GetPackageItems())
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
                    slotNameText.text = ItemSystem.GetItemName(slot.id);
                if (slotValueText != null)
                {
                    if(slot.id==9041)
                    {
                        slotValueText.text = string.Format("<size='20'>6,500 > </size>{0}",Common.GetThousandCommaText(slot.value));
                    }
                    else
                    {
                        slotValueText.text = Common.GetThousandCommaText(slot.value);
                    }
                }

                slotItemImage = slotPrefab.transform.GetChild(0).GetChild(0).GetComponent<Image>();
                slotMoneyImage = slotPrefab.transform.GetChild(2).GetChild(0).GetComponent<Image>();
                discountImage = slotPrefab.transform.GetChild(3).GetComponent<Image>();
                if(discountImage!=null)
                {
                    if (slot.id == 9041)
                        discountImage.enabled = true;
                    else
                        discountImage.enabled = false;
                }
                if (slotItemImage != null)
                    slotItemImage.sprite = Resources.Load<Sprite>(slot.image);
                if (slotMoneyImage != null)
                {
                    slotMoneyImage.sprite = Resources.Load<Sprite>("UI/won");
                }
                SetPurchaseAblePackageItem(slot.id, slotPrefab,slot);
            }
        }
        infoImage.enabled = false;
        infoItemDescriptionText.text = "";
        infoItemNameText.text = "";
        infoItemValueText.text = "";
        ItemInfoView.SetActive(false);
        BuyButton.GetComponent<Button>().enabled = false;
    }

    public void OnItemSlotClick(Transform trans, Item itemData, bool isPurchaseAble)
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
        infoItemDescriptionText.text = ItemSystem.GetItemDescription(itemData.id);
        infoItemNameText.text = slotNameText.text;
        infoItemValueText.text = slotValueText.text;
        if(isPurchaseAble)
        {
            BuyButton.GetComponent<Button>().enabled = true;
            BuyButton.GetComponent<UI_Button>().callBackScript = this.gameObject;
            BuyButton.GetComponent<UI_Button>().buyItemId = itemData.id;
            BuyButton.GetComponent<UI_Button>().buttonType = UI_Button.ButtonType.ItemBuy;
            BuyButton.GetComponent<UI_Button>().paymentType = UI_Button.PaymentType.Cash;
            BuyButton.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/won");
            BuyButton.GetComponent<UI_Button>().paymentAmount = itemData.value;
            BuyButton.GetComponentInChildren<Text>().text = Common.GetThousandCommaText(itemData.value);
        }
        else
        {
            BuyButton.GetComponent<Button>().enabled = false;
            BuyButton.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/won");
            BuyButton.GetComponentInChildren<Text>().text = string.Format("<color='red'>{0}</color>",User.language=="ko" ? "구매불가" : "Can't Buy");
        }
    }

    void SetPurchaseAblePackageItem(int buyItemId, GameObject slotPrefab, Item data)
    {
        int unableType = 0;
        if (buyItemId == 9041)
        {
            if (User.isAdsRemove || User.isAdsSkip)
            {
                unableType = 1;
            }
            unableType = 0;
        }
        else if(buyItemId == 9042)
        {
            if (SaveSystem.IsPremiumPassAble())
                unableType = 2;
            else
                unableType = 0;
        }
        else
        {
            if(SaveSystem.IsAlreadyPurchasePackage(buyItemId))
                unableType = 1;
            else
                unableType = 0;
        }
        if (unableType==1)
        {
            slotPrefab.transform.GetChild(5).gameObject.SetActive(true);
            slotPrefab.transform.GetChild(5).GetComponentInChildren<Text>().text = string.Format("1/1 {0}", User.language == "ko" ? "구매완료" : "Purchased");
        }
        else if(unableType==2)
        {
            slotPrefab.transform.GetChild(5).gameObject.SetActive(true);
            slotPrefab.transform.GetChild(5).GetComponentInChildren<Text>().text = string.Format("{0}\r\n\r\n {1}", SaveSystem.getPremiumRemainingPeriodText(), User.language == "ko" ? "이용권 만료후 구매가능" : "Available after expiration");
        }
        else
        {
            slotPrefab.transform.GetChild(5).gameObject.SetActive(false);
        }
        bool isBuyAble = unableType == 0;
        slotPrefab.GetComponent<Button>().onClick.RemoveAllListeners();
        slotPrefab.GetComponent<Button>().onClick.AddListener(delegate
        {
            OnItemSlotClick(slotPrefab.transform, data, isBuyAble);
        });
    }
}
