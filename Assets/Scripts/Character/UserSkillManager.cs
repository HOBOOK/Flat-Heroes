using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "스킬", menuName = "사용자스킬")]
public class UserSkill : ScriptableObject
{
    public enum SkillType { ATTACK,HEAL,BUFF,DEBUFF};
    public SkillType skillType;
    public enum ApplyType { All, Allys, Enemys};
    public ApplyType applyType;
    public string skillName;
    public string skillDescription;
    public int skillLevel;
    public int skillAbillity;
    public float skillDelayTime;
    public Sprite skillImage;
    public GameObject skillEffect;
    public AudioClip skillSound;

    public UserSkill() { }
    public UserSkill(SkillType skilltype, ApplyType applytype, string skillname, string skilldescription, int skilllevel, int skillabillity, float skilldelaytime)
    {
        this.skillType = skilltype;
        this.applyType = applytype;
        this.skillName = skillname;
        this.skillDescription = skilldescription;
        this.skillLevel = skilllevel;
        this.skillAbillity = skillabillity;
        this.skillDelayTime = skilldelaytime;
    }
}

public class UserSkillManager : MonoBehaviour
{
    public static UserSkillManager instance = null;
    //private List<UserSkill> userSkills = new List<UserSkill>();
    public UserSkill[] selectedSkills = new UserSkill[2];
    private float[] selectedSkillDelayTime = new float[2];
    private bool[] selectedSkillEnable = new bool[2];
    private bool isSkillSettingCompleted = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);

    }

    private void FixedUpdate()
    {
        SkillUpdate();
    }

    private void Start()
    {
        InitSkill();
    }

    void InitSkill()
    {
        if (!isSkillSettingCompleted)
        {
            for(int i =0; i<2;i++)
            {
                SetSkill(selectedSkills[i], i);
            }
            isSkillSettingCompleted = true;
        }
    }

    public void SetSkill(UserSkill userSkill, int skillNumber)
    {
        selectedSkillDelayTime[skillNumber] = userSkill.skillDelayTime;
        selectedSkillEnable[skillNumber] = false;
    }

    public void ClearSkill()
    {
        if(isSkillSettingCompleted)
        {
            selectedSkills = new UserSkill[2];
            selectedSkillDelayTime = new float[2];
            selectedSkillEnable = new bool[2];
            isSkillSettingCompleted = false;
        }
    }

    public void SkillUpdate()
    {
        if(isSkillSettingCompleted)
        {
            for(int i = 0; i < 2; i++)
            {
                if(selectedSkills[i]!=null)
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
                                target.GetComponent<Hero>().HittedByObject(selectedSkills[order].skillAbillity * 50,false,new Vector2(15,15));
                            }
                            break;
                        case UserSkill.SkillType.HEAL:
                            foreach (var target in targetList)
                            {
                                target.GetComponent<Hero>().Healing((int)(target.GetComponent<Hero>().status.maxHp*(selectedSkills[order].skillAbillity*0.01f)));
                            }
                            break;
                        case UserSkill.SkillType.BUFF:
                            break;
                        case UserSkill.SkillType.DEBUFF:
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
