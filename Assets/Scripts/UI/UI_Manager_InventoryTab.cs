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
    Image itemInfoImage;
    Text itemInfoName;
    Text itemInfoDescription;

    private void OnValidate()
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
            itemInfoImage.enabled = false;
            itemInfoName.enabled = false;
            itemInfoDescription.enabled = false;
        }
    }

    private void RefreshUI(Common.OrderByType orderByType=Common.OrderByType.NONE)
    {
        items = ItemSystem.GetUserItems(orderByType);
        if (items!=null)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (i < items.Count && i < itemSlots.Length)
                {
                    itemSlots[i].Item = items[i];
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
                else
                {
                    itemSlots[i].Item = null;
                    itemSlots[i].GetComponent<Button>().enabled = false;
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
            itemInfoDescription.text = itemSlots[index].Item.description;
            itemInfoImage.enabled = true;
            itemInfoName.enabled = true;
            itemInfoDescription.enabled = true;
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
