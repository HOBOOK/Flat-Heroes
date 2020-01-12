using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossModeHpBar : MonoBehaviour
{
    float currentHp = 0;
    float currentMaxHp = 0;
    private float hpBarSetpsLength = 10;
    private bool isOnPanelHP = false;
    public float panelHpTime;
    private float currentValue;
    public Image thumbnail;
    public GameObject target=null;

    private void Update()
    {
        UpdatePanelHP();
    }
    void UpdatePanelHP()
    {
        if (isOnPanelHP && target != null)
        {
            currentValue = DecrementSliderValue(transform.GetChild(0).GetComponent<Slider>().value, GetCurrentHp() / GetMaxHp());
            transform.GetChild(0).GetComponent<Slider>().value = currentValue;
            transform.GetChild(1).GetComponentInChildren<Text>().text = string.Format("{0}/{1}", currentHp.ToString("N0"), currentMaxHp.ToString("N0"));
        }
    }
    public void OpenHpUI(GameObject targetObj)
    {
        target = targetObj;
        panelHpTime = 0.0f;
        currentValue = GetCurrentHp();
        transform.GetChild(0).GetComponent<Slider>().value = GetCurrentHp() / GetMaxHp();
        transform.GetChild(1).GetComponentInChildren<Text>().text = currentHp.ToString();
        string difficulty = Common.bossModeDifficulty == 0 ? "쉬움" : Common.bossModeDifficulty == 1 ? "보통" : "어려움";
        transform.GetChild(3).GetComponent<Text>().text = string.Format("{0} ({1})", HeroSystem.GetHeroName(targetObj.GetComponent<Hero>().id), difficulty);
        if (thumbnail != null)
            thumbnail.sprite = HeroSystem.GetHeroThumbnail(targetObj.GetComponent<Hero>().id);
        isOnPanelHP = true;
    }
    
    float GetCurrentHp()
    {
        if (target != null)
        {
            if (target.GetComponent<Hero>() != null)
            {
                currentHp = (float)target.GetComponent<Hero>().status.hp;
                return currentHp;
            }
            else
                return 1;
        }
        else
            return 1;
    }
    float GetMaxHp()
    {
        if (target != null)
        {
            if (target.GetComponent<Hero>() != null)
            {
                currentMaxHp = (float)target.GetComponent<Hero>().status.maxHp;
                return currentMaxHp;
            }
            else
                return 1;
        }
        else
            return 1;
    }

    float DecrementSliderValue(float n, float target)
    {
        if (target < n)
            n -= Time.deltaTime * 0.5f;
        return n;
    }
}
