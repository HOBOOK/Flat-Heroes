using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AiryUIAutoHide : MonoBehaviour
{
    public float hideTime;
    private float showTime;
    void Update()
    {
        if(this.GetComponent<AiryUIAnimatedElement>()!=null)
        {
            showTime += Time.deltaTime;
            if (showTime > hideTime)
            {
                this.GetComponent<AiryUIAnimatedElement>().HideElement();
            }
        }

    }
}
