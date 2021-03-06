﻿using System;
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
                EquipmentActionPanel.transform.GetChild(0).GetComponent<Text>().text = LocalizationManager.GetText("equipmentItemEmptyInformation");
                EquipmentActionPanel.GetComponentInChildren<Button>().enabled = false;
                EquipmentActionPanel.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "";
            }
            else
            {
                Item equipmentItemInfo = ItemSystem.GetUserEquipmentItem(equipmentItemId);
                InformationPanel.transform.GetChild(0).GetComponent<Image>().enabled = true;
                InformationPanel.transform.GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemImage(equipmentItemId, true);
                InformationPanel.transform.GetComponentInChildren<Text>().enabled = true;
                InformationPanel.transform.GetComponentInChildren<Text>().text = string.Format("{0}({1})", ItemSystem.GetItemName(equipmentItemInfo.id), Enum.GetName(typeof(GachaSystem.GachaClass), (GachaSystem.GachaClass)equipmentItemInfo.itemClass - 1));
                InformationPanel.transform.GetComponentInChildren<Text>().color = ItemColor.GetItemColor(equipmentItemInfo.itemClass);
                EquipmentActionPanel.transform.GetChild(0).GetComponent<Text>().text = ItemSystem.GetEquipmentItemDescription(equipmentItemInfo);
                EquipmentActionPanel.GetComponentInChildren<Button>().enabled = true;
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
        if(InformationPanel!=null&&EquipmentActionPanel!=null)
        {
            if (selectedButton != null)
                selectedButton.interactable = true;
            selectedButton = button;
            selectedButton.interactable = false;
            Item item = ItemSystem.GetUserEquipmentItem(itemid);
            InformationPanel.transform.GetChild(0).GetComponent<Image>().enabled = true;
            InformationPanel.transform.GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemImage(item.id);
            InformationPanel.transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(true);
            InformationPanel.transform.GetComponentInChildren<Text>().enabled = true;
            InformationPanel.transform.GetComponentInChildren<Text>().text = string.Format("{0}({1})", ItemSystem.GetItemName(item.id), Enum.GetName(typeof(GachaSystem.GachaClass), (GachaSystem.GachaClass)item.itemClass - 1));
            InformationPanel.transform.GetComponentInChildren<Text>().color = ItemColor.GetItemColor(item.itemClass);
            EquipmentActionPanel.SetActive(true);
            EquipmentActionPanel.transform.GetChild(0).GetComponent<Text>().text = ItemSystem.GetEquipmentItemDescription(item);
            EquipmentActionPanel.GetComponentInChildren<Button>().enabled = true;
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
        HeroSystem.EquipHeroEquimentItem(equipmentSlotIndex, targetHeroId, ItemSystem.GetUserEquipmentItem(id));
        Debugging.Log(id + " 아이템 장착성공");
        this.gameObject.SetActive(false);
        
    }
    public void DismountEvent(int id)
    {
        HeroSystem.DismountHeroEquimentItem(equipmentSlotIndex,targetHeroId);
        equipmentItemId = 0;
        RefreshUI();
        Debugging.Log(id + " 아이템 장착해제");
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
