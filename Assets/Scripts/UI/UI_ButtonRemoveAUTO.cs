using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ButtonRemoveAUTO : MonoBehaviour
{
    GameObject informationParent;
    Text passInformationText;
  
    private void Awake()
    {
        informationParent = transform.GetChild(1).gameObject;
        passInformationText = informationParent.GetComponentInChildren<Text>();
    }
    private void Start()
    {
        if(SaveSystem.IsPremiumPassAble())
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
        if(informationParent.activeSelf)
        {
            RefreshUI();
        }
    }
    void RefreshUI()
    {
        if (informationParent != null)
        {
            passInformationText.text = string.Format("<color='white'>{0}</color>\r\n<color='yellow'>({1})</color>\r\n\r\n{2}", LocalizationManager.GetText("PremiumPassInUse"), SaveSystem.getPremiumRemainingPeriodText(), SaveSystem.getPremiumBenefitsText());
        }
    }
    public void OnClickPremiumPassInfo()
    {
        bool isActive = informationParent.activeSelf;
        informationParent.SetActive(!isActive);
    }
}
