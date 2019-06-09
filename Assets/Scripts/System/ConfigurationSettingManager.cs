using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationSettingManager : MonoBehaviour
{
    public GameObject SoundSettingPanel;
    public GameObject LanguageSettingPanel;
    public GameObject CloudSettingPanel;

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
}
