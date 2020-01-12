using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossModeManager : MonoBehaviour
{
    public static BossModeManager instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

}


public class BossModeData
{
    public static int mapStage;
    public static int bossId;
    public static int rewardCoin;
    public static int rewardCrystal;
    public static int rewardScroll;
    public static int rewardTranscendenceStone;
    public static int rewardBoxType;
    public static int rewardBoxCount;

    public static void SetBossModeData(int id, int coin, int crystal, int scroll,int tStone ,int stage, int boxType, int boxCcount)
    {
        bossId = id;
        rewardCoin = coin;
        rewardCrystal = crystal;
        rewardScroll = scroll;
        rewardTranscendenceStone = tStone;
        mapStage = stage;
        rewardBoxType = boxType;
        rewardBoxCount = boxCcount;
    }
    public static void ClearBossModeData()
    {
        bossId = 0;
        rewardCoin = 0;
        rewardCrystal = 0;
        rewardScroll = 0;
        rewardTranscendenceStone = 0;
        rewardBoxType = 0;
        rewardBoxCount = 0;
    }
}
