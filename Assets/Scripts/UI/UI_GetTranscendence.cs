using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GetTranscendence : MonoBehaviour
{
    public GameObject heroObj;
    public GameObject heroObject;
    public GameObject transcendenceEffect;
    public Transform heroPoint;
    public Transform informationTransform;
    public GameObject ButtonYes;
    GameObject transEffect;
    private void OnEnable()
    {
        ButtonYes.SetActive(false);
        informationTransform.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        if(heroObj!=null)
            heroObj.SetActive(true);
    }
    public void ShowHero(GameObject hero, HeroData data)
    {
        if (hero != null)
        {
            heroObj = hero;
            heroObj.SetActive(false);
            heroObject = Instantiate(hero, heroPoint);
            heroObject.transform.localScale = new Vector3(200, 200, 200);
            heroObject.transform.localPosition = Vector3.zero;

            if (heroObject.GetComponent<Hero>() != null)
                Destroy(heroObject.GetComponent<Hero>());
            if (heroObject.GetComponent<Rigidbody2D>() != null)
                Destroy(heroObject.GetComponent<Rigidbody2D>());
            foreach (var sp in heroObject.GetComponentsInChildren<SpriteRenderer>())
            {
                sp.sortingLayerName = "ShowObject";
                sp.gameObject.layer = 16;
            }
            heroObject.gameObject.SetActive(true);
            StartCoroutine(ShowAnimation(data));
        }
    }

    IEnumerator ShowAnimation(HeroData data)
    {
        yield return new WaitForSeconds(0.2f);
        TranscendenceEffect();
        yield return new WaitForSeconds(2.0f);
        if (transEffect != null)
            Destroy(transEffect);
        heroObject.SetActive(false);
        float alpha = 0.0f;
        informationTransform.gameObject.SetActive(true);
        informationTransform.GetChild(0).GetComponentInChildren<Text>().text = string.Format("{0} {1}{2}", LocalizationManager.GetText("Transcendence"), data.over,LocalizationManager.GetText("Phase"));
        informationTransform.GetChild(0).gameObject.SetActive(true);
        informationTransform.GetChild(0).GetComponent<AiryUIAnimatedElement>().ShowElement();
        yield return new WaitForSeconds(0.5f);
        informationTransform.GetChild(1).gameObject.SetActive(true);
        informationTransform.GetChild(1).GetComponent<Text>().text = string.Format("Max Lv <size='30'>{0}</size>  >  {1}", (data.over-1)*10+100,data.over*10+100);
        while (alpha > 1)
        {
            informationTransform.GetChild(2).GetComponent<Text>().color = new Color(1, 1, 1, alpha);
            alpha += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        ButtonYes.SetActive(true);
    }

    public void TranscendenceEffect()
    {
        if (transcendenceEffect != null)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.smoke);
            transEffect = Instantiate(transcendenceEffect, heroPoint.transform);
            transEffect.transform.localPosition = Vector3.zero;
            transEffect.transform.localScale = new Vector3(200, 200, 200);
            transEffect.SetActive(true);
        }
    }
}
