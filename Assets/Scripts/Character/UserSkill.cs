using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "사용자스킬", menuName = "사용자스킬")]
public class UserSkill : ScriptableObject
{
    public int skillId;
    [HideInInspector]
    public enum SkillType { ATTACK, HEAL, BUFF, DEBUFF,RESURRENCTION,STUN };
    [HideInInspector]
    public SkillType skillType;
    [HideInInspector]
    public enum ApplyType { All, Allys, Enemys,DeadAllys };
    [HideInInspector]
    public ApplyType applyType;
    [HideInInspector]
    public string skillName;
    [HideInInspector]
    public string skillDescription;
    [HideInInspector]
    public int skillLevel;
    [HideInInspector]
    public int skillAbillity;
    [HideInInspector]
    public float skillDelayTime;
    [HideInInspector]
    public Sprite skillImage;
    public GameObject skillEffect;
    public AudioClip skillSound;

    public UserSkill() { }
    public void SetSkill()
    {
        Skill data = SkillSystem.GetSkill(skillId);
        skillType = (SkillType)data.skillType;
        applyType = (ApplyType)data.targetType;
        skillName = data.name;
        skillDescription = data.description;
        skillLevel = SkillSystem.GetUserSkillLevel(skillId);
        skillAbillity = SkillSystem.GetUserSkillPower(skillId);
        skillDelayTime = SkillSystem.GetUserSkillDelay(skillId);
        skillImage = SkillSystem.GetSkillImage(skillId);
        Debugging.Log(skillId + " 스킬 세팅완료");
    }
}

