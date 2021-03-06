﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageEnergy : MonoBehaviour
{
    Text energyText;
    Image energyImage;
    float imageSize = 1;
    bool isSizeOver = false;
    private void Awake()
    {
        energyText = this.GetComponentInChildren<Text>();
        energyImage = this.GetComponentInChildren<Image>();
    }
    void LateUpdate()
    {
        if(energyText != null&& StageManagement.instance!=null&&StageManagement.instance.stageInfo!=null)
            energyText.text = string.Format("{0}/{1}",Common.GetThousandCommaText(StageManagement.instance.stageInfo.stageEnergy), Common.GetThousandCommaText(StageManagement.instance.stageInfo.stageMaxEnergy));
        if(energyImage != null)
        {
            if (imageSize > 1.1f)
                isSizeOver = true;
            else if (imageSize < 1)
                isSizeOver = false;
            imageSize = isSizeOver ? imageSize - Time.deltaTime * 0.1f : imageSize + Time.deltaTime*0.1f;
            energyImage.transform.localScale = new Vector3(imageSize, imageSize, imageSize);
        }
    }
}
