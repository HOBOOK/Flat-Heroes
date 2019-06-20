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
            labNeedCoinText = ScrollContentView.transform.GetChild(i).GetComponentInChildren<Button>().GetComponentInChildren<Text>();

            int labTypeLevel = LabSystem.GetLapLevel(i);
            labLevelText.text = string.Format("LEVEL {0}", labTypeLevel);
            labPowerText.text = string.Format("{0} > <color='yellow'>{1}</color>",LabSystem.GetLapPower(i, labTypeLevel), LabSystem.GetLapPower(i, labTypeLevel+1));
            labNeedCoinText.text = Common.GetThousandCommaText(LabSystem.GetNeedMoney(labTypeLevel));
        }
    }


}
