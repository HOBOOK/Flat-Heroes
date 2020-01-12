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
    public GameObject PanelReinforce;
    public GameObject PanelGetReinforce;
    public GameObject PanelItemSellPanel;
    GameObject itemInfoParent;
    
    Image itemInfoImage;
    Image itemInfoCoverImage;
    Image itemInfoContainerImage;
    Text itemInfoName;
    Text itemInfoDescription;
    GameObject itemInfoReinforcementPanel;
    public Text inventoryCountText;
    public Button inventoryAddButton;
    public Button onEquipListButton;
    public Dropdown orderByDropdown;
    Button itemSellButton;
    Button itemReinforceButton;
    GameObject isEquipPanel;
    Item selectedItem;

    List<Item> evolutionAbleItems;

    bool isEquipItemList = false;
    int orderByType = 0;

    #region 강화 변수
    Transform panelReinforceInfo;
    Transform panelStatsInfo;
    Transform panelPercentInfo;
    Transform panelCostInfo;
    Button btnReinforce;
    Button btnReinforceReinit;
    bool isCheckAlertOn = false;
    List<string> enhancementTypeText = new List<string>();
    #endregion

    private void Awake()
    {
        ScrollContentView = transform.GetComponentInChildren<ContentSizeFitter>().gameObject;
        enhancementTypeText.Add("");
        enhancementTypeText.Add(LocalizationManager.GetText("heroInfoAttack"));
        enhancementTypeText.Add(LocalizationManager.GetText("heroInfoDefence"));
        enhancementTypeText.Add(LocalizationManager.GetText("heroInfoHp"));
        enhancementTypeText.Add(LocalizationManager.GetText("heroInfoCritical"));
        enhancementTypeText.Add(LocalizationManager.GetText("heroInfoAttackSpeed"));
        enhancementTypeText.Add(LocalizationManager.GetText("heroInfoMoveSpeed"));
        enhancementTypeText.Add(LocalizationManager.GetText("heroInfoSkillEnergy"));
        enhancementTypeText.Add(LocalizationManager.GetText("heroInfoPenetration"));
        if (PanelItemInfo != null)
        {
            if (itemParent != null)
            {
                for (int i = 0; i < User.inventoryCount; i++)
                {
                    Instantiate(ItemSlotPrefab, itemParent.transform);
                }
                itemSlots = itemParent.GetComponentsInChildren<ItemSlot>();
            }

            itemInfoParent = PanelItemInfo.transform.GetChild(0).gameObject;
            if (itemInfoImage == null)
                itemInfoImage = itemInfoParent.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            if(itemInfoCoverImage==null)
                itemInfoCoverImage = itemInfoParent.transform.GetChild(0).GetComponent<Image>();
            if (itemInfoContainerImage == null)
                itemInfoContainerImage = itemInfoParent.transform.GetChild(1).GetComponent<Image>();
            if (itemInfoName == null || itemInfoDescription == null)
            {
                itemInfoName = PanelItemInfo.transform.GetChild(1).GetComponent<Text>();
                itemInfoDescription = PanelItemInfo.transform.GetChild(2).GetComponent<Text>();
            }
            if (itemInfoReinforcementPanel == null)
            {
                itemInfoReinforcementPanel = itemInfoParent.transform.GetChild(2).gameObject;
            }

            if (itemSellButton == null)
            {
                itemSellButton = PanelItemInfo.transform.GetChild(3).GetComponent<Button>();
            }
            if(itemReinforceButton==null)
            {
                itemReinforceButton = PanelItemInfo.transform.GetChild(4).GetComponent<Button>();
            }
        }

        if(PanelReinforce!=null)
        {
            panelReinforceInfo = PanelReinforce.transform.GetChild(0).GetChild(0).transform;
            panelStatsInfo = PanelReinforce.transform.GetChild(0).GetChild(1).transform;
            panelPercentInfo = PanelReinforce.transform.GetChild(0).GetChild(2).transform;
            panelCostInfo = PanelReinforce.transform.GetChild(0).GetChild(3).transform;
            btnReinforce = PanelReinforce.transform.GetChild(1).GetChild(0).GetComponent<Button>();
            btnReinforceReinit = PanelReinforce.transform.GetChild(1).GetChild(1).GetComponent<Button>();
        }

        orderByDropdown.options.Clear();
        for(int i = 0; i < 2; i++)
        {
            orderByDropdown.options.Add(new Dropdown.OptionData((LocalizationManager.GetText("manageTab2OrderButton" + i.ToString()))));
        }
        orderByDropdown.RefreshShownValue();
    }
    public void RefreshUI()
    {
        if(isEquipItemList)
        {
            onEquipListButton.transform.GetChild(1).gameObject.SetActive(false);
            items = ItemSystem.GetUserOnEquipmentItems((Common.OrderByType)orderByType);
        }
        else
        {
            onEquipListButton.transform.GetChild(1).gameObject.SetActive(true);
            items = ItemSystem.GetUserUnEquipmentItems((Common.OrderByType)orderByType);
        }
        inventoryCountText.text = string.Format("{0} / {1}", items.Count, User.inventoryCount);

        if (items!=null)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i].Item = null;
                itemSlots[i].GetComponent<Button>().enabled = false;
                itemSlots[i].GetComponent<Button>().interactable = true;

                itemSlots[i].transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>().color = ItemColor.C;
                isEquipPanel = itemSlots[i].transform.GetChild(0).GetChild(0).GetChild(2).gameObject;
                isEquipPanel.SetActive(false);
                itemSlots[i].transform.GetChild(4).gameObject.SetActive(false);
                itemSlots[i].transform.GetChild(5).gameObject.SetActive(false);
                if (i < items.Count && i < itemSlots.Length)
                {
                    itemSlots[i].Item = items[i];
                    itemSlots[i].transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>().color = ItemColor.GetItemColor(items[i].itemClass);
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
            if(selectedItem!=null)
            {
                SetSelectedItemInfoPanel(selectedItem);
            }
            else
            {
                OffInfoPanel();
            }
        }
        Debugging.Log(items.Count + " 개의 아이템 로드됨");
    }

    void SetItemSlotEffect(Transform parent, Item data)
    {
        var main = parent.GetComponentInChildren<ParticleSystem>().main;
        Color color = ItemColor.GetItemColor(data.itemClass);
        main.startColor = new Color(color.r, color.g, color.b, 0.2f);
    }
    public void DisableInformationPanel()
    {
        itemInfoParent.SetActive(false);
    }
    private void OnEnable()
    {
        RefreshUI();
        PanelItemInfo.SetActive(true);
        PanelEvolutionPanel.SetActive(false);
        PanelReinforce.SetActive(false);
    }
    public void OffInfoPanel()
    {
        itemInfoParent.gameObject.SetActive(false);
        itemInfoName.enabled = false;
        itemInfoDescription.enabled = false;
        itemSellButton.enabled = false;
        itemSellButton.gameObject.SetActive(false);
        itemReinforceButton.gameObject.SetActive(false);
    }
    public void SetSelectedItem(Item item)
    {
        selectedItem = item;
    }

    void SetSelectedItemInfoPanel(Item item)
    {
        itemInfoImage.sprite = ItemSystem.GetItemImage(item.id);
        itemInfoContainerImage.color = ItemColor.GetItemColor(item.itemClass);
        itemInfoCoverImage.sprite = ItemSystem.GetItemClassImage(item.id);
        itemInfoName.text = string.Format("<color='white'>{0}</color> {1} {2}", ItemSystem.GetItemEnhancementText(item.customId), ItemSystem.GetItemName(item.id), ItemSystem.GetIemClassName(item.itemClass));
        itemInfoName.color = ItemColor.GetItemColor(item.itemClass);
        itemInfoDescription.text = ItemSystem.GetEquipmentItemDescription(item);
        itemInfoParent.gameObject.SetActive(true);
        itemInfoName.enabled = true;
        itemInfoDescription.enabled = true;
        if(item.enhancement>0)
        {
            itemInfoReinforcementPanel.SetActive(true);
            itemInfoReinforcementPanel.GetComponentInChildren<Text>().text = string.Format("<size='15'>+</size>{0}", item.enhancement.ToString());
        }
        else
        {
            itemInfoReinforcementPanel.SetActive(false);
        }
        if (selectedItem.itemClass < 8 && selectedItem.equipCharacterId == 0)
        {
            itemSellButton.enabled = true;
            itemSellButton.gameObject.SetActive(true);
        }
        else
        {
            itemSellButton.enabled = false;
            itemSellButton.gameObject.SetActive(false);
        }
        itemReinforceButton.gameObject.SetActive(true);
    }

    public void OnItemSlotClick(int index)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        if (itemSlots[index]!=null)
        {
            if(!PanelItemInfo.activeSelf)
            {
                SetSelectedItem(itemSlots[index].Item);
            }
            else
            {
                SetSelectedItem(itemSlots[index].Item);
                if (itemInfoParent.GetComponent<AiryUIAnimatedElement>() != null)
                    itemInfoParent.GetComponent<AiryUIAnimatedElement>().ShowElement();
                Debugging.Log(index + " 아이템 슬롯버튼 클릭");
            }
        }
        RefreshUI();
    }

    public void OnItemSellButtonClick()
    {
        if(PanelItemSellPanel!=null&&PanelItemInfo.activeSelf)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
            PanelItemSellPanel.SetActive(true);
            PanelItemSellPanel.GetComponentInChildren<AiryUIAnimatedElement>().ShowElement();
        }
    }


    public void OnOrderByButtonClick(Dropdown dropdown)
    {
        orderByType = dropdown.value;
        Debugging.Log(orderByType + " 정렬됨");
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        items = null;
        if (PanelItemInfo.activeSelf)
            RefreshUI();
        else if (PanelEvolutionPanel.activeSelf)
            RefreshEvolutionUI();
        else if (PanelReinforce.activeSelf)
            RefreshReinforcePanel();
    }

    public void OnOrderByOnEquipmentButtonClick()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        items = null;
        isEquipItemList = !isEquipItemList;
        if (PanelItemInfo.activeSelf)
            RefreshUI();
    }

    public void OnClickEvolutionButton()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        PanelItemInfo.gameObject.SetActive(false);
        PanelReinforce.gameObject.SetActive(false);
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
    public void AutoItemSlotToEvolution()
    {
        if(evolutionAbleItems!=null&& evolutionAbleItems.Count>0)
        {
            if (selectedItem == null)
            {
                Item autoSelectedItem = AutoSelectedItem(evolutionAbleItems);
                if(autoSelectedItem!=null)
                {
                    selectedItem = autoSelectedItem;
                    PanelEvolutionPanel.GetComponent<UI_EvolutionItem>().AddMatItem(autoSelectedItem);
                    RefreshEvolutionUI();
                    if (evolutionAbleItems != null && evolutionAbleItems.Count > 0)
                    {
                        for (int i = 0; i < 2 && i < evolutionAbleItems.Count; i++)
                        {
                            PanelEvolutionPanel.GetComponent<UI_EvolutionItem>().AddMatItem(evolutionAbleItems[i]);
                        }
                    }
                }
                else
                {
                    Debugging.Log("<color='cyan'>조합가능한 클래스의 장비가 없습니다.</color>");
                }
            }
            else
            {
                if (evolutionAbleItems != null && evolutionAbleItems.Count > 0)
                {
                    for (int i = 0; i < 3 && i < evolutionAbleItems.Count; i++)
                    {
                        PanelEvolutionPanel.GetComponent<UI_EvolutionItem>().AddMatItem(evolutionAbleItems[i]);
                    }
                }
                RefreshEvolutionUI();
            }
        }
    }
    Item AutoSelectedItem(List<Item> lists)
    {
        int[] itemClass = { 0, 0, 0, 0, 0, 0, 0 };
        foreach(var i in lists)
        {
            itemClass[i.itemClass-1]++;
            if (itemClass[i.itemClass - 1] >= 3)
                return i;
        }
        return null;
    }
    public void RefreshEvolutionUI()
    {
        items = ItemSystem.GetUserUnEquipmentItems((Common.OrderByType)orderByType);
        if (items != null)
        {
            evolutionAbleItems = new List<Item>();
            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i].Item = null;
                itemSlots[i].GetComponent<Button>().enabled = false;
                itemSlots[i].transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>().color = ItemColor.C;
                isEquipPanel = itemSlots[i].transform.GetChild(0).GetChild(0).GetChild(2).gameObject;
                isEquipPanel.SetActive(false);
                itemSlots[i].transform.GetChild(4).gameObject.SetActive(false);
                itemSlots[i].transform.GetChild(5).gameObject.SetActive(false);
                if (i < items.Count && i < itemSlots.Length)
                {
                    itemSlots[i].Item = items[i];
                    itemSlots[i].transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>().color = ItemColor.GetItemColor(items[i].itemClass);
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
                        if (itemSlots[i].Item.equipCharacterId != 0|| itemSlots[i].Item.itemClass > 7||itemSlots[i].Item.enhancement>0)
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
                            evolutionAbleItems.Add(itemSlots[index].Item);
                        }
                    }
                    else
                    {
                        if (itemSlots[i].Item.itemClass != selectedItem.itemClass || itemSlots[i].Item.equipCharacterId != 0||itemSlots[i].Item.itemClass>7 || itemSlots[i].Item.enhancement > 0)
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

                            // Select
                            if (PanelEvolutionPanel.GetComponent<UI_EvolutionItem>().IsSelectedItem(itemSlots[index].Item))
                            {
                                itemSlots[i].transform.GetChild(5).gameObject.SetActive(true);
                                itemSlots[i].GetComponent<Button>().enabled = false;
                            }
                            else
                            {
                                evolutionAbleItems.Add(itemSlots[index].Item);
                                itemSlots[i].transform.GetChild(5).gameObject.SetActive(false);
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

    #region 강화
    // 강화 패널 온
    public void OnClickReinforceButton()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        RefreshReinforcePanel();
    }
    // 강화 패널 오프
    public void OnClickReinforceOffButton()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        PanelReinforce.SetActive(false);
        PanelEvolutionPanel.SetActive(false);
        PanelItemInfo.SetActive(true);
        RefreshUI();
    }
    // 강화 아이템 타겟 변경
    void OnClickReinforceItemChange(Item item)
    {
        SetSelectedItem(item);
        RefreshReinforcePanel();
    }
    // 강화 시작 이벤트
    public void OnClickReinforceEquipment()
    {
        if(!isCheckAlertOn)
        {
            StartCoroutine("CheckingReinforceItem");
        }
    }
    IEnumerator CheckingReinforceItem()
    {
        isCheckAlertOn = true;

        var alertPanel = UI_Manager.instance.ShowNeedAlert(ItemSystem.GetItemImage(selectedItem.id), string.Format("<size='40'><color='white'>\"{0}\"</color></size>\r\n\r\n{1}",ItemSystem.GetItemName(selectedItem.id),LocalizationManager.GetText("alertReinforceReinforceMessage")));
        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult&&selectedItem!=null)
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 예를 클릭시
            bool IsCostCheck = false;
            int needCoin = ItemSystem.GetItemReinforceNeedCost(selectedItem, false);
            int needScroll = ItemSystem.GetItemReinforceNeedCost(selectedItem, true);
            if (Common.PaymentAbleCheck(ref User.coin, needCoin) &&ItemSystem.GetUserScrollCount()>= needScroll)
            {
                IsCostCheck = true;
            }
            if(IsCostCheck&&Common.PaymentCheck(ref User.coin, needCoin) && ItemSystem.UseItem(ItemSystem.GetUserScroll().customId, needScroll))
            {
                UI_Manager.instance.PopupInterActiveCover.SetActive(true);
                SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.magic);
                ReinforcementEffect(panelReinforceInfo.GetChild(0));
                bool isSuccess = false;
                if (UnityEngine.Random.Range(0, 100) < ItemSystem.GetItemReinforceSuccessRate(selectedItem))
                {
                    isSuccess = true;
                }
                ItemSystem.ReinforceItem(selectedItem.customId, isSuccess);
                MissionSystem.AddClearPoint(MissionSystem.ClearType.TotalUseScroll, needScroll);
                GoogleSignManager.SaveData();
                yield return new WaitForSeconds(1f);
                UI_Manager.instance.PopupInterActiveCover.SetActive(false);
                if (isSuccess)
                    SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.dropItem);
                else
                    SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.shootPistolSilence);
                RefreshReinforceResultPanel(selectedItem, isSuccess);
                PanelGetReinforce.SetActive(true);
                foreach (AiryUIAnimatedElement child in PanelGetReinforce.GetComponentsInChildren<AiryUIAnimatedElement>())
                {
                    child.ShowElement();
                }
                PanelGetReinforce.transform.GetChild(0).GetComponent<Button>().interactable = false;
                yield return new WaitForSeconds(1.0f);
                PanelGetReinforce.transform.GetChild(0).GetComponent<Button>().interactable = true;
                RefreshReinforcePanel();
            }
            else
            {
                UI_Manager.instance.ShowAlert("", LocalizationManager.GetText("alertReinforceFailMessage"));
            }
        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 아니오를 클릭시
        }
        isCheckAlertOn = false;
        yield return null;
    }
    // 강화 초기화 이벤트
    public void OnClickReinforceReinit()
    {
        if(!isCheckAlertOn)
        {
            StartCoroutine("CheckingReinforceReinit");
        }
    }
    IEnumerator CheckingReinforceReinit()
    {
        isCheckAlertOn = true;

        var alertPanel = UI_Manager.instance.ShowNeedAlert(ItemSystem.GetItemImage(selectedItem.id), string.Format("<size='40'><color='white'>\"{0}\"</color></size>\r\n\r\n{1}", ItemSystem.GetItemName(selectedItem.id), LocalizationManager.GetText("alertReinforceReinitMessage")));
        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult && selectedItem != null)
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 예를 클릭시
            int needCrystal = 200;
            if (Common.PaymentCheck(ref User.blackCrystal, needCrystal))
            {
                UI_Manager.instance.PopupInterActiveCover.SetActive(true);
                SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.magic);
                ReinforcementEffect(panelReinforceInfo.GetChild(0));
                ItemSystem.ReinforceReinitItem(selectedItem.customId);
                GoogleSignManager.SaveData();
                yield return new WaitForSeconds(0.3f);
                UI_Manager.instance.PopupInterActiveCover.SetActive(false);
                SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.dropItem);
                RefreshReinforcePanel();
            }
            else
            {
                UI_Manager.instance.ShowAlert("", LocalizationManager.GetText("alertReinforceFailMessage2"));
            }
        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 아니오를 클릭시
        }
        isCheckAlertOn = false;
        yield return null;
    }
    // UI 드로잉
    void RefreshReinforcePanel()
    {
        if (selectedItem != null)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
            PanelItemInfo.gameObject.SetActive(false);
            PanelEvolutionPanel.gameObject.SetActive(false);
            PanelReinforce.gameObject.SetActive(true);
            // 강화 타겟 아이템 표시
            panelReinforceInfo.GetChild(0).GetComponent<AiryUIAnimatedElement>().ShowElement();
            panelReinforceInfo.GetChild(0).GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemClassImage(selectedItem.id);
            panelReinforceInfo.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemImage(selectedItem.id);
            panelReinforceInfo.GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>().color = ItemColor.GetItemColor(selectedItem.itemClass);
            panelReinforceInfo.GetChild(3).GetComponent<Text>().text = string.Format("+{0} {1}", selectedItem.enhancement, ItemSystem.GetItemName(selectedItem.id));
            panelReinforceInfo.GetChild(3).GetComponent<Text>().color = ItemColor.GetItemColor(selectedItem.itemClass);
            // 스크롤 카운트
            panelReinforceInfo.GetChild(1).GetChild(1).GetComponentInChildren<Text>().text = ItemSystem.GetUserScrollCount().ToString();
            // 강화 세부정보 표시
            int ableCount = 7 - selectedItem.enhancementCount;
            panelStatsInfo.GetComponentInChildren<Text>().text = string.Format("<color='white'>{0}</color>  {1}   ->   <color='cyan'>{2}</color>" +
                "\r\n<color='white'>{3}</color>  {4}   ->   <color='cyan'>{5}</color>", enhancementTypeText[ItemSystem.GetEnhancementType(selectedItem)],ItemSystem.GetReinforceCurrentValue(selectedItem), ItemSystem.GetReinforceAfterValue(selectedItem), LocalizationManager.GetText("ReinforcementRemainCount"), ableCount, ableCount-1);
            // 강화 확률 표시
            panelPercentInfo.GetComponentInChildren<Text>().text = string.Format("{0}     <color='yellow'><size='50'>{1}%</size></color>", LocalizationManager.GetText("ReinforcementSuccessRate"), ItemSystem.GetItemReinforceSuccessRate(selectedItem).ToString());
            // 강화 비용 표시
            panelCostInfo.GetChild(0).GetComponentInChildren<Text>().text = ItemSystem.GetItemReinforceNeedCost(selectedItem, false).ToString("N0");
            panelCostInfo.GetChild(1).GetComponentInChildren<Text>().text = ItemSystem.GetItemReinforceNeedCost(selectedItem, true).ToString("N0");

            if (selectedItem.enhancementCount > 0)
                btnReinforceReinit.gameObject.SetActive(true);
            else
                btnReinforceReinit.gameObject.SetActive(false);
            if (selectedItem.enhancementCount>=7)
            {
                btnReinforce.interactable = false;
                panelStatsInfo.gameObject.SetActive(false);
                panelCostInfo.gameObject.SetActive(false);
                panelPercentInfo.GetComponentInChildren<Text>().text = string.Format("{0}", LocalizationManager.GetText("ReinforcementCompletedMessage"));
            }
            else
            {
                btnReinforce.interactable = true;
                panelStatsInfo.gameObject.SetActive(true);
                panelCostInfo.gameObject.SetActive(true);
            }

        }

        items = ItemSystem.GetUserUnEquipmentItems((Common.OrderByType)orderByType);
        if (items != null)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i].Item = null;
                itemSlots[i].GetComponent<Button>().enabled = false;
                itemSlots[i].transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>().color = ItemColor.C;
                isEquipPanel = itemSlots[i].transform.GetChild(0).GetChild(0).GetChild(2).gameObject;
                isEquipPanel.SetActive(false);
                itemSlots[i].transform.GetChild(4).gameObject.SetActive(false);
                itemSlots[i].transform.GetChild(5).gameObject.SetActive(false);
                if (i < items.Count && i < itemSlots.Length)
                {
                    itemSlots[i].Item = items[i];
                    itemSlots[i].transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>().color = ItemColor.GetItemColor(items[i].itemClass);
                    itemSlots[i].transform.GetComponentInChildren<Text>().color = ItemColor.GetItemColor(items[i].itemClass);
                    itemSlots[i].GetComponent<Button>().enabled = true;
                    if (items[i].equipCharacterId > 0)
                        isEquipPanel.SetActive(true);
                    int index = i;
                    if (index < items.Count && index < itemSlots.Length)
                    {
                        itemSlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    }
                    // 강화아이템 타겟변경 인벤토리 이벤트 추가
                    if (itemSlots[i].Item.equipCharacterId != 0)
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
                            OnClickReinforceItemChange(itemSlots[index].Item);
                        });
                    }

                    // Select
                    if (itemSlots[i].Item.customId == selectedItem.customId)
                    {
                        itemSlots[i].transform.GetChild(5).gameObject.SetActive(true);
                        itemSlots[i].GetComponent<Button>().enabled = false;
                    }
                    else
                    {
                        itemSlots[i].transform.GetChild(5).gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    // Result UI 드로잉
    void RefreshReinforceResultPanel(Item item, bool isSuccess)
    {
        //타이틀
        Text titleText = PanelGetReinforce.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>();
        titleText.text = isSuccess ? LocalizationManager.GetText("ReinforcementSuccessMessage") : LocalizationManager.GetText("ReinforcementFailMessage");
        //이펙트
        GameObject successEffectObject = PanelGetReinforce.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        successEffectObject.SetActive(isSuccess);
        //아이템이미지
        Transform itemSlot = PanelGetReinforce.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).transform;
        itemSlot.GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemClassImage(item.itemClass);
        itemSlot.GetChild(1).GetComponent<Image>().sprite = ItemSystem.GetItemImage(item.id);
        itemSlot.GetChild(2).GetComponent<Image>().color = ItemColor.GetItemColor(item.itemClass);
        //아이템이름
        Text itemNameText = PanelGetReinforce.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Text>();
        itemNameText.color = ItemColor.GetItemColor(item.itemClass);
        itemNameText.text = string.Format("<color='white'>+{0}</color> {1}", item.enhancement, ItemSystem.GetItemName(item.id));
        //아이템설명
        Text itemInfoText = PanelGetReinforce.transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Text>();
        itemInfoText.text = string.Format("{0}",ItemSystem.GetEquipmentItemDescription(selectedItem));
    }
    void ReinforcementEffect(Transform target = null)
    {
        if (target == null) target = this.transform;
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
        GameObject effect = EffectPool.Instance.PopFromPool("MagicEnchantBlue", target);
        effect.transform.localScale = new Vector3(1, 1, 1);
        effect.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f);
        Vector3 pos = target.position;
        pos.z = 0;
        effect.transform.localPosition = pos;
        effect.gameObject.SetActive(true);
    }
    #endregion
}
