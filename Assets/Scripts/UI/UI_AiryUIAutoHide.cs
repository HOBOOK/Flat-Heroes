using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AiryUIAutoHide : MonoBehaviour
{
    public float hideTime;
    private float showTime;
    private void OnEnable()
    {
        showTime = 0;
    }
    void Update()
    {
        if(this.GetComponent<AiryUIAnimatedElement>()!=null)
        {
            showTime += Time.unscaledDeltaTime;
            if (showTime > hideTime)
            {
                this.GetComponent<AiryUIAnimatedElement>().HideElement();
            }
        }
        else
        {
            showTime += Time.unscaledDeltaTime;
            if (showTime > hideTime)
            {
                this.gameObject.SetActive(false);
            }
        }

    }
}
