using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ui_StageCoin : MonoBehaviour
{
    Text coinText;
    Image coinImage;
    float imageSize = 1;
    bool isSizeOver = false;
    private void Awake()
    {
        coinText = this.GetComponentInChildren<Text>();
        coinImage = this.GetComponentInChildren<Image>();
    }
    void LateUpdate()
    {
        if(coinText!=null&& StageManagement.instance!=null)
            coinText.text = Common.GetThousandCommaText(StageManagement.instance.stageInfo.stageCoin).ToString();
        if(coinImage!=null)
        {
            if (imageSize > 1.1f)
                isSizeOver = true;
            else if (imageSize < 1)
                isSizeOver = false;
            imageSize = isSizeOver ? imageSize - Time.deltaTime * 0.1f : imageSize + Time.deltaTime*0.1f;
            coinImage.transform.localScale = new Vector3(imageSize, imageSize, imageSize);
        }
    }
}
