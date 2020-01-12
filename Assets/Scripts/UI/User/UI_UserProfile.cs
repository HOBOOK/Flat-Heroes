using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UserProfile : MonoBehaviour
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
            userNameText.text = User.name;
        if (userLevelText != null&& !isResultProfile)
            userLevelText.text = User.level.ToString();

        ChangeProfile();
    }

    public void ChangeProfile()
    {
        if (userProfileImage != null)
        {
            userProfileImage.sprite = HeroSystem.GetHeroThumbnail(User.profileHero);
        }
    }
}
