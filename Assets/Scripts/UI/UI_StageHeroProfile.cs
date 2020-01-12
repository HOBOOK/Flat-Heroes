using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageHeroProfile : MonoBehaviour
{
    public Hero hero;
    Slider hpBar;
    private void Awake()
    {
        hpBar = this.transform.GetComponentInChildren<Slider>();
    }
    float reTime = 0;
    public void SetHero(Hero h)
    {
        this.hero = h;
        this.hero.stageHeroProfileUI = this.gameObject;
    }
    public void ReSetHero()
    {
        try
        {
            this.hero = StageManagement.instance.GetStageHero(hero.id);
            this.hero.stageHeroProfileUI = this.gameObject;
        }
        catch
        {

        }
        finally
        {
            DisableResurrectionUI();
        }

    }
    void ShowResurrectionUI(float time)
    {
        this.transform.GetChild(2).gameObject.SetActive(true);
        this.transform.GetChild(2).GetComponentInChildren<Text>().text = time.ToString("N0");
    }
    void DisableResurrectionUI()
    {
        this.transform.GetChild(2).gameObject.SetActive(false);
        this.transform.GetChild(2).GetComponentInChildren<Text>().text = "";
    }
    private void Update()
    {
        if(hero!=null)
        {
            SetHpSlider();
        }
    }
    void SetHpSlider()
    {
        if(hpBar!=null)
            hpBar.value = (float)hero.status.hp / (float)hero.status.maxHp;
    }

    public void ShowResurrectionTime()
    {
        reTime = HeroSystem.GetHeroResurrectionTime(hero.id, StageManagement.instance.stageInfo.stageTime * 0.1f);
        ShowResurrectionUI(reTime);
        Debugging.Log(reTime + " 부활대기시작>" + hero.name);
        StartCoroutine("ShowingTime");
    }

    public IEnumerator ShowingTime()
    {
        while(reTime>0.0f)
        {
            ShowResurrectionUI(reTime);
            yield return new WaitForSeconds(1.0f);
            reTime -= 1;
        }
        CharactersManager.instance.ResurrectionHero(hero.id);
        ReSetHero();
        yield return null;
    }
}
