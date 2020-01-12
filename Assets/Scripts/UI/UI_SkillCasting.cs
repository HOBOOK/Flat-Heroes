using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillCasting : MonoBehaviour
{
    bool isShowing = false;
    public Transform parent;
    RectTransform rectTransform;
    Text skillNameText;
    Image heroThumbnailImage;
    Image skillImage;
    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
        skillNameText = this.GetComponentInChildren<Text>();
        heroThumbnailImage = this.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        skillImage = this.transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }
    private void OnEnable()
    {
        isShowing = false;
        if(parent==null)
        {
            foreach(var trans in FindObjectsOfType<Canvas>())
            {
                if(trans.name.Equals("CanvasShow"))
                {
                    parent = trans.transform;
                    break;
                }
            }
        }
    }
    public void ShowCastingUI(Sprite heroThumbnail, string skillName, bool isRight = false, Sprite img = null)
    {
        if(!isShowing)
        {
            heroThumbnailImage.sprite = heroThumbnail;
            skillImage.sprite = img;
            skillNameText.text = skillName;
            if(isRight)
            {
                StartCoroutine(ShowingCastingRightUI());
            }
            else
            {
                StartCoroutine(ShowingCastingUI());
            }

            isShowing = true;
        }
    }

    IEnumerator ShowingCastingUI()
    {
        SetAlpha(1);
        rectTransform.anchoredPosition = new Vector3(0, 150, 0);
        float scaleX = 0.0f;
        while(scaleX < 1)
        {
            this.transform.localScale = new Vector3(scaleX, 1, 1);
            scaleX += (3.5f - scaleX) * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        float deltaY = 150;
        while(deltaY >0)
        {
            rectTransform.anchoredPosition = new Vector3(0, deltaY, 0);
             deltaY -= (deltaY+20) * Time.deltaTime;
            SetAlpha(deltaY * 0.01f);
            yield return new WaitForEndOfFrame();
        }
        isShowing = false;
        ObjectPool.Instance.PushToPool("SkillCastingUI", this.gameObject,parent);
        yield return null;
    }

    IEnumerator ShowingCastingRightUI()
    {
        SetAlpha(1);
        rectTransform.anchoredPosition = new Vector3(0, 150, 0);
        float scaleX = 0.0f;
        while (scaleX < 1)
        {
            this.transform.localScale = new Vector3(scaleX, 1, 1);
            scaleX += (3.5f - scaleX) * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        float deltaY = 150;
        while (deltaY > 0)
        {
            rectTransform.anchoredPosition = new Vector3(0, deltaY, 0);
            deltaY -= (deltaY + 20) * Time.deltaTime;
            SetAlpha(deltaY * 0.01f);
            yield return new WaitForEndOfFrame();
        }
        isShowing = false;
        ObjectPool.Instance.PushToPool("SkillCastingReverseUI", this.gameObject, parent);
        yield return null;
    }

    void SetAlpha(float alpha)
    {
        foreach(Image sp in GetComponentsInChildren<Image>())
        {
            if(!sp.name.Equals("back"))
                sp.color = new Color(1, 1, 1, alpha);
        }
        skillNameText.color = new Color(1, 1, 1, alpha);
    }
}
