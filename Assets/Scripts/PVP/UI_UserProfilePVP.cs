using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UserProfilePVP : MonoBehaviour
{
    protected Text userNameText;
    protected Text userLevelText;
    protected Image userProfileImage;
    public bool isResultProfile = false;

    private void Awake()
    {
        foreach(var i in GetComponentsInChildren<Text>())
        {
            if (i.name.Equals("ProfileName"))
                userNameText = i;
            else if (i.name.Equals("ProfileLevelText"))
                userLevelText = i;
        }
        userProfileImage = this.transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }
    
    private void Start()
    {
        if (userNameText != null)
            userNameText.text = User.battleRankPoint.ToString();
        if (userLevelText != null && !isResultProfile)
            userLevelText.text = GetRankText(User.battleRankPoint);

        ChangeProfile();
    }

    public void ChangeProfile()
    {
        if (userProfileImage != null)
        {
            userProfileImage.sprite = HeroSystem.GetHeroThumbnail(User.profileHero);
        }
    }

    public string GetRankText(int rankPoint)
    {
        if (rankPoint >= 0 && rankPoint <= 800)
            return "D";
        else if (rankPoint > 800 && rankPoint <= 1100)
            return "C";
        else if (rankPoint > 1100 && rankPoint <= 1500)
            return "B";
        else if (rankPoint > 1500 && rankPoint <= 1900)
            return "A";
        else if (rankPoint > 1900 && rankPoint <= 2300)
            return "S";
        else if (rankPoint > 2300 && rankPoint <= 2700)
            return "SS";
        else if (rankPoint > 2700 && rankPoint <= 3000)
            return "SSS";
        else
            return "Lenged";
    }
}
