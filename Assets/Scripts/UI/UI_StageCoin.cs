using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageCoin : MonoBehaviour
{
    Text coinText;
    Image coinImage;
    float imageSize;
    float effectTimer = 0;
    private void Awake()
    {
        coinText = this.GetComponentInChildren<Text>();
        coinImage = this.transform.GetChild(0).GetComponent<Image>();
    }
    void LateUpdate()
    {
        if (coinText != null && StageManagement.instance != null && StageManagement.instance.stageInfo != null)
            coinText.text = Common.GetThousandCommaText(StageManagement.instance.stageInfo.stageCoin).ToString();
    }

    public void GetEffect()
    {
        if(effectTimer<=0)
        {
            StartCoroutine("GettingEffect");
        }
    }
    IEnumerator GettingEffect()
    {
        effectTimer = 1.0f;
        imageSize = 1.25f;
        while (effectTimer > 0||imageSize>1)
        {
            imageSize -= Time.deltaTime * 0.3f;
            coinImage.transform.localScale = new Vector3(imageSize, imageSize, imageSize);
            yield return new WaitForEndOfFrame();
            effectTimer -= Time.deltaTime;
        }
        coinImage.transform.localScale = Vector3.one;
        yield return null;
    }
}
