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
        if(!Common.GetSceneCompareTo(Common.SCENE.TUTORIAL))
        {
            if (StageManagement.instance != null && StageManagement.instance.isStageStart())
                SkillUpdate();
        }
        else
        {
            if (TutorialStageManager.instance != null && TutorialStageManager.instance.isStartGame)
                SkillUpdate();
        }
    }

    private void Start()
    {
        InitSkill();
    }

    void InitSkill()
    {
        selectedSkills = new UserSkill[2];
        if(User.playerSkill[0]!=0&&skill1Button!=null)
        {
            Debugging.Log(User.playerSkill[0]);
            selectedSkills[0] = Resources.Load<UserSkill>("UserSkills/"+User.playerSkill[0].ToString()) as UserSkill;
            selectedSkills[0].SetSkill();
            skill1Button.GetComponent<UI_UserSkillButton>().StartButton(true, selectedSkills[0].skillImage);
        }
        else
        {
            if (skill1Button != null)
                skill1Button.GetComponent<UI_UserSkillButton>().StartButton(false, null);
        }
        if (User.playerSkill[1] != 0&&skill2Button!=null)
        {
            selectedSkills[1] = Resources.Load<UserSkill>("UserSkills/" + User.playerSkill[1].ToString()) as UserSkill;
            selectedSkills[1].SetSkill();
            skill2Button.GetComponent<UI_UserSkillButton>().StartButton(true, selectedSkills[1].skillImage);
        }
        else
        {
            if(skill2Button!=null)
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
        selectedSkillDelayTime[skillNumber] = 5f;
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
                if (Common.GetSceneCompareTo(Common.SCENE.TUTORIAL))
                {
                    targetList = Common.FindTutorialEnemy();
                }
                else
                {
                    switch (selectedSkills[order].applyType)
                    {
                        case UserSkill.ApplyType.All:
                            targetList = Common.FindAll();
                            break;
                        case UserSkill.ApplyType.Allys:
                            targetList = Common.FindAlly(true);
                            break;
                        case UserSkill.ApplyType.Enemys:
                            targetList = Common.FindEnemy(true);
                            break;
                        case UserSkill.ApplyType.DeadAllys:
                            targetList = Common.FindDeadAlly();
                            break;
                    }
                }
                if(targetList!=null&&targetList.Count>0)
                {
                    Debugging.Log(selectedSkills[order].skillName + " 시전!!");
                    if (selectedSkills[order].skillEffect != null && selectedSkills[order].skillEffect.GetComponent<ParticleSystem>() != null&&selectedSkills[order].skillId!=101)
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
                    switch (selectedSkills[order].skillId)
                    {
                        case 101:
                            StartCoroutine(Skill101(targetList,order));
                            break;
                        case 102:
                            foreach (var target in targetList)
                            {
                                if (target.GetComponent<Hero>() != null)
                                    target.GetComponent<Hero>().Healing((int)(selectedSkills[order].skillAbillity));
                                else if (target.GetComponent<TutorialHero>() != null)
                                    target.GetComponent<TutorialHero>().Healing((int)(selectedSkills[order].skillAbillity));
                            }
                            break;
                        case 103:
                            CharactersManager.instance.ResurrectionHero(targetList[UnityEngine.Random.Range(0, targetList.Count)].GetComponent<Hero>().id);
                            break;
                        case 104:
                            foreach(var target in targetList)
                            {
                                if (target.GetComponent<Hero>() != null)
                                    target.GetComponent<Hero>().Stunned(2.0f + (selectedSkills[order].skillAbillity*0.1f),true);
                                else if (target.GetComponent<TutorialHero>() != null)
                                    target.GetComponent<TutorialHero>().Stunned(2.0f, true);
                            }
                            break;
                        case 105:
                            Hero heroPrefabData = null;
                            foreach (var target in targetList)
                            {
                                if (target.GetComponent<Hero>() != null)
                                {
                                    heroPrefabData = target.GetComponent<Hero>();
                                    heroPrefabData.status.hp -= (int)(heroPrefabData.status.hp * 0.3f);
                                    heroPrefabData.SpeedBuff(10, 0.3f);
                                }
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
                if(skill1Button!=null&&skill2Button!=null)
                {
                    if (order == 0)
                        Common.Message(LocalizationManager.GetText("alertUnablePlayerSkillMessage"), skill1Button.transform);
                    else
                        Common.Message(LocalizationManager.GetText("alertUnablePlayerSkillMessage"), skill2Button.transform);
                }
                Debugging.Log(selectedSkills[order].skillName + " 의 쿨타임이 " + selectedSkillDelayTime[order] + " 초 남았습니다.");
            }
        }
    }
    IEnumerator Skill101(List<GameObject> targets, int order)
    {
        int maxCount = 0;
        StartCoroutine("UserSkillEffectOn");
        Common.isShake = true;
        foreach (var target in targets)
        {
            maxCount++;
            if (target.GetComponent<Hero>() != null)
            {
                GameObject effect = EffectPool.Instance.PopFromPool("SpikyFireTrail");
                Vector3 targetPos = target.transform.position;
                effect.transform.position = targetPos + new Vector3(-20, 20, 0);
                effect.gameObject.SetActive(true);

                while (Vector2.Distance(effect.transform.position, targetPos)>0.3f)
                {
                    effect.transform.position = Vector2.MoveTowards(effect.transform.position, targetPos, 1f);
                    yield return new WaitForEndOfFrame();
                }
                GameObject skillEffect = Instantiate(selectedSkills[order].skillEffect);
                skillEffect.transform.position = effect.transform.position;
                skillEffect.SetActive(true);
                skillEffect.GetComponent<ParticleSystem>().Play();
                target.GetComponent<Hero>().HittedByObject(selectedSkills[order].skillAbillity, false, new Vector2(15, 15),0.8f);
                SoundManager.instance.EffectSourcePlay(selectedSkills[order].skillSound);

            }
            else if (target.GetComponent<TutorialHero>() != null)
            {
                GameObject effect = EffectPool.Instance.PopFromPool("SpikyFireTrail");
                Vector3 targetPos = target.transform.position;
                effect.transform.position = targetPos + new Vector3(-20, 20, 0);
                effect.gameObject.SetActive(true);

                while (Vector2.Distance(effect.transform.position, targetPos) > 0.3f)
                {
                    effect.transform.position = Vector2.MoveTowards(effect.transform.position, targetPos, 1f);
                    yield return new WaitForEndOfFrame();
                }
                GameObject skillEffect = Instantiate(selectedSkills[order].skillEffect);
                skillEffect.transform.position = effect.transform.position;
                skillEffect.SetActive(true);
                skillEffect.GetComponent<ParticleSystem>().Play();
                target.GetComponent<TutorialHero>().HittedByObject(selectedSkills[order].skillAbillity, false, new Vector2(15, 15));
                SoundManager.instance.EffectSourcePlay(selectedSkills[order].skillSound);
            }
            if (maxCount > 10)
                break;
        }
        Common.isShake = false;
        StartCoroutine("UserSkillEffectOff");
        yield return null;
    }

    IEnumerator UserSkillEffectOn()
    {
        GameObject backgroundImage = GameObject.FindGameObjectWithTag("BackgroundImage");
        if (backgroundImage != null)
        {
            float rgb = 255;
            if (backgroundImage.GetComponent<Image>() != null)
            {
                Image backgroundImg = backgroundImage.GetComponent<Image>();
                while (rgb > 100)
                {
                    backgroundImg.color = new Color(rgb / 255, rgb / 255, rgb / 255, 1);
                    rgb -= Time.deltaTime * 500;
                    yield return new WaitForEndOfFrame();
                }
            }
            else if (backgroundImage.GetComponent<SpriteRenderer>() != null)
            {
                SpriteRenderer backgroundSp = backgroundImage.GetComponent<SpriteRenderer>();
                while (rgb > 100)
                {
                    backgroundSp.color = new Color(rgb / 255, rgb / 255, rgb / 255, 1);
                    rgb -= Time.deltaTime * 500;
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }

    IEnumerator UserSkillEffectOff()
    {
        GameObject backgroundImage = GameObject.FindGameObjectWithTag("BackgroundImage");
        if (backgroundImage != null)
        {
            float rgb = 100;
            if (backgroundImage.GetComponent<Image>() != null)
            {
                Image backgroundImg = backgroundImage.GetComponent<Image>();
                while (rgb < 255)
                {
                    backgroundImg.color = new Color(rgb / 255, rgb / 255, rgb / 255, 1);
                    rgb += Time.deltaTime * 500;
                    yield return new WaitForEndOfFrame();
                }
                backgroundImg.color = Color.white;
            }
            else if (backgroundImage.GetComponent<SpriteRenderer>() != null)
            {
                SpriteRenderer backgroundSp = backgroundImage.GetComponent<SpriteRenderer>();
                while (rgb < 255)
                {
                    backgroundSp.color = new Color(rgb / 255, rgb / 255, rgb / 255, 1);
                    rgb += Time.deltaTime * 500;
                    yield return new WaitForEndOfFrame();
                }
                backgroundSp.color = Color.white;
            }
        }
    }
}
