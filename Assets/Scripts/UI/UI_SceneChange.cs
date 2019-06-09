using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_SceneChange : MonoBehaviour {
    public Image cover;
    public void SceneStart()
    {
        StartCoroutine("SceneChanging");
    }
    IEnumerator SceneChanging()
    {
        var cnt = 0;
        float alpha = 0.0f;
        while(cnt<60)
        {
            if (alpha < 1)
                alpha = cnt * 0.02f;
            else
                alpha = 1;
            cover.color = new Color(cover.color.r, cover.color.g, cover.color.b, alpha);
            yield return new WaitForSeconds(0.05f);
            cnt++;
        }
        cover.color = new Color(cover.color.r, cover.color.g, cover.color.b, 1);
        SceneManager.LoadScene("Stage_0");
        yield return null;
    }
}
