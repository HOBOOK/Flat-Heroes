using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance = null;
    public GameObject Title;
    public GameObject CoverUI;
    public GameObject PopHeroInfoSummaryUI;
    public GameObject PopHeroInfoUI;
    public GameObject PopGetAbilityUI;
    public GameObject PopupInterActiveCover;
    public GameObject PopupAlertUI;

    public enum PopupAlertTYPE { energy,scroll,coin,blackCrystal}

    GameObject popupPanel;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
        if (popupPanel == null)
            popupPanel = GameObject.FindWithTag("PopupPanel") as GameObject;
    }
    private void OnEnable()
    {
        if (Camera.main.GetComponent<BlurOptimized>()!=null)
            Camera.main.GetComponent<BlurOptimized>().enabled = false;
        if (Title!=null)
        {
            if (Title.GetComponent<AiryUIAnimatedElement>() != null)
                Title.GetComponent<AiryUIAnimatedElement>().ShowElement();
            else
                Title.gameObject.SetActive(true);
        }
    }

    public GameObject GetPopupPanel(string popupName)
    {
        GameObject popupPanel = GameObject.FindWithTag("PopupPanel") as GameObject;
        if (popupPanel != null && popupPanel.transform.childCount > 0)
        {
            for(var i = 0; i < popupPanel.transform.childCount; i++)
            {
                if (popupPanel.transform.GetChild(i).name.Replace("PanelPop", "").Equals(popupName))
                {
                    Debugging.Log("Get Panel POP Succeed. ==> " + popupPanel.transform.GetChild(i).name);
                    return popupPanel.transform.GetChild(i).gameObject;
                }
            }
        }
        Debugging.Log("Panel POP! ==> " + popupName + " is NULL ");
        return null;
    }

    public void OpenPopupPanel(string popupName)
    {
        if(popupPanel==null)
            popupPanel = GameObject.FindWithTag("PopupPanel") as GameObject;
        GameObject popupObject = null;
        if (popupPanel != null && popupPanel.transform.childCount > 0)
        {
            for (var i = 0; i < popupPanel.transform.childCount; i++)
            {
                if (popupPanel.transform.GetChild(i).name.Replace("PanelPop", "").Equals(popupName))
                {
                    popupObject = popupPanel.transform.GetChild(i).gameObject;
                    break;
                }
            }
        }
        if(popupObject!=null&&popupObject.GetComponentInChildren<AiryUIAnimatedElement>()!=null)
        {
            popupObject.gameObject.SetActive(true);
            showUIanimation(popupObject);
        }
    }
    public void OpenEndGamePanel(bool isWin)
    {
        if (popupPanel == null)
            popupPanel = GameObject.FindWithTag("PopupPanel") as GameObject;
        GameObject popupObject = null;
        if (popupPanel != null && popupPanel.transform.childCount > 0)
        {
            for (var i = 0; i < popupPanel.transform.childCount; i++)
            {
                if (popupPanel.transform.GetChild(i).name.Equals("PanelPopEndGame"))
                {
                    popupObject = popupPanel.transform.GetChild(i).gameObject;
                    break;
                }
            }
        }
        if (popupObject != null && popupObject.GetComponentInChildren<AiryUIAnimatedElement>() != null)
        {
            if(Camera.main.GetComponent<BlurOptimized>()!=null)
                Camera.main.GetComponent<BlurOptimized>().enabled = true;
            GameObject information = popupObject.transform.GetChild(2).gameObject;
            if(isWin)
            {
                information.transform.GetChild(0).gameObject.SetActive(true);
                information.transform.GetChild(1).GetComponent<Text>().text = "이겼습니다 !!";
            }
            else
            {
                information.transform.GetChild(0).gameObject.SetActive(false);
                information.transform.GetChild(1).GetComponent<Text>().text = "...패배하였습니다.";
            }
            popupObject.gameObject.SetActive(true);
            showUIanimation(popupObject);
        }
    }
    public void CloseAllPopupPanel()
    {
        GameObject popupPanel = GameObject.FindWithTag("PopupPanel") as GameObject;

        if (popupPanel != null && popupPanel.transform.childCount > 0)
        {
            for (var i = 0; i < popupPanel.transform.childCount; i++)
            {
                if (popupPanel.transform.GetChild(i).gameObject.activeSelf)
                {
                    if (popupPanel.transform.GetChild(i).GetComponent<AiryUIAnimatedElement>() != null)
                        popupPanel.transform.GetChild(i).GetComponent<AiryUIAnimatedElement>().HideElement();
                    else
                        popupPanel.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            if (Camera.main.GetComponent<BlurOptimized>() != null)
            {
                Camera.main.GetComponent<BlurOptimized>().enabled = false;
            }
        }
    }

    public void CoverFadeIn()
    {
        if(CoverUI!=null)
            StartCoroutine(FadeIn(CoverUI.GetComponent<Image>()));
    }
    IEnumerator FadeIn(Image image)
    {
        if (image == null)
            yield return null;
        float cnt = 0;
        Color color = image.color;
        while(cnt<1)
        {
            image.color = new Color(color.r, color.g, color.b, cnt);
            yield return new WaitForEndOfFrame();
            cnt += 0.01f;
        }
        yield return null;
    }
    public int GetPopupPanelCount()
    {
        int cnt = 0;
        if (popupPanel!=null)
        {
            for(int i = 0; i < popupPanel.transform.childCount; i++)
            {
                if (popupPanel.transform.GetChild(i).gameObject.activeSelf)
                    cnt += 1;
            }
        }
        return cnt;
    }

    public void PopupGetAbility(Ability ability)
    {
        if (PopGetAbilityUI != null)
        {
            PopGetAbilityUI.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Ability/" + ability.image);
            PopGetAbilityUI.transform.GetChild(1).GetComponent<Text>().text = ability.name;
            showUIanimation(PopGetAbilityUI);
        }
        else
            Debugging.Log("어빌리티 팝업창이 없습니다.");
    }

    void showUIanimation(GameObject obj)
    {
        if (obj.GetComponentsInChildren<AiryUIAnimatedElement>() != null)
        {
            foreach (var element in obj.GetComponentsInChildren<AiryUIAnimatedElement>())
                element.ShowElement();
        }
    }
    void hideUIanimation(GameObject obj)
    {
        if (obj.GetComponentsInChildren<AiryUIAnimatedElement>() != null)
        {
            foreach (var element in obj.GetComponentsInChildren<AiryUIAnimatedElement>())
                element.HideElement();
        }
    }
    public void ShowGetAlert(string spritePath, string txt)
    {
        if (PopupAlertUI != null)
        {
            foreach (Transform otherUI in PopupAlertUI.transform.GetChild(0).transform)
            {
                otherUI.gameObject.SetActive(false);
            }
            GameObject getAlertPanel = PopupAlertUI.transform.GetChild(0).GetChild(1).gameObject;
            getAlertPanel.gameObject.SetActive(true);
            Image getImage = getAlertPanel.transform.GetChild(0).GetComponent<Image>();
            Text getText = getAlertPanel.transform.GetChild(1).GetComponent<Text>();
            getImage.sprite = Resources.Load<Sprite>(spritePath);
            getText.text = txt;
            PopupAlertUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
        }
    }
    public void ClosePopupAlertUI()
    {
        PopupAlertUI.gameObject.SetActive(false);
    }

    public GameObject ShowNeedAlert(string spritePath, string alertText)
    {
        if (PopupAlertUI != null)
        {
            foreach(Transform otherUI in PopupAlertUI.transform.GetChild(0).transform)
            {
                otherUI.gameObject.SetActive(false);
            }
            GameObject getAlertPanel = PopupAlertUI.transform.GetChild(0).GetChild(2).gameObject;
            getAlertPanel.gameObject.SetActive(true);
            Image getImage = getAlertPanel.transform.GetChild(0).GetComponent<Image>();
            Text getText = getAlertPanel.transform.GetChild(1).GetComponent<Text>();
            getImage.sprite = Resources.Load<Sprite>(spritePath);
            getText.text = alertText;
            getAlertPanel.GetComponentInChildren<UI_CheckButton>().isChecking = false;
            PopupAlertUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
            return getAlertPanel;
        }
        return null;
    }

    public void ShowAlert(PopupAlertTYPE alertType, int needAmount)
    {
        if(PopupAlertUI!=null)
        {
            foreach (Transform otherUI in PopupAlertUI.transform.GetChild(0).transform)
            {
                otherUI.gameObject.SetActive(false);
            }
            GameObject ShowtageAlertPanel = PopupAlertUI.transform.GetChild(0).GetChild(0).gameObject;
            ShowtageAlertPanel.gameObject.SetActive(true);
            Image shortageImage = ShowtageAlertPanel.transform.GetChild(0).GetComponent<Image>();
            Text shortageText = ShowtageAlertPanel.transform.GetChild(1).GetComponent<Text>();
            switch (alertType)
            {
                case PopupAlertTYPE.energy:
                    shortageImage.sprite = Resources.Load<Sprite>("Items/portalEnergy");
                    shortageText.text = string.Format("전장으로 이동할 포탈에너지가 부족합니다. \r\n <color='red;>필요 : {0}</color> 현재 : {1}", needAmount, User.portalEnergy);
                    break;
                case PopupAlertTYPE.scroll:
                    shortageImage.sprite = Resources.Load<Sprite>("Items/abilityScroll");
                    shortageText.text = string.Format("주문서가 부족합니다. \r\n <color='red;>필요 : {0}</color> 현재 : {1}", needAmount, 0);
                    break;
                case PopupAlertTYPE.coin:
                    shortageImage.sprite = Resources.Load<Sprite>("Items/coin");
                    shortageText.text = string.Format("코인이 부족합니다. \r\n <color='red;>필요 : {0}</color> 현재 : {1}", needAmount, User.coin);
                    break;
                case PopupAlertTYPE.blackCrystal:
                    shortageImage.sprite = Resources.Load<Sprite>("Items/blackCrystal");
                    shortageText.text = string.Format("수정이 부족합니다. \r\n <color='red;>필요 : {0}</color> 현재 : {1}", needAmount, User.blackCrystal);
                    break;
            }
            PopupAlertUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
        }
    }


}
