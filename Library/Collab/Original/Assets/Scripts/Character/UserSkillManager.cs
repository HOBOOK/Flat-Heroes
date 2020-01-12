using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class UserSkillManager : MonoBehaviour
{
    public static UserSkillManager instance = null;
    public UserSkill[] selectedSkills = new UserSkill[2];
    private float[] selectedSkillDelayTime = new float[2];
    private bool[] selectedSkillEnable = new bool[2];

    public Button skill1Button;
    public Button skill2Button;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void FixedUpdate()
    {
        if(StageManagement.instance.isStageStart)
            SkillUpdate();
    }

    private void Start()
    {
        InitSkill();
    }

    void InitSkill()
    {
        selectedSkills = new UserSkill[2];

        if(User.playerSkill[0]!=0)
        {
            Debugging.Log(User.playerSkill[0]);
            selectedSkills[0] = Resources.Load<UserSkill>("UserSkills/"+User.playerSkill[0].ToString()) as UserSkill;
            selectedSkills[0].SetSkill();
            skill1Button.GetComponent<UI_UserSkillButton>().StartButton(true, selectedSkills[0].skillImage);
        }
        else
        {
            skill1Button.GetComponent<UI_UserSkillButton>().StartButton(false, null);
        }
        if (User.playerSkill[1] != 0)
        {
            selectedSkills[1] = Resources.Load<UserSkill>("UserSkills/" + User.playerSkill[1].ToString()) as UserSkill;
            selectedSkills[1].SetSkill();
            skill2Button.GetComponent<UI_UserSkillButton>().StartButton(true, selectedSkills[1].skillImage);
        }
        else
        {
            skill2Button.GetComponent<UI_UserSkillButton>().StartButton(false, null);
        }

        for (int i = 0; i < selectedSkills.Length; i++)
        {
            if(selectedSkills[i]!=null)
                SetSkill(selectedSkills[i], i);
        }
    }

    public void SetSkill(UserSkill userSkill, int skillNumber)
    {
        Debugging.Log(userSkill.name);
        selectedSkillDelayTime[skillNumber] = 10f;
        selectedSkillEnable[skillNumber] = false;
    }

    public void ClearSkill()
    {
        selectedSkills = new UserSkill[2];
        selectedSkillDelayTime = new float[2];
        selectedSkillEnable = new bool[2];
    }

    public void SkillUpdate()
    {
        for (int i = 0; i < 2; i++)
        {
            if (selectedSkills[i] != null)
            {
                if (selectedSkillDelayTime[i] <= 0)
                {
                    selectedSkillEnable[i] = true;
                }
                if (!selectedSkillEnable[i])
                {
                    selectedSkillDelayTime[i] -= Time.deltaTime;
                }
            }
        }
    }
    public float GetSkillDelayTime(int skillNumber)
    {
        return selectedSkillDelayTime[skillNumber];
    }
    public bool GetSkillEnable(int skillNumber)
    {
        return selectedSkillEnable[skillNumber];
    }

    public void CastingSkill(int order)
    {
        if(selectedSkills[order]!=null)
        {
            if(selectedSkillEnable[order])
            {
                List<GameObject> targetList = new List<GameObject>();
                switch(selectedSkills[order].applyType)
                {
                    case UserSkill.ApplyType.All:
                        targetList = Common.FindAll();
                        break;
                    case UserSkill.ApplyType.Allys:
                        targetList = Common.FindAlly();
                        break;
                    case UserSkill.ApplyType.Enemys:
                        targetList = Common.FindEnemy();
                        break;
                    case UserSkill.ApplyType.DeadAllys:
                        targetList = Common.FindDeadAlly();
                        break;
                }
                if(targetList!=null&&targetList.Count>0)
                {
                    Debugging.Log(selectedSkills[order].skillName + " 시전!!");
                    if (selectedSkills[order].skillEffect != null && selectedSkills[order].skillEffect.GetComponent<ParticleSystem>() != null)
                    {
                        SoundManager.instance.EffectSourcePlay(selectedSkills[order].skillSound);
                        for (int i = 0; i < targetList.Count; i++)
                        {
                            GameObject skillEffect = Instantiate(selectedSkills[order].skillEffect, this.transform);
                            skillEffect.transform.position = targetList[i].transform.position;
                            skillEffect.SetActive(true);
                            skillEffect.GetComponent<ParticleSystem>().Play();
                        }
                    }
                    switch (selectedSkills[order].skillType)
                    {
                        case UserSkill.SkillType.ATTACK:
                            foreach (var target in targetList)
                            {
                                target.GetComponent<Hero>().HittedByObject(selectedSkills[order].skillAbillity,false,new Vector2(15,15));
                            }
                            break;
                        case UserSkill.SkillType.HEAL:
                            foreach (var target in targetList)
                            {
                                target.GetComponent<Hero>().Healing((int)(selectedSkills[order].skillAbillity));
                            }
                            break;
                        case UserSkill.SkillType.BUFF:
                            break;
                        case UserSkill.SkillType.DEBUFF:
                            break;
                        case UserSkill.SkillType.RESURRENCTION:
                            CharactersManager.instance.ResurrectionHero(targetList[UnityEngine.Random.Range(0, targetList.Count)].GetComponent<Hero>().id);
                            break;
                        case UserSkill.SkillType.STUN:
                            foreach(var target in targetList)
                            {
                                target.GetComponent<Hero>().Stunned(2.0f,true);
                            }
                            break;
                    }
                    selectedSkillEnable[order] = false;
                    selectedSkillDelayTime[order] = selectedSkills[order].skillDelayTime;
                }
                else
                {
                    Debugging.Log(selectedSkills[order].skillName + " 의 타겟이 존재하지 않습니다.");
                }
            }
            else
            {
                Debugging.Log(selectedSkills[order].skillName + " 의 쿨타임이 " + selectedSkillDelayTime[order] + " 초 남았습니다.");
            }
        }
    }
}
