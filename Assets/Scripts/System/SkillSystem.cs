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

        SetObtainPlayerSkill();
    }

    public static void SetObtainPlayerSkill()
    {
        List<Skill> playerSkillList = skills.FindAll(x => x.id > 100 && x.level <= User.level);
        List<Skill> userPlayerSkillList = userSkills.FindAll(x => x.id > 100);

        foreach(var skill in playerSkillList)
        {
            Skill s = userPlayerSkillList.Find(x => x.id == skill.id);
            if(s==null)
            {
                SetObtainSkill(skill.id);
                UI_Manager.instance.ShowGetAlert(skill.image, string.Format("<color='yellow'>'{0}'</color> {1}",skill.name,LocalizationManager.GetText("alertGetMessage2")));
            }
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
            return 0;
    }
    public static int GetNeedSkillEnergy(Skill skill)
    {
        if (skill != null)
            return skill.energy + ((skill.energy * GetUserSkillLevel(skill.id)) / 10);
        return 0;
    }
    public static int GetNeedSkillEnergy(int id)
    {
        Skill skill = GetSkill(id);
        if (skill != null)
            return skill.energy + ((skill.energy * GetUserSkillLevel(skill.id)) / 10);
        return 0;
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
                userSkills.Add(skill);
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
    public static string GetUserSkillDescription(Skill skillData, HeroData heroData)
    {
        return string.Format("\r\n{0}\r\n\r\n<color='yellow'>스킬공격력 : {1}</color>  <color='cyan'>소모에너지 : {2}</color><color='yellow'>(-{3})</color>",GetSkill(skillData.id).description,GetUserSkillPower(skillData.id), HeroSystem.GetHeroNeedEnergy(heroData.id,skillData),HeroSystem.GetHeroStatusSkillEnergy(ref heroData));
    }
    public static List<Skill> GetPlayerSkillList()
    {
        return skills.FindAll(x => x.id > 100);
    }
    public static List<Skill> GetAblePlayerSkillList()
    {
        return skills.FindAll(x => x.id > 100&&!x.id.Equals(User.playerSkill[0])&&!x.id.Equals(User.playerSkill[1]));
    }
    public static List<Skill> GetSelectSkillList()
    {
        List<Skill> selectSkill = new List<Skill>();

        for(var i = 0; i < User.playerSkill.Length; i++)
        {
            if(User.playerSkill[i]!=0)
            {
                Skill skill = GetUserSkill(User.playerSkill[i]);
                if (skill != null)
                    selectSkill.Add(skill);
            }
        }
        return selectSkill;
    }
    public static void SetPlayerSkill(List<Skill> skillList)
    {
        for (var i = 0; i < skillList.Count; i++)
        {
            if(skillList[i]!=null)
                User.playerSkill[i] = skillList[i].id;
        }
        for(var i = skillList.Count; i<User.playerSkill.Length; i++)
        {
            User.playerSkill[i] = 0;
        }
    }
    public static string GetPlayerSkillDescription(Skill skill)
    {
        string des = "";

        des = string.Format("Lv{0} {1}\r\n\r\n{2}\r\n<color='yellow'><size='20'>스킬 공격력 : {3}\r\n재사용 대기시간 : {4}초</size></color>", GetUserSkillLevel(skill.id), skill.name, skill.description,GetUserSkillPower(skill.id),skill.energy);

        return des;
    }
    public static bool isPlayerSkillAble(int id)
    {
        Skill skill = userSkills.Find(x => x.id == id || x.id.Equals(id));
        if (skill != null)
            return true;
        else
            return false;
    }
    public static bool isPlayerSkillUpgradeAble(int id)
    {
        Skill skill = userSkills.Find(x => x.id == id || x.id.Equals(id));
        Skill refSkill = skills.Find(x => x.id == id || x.id.Equals(id));
        if (skill != null&&refSkill!=null && (refSkill.level+skill.level) <= User.level)
            return true;
        else
            return false;
    }
    public static bool isHeroSkillUpgradeAble(int id, HeroData data)
    {
        Skill skill = userSkills.Find(x => x.id == id || x.id.Equals(id));
        if (skill != null && skill.level < data.level)
            return true;
        else
            return false;
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

