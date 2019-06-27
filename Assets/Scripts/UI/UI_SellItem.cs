﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SellItem : MonoBehaviour
{
    public GameObject ItemSlotPrefab;
    public GameObject ItemListContentView;
    public GameObject SellListContentView;
    public GameObject SellActionView;
    public GameObject parent;

    List<Item> userItemList = new List<Item>();
    List<Item> sellItemList = new List<Item>();

    Text sellItemCountText;
    Text sellItemTotalCoinText;
    Button sellItemButton;

    bool isCheckAlertOn;
    int totalValue;
    int totalCount;

    private void Awake()
    {
        if(SellActionView!=null)
        {
            sellItemCountText = SellActionView.transform.GetChild(0).GetComponent<Text>();
            sellItemTotalCoinText = SellActionView.transform.GetChild(1).GetComponentInChildren<Text>();
            sellItemButton = SellActionView.transform.GetChild(2).GetComponent<Button>();
        }
    }
    void SetActionInfo(int count, int coin, bool isSellAble)
    {
        sellItemCountText.text = string.Format("{0}/15",count);
        sellItemTotalCoinText.text = coin.ToString();
        sellItemButton.enabled = isSellAble;
    }

    private void OnEnable()
    {
        EnableUI();
    }

    private void OnDisable()
    {
        parent.GetComponent<UI_Manager_InventoryTab>().RefreshUI();
    }
    public void EnableUI()
    {
        totalValue = 0;
        totalCount = 0;
        if (ItemSlotPrefab != null && ItemListContentView != null)
        {
            userItemList.Clear();
            sellItemList.Clear();
            foreach (Transform child in ItemListContentView.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in SellListContentView.transform)
            {
                Destroy(child.gameObject);
            }
            userItemList = ItemSystem.GetUserUnEquipmentItems(Common.OrderByType.VALUE);

            for (var i = 0; i < userItemList.Count; i++)
            {
                GameObject itemSlot = Instantiate(ItemSlotPrefab, ItemListContentView.transform);
                itemSlot.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(userItemList[i].image);
                itemSlot.GetComponentInChildren<Text>().color = ItemColor.GetItemColor(userItemList[i].itemClass);
                itemSlot.GetComponentInChildren<Text>().text = userItemList[i].name;
                itemSlot.transform.GetChild(2).GetComponent<Image>().color = ItemColor.GetItemColor(userItemList[i].itemClass);
                int index = i;
                itemSlot.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                itemSlot.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    OnClickItemToSellList(userItemList[index], itemSlot.GetComponentInChildren<Button>());
                });
                itemSlot.gameObject.SetActive(true);
            }
            SetActionInfo(0, 0,false);
        }
    }

    public void RefreshUI()
    {
        if (ItemSlotPrefab != null && ItemListContentView != null)
        {
            foreach (Transform child in ItemListContentView.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in SellListContentView.transform)
            {
                Destroy(child.gameObject);
            }
            for (var i = 0; i < userItemList.Count; i++)
            {
                GameObject itemSlot = Instantiate(ItemSlotPrefab, ItemListContentView.transform);
                itemSlot.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(userItemList[i].image);
                itemSlot.GetComponentInChildren<Text>().color = ItemColor.GetItemColor(userItemList[i].itemClass);
                itemSlot.GetComponentInChildren<Text>().text = userItemList[i].name;
                itemSlot.transform.GetChild(2).GetComponent<Image>().color = ItemColor.GetItemColor(userItemList[i].itemClass);
                int index = i;
                itemSlot.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                itemSlot.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    OnClickItemToSellList(userItemList[index], itemSlot.GetComponentInChildren<Button>());
                });
                itemSlot.gameObject.SetActive(true);
            }

            for(var i = 0; i< sellItemList.Count&&i<15; i++)
            {
                GameObject itemSlot = Instantiate(ItemSlotPrefab, SellListContentView.transform);
                itemSlot.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(sellItemList[i].image);
                itemSlot.GetComponentInChildren<Text>().color = ItemColor.GetItemColor(sellItemList[i].itemClass);
                itemSlot.GetComponentInChildren<Text>().text = sellItemList[i].name;
                itemSlot.transform.GetChild(2).GetComponent<Image>().color = ItemColor.GetItemColor(sellItemList[i].itemClass);
                int index = i;
                itemSlot.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                itemSlot.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    OnClickItemToSellList(userItemList[index], itemSlot.GetComponentInChildren<Button>());
                });
                itemSlot.gameObject.SetActive(true);

                Debugging.Log(sellItemList[i].name);
            }
        }
    }

    public void OnClickItemToSellList(Item sellItem, Button button)
    {
        if (SellListContentView != null)
        {
            if(sellItemList.Count < 15)
            {
                Debugging.Log(sellItem.name + " 추가됨!");
                userItemList.Remove(sellItem);
                sellItemList.Add(sellItem);

                totalValue = 0;
                totalCount = sellItemList.Count;
                foreach (var item in sellItemList)
                {
                    totalValue += item.value;
                }
                SetActionInfo(totalCount, totalValue, true);
                RefreshUI();
            }
            else
            {
                Debugging.Log("리스트가 가득참");
            }

        }
    }

    public void OnSellStart()
    {
        if (!isCheckAlertOn)
        {
            StartCoroutine("CheckingSellAlert");
        }
    }
    IEnumerator CheckingSellAlert()
    {
        isCheckAlertOn = true;
        var alertPanel = UI_Manager.instance.ShowNeedAlert("", string.Format("<color='red'>선택된 아이템</color> {0}개를 <color='yellow'>{1} 코인</color>  에 판매하시겠습니까?", totalCount, totalValue));
        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            UI_Manager.instance.ClosePopupAlertUI();
            SellItemProcessing();

        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
        }
        isCheckAlertOn = false;
        yield return null;
    }

    void SellItemProcessing()
    {
        if (SellAbleCheck())
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.coinGet);
            for(int i = 0; i < sellItemList.Count; i++)
            {
                ItemSystem.UseItem(sellItemList[i].customId, 1);
            }
            SaveSystem.AddUserCoin(totalValue);
            UI_Manager.instance.ShowGetAlert("Items/coin", string.Format("<color='yellow'>{0}</color> 코인을 획득했습니다.", totalValue));
            EnableUI();
        }
        else
        {
            UI_Manager.instance.ShowAlert("", string.Format("<color='yellow'></color> 은(는) 현재 판매할 수 없습니다. \r\n <color='grey'><size='20'>판매</size></color>"));
        }
    }

    bool SellAbleCheck()
    {
        foreach(var item in sellItemList)
        {
            if(item.count==0 && item.equipCharacterId>0)
            {
                return false;
            }
        }
        return true;
    }
}