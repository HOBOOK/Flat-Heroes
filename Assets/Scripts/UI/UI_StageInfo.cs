using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageInfo : MonoBehaviour
{
    Text stageTimeText;
    Text kPointText;
    Text dPointText;
    private void Awake()
    {
        foreach(var txt in this.transform.GetComponentsInChildren<Text>())
        {
            if (txt.name.Equals("stageTime"))
                stageTimeText = txt;
            else if (txt.name.Equals("kPoint"))
                kPointText = txt;
            else if (txt.name.Equals("dPoint"))
                dPointText = txt;
        }
    }
    private void FixedUpdate()
    {
        if (stageTimeText != null)
            stageTimeText.text = StageManagement.instance.GetStageTime();
        if (kPointText != null)
            kPointText.text = StageManagement.instance.GetKPoint().ToString();
        if (dPointText.text != null)
            dPointText.text = StageManagement.instance.GetDPoint().ToString();
    }
}
