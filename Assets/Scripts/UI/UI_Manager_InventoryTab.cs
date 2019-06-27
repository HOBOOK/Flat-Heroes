using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager_InventoryTab : MonoBehaviour
{
    [SerializeField] List<Item> items;
    [SerializeField] Transform itemParent;
    [SerializeField] ItemSlot[] itemSlots;

    public GameObject ItemSlotPrefab;
    public GameObject PanelItemInfo;
    public GameObject ScrollContentView;
    Image itemInfoImage;
    Text itemInfoName;
    Text itemInfoDescription;
    Button itemSellButton;
    private void Awake()
    {
        ScrollContentView = transform.GetComponentInChildren<ContentSizeFitter>().gameObject;
    }
    public void OnValidate()
    {
        if(itemParent!=null)
        {
            itemSlots = itemParent.GetComponentsInChildren<ItemSlot>();
        }
        if(PanelItemInfo!=null)
        {
            if (itemInfoImage == null)
                itemInfoImage = PanelItemInfo.transform.GetChild(0).GetComponent<Image>();
            if(itemInfoName==null||itemInfoDescription==null)
            {
                itemInfoName = PanelItemInfo.transform.GetChild(1).GetComponent<Text>();
                itemInfoDescription = PanelItemInfo.transform.GetChild(2).GetComponent<Text>();
            }
            if (itemSellButton==null)
            {
                itemSellButton = PanelItemInfo.GetComponentInChildren<Button>();
            }
            itemInfoImage.enabled = false;
            itemInfoName.enabled = false;
            itemInfoDescription.enabled = false;
            itemSellButton.enabled = false;
            itemSellButton.GetComponent<Image>().enabled = false;
            itemSellButton.GetComponentInChildren<Text>().enabled = false;
        }
    }

    public void RefreshUI(Common.OrderByType orderByType=Common.OrderByType.NONE)
    {
        items = ItemSystem.GetUserItems(orderByType);
        if (items!=null)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i].Item = null;
                itemSlots[i].GetComponent<Button>().enabled = false;
                itemSlots[i].transform.GetChild(0).GetChild(1).GetComponent<Image>().color = ItemColor.BC;
                if (i < items.Count && i < itemSlots.Length)
                {
                    itemSlots[i].Item = items[i];
                    itemSlots[i].transform.GetChild(0).GetChild(1).GetComponent<Image>().color = ItemColor.GetItemColor(items[i].itemClass);
                    itemSlots[i].transform.GetComponentInChildren<Text>().color = ItemColor.GetItemColor(items[i].itemClass);
                    itemSlots[i].GetComponent<Button>().enabled = true;
                    int index = i;
                    if(index < items.Count && index < itemSlots.Length)
                    {
                        itemSlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                        itemSlots[i].GetComponent<Button>().onClick.AddListener(delegate
                        {
                            OnItemSlotClick(index);
                        });
                    }
                }
            }
        }
        Debugging.Log(items.Count + " 개의 아이템 로드됨");
    }
    private void OnEnable()
    {
        RefreshUI();
    }

    public void OnItemSlotClick(int index)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        if (itemSlots[index]!=null)
        {
            itemInfoImage.sprite = ItemSystem.GetItemImage(itemSlots[index].Item.id);
            itemInfoName.text = itemSlots[index].Item.name;
            itemInfoName.color = ItemColor.GetItemColor(itemSlots[index].Item.itemClass);
            itemInfoDescription.text = ItemSystem.GetEquipmentItemDescription(itemSlots[index].Item);
            itemInfoImage.enabled = true;
            itemInfoName.enabled = true;
            itemInfoDescription.enabled = true;
            itemSellButton.enabled = true;
            itemSellButton.GetComponent<Image>().enabled = true;
            itemSellButton.GetComponentInChildren<Text>().enabled = true;
            itemSellButton.GetComponent<UI_Button>().callBackScript = this.gameObject;
            itemSellButton.GetComponent<UI_Button>().sellItemId = itemSlots[index].Item.customId;
            if (itemInfoImage.GetComponent<AiryUIAnimatedElement>() != null)
                itemInfoImage.GetComponent<AiryUIAnimatedElement>().ShowElement();
            Debugging.Log(index + " 아이템 슬롯버튼 클릭");
        }
    }

    public void OnOrderByButtonClick(int type)
    {
        if (type > 2)
            type = 0;
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        items = null;
        RefreshUI((Common.OrderByType)type);
    }
}
