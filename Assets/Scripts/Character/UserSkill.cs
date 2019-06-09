using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "사용자스킬", menuName = "사용자스킬")]
public class UserSkill : ScriptableObject
{
    public enum SkillType { ATTACK, HEAL, BUFF, DEBUFF };
    public SkillType skillType;
    public enum ApplyType { All, Allys, Enemys };
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

