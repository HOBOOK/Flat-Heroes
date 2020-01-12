using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PanelObelisk : MonoBehaviour
{
    public UI_RewardChestOpen ChestPanel;
    public Transform BoxSlotParentTransform;
    private void OnEnable()
    {
        RefreshUI();
    }

    public void OnClickUncealButton(int type)
    {
        bool UncealAble = false;
        if (ItemSystem.GetUserChestCount(type) >= 1)
            UncealAble = true;
        if (UncealAble)
        {
            if(ItemSystem.UseItem(ItemSystem.GetUserChest(type).customId,1))
            {
                ChestPanel.gameObject.SetActive(true);
                ChestPanel.ShowChestReward(type);
            }
            else
            {
                int chestId = type == 2 ? 8005 : type == 1 ? 8004 : 8003;
                UI_Manager.instance.ShowAlert(ItemSystem.GetItemImage(chestId), LocalizationManager.GetText("alertUnableUncealBoxMessage"));
            }
        }
        else
        {
            int chestId = type == 2 ? 8005 : type == 1 ? 8004 : 8003;
            UI_Manager.instance.ShowAlert(ItemSystem.GetItemImage(chestId), LocalizationManager.GetText("alertUnableUncealBoxMessage"));
        }
        RefreshUI();
    }

    void RefreshUI()
    {
        if(BoxSlotParentTransform!=null)
        {
            for (var i = 0; i < BoxSlotParentTransform.childCount; i++)
            {
                BoxSlotParentTransform.GetChild(i).GetChild(0).GetComponentInChildren<Text>().text = string.Format("{0}\r\nx {1}", ItemSystem.GetItemName(8003 + i), ItemSystem.GetUserChestCount(i));
            }
        }
    }
}
