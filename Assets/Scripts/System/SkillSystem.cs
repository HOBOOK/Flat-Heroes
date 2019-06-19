using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class SkillSystem
{
    // 스킬 전체 데이터베이스
    private static List<Skill> skills = new List<Skill>();
    // 유저가 가진 스킬 데이터베이스
    private static List<Skill> userSkills = new List<Skill>();
    public static void LoadSkill()
    {
        skills.Clear();
        userSkills.Clear();
        string path = Application.persistentDataPath + "/Xml/Skill.Xml";
        SkillDatabase sd = null;
        SkillDatabase userSd = null;

        if (System.IO.File.Exists(path))
        {
            sd = SkillDatabase.Load();
            userSd = SkillDatabase.LoadUser();
        }
        else
        {
            sd = SkillDatabase.InitSetting();
            userSd = SkillDatabase.LoadUser();
        }

        if(sd != null)
        {
            foreach (Skill skill in sd.skills)
            {
                skills.Add(skill);
            }
        }
        if(userSd != null)
        {
            foreach (Skill skill in userSd.skills)
            {
                userSkills.Add(skill);
            }
        }
        if(sd!=null&& userSd != null)
        {
            Debugging.LogSystem("SkillDatabase is loaded Succesfully.");
        }
    }

    #region 유저스킬정보
    public static Skill GetUserSkill(int id)
    {
        return userSkills.Find(x => x.id == id || x.id.Equals(id));
    }
    public static int GetUserSkillLevel(int id)
    {
        Skill userSkill = userSkills.Find(s => s.id == id || s.id.Equals(id));
        if (userSkill != null)
            return userSkill.level;
        else
            return 1;
    }
    public static void SetObtainSkill(int id)
    {
        Skill userSkill = userSkills.Find(x => x.id == id || x.id.Equals(id));
        if(userSkill != null)
        {
            userSkill.level += 1;
            SkillDatabase.SaveSkill(id);
        }
        else
        {
            Skill skill = skills.Find(s => s.id == id || s.id.Equals(id));
            if (skill != null)
            {
                skill.level = 1;
                SkillDatabase.AddSkill(id);
            }
        }
    }
    public static int GetUserSkillPower(int id)
    {
        Skill userSkill = userSkills.Find(s => s.id == id || s.id.Equals(id));
        Skill skill = skills.Find(s => s.id == id || s.id.Equals(id));
        if (userSkill != null&& skill != null)
        {
            return skill.power + (skill.addPower * userSkill.level);
        }
        else
        {
            return 0;
        }
    }
    public static int GetUserSkillLevelUpNeedCoin(int id)
    {
        int x = GetUserSkillLevel(id);
        return x * x * 1000;
    }
    public static string GetUserSkillDescription(int id)
    {
        return string.Format("{0}\r\n스킬공격력 : {1}",GetSkill(id).description,GetUserSkillPower(id));
    }
    #endregion

    #region 전체어빌리티정보
    public static Skill GetSkill(int id)
    {
        return skills.Find(x => x.id == id || x.id.Equals(id));
    }
    public static List<Skill> GetAllSkills()
    {
        return skills;
    }
    public static Sprite GetSkillImage(int id)
    {
        Sprite sprite = Resources.Load<Sprite>(skills.Find(x => x.id == id || x.id.Equals(id)).image);
        if (sprite == null)
            sprite = ItemSystem.GetItemNoneImage();
        return sprite;
    }
    #endregion
}

