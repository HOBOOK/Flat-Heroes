using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RewardChestOpen : MonoBehaviour
{
    public GameObject Chest;
    public GameObject RewardInformationPanel;
    Image ChestImage;
    public Sprite ChestSprite;
    public Sprite ChestOpenSprite;
    public Sprite Chest2Sprite;
    public Sprite Chest2OpenSprite;
    public Sprite Chest3Sprite;
    public Sprite Chest3OpenSprite;
    Transform RewardItemSlotParentTransform;
    Text RewardItemInformationText;

    private Item rewardItem;
    private int rewardCount;
    public Button CloseButton;
    
    private void Awake()
    {
        ChestImage = Chest.GetComponent<Image>();
        RewardItemSlotParentTransform = RewardInformationPanel.transform.GetChild(0).GetChild(0);
        RewardItemInformationText = RewardInformationPanel.transform.GetChild(1).GetComponent<Text>();
    }
    private void OnEnable()
    {
        rewardItem = null;
        RewardInformationPanel.gameObject.SetActive(false);
        CloseButton.gameObject.SetActive(false);
        ShowChestReward();
    }
    public void ShowChestReward(int type=0)
    {
        StartCoroutine(OpenAnimation(type));
    }

    IEnumerator OpenAnimation(int type)
    {
        ChestImage.sprite = GetBeforeCheastSprite(type);
        Chest.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(1.5f);
        ChestImage.sprite = GetAfterChestSprtie(type);
        GetItem(type);
        yield return new WaitForSeconds(0.5f);
        ShowRewardInformation();
    }

    void ShowRewardInformation()
    {
        if(rewardItem!=null)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.dropItem);
            RewardInformationPanel.gameObject.SetActive(true);
            if (RewardItemSlotParentTransform != null)
            {
                RewardItemSlotParentTransform.GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemClassImage(rewardItem.id);
                RewardItemSlotParentTransform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemImage(rewardItem.id);
                RewardItemSlotParentTransform.GetChild(0).GetChild(1).GetComponent<Image>().color = ItemColor.GetItemColor(rewardItem.itemClass);
            }
            if (RewardItemInformationText != null)
            {
                string itemCountText = rewardCount > 1 ? "x " + rewardCount : "";
                RewardItemInformationText.text = User.language == "ko" ?
                    string.Format("봉인된 상자에서\r\n<size='40'>'{0} {1}'</size>\r\n가 나왔습니다!", ItemSystem.GetItemName(rewardItem.id), itemCountText) :
                    string.Format("<size='40'>'{0} {1}'</size>\r\n came out of the sealed box!", ItemSystem.GetItemName(rewardItem.id), itemCountText);
            }
        }
        if (CloseButton != null)
            CloseButton.gameObject.SetActive(true);
    }

    Sprite GetBeforeCheastSprite(int type)
    {
        if (type == 2)
            return Chest3Sprite;
        else if (type == 1)
            return Chest2Sprite;
        else
            return ChestSprite;
    }
    Sprite GetAfterChestSprtie(int type)
    {
        if (type == 2)
            return Chest3OpenSprite;
        else if (type == 1)
            return Chest2OpenSprite;
        else
            return ChestOpenSprite;
    }

    public void GetItem(int type)
    {
        int randomValue = UnityEngine.Random.Range(0, 100);
        int rewardType = -1;
        if (randomValue < 25) rewardType = 0;
        else if (randomValue >= 25 && randomValue < 40) rewardType = 1;
        else if (randomValue >= 40 && randomValue < 55) rewardType = 2;
        else if (randomValue >= 55 && randomValue < 70) rewardType = 3;
        else if (randomValue >= 70 && randomValue < 85) rewardType = 4;
        else rewardType = 5;
        if (type==2)
        {
            if (rewardType == 0)
            {
                rewardItem = GachaSystem.GetLegendaryChestBox();
                rewardCount = 1;
            }
            else if (rewardType == 1)
            {
                rewardCount = UnityEngine.Random.Range(50, 150);
                rewardItem = ItemSystem.GetItem(10002);
                SaveSystem.AddUserCrystal(rewardCount);
            }
            else if (rewardType == 2)
            {
                rewardCount = UnityEngine.Random.Range(1, 4);
                rewardItem = ItemSystem.GetItem(8004);
                ItemSystem.SetObtainItem(8004, rewardCount);
            }
            else if (rewardType == 3)
            {
                rewardCount = UnityEngine.Random.Range(300000, 700000);
                rewardItem = ItemSystem.GetItem(10001);
                SaveSystem.AddUserCoin(rewardCount);
            }
            else if (rewardType == 4)
            {
                rewardCount = UnityEngine.Random.Range(100, 200);
                rewardItem = ItemSystem.GetItem(10003);
                SaveSystem.AddUserEnergy(rewardCount);
            }
            else
            {
                rewardCount = UnityEngine.Random.Range(100, 250);
                rewardItem = ItemSystem.GetItem(8001);
                ItemSystem.SetObtainItem(8001, rewardCount);
            }
        }
        else if(type==1)
        {
            if (rewardType == 0)
            {
                rewardItem = GachaSystem.GetSpecialChestBox();
                rewardCount = 1;
            }
            else if (rewardType == 1)
            {
                rewardCount = UnityEngine.Random.Range(20, 60);
                rewardItem = ItemSystem.GetItem(10002);
                SaveSystem.AddUserCrystal(rewardCount);
            }
            else if (rewardType == 2)
            {
                rewardCount = UnityEngine.Random.Range(1, 4);
                rewardItem = ItemSystem.GetItem(8003);
                ItemSystem.SetObtainItem(8003, rewardCount);
            }
            else if (rewardType == 3)
            {
                rewardCount = UnityEngine.Random.Range(100000, 300000);
                rewardItem = ItemSystem.GetItem(10001);
                SaveSystem.AddUserCoin(rewardCount);
            }
            else if (rewardType == 4)
            {
                rewardCount = UnityEngine.Random.Range(50, 100);
                rewardItem = ItemSystem.GetItem(10003);
                SaveSystem.AddUserEnergy(rewardCount);
            }
            else
            {
                rewardCount = UnityEngine.Random.Range(50, 150);
                rewardItem = ItemSystem.GetItem(8001);
                ItemSystem.SetObtainItem(8001, rewardCount);
            }
        }
        else
        {
            if (rewardType == 0)
            {
                rewardItem = GachaSystem.GetNormalChestBox();
                rewardCount = 1;
            }
            else if (rewardType == 1)
            {
                rewardCount = UnityEngine.Random.Range(10, 30);
                rewardItem = ItemSystem.GetItem(10002);
                SaveSystem.AddUserCrystal(rewardCount);
            }
            else if (rewardType == 2)
            {
                rewardCount = UnityEngine.Random.Range(50, 100);
                rewardItem = ItemSystem.GetItem(10004);
                SaveSystem.AddUserMagicStone(rewardCount);
            }
            else if (rewardType == 3)
            {
                rewardCount = UnityEngine.Random.Range(30000, 120000);
                rewardItem = ItemSystem.GetItem(10001);
                SaveSystem.AddUserCoin(rewardCount);
            }
            else if (rewardType == 4)
            {
                rewardCount = UnityEngine.Random.Range(30, 50);
                rewardItem = ItemSystem.GetItem(10003);
                SaveSystem.AddUserEnergy(rewardCount);
            }
            else
            {
                rewardCount = UnityEngine.Random.Range(20, 50);
                rewardItem = ItemSystem.GetItem(8001);
                ItemSystem.SetObtainItem(8001, rewardCount);
            }
        }
    }
}
