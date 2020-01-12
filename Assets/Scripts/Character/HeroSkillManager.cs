using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSkillManager : MonoBehaviour
{
    List<GameObject> skillbuttons;
    List<float> skillNeedEnergys;

    Image skillImage;
    Text skillEnergyText;
    Hero[] heros = new Hero[5];
    bool isAutoSkillCasting = false;

    int maxNeedEnergy = 0;
    public int MaxNeedEnergy
    {
        get
        {
            return maxNeedEnergy;
        }
    }

    public static HeroSkillManager instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        for (var i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void ShowUI(bool isBattleMode=false)
    {
        skillNeedEnergys = new List<float>();
        skillbuttons = new List<GameObject>();

        for (var i = 0; i < this.transform.childCount; i++)
        {
            int heroIndex = i;
            if (isBattleMode)
            {
                if (User.battleHeros[i] == 0)
                {
                    this.transform.GetChild(heroIndex).gameObject.SetActive(false);
                    this.transform.GetChild(heroIndex).gameObject.SetActive(false);
                    skillNeedEnergys.Add(0);
                }
                else
                {
                    heros[heroIndex] = StageBattleManager.instance.GetCurrentInBattleHero(User.battleHeros[heroIndex]).GetComponent<Hero>();
                    HeroProfileSet(heros[heroIndex], heroIndex);
                    skillImage = this.transform.GetChild(heroIndex).GetChild(0).GetChild(0).GetComponent<Image>();
                    skillEnergyText = this.transform.GetChild(heroIndex).GetComponentInChildren<Button>().GetComponentInChildren<Text>();
                    int skillId = HeroSystem.GetUserHero(User.battleHeros[heroIndex]).skill;
                    Skill skill = SkillSystem.GetSkill(skillId);
                    skillImage.sprite = SkillSystem.GetSkillImage(skill.id);
                    int needEnergy = HeroSystem.GetHeroNeedEnergy(User.battleHeros[heroIndex], skill);
                    skillEnergyText.text = needEnergy.ToString();
                    skillNeedEnergys.Add(needEnergy);
                    if(needEnergy>maxNeedEnergy)
                    {
                        maxNeedEnergy = needEnergy;
                    }

                    this.transform.GetChild(heroIndex).GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                    this.transform.GetChild(heroIndex).GetComponentInChildren<Button>().onClick.AddListener(delegate
                    {
                        OnSkillButtonClick(heroIndex);
                    });
                    this.transform.GetChild(heroIndex).gameObject.SetActive(true);
                }
                skillbuttons.Add(this.transform.GetChild(heroIndex).gameObject);
            }
            else
            {
                if (User.stageHeros[i] == 0)
                {
                    this.transform.GetChild(heroIndex).gameObject.SetActive(false);
                    this.transform.GetChild(heroIndex).gameObject.SetActive(false);
                    skillNeedEnergys.Add(0);
                }
                else
                {
                    heros[heroIndex] = CharactersManager.instance.GetCurrentInStageHero(User.stageHeros[heroIndex]).GetComponent<Hero>();
                    HeroProfileSet(heros[heroIndex],heroIndex);
                    skillImage = this.transform.GetChild(heroIndex).GetChild(0).GetChild(0).GetComponent<Image>();
                    skillEnergyText = this.transform.GetChild(heroIndex).GetComponentInChildren<Button>().GetComponentInChildren<Text>();
                    int skillId = HeroSystem.GetUserHero(User.stageHeros[heroIndex]).skill;
                    Skill skill = SkillSystem.GetSkill(skillId);
                    skillImage.sprite = SkillSystem.GetSkillImage(skill.id);
                    int needEnergy = HeroSystem.GetHeroNeedEnergy(User.stageHeros[heroIndex], skill);
                    skillEnergyText.text = needEnergy.ToString();
                    skillNeedEnergys.Add(needEnergy);

                    this.transform.GetChild(heroIndex).GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                    this.transform.GetChild(heroIndex).GetComponentInChildren<Button>().onClick.AddListener(delegate
                    {
                        OnSkillButtonClick(heroIndex);
                    });
                    this.transform.GetChild(heroIndex).gameObject.SetActive(true);
                }
                skillbuttons.Add(this.transform.GetChild(heroIndex).gameObject);
            }
            
        }
        StartCoroutine("OpenUIEffect");
    }
    IEnumerator OpenUIEffect()
    {
        float scale = 1.2f;
        while (scale > 1.0f)
        {
            this.transform.localScale = new Vector3(scale, scale, scale);
            scale -= Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    public void ResurrectionHero(Hero data)
    {
        if(data!=null&&heros!=null&&heros.Length>0)
        {
            for (var i = 0; i < heros.Length; i++)
            {
                if (heros[i].id == data.id)
                {
                    heros[i] = data;
                    break;
                }
            }
        }
    }
    public void HeroProfileSet(Hero data, int index)
    {
        this.transform.GetChild(index).GetChild(1).GetComponent<UI_StageHeroProfile>().SetHero(data);
        this.transform.GetChild(index).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().sprite = HeroSystem.GetHeroClassImage(data.heroData);
        this.transform.GetChild(index).GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().sprite = HeroSystem.GetHeroThumbnail(data.id);
    }
    private void Update()
    {
        if(skillbuttons!=null&&skillbuttons.Count>0&&StageManagement.instance.stageInfo!=null)
        {
            for(var i = 0; i<skillbuttons.Count; i++)
            {
                if (skillbuttons[i]!=null)
                {
                    if(heros[i]!=null&&!heros[i].isDead)
                    {
                        float delay = SetEnergyPercent(i);
                        if (delay <= 0)
                        {
                            skillbuttons[i].GetComponentInChildren<Button>().interactable = true;
                        }
                        else
                        {
                            skillbuttons[i].GetComponentInChildren<Button>().interactable = false;
                            skillbuttons[i].GetComponentInChildren<Button>().transform.GetChild(0).GetChild(1).GetComponent<Image>().fillAmount = delay;
                        }
                    }
                    else
                    {
                        skillbuttons[i].GetComponentInChildren<Button>().interactable = false;
                        skillbuttons[i].GetComponentInChildren<Button>().transform.GetChild(0).GetChild(1).GetComponent<Image>().fillAmount = 1;
                    }

                }
            }
        }
    }

    float SetEnergyPercent(int index)
    {
        float currentEnergyPercent = Mathf.Clamp(1-(StageManagement.instance.GetStageEnergy()/skillNeedEnergys[index]),0,1);
        return currentEnergyPercent;
    }

    IEnumerator ClickingSkillButton(int index)
    {
        Image cover = this.transform.GetChild(index).GetChild(0).GetChild(1).GetComponent<Image>();
        float delay = 0.0f;
        while(delay<2.0f)
        {
            cover.fillAmount = 1-(delay * 0.5f);
            delay += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        cover.fillAmount = 0;
        yield return null;
    }

    void OnSkillButtonClick(int index)
    {
        if(Common.stageModeType==Common.StageModeType.Battle)
        {
            Common.Message(LocalizationManager.GetText("alertUnablePlayerSkillMessage2"), this.transform.GetChild(index).GetComponentInChildren<Button>().transform);
            return;
        }
        else
        {
            if (heros[index] != null)
            {
                var stageHero = heros[index];
                int needEnergy = (int)skillNeedEnergys[index];
                if (stageHero != null && stageHero.isSkillAble() && StageManagement.instance.IsSkillAble(needEnergy))
                {
                    SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_skill);
                    stageHero.SkillAttack();
                    this.transform.GetChild(index).GetComponentInChildren<Button>().interactable = false;
                    GameObject clickEffect = EffectPool.Instance.PopFromPool("BalloonPopExplosion");
                    clickEffect.transform.position = this.transform.GetChild(index).transform.position;
                    clickEffect.SetActive(true);
                    StartCoroutine(ClickingSkillButton(index));
                    StageManagement.instance.UseSkill(needEnergy);
                }
                else
                {
                    SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_cancel);
                }
            }
        }
    }
    public void AutoSkillClick()
    {
        List<int> indexs = new List<int>();
        for (var i = 0; i < heros.Length; i++)
        {
            if (heros[i] != null && !heros[i].isDead)
            {
                indexs.Add(i);
            }
        }
        if(indexs != null&& indexs.Count>0)
        {
            int index = indexs[UnityEngine.Random.Range(0, indexs.Count)];
            var stageHero = heros[index];
            int needEnergy = (int)skillNeedEnergys[index];
            if (stageHero != null && stageHero.isSkillAble() && StageManagement.instance.IsSkillAble(needEnergy))
            {
                if(!isAutoSkillCasting)
                {
                    StartCoroutine(AutoSkillCast(stageHero,needEnergy,index));
                }
            }
        }
    }

    IEnumerator AutoSkillCast(Hero targetHero, int energy, int index)
    {
        isAutoSkillCasting = true;
        float maxDelayTime = 3.0f;
        bool isDelayOver = false;
        while (!targetHero.IsReached() && maxDelayTime > 0)
        {
            maxDelayTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (maxDelayTime <= 0)
            isDelayOver = true;
        if(!isDelayOver)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_skill);
            targetHero.SkillAttack();
            this.transform.GetChild(index).GetComponentInChildren<Button>().interactable = false;
            GameObject clickEffect = EffectPool.Instance.PopFromPool("BalloonPopExplosion");
            clickEffect.transform.position = this.transform.GetChild(index).transform.position;
            clickEffect.SetActive(true);
            StartCoroutine(ClickingSkillButton(index));
            StageManagement.instance.UseSkill(energy);
        }
        isAutoSkillCasting = false;

    }

}
