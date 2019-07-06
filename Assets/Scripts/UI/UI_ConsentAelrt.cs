using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ConsentAelrt : MonoBehaviour
{
    private bool isServiceConsent = false;
    private bool isPrivacyConsent = false;


    public Button serviceButton;
    public Button privacyButton;

    private void OnEnable()
    {
        isServiceConsent = false;
        isPrivacyConsent = false;
        SetConsentButton(serviceButton,isServiceConsent);
        SetConsentButton(privacyButton, isPrivacyConsent);
    }

    void SetConsentButton(Button btn, bool isConsent)
    {
        if(isConsent)
        {
            btn.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 0.9f);
            btn.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0.9f);
        }
        else
        {
            btn.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 0.2f);
            btn.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
        }
    }

    public void OnClickServiceConsent()
    {
        isServiceConsent = isServiceConsent ? false : true;
        SetConsentButton(serviceButton, isServiceConsent);
        OnCompletedConsent();

    }
    public void OnClickPrivacyConsent()
    {
        isPrivacyConsent = isPrivacyConsent ? false : true;
        SetConsentButton(privacyButton, isPrivacyConsent);
        OnCompletedConsent();
    }

    public void OnCompletedConsent()
    {
        if(isServiceConsent&&isPrivacyConsent)
        {
            Debugging.Log("서비스 및 개인정보 약관동의 완료");
            PlayerPrefs.SetInt("ServiceConsent", 1);
            GoogleSignManager.Instance.Init();
            this.gameObject.SetActive(false);
        }
    }
}
