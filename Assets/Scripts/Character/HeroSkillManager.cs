using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSkillManager : MonoBehaviour
{
    List<GameObject> skillbuttons;
    List<float> skillNeedEnergys;
    void Start()
    {
        skillNeedEnergys = new List<float>();
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
                this.transform.GetChild(heroIndex).transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite = HeroSystem.GetHeroThumbnail(User.stageHeros[heroIndex]);

                this.transform.GetChild(heroIndex).GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = User.stageHeros[heroIndex].ToString();
                skillNeedEnergys.Add(User.stageHeros[heroIndex]);

                this.transform.GetChild(heroIndex).GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                this.transform.GetChild(heroIndex).GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    OnSkillButtonClick(heroIndex, User.stageHeros[heroIndex]);
                });
                this.transform.GetChild(heroIndex).gameObject.SetActive(true);
                this.transform.GetChild(heroIndex).GetComponentInChildren<Animator>().SetTrigger("showing");
                this.transform.GetChild(heroIndex).GetComponentInChildren<Animator>().SetBool("isAble", true);
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
                float delay = SetEnergyPercent(i);
                if (delay <= 0)
                {
                    skillbuttons[i].GetComponentInChildren<Button>().interactable = true;
                    skillbuttons[i].GetComponentInChildren<Animator>().SetBool("isAble", true);
                }
                else
                {
                    skillbuttons[i].GetComponentInChildren<Animator>().SetBool("isAble", false);
                    skillbuttons[i].GetComponentInChildren<Button>().interactable = false ;
                    skillbuttons[i].GetComponentInChildren<Button>().transform.GetChild(0).GetChild(1).GetComponent<Image>().fillAmount = delay;
                }
            }
        }
    }

    float SetEnergyPercent(int index)
    {
        float currentEnergyPercent = Mathf.Clamp(1-((float)StageManagement.instance.GetStageEnergy()/skillNeedEnergys[index]),0,1);
        return currentEnergyPercent;
    }

    IEnumerator ClickingSkillButton(Animator anim)
    {
        anim.SetBool("isAble", false);
        anim.SetTrigger("clicking");
        yield return new WaitForSeconds(1.0f);
        anim.SetTrigger("showing");
        yield return null;
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
                StartCoroutine(ClickingSkillButton(this.transform.GetChild(index).GetComponentInChildren<Animator>()));
                this.transform.GetChild(index).GetComponentInChildren<Button>().interactable = false;
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
