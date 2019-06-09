using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_ToolTip : MonoBehaviour
{
    public string text;
    private void OnEnable()
    {
        foreach (var i in GetComponentsInChildren<Image>())
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        }
        foreach (var i in GetComponentsInChildren<Text>())
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
            i.text = text;
        }
        StartCoroutine("EnableToolTip");
    }

    IEnumerator EnableToolTip()
    {
        float cnt = 0;
        while(cnt<680)
        {
            cnt +=5f;
            if(cnt<250)
            {
                foreach (var i in GetComponentsInChildren<Image>())
                {
                    i.color = new Color(i.color.r, i.color.g, i.color.b, cnt / 255);
                }
            }
            if(cnt > 200)
            {
                foreach (var i in GetComponentsInChildren<Text>())
                {
                    i.color = new Color(i.color.r, i.color.g, i.color.b, ((cnt-200)*0.5f)/ 255);
                }
            }
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(2.0f);
        StartCoroutine("DisableToolTip");
        yield return null;
    }

    IEnumerator DisableToolTip()
    {
        float cnt =230;
        while (cnt > 0)
        {
            cnt -= 5f;
            foreach (var i in GetComponentsInChildren<Image>())
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, cnt / 255);
            }
            foreach (var i in GetComponentsInChildren<Text>())
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, cnt / 255);
            }
            yield return new WaitForSeconds(0.01f);
        }
        this.gameObject.SetActive(false);
        yield return null;
    }
}