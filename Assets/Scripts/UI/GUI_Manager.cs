using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_Manager : MonoBehaviour
{
    // Panel Prefab//
    public GameObject PanelCastleHP;
    public GameObject PanelChapter;
    public GameObject PanelToolTip;
    // Panel Prefab//

    // Castle UI //
    private GameObject targetGameObject;
    float damage = 0;
    float currentHp = 0;
    float currentMaxHp = 0;
    private float hpBarSetpsLength = 10;
    private bool isOnPanelHP = false;
    private float panelHpTime;
    private float currentValue;
    private RectTransform sliderContainerRectTransform;
    private RectTransform imageRectTransform;
    private Image hpImage;
    private GameObject canvasUI;
    // Castle UI //

    public static GUI_Manager instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        if(PanelChapter!=null)
            PanelChapter.SetActive(false);
        if(PanelToolTip!=null)
            PanelToolTip.SetActive(false);
        InitPanelHp();
    }
    void InitPanelHp()
    {
        if(PanelCastleHP!=null)
        {
            canvasUI = GameObject.Find("CanvasUI");
            sliderContainerRectTransform = PanelCastleHP.GetComponent<RectTransform>();
            foreach (var mat in PanelCastleHP.GetComponentsInChildren<Image>())
            {
                if (mat.name == "Fill")
                    hpImage = mat;
            }
            imageRectTransform = hpImage.GetComponent<RectTransform>();
            hpImage.material = Instantiate(hpImage.material);

            PanelCastleHP.SetActive(false);

        }
    }
    private void Update()
    {
        UpdatePanelHP();
    }
    void UpdatePanelHP()
    {
        if (isOnPanelHP && Common.hitTargetObject != null)
        {
            PanelCastleHP.transform.position = Common.hitTargetObject.transform.position + new Vector3(0, 2);
            SetDamage();
            currentValue = DecrementSliderValue(PanelCastleHP.transform.GetChild(0).GetComponent<Slider>().value, GetCurrentHp() / GetMaxHp());
            PanelCastleHP.transform.GetChild(0).GetComponent<Slider>().value = currentValue;
            PanelCastleHP.transform.GetChild(1).GetComponentInChildren<Text>().text = currentHp.ToString();
            DisablePanelHP();
        }
    }
    public void OpenHpUI(GameObject target, bool isBlue = false)
    {
        if (Common.hitTargetObject == null || Common.hitTargetObject != target)
            Common.hitTargetObject = target;
        sliderContainerRectTransform.sizeDelta = new Vector2(Mathf.Clamp(GetMaxHp() * 0.01f, 250, 300), 80);
        panelHpTime = 0.0f;
        currentValue = GetCurrentHp();
        PanelCastleHP.transform.GetChild(0).GetComponent<Slider>().value = GetCurrentHp() / GetMaxHp();
        PanelCastleHP.transform.GetChild(1).GetComponentInChildren<Text>().text = currentHp.ToString();
        if (isBlue)
            hpImage.material.SetColor("_Color", new Color(0, 0.4f, 1));
        else
            hpImage.material.SetColor("_Color", new Color(1, 0.4f, 0));

        hpImage.material.SetVector("_ImageSize", new Vector4(imageRectTransform.rect.size.x - 10, imageRectTransform.rect.size.y, 0, 0));
        hpBarSetpsLength = (currentMaxHp * 0.01f) > 10 ? 10 : (currentMaxHp * 0.01f);
        hpImage.material.SetFloat("_Steps", hpBarSetpsLength);
        isOnPanelHP = true;
        PanelCastleHP.SetActive(true);
    }

    void DisablePanelHP()
    {
        if (Common.hitTargetObject != null && Common.hitTargetObject.GetComponent<Castle>() != null)
        {
            if (Common.hitTargetObject.GetComponent<Castle>().isDead || GetCurrentHp() <= 0)
            {
                Common.hitTargetObject = null;
                panelHpTime = 0.0f;
                isOnPanelHP = false;
                PanelCastleHP.SetActive(false);
            }

        }
    }

    float GetCurrentHp()
    {
        if (Common.hitTargetObject != null)
        {
            if (Common.hitTargetObject.GetComponent<Castle>() != null)
            {
                currentHp = (float)Common.hitTargetObject.GetComponent<Castle>().hp;
                hpImage.material.SetFloat("_Percent", currentHp / currentMaxHp);
                return currentHp;
            }
            else if(Common.hitTargetObject.GetComponent<Boss>()!=null)
            {
                currentHp = (float)Common.hitTargetObject.GetComponent<Boss>().hp;
                hpImage.material.SetFloat("_Percent", currentHp / currentMaxHp);
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
        if (Common.hitTargetObject != null)
        {
            if (Common.hitTargetObject.GetComponent<Castle>() != null)
            {
                currentMaxHp = (float)Common.hitTargetObject.GetComponent<Castle>().maxHp;
                return currentMaxHp;
            }
            else if (Common.hitTargetObject.GetComponent<Boss>() != null)
            {
                currentMaxHp = (float)Common.hitTargetObject.GetComponent<Boss>().maxHp;
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

    public void ToolTipOn(string text)
    {
        PanelToolTip.gameObject.SetActive(false);
        PanelToolTip.GetComponent<GUI_ToolTip>().text = text;
        PanelToolTip.gameObject.SetActive(true);
    }
}
