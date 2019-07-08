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
    public GameObject PanelEvolutionPanel;
    public GameObject ScrollContentView;
    Image itemInfoImage;
    Text itemInfoName;
    Text itemInfoDescription;
    Button itemSellButton;
    GameObject isEquipPanel;
    Item selectedItem;

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
                isEquipPanel = itemSlots[i].transform.GetChild(0).GetChild(2).gameObject;
                isEquipPanel.SetActive(false);
                itemSlots[i].transform.GetChild(3).gameObject.SetActive(false);
                itemSlots[i].transform.GetChild(4).gameObject.SetActive(false);
                if (i < items.Count && i < itemSlots.Length)
                {
                    itemSlots[i].Item = items[i];
                    itemSlots[i].transform.GetChild(0).GetChild(1).GetComponent<Image>().color = ItemColor.GetItemColor(items[i].itemClass);
                    itemSlots[i].transform.GetComponentInChildren<Text>().color = ItemColor.GetItemColor(items[i].itemClass);
                    itemSlots[i].GetComponent<Button>().enabled = true;
                    if (items[i].equipCharacterId > 0)
                        isEquipPanel.SetActive(true);
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
        PanelItemInfo.SetActive(true);
        PanelEvolutionPanel.SetActive(false);
    }

    public void OnItemSlotClick(int index)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        if (itemSlots[index]!=null&& PanelItemInfo.activeSelf)
        {
            if(!PanelItemInfo.activeSelf)
            {
                selectedItem = itemSlots[index].Item;
            }
            else
            {
                selectedItem = itemSlots[index].Item;
                itemInfoImage.sprite = ItemSystem.GetItemImage(itemSlots[index].Item.id);
                itemInfoName.text = ItemSystem.GetItemName(itemSlots[index].Item.id) + string.Format(" {0}", ItemSystem.GetIemClassName(itemSlots[index].Item.itemClass));
                itemInfoName.color = ItemColor.GetItemColor(itemSlots[index].Item.itemClass);
                itemInfoDescription.text = ItemSystem.GetEquipmentItemDescription(itemSlots[index].Item);
                itemInfoImage.enabled = true;
                itemInfoName.enabled = true;
                itemInfoDescription.enabled = true;
                itemSellButton.enabled = true;
                itemSellButton.GetComponent<Image>().enabled = true;
                itemSellButton.GetComponentInChildren<Text>().enabled = true;
                if (itemInfoImage.GetComponent<AiryUIAnimatedElement>() != null)
                    itemInfoImage.GetComponent<AiryUIAnimatedElement>().ShowElement();
                Debugging.Log(index + " 아이템 슬롯버튼 클릭");
            }

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

    public void OnClickEvolutionButton()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        PanelItemInfo.gameObject.SetActive(false);
        PanelEvolutionPanel.gameObject.SetActive(true);
        PanelEvolutionPanel.GetComponent<UI_EvolutionItem>().OpenUI(selectedItem);
        RefreshEvolutionUI();
    }
    public void OnItemSlotToEvolutionButton(Item item)
    {
        Debugging.Log("합성 재료 선택 > " + item.customId);
        selectedItem = item;
        PanelEvolutionPanel.GetComponent<UI_EvolutionItem>().AddMatItem(item);
        RefreshEvolutionUI();
    }
    public void SetSelectedItem(Item itemSlot)
    {
        selectedItem = itemSlot;
        itemInfoImage.sprite = ItemSystem.GetItemImage(itemSlot.id);
        itemInfoName.text = ItemSystem.GetItemName(itemSlot.id) + string.Format(" {0}", ItemSystem.GetIemClassName(itemSlot.itemClass));
        itemInfoName.color = ItemColor.GetItemColor(itemSlot.itemClass);
        itemInfoDescription.text = ItemSystem.GetEquipmentItemDescription(itemSlot);
        itemInfoImage.enabled = true;
        itemInfoName.enabled = true;
        itemInfoDescription.enabled = true;
        itemSellButton.enabled = true;
        itemSellButton.GetComponent<Image>().enabled = true;
        itemSellButton.GetComponentInChildren<Text>().enabled = true;
        if (itemInfoImage.GetComponent<AiryUIAnimatedElement>() != null)
            itemInfoImage.GetComponent<AiryUIAnimatedElement>().ShowElement();
        Debugging.Log(itemSlot.customId + " 아이템 슬롯버튼 클릭");
    }

    public void RefreshEvolutionUI()
    {
        items = ItemSystem.GetUserItems();
        if (items != null)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i].Item = null;
                itemSlots[i].GetComponent<Button>().enabled = false;
                itemSlots[i].transform.GetChild(0).GetChild(1).GetComponent<Image>().color = ItemColor.BC;
                isEquipPanel = itemSlots[i].transform.GetChild(0).GetChild(2).gameObject;
                isEquipPanel.SetActive(false);
                itemSlots[i].transform.GetChild(3).gameObject.SetActive(false);
                itemSlots[i].transform.GetChild(4).gameObject.SetActive(false);
                if (i < items.Count && i < itemSlots.Length)
                {
                    itemSlots[i].Item = items[i];
                    itemSlots[i].transform.GetChild(0).GetChild(1).GetComponent<Image>().color = ItemColor.GetItemColor(items[i].itemClass);
                    itemSlots[i].transform.GetComponentInChildren<Text>().color = ItemColor.GetItemColor(items[i].itemClass);
                    itemSlots[i].GetComponent<Button>().enabled = true;
                    if (items[i].equipCharacterId > 0)
                        isEquipPanel.SetActive(true);
                    int index = i;
                    if (index < items.Count && index < itemSlots.Length)
                    {
                        itemSlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    }
                    // Lock

                    if(PanelEvolutionPanel.GetComponent<UI_EvolutionItem>().isEndEvolution)
                    {
                        if (itemSlots[i].Item.equipCharacterId != 0)
                        {
                            itemSlots[i].transform.GetChild(3).gameObject.SetActive(true);
                            itemSlots[i].GetComponent<Button>().enabled = false;
                        }
                        else
                        {
                            itemSlots[i].transform.GetChild(3).gameObject.SetActive(false);
                            itemSlots[i].GetComponent<Button>().enabled = true;
                            itemSlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                            itemSlots[i].GetComponent<Button>().onClick.AddListener(delegate
                            {
                                OnItemSlotToEvolutionButton(itemSlots[index].Item);
                            });
                        }
                    }
                    else
                    {
                        if (itemSlots[i].Item.id != selectedItem.id|| itemSlots[i].Item.equipCharacterId != 0)
                        {
                            itemSlots[i].transform.GetChild(3).gameObject.SetActive(true);
                            itemSlots[i].GetComponent<Button>().enabled = false;
                        }
                        else
                        {
                            itemSlots[i].transform.GetChild(3).gameObject.SetActive(false);
                            itemSlots[i].GetComponent<Button>().enabled = true;
                            itemSlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                            itemSlots[i].GetComponent<Button>().onClick.AddListener(delegate
                            {
                                OnItemSlotToEvolutionButton(itemSlots[index].Item);
                            });

                            // Select
                            if (PanelEvolutionPanel.GetComponent<UI_EvolutionItem>().IsSelectedItem(itemSlots[index].Item))
                            {
                                itemSlots[i].transform.GetChild(4).gameObject.SetActive(true);
                                itemSlots[i].GetComponent<Button>().enabled = false;
                            }
                            else
                            {
                                itemSlots[i].transform.GetChild(4).gameObject.SetActive(false);
                                itemSlots[i].GetComponent<Button>().enabled = true;
                                itemSlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                                itemSlots[i].GetComponent<Button>().onClick.AddListener(delegate
                                {
                                    OnItemSlotToEvolutionButton(itemSlots[index].Item);
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}
