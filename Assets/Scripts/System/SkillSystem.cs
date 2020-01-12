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

    public static void SetObtainPlayerSkill()
    {
        List<Skill> playerSkillList = skills.FindAll(x =>x.id<200 &&x.id > 100 && x.level <= User.level);
        List<Skill> userPlayerSkillList = userSkills.FindAll(x => x.id<200&&x.id > 100);

        foreach(var skill in playerSkillList)
        {
            Skill s = userPlayerSkillList.Find(x => x.id == skill.id);
            if(s==null)
            {
                SetObtainSkill(skill.id);
                if(skill.id!=101)
                    UI_Manager.instance.ShowGetAlert(skill.image, string.Format("<color='yellow'>'{0}'</color> {1}",skill.name,LocalizationManager.GetText("alertGetMessage2")));
            }
        }
    }

    #region 유저스킬정보
    public static Skill GetUserSkill(int id)
    {
        return userSkills.Find(x => x.id == id || x.id.Equals(id));
    }
    public static List<Skill> GetUserHerosSkills()
    {
        return userSkills.FindAll(x => x.id < 100);
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
            return skill.energy + ((skill.energy * GetUserSkillLevel(skill.id)) / 20);
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
            switch(id)
            {
                case 101:
                    return ((skill.power * User.level) + (skill.addPower * userSkill.level));
                case 102:
                    return skill.power + (skill.addPower * userSkill.level) + User.level;
                case 103:
                    return skill.power + (skill.addPower * userSkill.level) + User.level;
                case 104:
                    return (skill.addPower * userSkill.level);
                case 105:
                    return skill.power + (skill.addPower * userSkill.level) + User.level;
                default:
                    return skill.power + (skill.addPower * userSkill.level) + User.level;
            }
        }
        else if(userSkill==null&&skill!=null)
        {
            return skill.power;
        }
        else
        {
            return 0;
        }
    }
    public static int GetUserSkillDelay(int id)
    {
        Skill userSkill = userSkills.Find(s => s.id == id || s.id.Equals(id));
        Skill skill = skills.Find(s => s.id == id || s.id.Equals(id));
        if (userSkill != null && skill != null&&(skill.skillType==3||skill.skillType==4))
        {
            return skill.energy - (skill.addPower * userSkill.level);
        }
        else
        {
            return skill.energy;
        }
    }
    public static int GetUserSkillLevelUpNeedCoin(int id)
    {
        int x = GetUserSkillLevel(id);
        return 1000+(int)(x * x * 100*0.1f);
    }
    public static string GetUserSkillDescription(Skill skillData, HeroData heroData)
    {
        if(skillData.id==7) //공격력 버프
        {
            return string.Format("\r\n{0}\r\n\r\n{1} : <color='magenta'>{2}%</color>  {3} : {4}<color='magenta'>(-{5})</color>", GetSkillDescription(GetSkill(skillData.id).id), LocalizationManager.GetText("SkillAttackBuff"), GetUserSkillPower(skillData.id), LocalizationManager.GetText("SkillUseEnergy"), HeroSystem.GetHeroNeedEnergy(heroData.id, skillData), HeroSystem.GetHeroStatusSkillEnergy(ref heroData));
        }
        else if(skillData.id==9) //체력회복
        {
            return string.Format("\r\n{0}\r\n\r\n{1} : <color='magenta'>{2}%</color>  {3} : {4}<color='magenta'>(-{5})</color>", GetSkillDescription(GetSkill(skillData.id).id), LocalizationManager.GetText("SkillHeal"), GetUserSkillPower(skillData.id), LocalizationManager.GetText("SkillUseEnergy"), HeroSystem.GetHeroNeedEnergy(heroData.id, skillData), HeroSystem.GetHeroStatusSkillEnergy(ref heroData));

        }
        else
            return string.Format("\r\n{0}\r\n\r\n{1} : <color='magenta'>{2}%</color>  {3} : {4}<color='magenta'>(-{5})</color>", GetSkillDescription(GetSkill(skillData.id).id),LocalizationManager.GetText("SkillAttack"),GetUserSkillPower(skillData.id), LocalizationManager.GetText("SkillUseEnergy"), HeroSystem.GetHeroNeedEnergy(heroData.id,skillData),HeroSystem.GetHeroStatusSkillEnergy(ref heroData));
    }
    public static List<Skill> GetPlayerSkillList()
    {
        return skills.FindAll(x => x.id < 200 && x.id > 100);
    }
    public static List<Skill> GetAblePlayerSkillList()
    {
        return skills.FindAll(x =>x.id<200&& x.id > 100&&!x.id.Equals(User.playerSkill[0])&&!x.id.Equals(User.playerSkill[1]));
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

        switch(skill.id)
        {
            case 101:
                des = string.Format("Lv{0} {1}\r\n\r\n{2}\r\n<color='yellow'><size='20'>{3} : {4}\r\n{5} : {6}{7}</size></color>", GetUserSkillLevel(skill.id), GetSkillName(skill.id), GetSkillDescription(skill.id), LocalizationManager.GetText("SkillAttack"), GetUserSkillPower(skill.id), LocalizationManager.GetText("SkillDelay"), GetUserSkillDelay(skill.id), LocalizationManager.GetText("Sec"));
                break;
            case 102:
                des = string.Format("Lv{0} {1}\r\n\r\n{2}\r\n<color='yellow'><size='20'>{3} : {4}\r\n{5} : {6}{7}</size></color>", GetUserSkillLevel(skill.id), GetSkillName(skill.id), GetSkillDescription(skill.id), LocalizationManager.GetText("SkillHeal"), GetUserSkillPower(skill.id), LocalizationManager.GetText("SkillDelay"), GetUserSkillDelay(skill.id), LocalizationManager.GetText("Sec"));
                break;
            case 103:
                des = string.Format("Lv{0} {1}\r\n\r\n{2}\r\n<color='yellow'><size='20'>{3} : {4}{5}</size></color>", GetUserSkillLevel(skill.id), GetSkillName(skill.id), GetSkillDescription(skill.id), LocalizationManager.GetText("SkillDelay"), GetUserSkillDelay(skill.id), LocalizationManager.GetText("Sec"));
                break;
            case 104:
                des = string.Format("Lv{0} {1}\r\n\r\n{2}\r\n<color='yellow'><size='20'>{3} : {4}{5}\r\n{6} : {7}{8}</size></color>", GetUserSkillLevel(skill.id), GetSkillName(skill.id), GetSkillDescription(skill.id), LocalizationManager.GetText("BuffTime"), (2.0f+GetUserSkillPower(skill.id)*0.1f), LocalizationManager.GetText("Sec"), LocalizationManager.GetText("SkillDelay"), GetUserSkillDelay(skill.id), LocalizationManager.GetText("Sec"));
                break;
            case 105:
                des = string.Format("Lv{0} {1}\r\n\r\n{2}\r\n<color='yellow'><size='20'>{3} : {4}{5}</size></color>", GetUserSkillLevel(skill.id), GetSkillName(skill.id), GetSkillDescription(skill.id), LocalizationManager.GetText("SkillDelay"), GetUserSkillDelay(skill.id), LocalizationManager.GetText("Sec"));
                break;
        }


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
        if (skill != null&&refSkill!=null && (refSkill.level+skill.level) <= User.level&&skill.level<30)
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
    public static string GetSkillName(int id)
    {
        string str = null;
        Skill skill = skills.Find(x => x.id == id || x.id.Equals(id));
        if(skill!=null)
        {
            str = LocalizationManager.GetText("SkillName" + skill.id);
;        }
        return str;
    }
    public static string GetSkillDescription(int id)
    {
        string str = null;
        Skill skill = skills.Find(x => x.id == id || x.id.Equals(id));
        if (skill != null)
        {
            str = LocalizationManager.GetText("SkillDescription" + skill.id);
        }
        return str;
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
        Skill skill = skills.Find(x => x.id == id || x.id.Equals(id));
        if(skill!=null)
        {
            Sprite sprite = Resources.Load<Sprite>(skill.image);
            if (sprite == null)
                sprite = ItemSystem.GetItemNoneImage();
            return sprite;
        }
        return ItemSystem.GetItemNoneImage();
    }
    #endregion
}

