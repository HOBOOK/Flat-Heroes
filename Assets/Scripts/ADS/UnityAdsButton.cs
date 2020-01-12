using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnityAdsButton : MonoBehaviour
{
    public double msToWait;
    public Text gachaTimeText;
    public Button gachaButton;
    public GameObject lockPanel;

    private ulong lastGachaOpen;
    public int type;
    private void Awake()
    {
        if (gachaButton == null)
            gachaButton = GetComponent<Button>();
        if (gachaTimeText == null)
            gachaTimeText = lockPanel.GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        if (type == 1)
        {
            if (PlayerPrefs.HasKey("LastGoldOpen"))
            {
                lastGachaOpen = ulong.Parse(PlayerPrefs.GetString("LastGoldOpen"));
            }
            else
            {
                lastGachaOpen = (ulong)0;
                PlayerPrefs.SetString("LastGoldOpen", "0");
            }
        }
        else if (type == 2)
        {
            if (PlayerPrefs.HasKey("LastCrystalOpen"))
            {
                lastGachaOpen = ulong.Parse(PlayerPrefs.GetString("LastCrystalOpen"));
            }
            else
            {
                lastGachaOpen = (ulong)0;
                PlayerPrefs.SetString("LastCrystalOpen", "0");
            }
        }
        else if (type == 3)
        {
            if (PlayerPrefs.HasKey("LastEnergyOpen"))
            {
                lastGachaOpen = ulong.Parse(PlayerPrefs.GetString("LastEnergyOpen"));
            }
            else
            {
                lastGachaOpen = (ulong)0;
                PlayerPrefs.SetString("LastEnergyOpen", "0");
            }
        }
        else if (type == 4)
        {
            if (PlayerPrefs.HasKey("LastGachaOpen"))
            {
                lastGachaOpen = ulong.Parse(PlayerPrefs.GetString("LastGachaOpen"));
            }
            else
            {
                Debugging.Log("가챠타임 없음");
                lastGachaOpen = (ulong)0;
                PlayerPrefs.SetString("LastGachaOpen", "0");
            }
        }

        if (!IsGachaReady())
        {
            gachaButton.interactable = false;
            if(lockPanel!=null)
                lockPanel.SetActive(true);
        }
    }

    private void Update()
    {
        if (!gachaButton.IsInteractable())
        {
            if (IsGachaReady())
            {
                gachaButton.interactable = true;
                if (lockPanel != null)
                    lockPanel.SetActive(false);
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
    public void GachaClick()
    {
        lastGachaOpen = (ulong)DateTime.Now.Ticks;

        if (type == 1)
        {
            PlayerPrefs.SetString("LastGoldOpen", lastGachaOpen.ToString());
        }
        else if (type == 2)
        {
            PlayerPrefs.SetString("LastCrystalOpen", lastGachaOpen.ToString());
        }
        else if (type == 3)
        {
            PlayerPrefs.SetString("LastEnergyOpen", lastGachaOpen.ToString());
        }
        else if (type == 4)
        {
            PlayerPrefs.SetString("LastGachaOpen", lastGachaOpen.ToString());
            MissionSystem.AddClearPoint(MissionSystem.ClearType.FreeGacha);
        }
        UnityAdsManager.instance.ShowDefaultRewardedAd(type);
        gachaButton.interactable = false;
        if (lockPanel != null)
            lockPanel.SetActive(true);

    }
    private bool IsGachaReady()
    {
        ulong diff = ((ulong)DateTime.Now.Ticks - lastGachaOpen);
        ulong m = diff / TimeSpan.TicksPerMillisecond;
        double secondsLeft = (double)(msToWait - m) / 1000.0f;

        if (secondsLeft < 0)
        {
            if (lockPanel != null)
                lockPanel.SetActive(false);
            return true;
        }
        return false;
    }
}
