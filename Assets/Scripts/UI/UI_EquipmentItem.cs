using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipmentItem : MonoBehaviour
{
    private int equipmentSlotIndex;
    private int equipmentItemId;
    private int targetHeroId;
    public GameObject ItemSlotPrefab;
    public GameObject InformationPanel;
    public GameObject EquipmentActionPanel;
    public GameObject ItemListContentView;
    public UI_HeroInfo heroInfoPanelScript;

    Button selectedButton;

    List<Item> userEquipmentItemList = new List<Item>();

    private void OnEnable()
    {
        ShowUI();
    }
    private void OnDisable()
    {
        if (heroInfoPanelScript.gameObject.activeSelf)
        {
            heroInfoPanelScript.EnableShowObject();
            heroInfoPanelScript.RefreshHeroStatusEquipmentPanel();
        }
    }

    public void SetSlotIndex(int index, int itemid, int targetHero,UI_HeroInfo uiScript)
    {
        index = Mathf.Clamp(index, 0, 5);
        equipmentSlotIndex = index;
        equipmentItemId = itemid;
        heroInfoPanelScript = uiScript;
        targetHeroId = targetHero;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if(ItemSlotPrefab!=null&&InformationPanel!=null&&EquipmentActionPanel!=null&&ItemListContentView!=null)
        {
            if(equipmentItemId==0)
            {
                InformationPanel.transform.GetChild(0).GetComponent<Image>().enabled = false;
                InformationPanel.transform.GetComponentInChildren<Text>().text = LocalizationManager.GetText("equipmentItemEmpty");
                InformationPanel.transform.GetChild(3).gameObject.SetActive(false);
                EquipmentActionPanel.transform.GetChild(0).GetComponent<Text>().text = LocalizationManager.GetText("equipmentItemEmptyInformation");
                InformationPanel.GetComponent<Image>().enabled = false;
                EquipmentActionPanel.GetComponentInChildren<Button>().enabled = false;
                EquipmentActionPanel.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "";
            }
            else
            {
                Item equipmentItemInfo = ItemSystem.GetUserEquipmentItem(equipmentItemId);
                InformationPanel.GetComponent<Image>().enabled = true;
                InformationPanel.GetComponent<Image>().sprite = ItemSystem.GetItemClassImage(equipmentItemInfo.id);
                InformationPanel.transform.GetChild(0).GetComponent<Image>().enabled = true;
                InformationPanel.transform.GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemImage(equipmentItemId, true);
                InformationPanel.transform.GetChild(3).gameObject.SetActive(true);
                InformationPanel.transform.GetComponentInChildren<Text>().enabled = true;
                InformationPanel.transform.GetComponentInChildren<Text>().text = string.Format("<color='white'>{0}</color> {1}({2})", ItemSystem.GetItemEnhancementText(equipmentItemInfo.customId), ItemSystem.GetItemName(equipmentItemInfo.id), Enum.GetName(typeof(GachaSystem.GachaClass), (GachaSystem.GachaClass)equipmentItemInfo.itemClass - 1));
                InformationPanel.transform.GetComponentInChildren<Text>().color = ItemColor.GetItemColor(equipmentItemInfo.itemClass);
                EquipmentActionPanel.transform.GetChild(0).GetComponent<Text>().text = ItemSystem.GetEquipmentItemDescription(equipmentItemInfo);
                EquipmentActionPanel.GetComponentInChildren<Button>().enabled = true;
                EquipmentActionPanel.GetComponentInChildren<Button>().GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 1);
                EquipmentActionPanel.GetComponentInChildren<Button>().GetComponentInChildren<Text>().color = Color.red;
                EquipmentActionPanel.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = LocalizationManager.GetText("equipmentItemUnEquipButton");
                EquipmentActionPanel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                EquipmentActionPanel.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    DismountEvent(equipmentItemInfo.id);
                });
            }
            foreach(Transform child in ItemListContentView.transform)
            {
                Destroy(child.gameObject);
            }
            userEquipmentItemList = ItemSystem.GetUserUnEquipmentItems(Common.OrderByType.VALUE);
            for(var i = 0; i <userEquipmentItemList.Count; i++)
            {
                GameObject itemSlot = Instantiate(ItemSlotPrefab, ItemListContentView.transform);
                itemSlot.transform.GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemClassImage(userEquipmentItemList[i].id);
                itemSlot.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemImage(userEquipmentItemList[i].id);
                itemSlot.GetComponentInChildren<Text>().color = ItemColor.GetItemColor(userEquipmentItemList[i].itemClass);
                itemSlot.GetComponentInChildren<Text>().text = ItemSystem.GetItemName(userEquipmentItemList[i].id);
                itemSlot.transform.GetChild(2).GetComponent<Image>().color = ItemColor.GetItemColor(userEquipmentItemList[i].itemClass);
                if(userEquipmentItemList[i].enhancement>0)
                {
                    itemSlot.transform.GetChild(3).gameObject.SetActive(true);
                    itemSlot.transform.GetChild(3).GetComponentInChildren<Text>().text = userEquipmentItemList[i].enhancement.ToString();

                }
                else
                {
                    itemSlot.transform.GetChild(3).gameObject.SetActive(false);
                }
                int index = i;
                itemSlot.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                itemSlot.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    OnClickItemInfoShow(userEquipmentItemList[index].customId, itemSlot.GetComponentInChildren<Button>());
                });
            }
        }
    }
    public void OnClickItemInfoShow(int itemid, Button button)
    {
        if(InformationPanel!=null&&EquipmentActionPanel!=null&&equipmentItemId==0)
        {
            if (selectedButton != null)
                selectedButton.interactable = true;
            selectedButton = button;
            selectedButton.interactable = false;
            Item item = ItemSystem.GetUserEquipmentItem(itemid);
            InformationPanel.GetComponent<Image>().enabled = true;
            InformationPanel.GetComponent<Image>().sprite = ItemSystem.GetItemClassImage(item.id);
            InformationPanel.transform.GetChild(0).GetComponent<Image>().enabled = true;
            InformationPanel.transform.GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemImage(item.id);
            InformationPanel.transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(true);
            InformationPanel.transform.GetComponentInChildren<Text>().enabled = true;
            InformationPanel.transform.GetComponentInChildren<Text>().text = string.Format("<color='white'>{0}</color> {1}({2})", ItemSystem.GetItemEnhancementText(item.customId),ItemSystem.GetItemName(item.id), Enum.GetName(typeof(GachaSystem.GachaClass), (GachaSystem.GachaClass)item.itemClass - 1));
            InformationPanel.transform.GetComponentInChildren<Text>().color = ItemColor.GetItemColor(item.itemClass);
            EquipmentActionPanel.SetActive(true);
            EquipmentActionPanel.transform.GetChild(0).GetComponent<Text>().text = ItemSystem.GetEquipmentItemDescription(item);
            EquipmentActionPanel.GetComponentInChildren<Button>().enabled = true;
            EquipmentActionPanel.GetComponentInChildren<Button>().GetComponent<Image>().color = Color.white;
            EquipmentActionPanel.GetComponentInChildren<Button>().GetComponentInChildren<Text>().color = new Color(0.1f, 0.1f, 0.1f, 1);
            EquipmentActionPanel.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = LocalizationManager.GetText("equipmentItemEquipButton");
            EquipmentActionPanel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            EquipmentActionPanel.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                EquipmentEvent(item.customId);
            });

        }
    }

    public void EquipmentEvent(int id)
    {
        int itemClass = ItemSystem.GetUserEquipmentItem(id).itemClass;
        if (itemClass > 4)
        {
            StartCoroutine(CheckingAlert(true, (itemClass - 3) * (itemClass - 2),id));
        }
        else
        {
            Equipment(id);
        }
    }
    public void DismountEvent(int id)
    {
        int itemClass = ItemSystem.GetItem(id).itemClass;
        if(itemClass>4)
        {
            StartCoroutine(CheckingAlert(false, (itemClass-3) * (itemClass - 2),id));
        }
        else
        {
            Dismount();
        }
        Debugging.Log(id + " 아이템 장착해제");
    }
    void Equipment(int id)
    {
        HeroSystem.EquipHeroEquimentItem(equipmentSlotIndex, targetHeroId, ItemSystem.GetUserEquipmentItem(id));
        Debugging.Log(id + " 아이템 장착성공");
        //this.gameObject.SetActive(false);
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.pickup);
        equipmentItemId = id;
        RefreshUI();
    }
    void Dismount()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        HeroSystem.DismountHeroEquimentItem(equipmentSlotIndex, targetHeroId);
        equipmentItemId = 0;
        RefreshUI();
    }

    IEnumerator CheckingAlert(bool isEquip, int paymentAmount,int id)
    {
        GameObject alertPanel;
        if(isEquip)
            alertPanel = UI_Manager.instance.ShowNeedAlert("",LocalizationManager.GetText("alertEquipMessage"));
        else
            alertPanel = UI_Manager.instance.ShowNeedAlert(Common.GetCoinCrystalEnergyImagePath(1), string.Format("<color='yellow'>'{0}' <size='24'>x </size>{1}</color>  {2}", Common.GetCoinCrystalEnergyText(1), paymentAmount, LocalizationManager.GetText("alertDismountMessage")));

        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            UI_Manager.instance.ClosePopupAlertUI();
            if(!isEquip)
            {
                if (Common.PaymentCheck(ref User.blackCrystal, paymentAmount))
                {
                    Dismount();
                }
                else
                {
                    UI_Manager.instance.ShowAlert(Common.GetCoinCrystalEnergyImagePath(1), LocalizationManager.GetText("manageTab2EvolutionPaymentWarning2"));
                }
            }
            else
            {
                Equipment(id);
            }
        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 아니오를 클릭시
        }
        yield return null;
    }


    public void ShowUI()
    {
        if (this.GetComponentsInChildren<AiryUIAnimatedElement>() != null)
        {
            foreach (var element in this.GetComponentsInChildren<AiryUIAnimatedElement>())
                element.ShowElement();
        }
    }


}
