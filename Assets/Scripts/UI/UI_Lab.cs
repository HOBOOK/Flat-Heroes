using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lab : MonoBehaviour
{
    GameObject ScrollContentView;

    Text labLevelText;
    Text labPowerText;
    Text labNeedCoinText;
    Button labLevelButton;

    private void Awake()
    {
        if (ScrollContentView == null)
            ScrollContentView = this.GetComponentInChildren<ContentSizeFitter>().gameObject;
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        for(int i = 0; i < ScrollContentView.transform.childCount; i++)
        {
            labLevelText = ScrollContentView.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>();
            labPowerText = ScrollContentView.transform.GetChild(i).GetChild(1).GetChild(1).GetComponentInChildren<Text>();
            labLevelButton = ScrollContentView.transform.GetChild(i).GetComponentInChildren<Button>();
            labNeedCoinText = labLevelButton.GetComponentInChildren<Text>();

            int labTypeLevel = LabSystem.GetLapLevel(i);
            labLevelText.text = string.Format("LEVEL {0}", labTypeLevel);
            if(i==2||i==3)// 뒤에 %가 붙는경우
                labPowerText.text = string.Format("{0}% > <color='yellow'>{1}%</color>", LabSystem.GetLapPower(i, labTypeLevel), LabSystem.GetLapPower(i, labTypeLevel + 1));
            else
                labPowerText.text = string.Format("{0} > <color='yellow'>{1}</color>",LabSystem.GetLapPower(i, labTypeLevel), LabSystem.GetLapPower(i, labTypeLevel+1));

            int levelupNeedCoin = LabSystem.GetNeedMoney(labTypeLevel);
            labNeedCoinText.text = Common.GetThousandCommaText(levelupNeedCoin);
            int slotIndex = i;
            labLevelButton.onClick.RemoveAllListeners();
            labLevelButton.onClick.AddListener(delegate
            {
                OnClickLabLevelUp(slotIndex, levelupNeedCoin);
            });
            if(Common.PaymentAbleCheck(ref User.coin, levelupNeedCoin))
            {
                labLevelButton.interactable = true;
            }
            else
            {
                labLevelButton.interactable = false;
            }
        }
    }

    void OnClickLabLevelUp(int type, int needMoney)
    {
        if(Common.PaymentCheck(ref User.coin, needMoney))
        {
            LabSystem.SetLapLevelUp(type);
            RefreshUI();
        }
        else
        {
            //
        }
    }
}
