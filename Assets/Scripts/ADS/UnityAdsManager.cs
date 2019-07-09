using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsManager : MonoBehaviour
{
    public static UnityAdsManager instance;
    public enum RewardItems { None, Coin, Crystal, Energy, SpeicalGachaOne};
    int[] defaultRewardAmount = { 0, 30000, 15, 30, 1};
    RewardItems rewardItems;
    int rewardAmount;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public void ShowRewardedAd(RewardItems reward, int amount=0)
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            rewardItems = reward;
            rewardAmount = amount;
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    public void ShowDefaultRewardedAd(int reward)
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            rewardItems = (RewardItems)reward;
            rewardAmount = defaultRewardAmount[reward];
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                GetReward();
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }

    private void GetReward()
    {
        if(rewardItems!=RewardItems.None)
        {
            switch (rewardItems)
            {
                case RewardItems.Coin:
                    SaveSystem.AddUserCoin(rewardAmount);
                    break;
                case RewardItems.Crystal:
                    SaveSystem.AddUserCrystal(rewardAmount);
                    break;
                case RewardItems.Energy:
                    SaveSystem.AddUserEnergy(rewardAmount);
                    break;
                case RewardItems.SpeicalGachaOne:
                    UI_Manager.instance.PopupGetGacha(GachaSystem.GachaType.SpecialOne);
                    break;
            }
            rewardItems = RewardItems.None;
        }

    }
}
