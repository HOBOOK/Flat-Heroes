using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class GoogleAdMobManager : MonoBehaviour
{
    RewardBasedVideoAd ad;
    [SerializeField] string appId;
    [SerializeField] string unitId;
    [SerializeField] bool isTest;
    [SerializeField] string deviceId;

    public static GoogleAdMobManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        MobileAds.Initialize(appId);
        ad = RewardBasedVideoAd.Instance;

        ad.OnAdLoaded += OnAdLoaded;
        ad.OnAdFailedToLoad += OnAdFailedToLoad;
        ad.OnAdOpening += OnAdOpening;
        ad.OnAdStarted += OnAdStarted;
        ad.OnAdRewarded += OnAdRewarded;
        ad.OnAdClosed += OnAdClosed;
        ad.OnAdLeavingApplication += OnAdLeavingApplication;

    }

    private void Start()
    {
        LoadAd();
    }

    void LoadAd()
    {
        AdRequest request = new AdRequest.Builder().Build();
        if(isTest)
        {
            if (deviceId.Length > 0)
                request = new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator).AddTestDevice(deviceId).Build();
            else
                unitId = "ca-app-pub-3940256099942544/1033173712";
        }

        ad.LoadAd(request, unitId);
    }

    public void OnBtnViewAdClicked()
    {
        if(ad.IsLoaded())
        {
            Debug.Log("View Ad");
            ad.Show();
        }
        else
        {
            Debug.Log("Ad is no loaded");
            LoadAd();
        }
    }

    void OnAdLoaded(object sender,EventArgs args) { Debug.Log("OnAdLoaded"); }
    void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e) { Debug.Log("OnAdFailedToLoad"); }
    void OnAdOpening(object sender, EventArgs e) { Debug.Log("OnAdOpening"); }
    void OnAdStarted(object sender, EventArgs e) { Debug.Log("OnAdStarted"); }
    void OnAdRewarded(object sender, Reward e) { Debug.Log("OnAdRewarded"); }
    void OnAdClosed(object sender, EventArgs e)
    {
        Debug.Log("OnAdClosed");
        LoadAd();
    }
    void OnAdLeavingApplication(object sender, EventArgs e)
    {
        Debug.Log("OnAdLeavingApplication");
    }

}
