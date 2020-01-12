using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UI_hp : MonoBehaviour
{
    [HideInInspector]
    public GameObject Target = null;
    Hero.Status targetStatus;
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
    private float hpBarSetpsLength = 4;

    private void OnEnable()
    {
        this.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }
    private void Awake()
    {
        canvasUI = GameObject.Find("CanvasUI");
        levelUI = this.transform.GetChild(1).gameObject;
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
            hpImage.material.SetFloat("_Percent", GetCurrentHp() / GetMaxHp());

            //transform.GetChild(0).GetComponent<Slider>().value = currentValue;
            //HidePanelHP();
            DisablePanelHP();
        }
    }
    public void OpenHpUI(GameObject target, bool isBlue = false)
    {
        if (Target == null||Target != target)
        {
            Target = target;
            if (target.GetComponent<Hero>() != null)
                targetStatus = target.GetComponent<Hero>().status;
            else if (target.GetComponent<TutorialHero>() != null)
                targetStatus = target.GetComponent<TutorialHero>().status;
        }

        this.name = string.Format("HP_BAR_OF_{0}", Target.name);
        this.transform.position = Target.transform.GetChild(0).position;
        sliderContainerRectTransform.sizeDelta = new Vector2(80, 30);
        panelHpTime = 0.0f;
        SetLevelUI(targetStatus.level);
        if (targetStatus.level >= 100)
        {
            levelUI.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        else
        {
            levelUI.transform.GetChild(0).GetComponent<Image>().color = Color.black;
        }
        if (isBlue)
            hpImage.material.SetColor("_Color", new Color(0.25f,0.5f,1f));
        else
            hpImage.material.SetColor("_Color", new Color(1f, 0.5f, 0.25f));

        hpImage.material.SetVector("_ImageSize", new Vector4(70, 18, 0, 0));
        hpBarSetpsLength = 4;
        hpImage.material.SetFloat("_Steps", hpBarSetpsLength);
        isOnPanelHP = true;
    }
    public void SetLevelUI(int level)
    {
        Text levelText = levelUI.GetComponentInChildren<Text>();
        levelText.text = level.ToString();
    }

    void DisablePanelHP()
    {
        if(Target!=null&& targetStatus != null)
        {
            if(GetCurrentHp() <= 0)
            {
                Target = null;
                panelHpTime = 0.0f;
                isOnPanelHP = false;
                ObjectPool.Instance.PushToPool("hpEnemyUI", this.gameObject, canvasUI.transform);
            }

        }
    }

    public void ForceDisableUI()
    {
        Target = null;
        panelHpTime = 0.0f;
        isOnPanelHP = false;
        ObjectPool.Instance.PushToPool("hpEnemyUI", this.gameObject, canvasUI.transform);
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
            if (targetStatus != null)
            {
                currentHp = (float)targetStatus.hp;
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
            if (targetStatus != null)
            {
                currentMaxHp = (float)targetStatus.maxHp;
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
            damage -= Time.deltaTime * currentMaxHp*0.3f;
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
