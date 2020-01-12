using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TabManager : MonoBehaviour
{
    List<GameObject> tabList = new List<GameObject>();
    List<GameObject> buttonList = new List<GameObject>();

    public GameObject PanelTabs;
    public GameObject PanelButtons;

    Vector2 buttonRectSizeDelta;

    private void Awake()
    {
        if(PanelTabs!=null)
        {
            for(var i = 0; i < PanelTabs.transform.childCount; i++)
            {
                tabList.Add(PanelTabs.transform.GetChild(i).gameObject);
            }
        }
        if (PanelButtons != null)
        {
            for (var i = 0; i < PanelButtons.transform.childCount; i++)
            {
                buttonList.Add(PanelButtons.transform.GetChild(i).gameObject);
            }
        }
        buttonRectSizeDelta = buttonList[0].GetComponent<RectTransform>().sizeDelta;
    }
    private void OnEnable()
    {
        if(tabList != null)
        {
            foreach(var i in tabList)
            {
                i.SetActive(false);
            }
            tabList[0].SetActive(true);
        }
        if(buttonList!=null)
        {
            foreach (var i in buttonList)
            {
                i.GetComponent<Button>().interactable = true;
                i.transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(false);
                i.GetComponent<RectTransform>().sizeDelta = buttonRectSizeDelta;
            }
            buttonList[0].transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(true);
            buttonList[0].GetComponent<RectTransform>().sizeDelta = new Vector2(buttonRectSizeDelta.x, 110);
        }
    }

    public void OnTabButtonClick(int tabNumber)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        if(tabList!=null && tabList.Count>0)
        {
            for(int i = 0; i<tabList.Count; i++)
            {
                if(i==tabNumber)
                {
                    tabList[i].SetActive(true);
                }
                else
                {
                    tabList[i].SetActive(false);
                }
            }
        }
        if(buttonList!=null && buttonList.Count>0)
        {
            for(int i = 0; i < buttonList.Count; i++)
            {
                if(i==tabNumber)
                {
                    buttonList[i].transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(true);
                    buttonList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(buttonRectSizeDelta.x, 110);
                }
                else
                {
                    buttonList[i].GetComponent<RectTransform>().sizeDelta = buttonRectSizeDelta;
                    buttonList[i].transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(false);
                }
            }
        }
    }
}
