﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnityAdsButton : MonoBehaviour
{
    public double msToWait;
    private Text gachaTimeText;
    private Button gachaButton;
    private UI_Button buttonScript;
    private ulong lastGachaOpen;

    private void Start()
    {
        gachaButton = GetComponent<Button>();
        gachaTimeText = GetComponentInChildren<Text>();
        buttonScript = GetComponent<UI_Button>();
        if(PlayerPrefs.HasKey("LastGachaOpen"))
        {
            lastGachaOpen = ulong.Parse(PlayerPrefs.GetString("LastGachaOpen"));
        }
        else
        {
            lastGachaOpen = (ulong)DateTime.Now.Ticks;
            PlayerPrefs.SetString("LastGachaOpen", lastGachaOpen.ToString());
        }


        if(!IsGachaReady())
        {
            gachaButton.interactable = false;
            buttonScript.enabled = false;
        }
    }

    private void Update()
    {
        if(!gachaButton.IsInteractable())
        {
            if(IsGachaReady())
            {
                gachaButton.interactable = true;
                buttonScript.enabled = true;
                return;
            }

            ulong diff = ((ulong)DateTime.Now.Ticks - lastGachaOpen);
            ulong m = diff / TimeSpan.TicksPerMillisecond;
            double secondsLeft = (double)(msToWait - m) / 1000.0f;

            string r = "";
            r += ((int)secondsLeft / 3600).ToString() + ": ";
            secondsLeft -= ((int)secondsLeft / 3600) * 3600;
            r += ((int)secondsLeft / 60).ToString("00") + ": ";
            r += (secondsLeft % 60).ToString("00");
            gachaTimeText.text = r;
        }
    }
    public void  GachaClick()
    {
        lastGachaOpen = (ulong)DateTime.Now.Ticks;
        PlayerPrefs.SetString("LastGachaOpen", lastGachaOpen.ToString());
        gachaButton.interactable = false;
        buttonScript.enabled = false;

    }
    private bool IsGachaReady()
    {
        ulong diff = ((ulong)DateTime.Now.Ticks - lastGachaOpen);
        ulong m = diff / TimeSpan.TicksPerMillisecond;
        double secondsLeft = (double)(msToWait - m) / 1000.0f;

        if (secondsLeft < 0)
        {
            gachaTimeText.text = "Free Ad";
            return true;
        }
        return false;
    }
}