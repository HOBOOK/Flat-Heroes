using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UI_hp : MonoBehaviour
{
    [HideInInspector]
    public GameObject Target = null;
    bool isOnPanelHP = false;
    float damage = 0;
    float currentValue = 0;
    public float panelHpTime = 0;
    float currentHp = 0;
    float currentMaxHp = 0;
    private GameObject canvasUI;
    private RectTransform sliderContainerRectTransform;
    private RectTransform sliderRectTransform;
    private RectTransform imageRectTransform;
    public GameObject levelUI;
    [SerializeField]
    public Image hpImage;
    [SerializeField]
    private float hpBarSetpsLength = 10;

    private void OnEnable()
    {
        this.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }
    private void Awake()
    {
        canvasUI = GameObject.Find("CanvasUI");
        levelUI = this.transform.GetChild(2).gameObject;
        sliderContainerRectTransform = GetComponent<RectTransform>();
        sliderRectTransform = GetComponentInChildren<Slider>().gameObject.GetComponent<RectTransform>();
        imageRectTransform = hpImage.GetComponent<RectTransform>();
        hpImage.material = Instantiate(hpImage.material);
    }

    // Update is called once per frame
    void Update ()
    {
        if (isOnPanelHP&&Target!=null)
        {
            this.transform.position = Target.transform.GetChild(0).position+ new Vector3(0, 0.5f + (Target.transform.localScale.y)) ;
            SetDamage();
            currentValue = DecrementSliderValue(transform.GetChild(0).GetComponent<Slider>().value, GetCurrentHp() / GetMaxHp());
            transform.GetChild(0).GetComponent<Slider>().value = currentValue;
            //HidePanelHP();
            DisablePanelHP();
        }
    }
    public void OpenHpUI(GameObject target, bool isBlue = false)
    {
        if (Target == null||Target != target)
            Target = target;
        this.name = string.Format("HP_BAR_OF_{0}", Target.name);
        this.transform.position = Target.transform.GetChild(0).position;
        sliderContainerRectTransform.sizeDelta = new Vector2(Mathf.Clamp(GetMaxHp()*0.01f, 120, 200), 70);
        panelHpTime = 0.0f;
        levelUI.GetComponentInChildren<Text>().text = Target.GetComponent<Hero>().status.level.ToString();
        levelUI.GetComponentInChildren<Slider>().value = (float)Target.GetComponent<Hero>().status.exp / (float)Common.GetHeroNeedExp(Target.GetComponent<Hero>().status.level);
        currentValue = GetCurrentHp();
        transform.GetChild(0).GetComponent<Slider>().value = GetCurrentHp() / GetMaxHp();
        transform.GetChild(1).GetComponent<Text>().text = Target.name;
        if (isBlue)
            hpImage.material.SetColor("_Color", new Color(0,0.4f,1));
        else
            hpImage.material.SetColor("_Color", new Color(1, 0.4f, 0));

        hpImage.material.SetVector("_ImageSize", new Vector4(imageRectTransform.rect.size.x-10, imageRectTransform.rect.size.y, 0, 0));
        hpBarSetpsLength = (currentMaxHp *0.01f) > 10 ? 10 : (currentMaxHp *0.01f);
        hpImage.material.SetFloat("_Steps", hpBarSetpsLength);
        isOnPanelHP = true;
    }

    void DisablePanelHP()
    {
        if(Target!=null&&Target.GetComponent<Hero>()!=null)
        {
            if(Target.GetComponent<Hero>().isDead || GetCurrentHp() <= 0)
            {
                Target = null;
                panelHpTime = 0.0f;
                isOnPanelHP = false;
                ObjectPool.Instance.PushToPool("hpEnemyUI", this.gameObject, canvasUI.transform);
            }

        }
    }

    void HidePanelHP()
    {
        panelHpTime += Time.deltaTime;
        if (panelHpTime > 3.0)
        {
            panelHpTime = 0;
            gameObject.SetActive(false);
        }
    }


    float GetCurrentHp()
    {
        if (Target != null)
        {
            if (Target.GetComponent<Hero>() != null)
            {
                currentHp = (float)Target.GetComponent<Hero>().status.hp;
                hpImage.material.SetFloat("_Percent", currentHp/currentMaxHp);
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
        if (Target != null)
        {
            if (Target.GetComponent<Hero>() != null)
            {
                currentMaxHp = (float)Target.GetComponent<Hero>().status.maxHp;
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
        if(currentHp>0)
        {
            damage -= Time.deltaTime * currentMaxHp*0.2f;
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
