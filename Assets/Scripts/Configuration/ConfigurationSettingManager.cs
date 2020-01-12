using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationSettingManager : MonoBehaviour
{
    public GameObject SoundSettingPanel;
    public GameObject LanguageSettingPanel;
    public GameObject CloudSettingPanel;
    public Text UserDeviceIdentifierText;
    private void OnEnable()
    {
        EnableUI();
    }

    public void OnSoundBGMToggleClick(Toggle toggle)
    {
        if(toggle.isOn)
        {
            SoundManager.instance.BgmOnOff(false);
            Debugging.Log("BGM 켬");
        }
        else
        {
            SoundManager.instance.BgmOnOff(true);
            Debugging.Log("BGM 끔");
        }
    }
    public void OnSoundEFFECTToggleClick(Toggle toggle)
    {
        if (toggle.isOn)
        {
            SoundManager.instance.EffectOnOff(false);
            Debugging.Log("EFXM 켬");
        }
        else
        {
            SoundManager.instance.EffectOnOff(true);
            Debugging.Log("EFXM 끔");
        }
    }
    void EnableUI()
    {
        UserDeviceIdentifierText.text = string.Format("{0} : {1}\r\n{2} : {3}", LocalizationManager.GetText("configPlayerIdText"), User.id, LocalizationManager.GetText("configGameVersionText"), Application.version);

        // 언어설정
        foreach (var btn in LanguageSettingPanel.transform.GetComponentsInChildren<Button>())
        {
            if (btn.name.Equals(User.language) || btn.name == User.language)
            {
                btn.enabled = false;
                btn.transform.GetChild(0).GetChild(0).GetComponent<Image>().gameObject.SetActive(true);
            }
            else
            {
                btn.enabled = true;
                btn.transform.GetChild(0).GetChild(0).GetComponent<Image>().gameObject.SetActive(false);
            }
        }
    }
    void RefreshUI()
    {
        UserDeviceIdentifierText.text = string.Format("{0} : {1}\r\n{2} : {3}", LocalizationManager.GetText("configPlayerIdText"), User.id, LocalizationManager.GetText("configGameVersionText"), Application.version);

        foreach (var btn in LanguageSettingPanel.transform.GetComponentsInChildren<Button>())
        {
            if (btn.name.Equals(User.language) || btn.name == User.language)
            {
                btn.enabled = false;
                btn.transform.GetChild(0).GetChild(0).GetComponent<Image>().gameObject.SetActive(true);
            }
            else
            {
                btn.enabled = true;
                btn.transform.GetChild(0).GetChild(0).GetComponent<Image>().gameObject.SetActive(false);
            }
        }
        LocalizationManager.RedrawLanguage();
    }

    public void OnLanguageButtonClick(string lang)
    {
        SaveSystem.ChangeLanguage(lang);
        LocalizationManager.LoadLanguage(lang);
        RefreshUI();
    }
}
