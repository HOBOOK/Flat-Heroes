using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public static IntroManager instance = null;

    public GameObject storyContext;

    public static float introSpeed = 50f;

    public Image FadeCoverImage;

    public GameObject Background;

    private void Awake()
    {
        if (instance = null)
            instance = this;
    }

    private void Start()
    {
        StartCoroutine("StartIntro");
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            introSpeed = 200f;
        }
        else
        {
            introSpeed = 50f;
        }
    }

    public void IntroSkip()
    {
        LoadSceneManager.instance.LoadScene(5);
    }

    public void IntroEnd()
    {
        LoadSceneManager.instance.LoadScene(5);
    }

    public IEnumerator StartIntro()
    {
        SoundManager.instance.BgmSourceChange(AudioClipManager.instance.Intro);
        float posY = -1300;
        while(posY<1100)
        {
            storyContext.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, posY);
            posY += Time.deltaTime * introSpeed;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(2.0f);
        float alpha = 0.0f;
        Background.GetComponent<Animation>().enabled = false;
        while (alpha<1.0f)
        {
            FadeCoverImage.color = new Color(0, 0, 0, alpha);
            Background.GetComponent<RectTransform>().localScale = new Vector3(1 + alpha, 1 + alpha, 1 + alpha);
            alpha += Time.deltaTime*0.5f;
            yield return new WaitForEndOfFrame();
        }

        IntroEnd();
    }
}
