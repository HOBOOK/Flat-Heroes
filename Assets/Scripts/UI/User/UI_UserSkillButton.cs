using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UserSkillButton : MonoBehaviour
{
    public int skillnumber;
    float skillDelayTime;
    public Text skillDelayText;
    public Image skillCover;
    public Image skillImage;

    bool isStart = false;

    public void StartButton(bool start, Sprite sprite)
    {
        if(start)
        {
            skillImage.enabled = true;
            skillImage.sprite = sprite;
            isStart = true;
        }
        else
        {
            skillImage.enabled = false;
            skillDelayText.enabled = false;
            isStart = false;
        }
    }
    void FixedUpdate()
    {
        if(isStart)
        {
            if (UserSkillManager.instance.selectedSkills[skillnumber] != null)
            {

                skillDelayTime = UserSkillManager.instance.GetSkillDelayTime(skillnumber);
                if (UserSkillManager.instance.GetSkillEnable(skillnumber))
                {
                    skillDelayText.text = "";
                }
                else
                {
                    skillDelayText.text = Convert.ToInt32(skillDelayTime).ToString() + "s";
                }
                skillCover.fillAmount = (skillDelayTime / UserSkillManager.instance.selectedSkills[skillnumber].skillDelayTime);
            }
        }
      
    }
    public void OnClick()
    {
        UserSkillManager.instance.CastingSkill(skillnumber);
    }
}
