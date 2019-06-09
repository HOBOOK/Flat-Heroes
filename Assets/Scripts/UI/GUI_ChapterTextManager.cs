using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_ChapterTextManager : MonoBehaviour
{
    public List<string> chapterTextList = new List<string>();

    public void SetChapterText(int chapterNum)
    {
        GetComponentInChildren<Text>().text = "<size=40>Chapter " + (chapterNum + 1) + "</size>\r\n";
        GetComponentInChildren<Text>().text += !string.IsNullOrEmpty(chapterTextList[chapterNum]) ? chapterTextList[chapterNum] : "";
        StartCoroutine("FadeText");
    }

    IEnumerator FadeText()
    {
        int cnt = 0;
        yield return new WaitForSeconds(1.0f);
        while(cnt<50)
        {
            GetComponentInChildren<Text>().color = new Color(1, 1, 1, cnt * 0.02f);
            yield return new WaitForSeconds(0.05f);
            cnt++;
        }
        yield return new WaitForSeconds(2.0f);
        while(cnt<100)
        {
            GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1-((cnt-50) * 0.02f));
            yield return new WaitForSeconds(0.05f);
            cnt++;
        }
        GetComponentInChildren<Text>().color = new Color(1, 1, 1, 0);
        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);
        yield return null;
    }
}
