using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoStageManager : MonoBehaviour
{
    public GameObject PanelAuto;
    private static AutoStageManager _instance = null;
    public static AutoStageManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<AutoStageManager>();
            return _instance;
        }
    }
    Hero currentSkillHero;
    bool isSkillCasting;
    bool isReadyAutoOff;

    void Start()
    {
        RefreshUI();
    }
    void Update()
    {
        ShowAutoUI();   
    }

    void ShowAutoUI()
    {
        if(PanelAuto!=null)
        {
            if(Common.IsAutoStagePlay&&!PanelAuto.activeSelf)
            {
                PanelAuto.SetActive(true);
            }
            else if(!Common.IsAutoStagePlay&&PanelAuto.activeSelf)
            {
                PanelAuto.SetActive(false);
            }
        }
    }
    void RefreshUI()
    {
        if(PanelAuto!=null)
        {
            if(isReadyAutoOff)
            {
                PanelAuto.transform.GetChild(1).gameObject.SetActive(true);
                PanelAuto.transform.GetChild(2).GetChild(0).GetComponent<Text>().color = Color.red;
                PanelAuto.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "OFF";
            }
            else
            {
                PanelAuto.transform.GetChild(1).gameObject.SetActive(false);
                PanelAuto.transform.GetChild(2).GetChild(0).GetComponent<Text>().color = Color.black;
                PanelAuto.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "ON";
            }
        }
    }
    public void AutoReadyOff()
    {
        if (PanelAuto != null)
        {
            Common.IsAutoStagePlay = false;
            PanelAuto.SetActive(false);
        }
    }
    public bool GetIsReadyAutoOff
    {
        get
        {
            return isReadyAutoOff;
        }
    }

    public void OnClickAutoOff()
    {
        isReadyAutoOff = !isReadyAutoOff;
        RefreshUI();
    }
}
