using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManagement : MonoBehaviour
{
    public Transform MapTransform;
    public GameObject postNotation;
    
    private void Start()
    {
        Time.timeScale = 1.0f;
        //SetLobbyMap();
        CharactersManager.instance.SetLobbyPositionHeros();
        SoundManager.instance.BgmSourceChange(AudioClipManager.instance.LobbyBgm);
        SkillSystem.SetObtainPlayerSkill();
        ModeUnLockAlert();
        PostCheck();
    }
    void ModeUnLockAlert()
    {
        if(PlayerPrefs.GetInt("BossMode") != 1)
        {
            Map bossModeMap = MapSystem.userMaps.Find(x => x.id > 10);
            if (bossModeMap != null)
            {
                PlayerPrefs.SetInt("BossMode", 1);
                UI_Manager.instance.ShowGetAlert("", LocalizationManager.GetText("alertUnlockBossMode"));
            }
        }
        if(PlayerPrefs.GetInt("InfinityMode") != 1)
        {
            if(User.level >= 8)
            {
                PlayerPrefs.SetInt("InfinityMode", 1);
                UI_Manager.instance.ShowGetAlert("", LocalizationManager.GetText("alertUnlockInfinityMode"));
            }
        }
    }
    public void PostCheck()
    {
        if(!string.IsNullOrEmpty(User.postItems))
        {
            postNotation.SetActive(true);
        }
        else
        {
            postNotation.SetActive(false);
        }
    }

    //void SetLobbyMap()
    //{
    //    string currentTime = DateTime.Now.ToString("hh");
    //    Debugging.Log(currentTime);

    //    if(true)
    //    {
    //        MapSystem.SetLobbySprite(true, ref MapTransform);
    //    }
    //    else
    //    {
    //        MapSystem.SetLobbySprite(false, ref MapTransform);
    //    }
    //}
}
