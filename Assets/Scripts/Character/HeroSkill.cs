using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "스킬", menuName = "사용자스킬")]
public class HeroSkill : ScriptableObject
{
    public enum SkillType { ATTACK, HEAL, BUFF, DEBUFF };
    public enum ApplyType { All, Allys, Enemys };

    public SkillType skillType;
    public ApplyType applyType;
    public int skillAnimationType;
    public string skillName;
    public string skillDescription;
    public int skillLevel;
    public int skillAbillity;
    public float skillDelayTime;
    public Sprite skillImage;
    public GameObject skillEffect;
    public AudioClip skillSound;

    public HeroSkill() { }
    public HeroSkill(SkillType skilltype, ApplyType applytype, string skillname, string skilldescription, int skilllevel, int skillabillity, float skilldelaytime)
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
