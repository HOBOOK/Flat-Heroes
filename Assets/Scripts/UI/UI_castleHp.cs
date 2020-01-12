using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_castleHp : MonoBehaviour
{
    private GameObject targetGameObject;
    float damage = 0;
    float currentHp = 0;
    float currentMaxHp = 0;
    private float hpBarSetpsLength = 10;
    private bool isOnPanelHP = false;
    public float panelHpTime;
    private float currentValue;
    private RectTransform sliderContainerRectTransform;
    private RectTransform imageRectTransform;
    private Image hpImage;
    private GameObject canvasUI;
    public GameObject target;

    private void OnEnable()
    {
        this.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }

    private void Awake()
    {
        InitPanelHp();
    }

    void InitPanelHp()
    {
        canvasUI = GameObject.Find("CanvasUI");
        sliderContainerRectTransform = GetComponent<RectTransform>();
        foreach (var mat in GetComponentsInChildren<Image>())
        {
            if (mat.name == "Fill")
                hpImage = mat;
        }
        imageRectTransform = hpImage.GetComponent<RectTransform>();
        hpImage.material = Instantiate(hpImage.material);
        target = null;
    }
    private void Update()
    {
        UpdatePanelHP();
    }
    void UpdatePanelHP()
    {
        if (isOnPanelHP && target != null)
        {
            if (target.GetComponent<Castle>() != null||target.GetComponent<TutorialCastle>()!=null)
                transform.position = target.transform.position + new Vector3(0, 2);
            else
                transform.position = target.transform.GetChild(0).position + new Vector3(0, 2);
            SetDamage();
            currentValue = DecrementSliderValue(transform.GetChild(1).GetComponent<Slider>().value, GetCurrentHp() / GetMaxHp());
            hpImage.material.SetFloat("_Percent", currentHp / currentMaxHp);
            transform.GetChild(0).GetComponentInChildren<Text>().text = currentHp.ToString();
            DisablePanelHP();
        }
    }
    public void OpenHpUI(GameObject targetObj, bool isBlue = false)
    {
        target = targetObj;
        transform.position = target.transform.position + new Vector3(0, 2);
        sliderContainerRectTransform.sizeDelta = new Vector2(180, 50);
        panelHpTime = 0.0f;
        currentValue = GetCurrentHp();
        transform.GetChild(1).GetComponent<Slider>().value = GetCurrentHp() / GetMaxHp();
        transform.GetChild(0).GetComponentInChildren<Text>().text = currentHp.ToString();
        if (isBlue)
            hpImage.material.SetColor("_Color", new Color(0.1f, 0.5f, 1));
        else
            hpImage.material.SetColor("_Color", new Color(1, 0.5f, 0.1f));

        hpImage.material.SetVector("_ImageSize", new Vector4(imageRectTransform.rect.size.x - 10, imageRectTransform.rect.size.y, 0, 0));
        hpBarSetpsLength = 7;
        hpImage.material.SetFloat("_Steps", hpBarSetpsLength);
        isOnPanelHP = true;
    }
    void DisablePanelHP()
    {
        if (target != null)
        {
            if(target.GetComponent<Castle>() != null)
            {
                if (target.GetComponent<Castle>().isDead || GetCurrentHp() <= 0)
                {
                    target = null;
                    panelHpTime = 0.0f;
                    isOnPanelHP = false;
                    ObjectPool.Instance.PushToPool("hpCastleUI", this.gameObject, canvasUI.transform);
                }
            }
            else if (target.GetComponent<Hero>() != null)
            {
                if (target.GetComponent<Hero>().isDead || GetCurrentHp() <= 0)
                {
                    target = null;
                    panelHpTime = 0.0f;
                    isOnPanelHP = false;
                    ObjectPool.Instance.PushToPool("hpCastleUI", this.gameObject, canvasUI.transform);
                }
            }
            else if (target.GetComponent<TutorialCastle>() != null)
            {
                if (target.GetComponent<TutorialCastle>().isDead || GetCurrentHp() <= 0)
                {
                    target = null;
                    panelHpTime = 0.0f;
                    isOnPanelHP = false;
                    ObjectPool.Instance.PushToPool("hpCastleUI", this.gameObject, canvasUI.transform);
                }
            }
        }

    }
    float GetCurrentHp()
    {
        if (target != null)
        {
            if (target.GetComponent<Castle>() != null)
            {
                currentHp = (float)target.GetComponent<Castle>().hp;
                return currentHp;
            }
            else if (target.GetComponent<Hero>() != null)
            {
                currentHp = (float)target.GetComponent<Hero>().status.hp;
                return currentHp;
            }
            else if (target.GetComponent<TutorialCastle>() != null)
            {
                currentHp = (float)target.GetComponent<TutorialCastle>().hp;
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
            if (target.GetComponent<Castle>() != null)
            {
                currentMaxHp = (float)target.GetComponent<Castle>().maxHp;
                return currentMaxHp;
            }
            else if (target.GetComponent<Hero>() != null)
            {
                currentMaxHp = (float)target.GetComponent<Hero>().status.maxHp;
                return currentMaxHp;
            }
            else if (target.GetComponent<TutorialCastle>() != null)
            {
                currentMaxHp = (float)target.GetComponent<TutorialCastle>().maxHp;
                return currentMaxHp;
            }
            else
                return 1;
        }
        else
            return 1;
    }
    public void GetDamage(int dam)
    {
        damage += dam;
    }
    public void SetDamage()
    {
        if (currentHp > 0)
        {
            damage -= Time.deltaTime * currentMaxHp * 0.2f;
            if (damage < 0)
                damage = 0;
            hpImage.material.SetFloat("_DamagesPercent", damage / currentMaxHp);

        }
    }

    float DecrementSliderValue(float n, float target)
    {
        if (target < n)
            n -= Time.deltaTime * 0.5f;
        return n;
    }
}
