using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHeroSkillManager : MonoBehaviour
{
    List<GameObject> skillbuttons;
    List<float> skillNeedEnergys;

    Image skillImage;
    Text skillEnergyText;
    public TutorialHero tutorialHero;
    bool isAutoSkillCasting = false;

    public static TutorialHeroSkillManager instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public void ShowUI(bool isBattleMode = false)
    {
        skillNeedEnergys = new List<float>();
        skillbuttons = new List<GameObject>();
        HeroProfileSet(tutorialHero, 0);
        skillImage = this.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        skillEnergyText = this.transform.GetChild(0).GetComponentInChildren<Button>().GetComponentInChildren<Text>();
        int skillId = HeroSystem.GetUserHero(101).skill;
        Skill skill = SkillSystem.GetSkill(skillId);
        skillImage.sprite = SkillSystem.GetSkillImage(skill.id);
        int needEnergy = HeroSystem.GetHeroNeedEnergy(101, skill);
        skillEnergyText.text = needEnergy.ToString();
        skillNeedEnergys.Add(needEnergy);

        this.transform.GetChild(0).GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        this.transform.GetChild(0).GetComponentInChildren<Button>().onClick.AddListener(delegate
        {
            OnSkillButtonClick(0);
        });
        this.transform.GetChild(0).gameObject.SetActive(true);
        skillbuttons.Add(this.transform.GetChild(0).gameObject);
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

    public void HeroProfileSet(TutorialHero data, int index)
    {
        this.transform.GetChild(index).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().sprite = HeroSystem.GetHeroClassImage(data.heroData);
        this.transform.GetChild(index).GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().sprite = HeroSystem.GetHeroThumbnail(data.id);
    }
    private void Update()
    {
        if (skillbuttons!=null&&skillbuttons[0] != null)
        {
            if (tutorialHero != null && !tutorialHero.isDead)
            {
                float delay = SetEnergyPercent(0);
                if (delay <= 0)
                {
                    skillbuttons[0].GetComponentInChildren<Button>().interactable = true;
                }
                else
                {
                    skillbuttons[0].GetComponentInChildren<Button>().interactable = false;
                    skillbuttons[0].GetComponentInChildren<Button>().transform.GetChild(0).GetChild(1).GetComponent<Image>().fillAmount = delay;
                }
            }
            else
            {
                skillbuttons[0].GetComponentInChildren<Button>().interactable = false;
                skillbuttons[0].GetComponentInChildren<Button>().transform.GetChild(0).GetChild(1).GetComponent<Image>().fillAmount = 1;
            }

        }
    }

    float SetEnergyPercent(int index)
    {
        float currentEnergyPercent = Mathf.Clamp(1 - (TutorialStageManager.instance.GetFlatEnergy() / skillNeedEnergys[index]), 0, 1);
        return currentEnergyPercent;
    }

    IEnumerator ClickingSkillButton(int index)
    {
        Image cover = this.transform.GetChild(index).GetChild(0).GetChild(1).GetComponent<Image>();
        float delay = 0.0f;
        while (delay < 2.0f)
        {
            cover.fillAmount = 1 - (delay * 0.5f);
            delay += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        cover.fillAmount = 0;
        yield return null;
    }

    public void OnSkillButtonClick(int index)
    {
        if (Common.stageModeType == Common.StageModeType.Battle)
        {
            Common.Message(LocalizationManager.GetText("alertUnablePlayerSkillMessage2"), this.transform.GetChild(index).GetComponentInChildren<Button>().transform);
            return;
        }
        else
        {
            if (tutorialHero != null)
            {
                var stageHero = tutorialHero;
                int needEnergy = (int)skillNeedEnergys[index];
                if (stageHero != null && stageHero.isSkillAble() && TutorialStageManager.instance.IsSkillAble(needEnergy))
                {
                    SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_skill);
                    stageHero.SkillAttack();
                    this.transform.GetChild(index).GetComponentInChildren<Button>().interactable = false;
                    GameObject clickEffect = EffectPool.Instance.PopFromPool("BalloonPopExplosion");
                    clickEffect.transform.position = this.transform.GetChild(index).transform.position;
                    clickEffect.SetActive(true);
                    StartCoroutine(ClickingSkillButton(index));
                    TutorialStageManager.instance.UseSkill(needEnergy);
                }
                else
                {
                    SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_cancel);
                }
            }
        }
    }
}
