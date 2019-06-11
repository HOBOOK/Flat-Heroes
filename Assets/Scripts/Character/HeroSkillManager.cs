using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSkillManager : MonoBehaviour
{
    List<GameObject> skillbuttons;
    float[] skillDelays = new float[5];
    void Start()
    {
        for (var i = 0; i < skillDelays.Length; i++)
            skillDelays[i] = 1;
        skillbuttons = new List<GameObject>();

        for (var i = 0; i < this.transform.childCount; i++)
        {
            int heroIndex = i;
            if (User.stageHeros[i] == 0)
            {
                Debugging.Log(this.transform.GetChild(heroIndex).name);
                this.transform.GetChild(heroIndex).gameObject.SetActive(false);
                this.transform.GetChild(heroIndex).gameObject.SetActive(false);
            }
            else
            {
                this.transform.GetChild(heroIndex).GetComponent<Button>().onClick.RemoveAllListeners();
                this.transform.GetChild(heroIndex).GetComponent<Button>().onClick.AddListener(delegate
                {
                    OnSkillButtonClick(heroIndex, User.stageHeros[heroIndex]);
                });
                this.transform.GetChild(heroIndex).gameObject.SetActive(true);
                this.transform.GetChild(heroIndex).GetComponent<Animator>().SetTrigger("showing");
                this.transform.GetChild(heroIndex).GetComponent<Animator>().SetBool("isAble", true);
                skillbuttons.Add(this.transform.GetChild(heroIndex).gameObject);
            }
        }
    }
    private void FixedUpdate()
    {
        if(skillbuttons!=null&&skillbuttons.Count>0)
        {
            for(var i = 0; i<skillbuttons.Count; i++)
            {

                if(!skillbuttons[i].GetComponent<Button>().interactable)
                {
                    float delay = SetDelayTime(i);
                    if(delay>0)
                        skillbuttons[i].transform.GetChild(1).GetComponent<Image>().fillAmount = skillDelays[i];
                    else
                    {
                        skillbuttons[i].GetComponent<Button>().interactable = true;
                        ResetDelayTime(i);
                        skillbuttons[i].GetComponent<Animator>().SetTrigger("showing");
                        skillbuttons[i].GetComponent<Animator>().SetBool("isAble", true);
                    }
                }
            }
        }
    }

    float SetDelayTime(int index)
    {
        if (skillDelays[index] > 0)
            skillDelays[index] -= Time.fixedUnscaledDeltaTime;
        return skillDelays[index];
    }
    void ResetDelayTime(int index)
    {
        skillDelays[index] = 1;
    }

    void OnSkillButtonClick(int index, int id)
    {
        if(id!=0)
        {
            var stageHero = CharactersManager.instance.GetCurrentInStageHero(id).GetComponent<Hero>();
            int needEnergy = stageHero.status.skillEnegry;
            needEnergy = 100;
            if (stageHero != null && stageHero.isSkillAble()&&StageManagement.instance.IsSkillAble(needEnergy))
            {
                stageHero.SkillAttack();
                this.transform.GetChild(index).GetComponent<Animator>().SetBool("isAble", false);
                this.transform.GetChild(index).GetComponent<Animator>().SetTrigger("clicking");
                this.transform.GetChild(index).GetComponent<Button>().interactable = false;
                this.transform.GetChild(index).GetChild(1).GetComponent<Image>().fillAmount = 1;
                GameObject clickEffect = EffectPool.Instance.PopFromPool("BalloonPopExplosion");
                clickEffect.transform.position = this.transform.GetChild(index).transform.position;
                clickEffect.SetActive(true);
                StageManagement.instance.UseSkill(needEnergy);
            }
            else
            {
                Debugging.Log(id + " 영웅의 스킬을 사용할 수 없습니다.");
            }
        }
    }
}
