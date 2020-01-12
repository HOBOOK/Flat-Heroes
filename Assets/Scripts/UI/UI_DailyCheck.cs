using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DailyCheck : MonoBehaviour
{

    public GameObject DailySixPanel;
    public GameObject DailySevenPanel;
    public Text currentDayText;

    Image itemImage;

    public void StartDailyUI(int day)
    {
        currentDayText.text = string.Format("{0} Day", day+1);
        for (var i = 0; i <= day && i < 6; i++)
        {
            itemImage = DailySixPanel.transform.GetChild(i).GetComponent<Image>();
            itemImage.color = Color.yellow;
            DailySixPanel.transform.GetChild(i).GetChild(3).gameObject.SetActive(true);
        }
        for (var i = day + 1; i < DailySixPanel.transform.childCount; i++)
        {
            itemImage = DailySixPanel.transform.GetChild(i).GetComponent<Image>();
            itemImage.color = Color.white;
            DailySixPanel.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
        }
        if(day==6)
        {
            DailySevenPanel.transform.GetComponent<Image>().color = Color.yellow;
            DailySevenPanel.transform.GetChild(3).gameObject.SetActive(true);
        }
        DailyReward(day);
    }

    void DailyReward(int day)
    {
        switch(day)
        {
            case 0:
                SaveSystem.AddUserEnergy(50);
                break;
            case 1:
                SaveSystem.AddUserCrystal(15);
                break;
            case 2:
                SaveSystem.AddUserCoin(50000);
                break;
            case 3:
                SaveSystem.AddUserMagicStone(50);
                break;
            case 4:
                SaveSystem.AddUserCrystal(50);
                break;
            case 5:
                UI_Manager.instance.PopupGetGacha(GachaSystem.GachaType.SpecialFive);
                break;
            case 6:
                ItemSystem.SetObtainItem(6, 1);
                break;
        }

        UI_Manager.instance.ShowGetAlert("", string.Format("{0}\r\n<color='yellow'>{1}</color> {2}",LocalizationManager.GetText("DailyCheckMessage"), LocalizationManager.GetText("DailyCheckItem" + (day + 1)), LocalizationManager.GetText("alertGetMessage3")));
    }
}
