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
                InformationPanel.transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(false);
                InformationPanel.transform.GetComponentInChildren<Text>().enabled = false;
                EquipmentActionPanel.SetActive(false);
            }
            else
            {
                Item equipmentItemInfo = ItemSystem.GetUserEquipmentItem(equipmentItemId);
                InformationPanel.transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(true);
                InformationPanel.transform.GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemImage(equipmentItemId,true);
                InformationPanel.transform.GetComponentInChildren<Text>().enabled = true;
                InformationPanel.transform.GetComponentInChildren<Text>().text = equipmentItemInfo.name;
                EquipmentActionPanel.SetActive(true);
                EquipmentActionPanel.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}\r\n\r\n<color='yellow'>공격력 + {1}\r\n방어력 + {2}</color>", equipmentItemInfo.description, equipmentItemInfo.attack, equipmentItemInfo.defence);
                EquipmentActionPanel.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "장착해제";
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
            userEquipmentItemList = ItemSystem.GetUserUnEquipmentItems(Common.OrderByType.NAME);
            for(var i = 0; i <userEquipmentItemList.Count; i++)
            {
                GameObject itemSlot = Instantiate(ItemSlotPrefab, ItemListContentView.transform);
                itemSlot.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(userEquipmentItemList[i].image);
                itemSlot.GetComponentInChildren<Text>().text = userEquipmentItemList[i].name;
                int index = i;
                itemSlot.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                itemSlot.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    OnClickItemInfoShow(userEquipmentItemList[index].customId);
                });
            }
        }
    }
    public void OnClickItemInfoShow(int itemid)
    {
        if(InformationPanel!=null&&EquipmentActionPanel!=null)
        {
            Item item = ItemSystem.GetUserEquipmentItem(itemid);
            InformationPanel.transform.GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemImage(item.id);
            InformationPanel.transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(true);
            InformationPanel.transform.GetComponentInChildren<Text>().enabled = true;
            InformationPanel.transform.GetComponentInChildren<Text>().text = item.name;
            EquipmentActionPanel.SetActive(true);
            EquipmentActionPanel.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}\r\n\r\n<color='yellow'>공격력 + {1}\r\n방어력 + {2}</color>", item.description, item.attack, item.defence);
            EquipmentActionPanel.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "장착하기";
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
