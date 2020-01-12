using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStageManager : MonoBehaviour
{
    public static TutorialStageManager _instance = null;
    public static TutorialStageManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(TutorialStageManager)) as TutorialStageManager;

                if (_instance == null)
                {
                    Debug.LogWarning("There's no active TutorialStageManagement object");
                }
            }

            return _instance;
        }
    }
    public bool isStartGame;
    float stageTime = 0.0f;
    float tutorialTime = 0.0f;
    public float energy = 0;
    int battleTutorialSequence = 0;

    public int enemyCount;
    bool playerSkillTrigger = false;
    bool castleTrigger = false;

    public TutorialHero tutorialHero;
    public GameObject albusObject;
    public GameObject Title;

    public float GetFlatEnergy()
    {
        return energy;
    }

    public void RemoveEnemyCount()
    {
        if (enemyCount - 1 < 0)
            enemyCount = 0;
        else
            enemyCount--;
    }

    private void FixedUpdate()
    {
        if(isStartGame)
        {
            tutorialTime += Time.deltaTime;
            if (energy + 5 > 100)
                energy = 100;
            else
                energy += 5 * Time.deltaTime;

            if (tutorialTime > 2.0f && battleTutorialSequence==0)
            {
                battleTutorialSequence = 1;
                UI_TutorialManager.instance.OpenPanel(0);
            }
            else if(energy>49 && battleTutorialSequence ==1)
            {
                battleTutorialSequence = 2;
                UI_TutorialManager.instance.OpenPanel(1);
            }
            if (isStartGame && enemyCount == 0 && battleTutorialSequence ==2)
            {
                battleTutorialSequence = 3;
                StartCoroutine("PlayerSkillTutorial");
            }
            if(playerSkillTrigger && battleTutorialSequence == 3)
            {
                FindObjectOfType<UI_UserSkillButton>().gameObject.GetComponent<Button>().interactable = true;
                battleTutorialSequence = 4;
                UI_TutorialManager.instance.OpenPanel(2);
            }
            if(!castleTrigger&&enemyCount==0 && battleTutorialSequence == 4)
            {
                battleTutorialSequence = 5;
                castleTrigger = true;
                StartCoroutine("CastleTarget");
            }
        }
    }
    IEnumerator PlayerSkillTutorial()
    {
        yield return new WaitForSeconds(2.0f);
        Camera.main.GetComponent<FollowCamera>().ChangeTarget(Common.hitTargetObject);
        //UI_TutorialManager.instance.OpenAlert("소환석을 파괴하세요!");
        FindObjectOfType<TutorialCastle>().Spawn();
        yield return new WaitForSeconds(2.0f);
        Camera.main.GetComponent<FollowCamera>().ChangeTarget(tutorialHero.gameObject);
        yield return new WaitForSeconds(5.0f);
        playerSkillTrigger = true;
    }

    IEnumerator CastleTarget()
    {
        yield return new WaitForSeconds(2.0f);
        Camera.main.GetComponent<FollowCamera>().ChangeTarget(Common.hitTargetObject);
        UI_TutorialManager.instance.OpenAlert("소환석을 파괴하세요!");
        yield return new WaitForSeconds(2.0f);
        Camera.main.GetComponent<FollowCamera>().ChangeTarget(tutorialHero.gameObject);
    }

    public void Start()
    {
        Time.timeScale = 1.0f;
        SoundManager.instance.BgmSourceChange(AudioClipManager.instance.StageBgm(0));
        isStartGame = false;
        energy = 0;
        var tutorialPlayerSkill = new List<Skill>();
        tutorialPlayerSkill.Add(SkillSystem.GetSkill(101));
        enemyCount = 5;
        SkillSystem.SetPlayerSkill(tutorialPlayerSkill);
        StartCoroutine("StageStartEffect");
    }

    IEnumerator StageStartEffect()
    {
        yield return new WaitForFixedUpdate();
        Common.isBlackUpDown = true;
        if(Title!=null)
        {
            Title.gameObject.SetActive(true);
            Title.GetComponent<AiryUIAnimatedElement>().ShowElement();
        }
        Camera.main.GetComponent<FollowCamera>().ChangeTarget(tutorialHero.gameObject);
        yield return new WaitForSeconds(2.0f);
        tutorialHero.Chat("!!");
        yield return new WaitForSeconds(2.0f);
        Camera.main.GetComponent<FollowCamera>().ChangeTarget(Common.hitTargetObject);
        Common.hitTargetObject.GetComponent<TutorialCastle>().SpawnEffect();
        yield return new WaitForSeconds(4.5f);
        Camera.main.GetComponent<FollowCamera>().ChangeTarget(tutorialHero.gameObject);
        yield return new WaitForSeconds(2.0f);
        tutorialHero.Chat(LocalizationManager.GetText("TutorialHeroChat1"));
        yield return new WaitForSeconds(5f);
        tutorialHero.Chat(LocalizationManager.GetText("TutorialHeroChat2"));
        yield return new WaitForSeconds(3.0f);
        Common.isBlackUpDown = false;
        while (!Camera.main.GetComponent<CameraEffectHandler>().isBlackEffectClear)
        {
            yield return null;
        }
        TutorialHeroSkillManager.instance.ShowUI();
        isStartGame = true;
    }
    bool isCheckAlertOn = false;
    IEnumerator CheckingAlert()
    {
        isCheckAlertOn = true;
        var alertPanel = UI_Manager.instance.ShowNeedAlert("", string.Format("{0}",LocalizationManager.GetText("alertTutorialSkipMessage")));

        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            Time.timeScale = 1.0f;
            UI_Manager.instance.ClosePopupAlertUI();
            User.tutorialSequence = 1;
            GoogleSignManager.SaveData();
            UI_TutorialManager.instance.OpenCompletedPanel();
        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 아니오를 클릭시
        }
        isCheckAlertOn = false;
        yield return null;
    }
    public void OnSkipTutorial()
    {
        if(isStartGame&&!isCheckAlertOn)
        {
            StartCoroutine("CheckingAlert");
        }
    }
    public void EndTutorial()
    {
        if(isStartGame)
        {
            StartCoroutine("EndingTutorial");
        }
    }
    public bool GetIsStartGame()
    {
        return isStartGame;
    }

    IEnumerator EndingTutorial()
    {
        User.playerSkill = new int[2];
        isStartGame = false;
        tutorialHero.hpUI.GetComponent<UI_hp>().ForceDisableUI();
        tutorialHero.Chat(LocalizationManager.GetText("TutorialHeroChat3"));
        yield return new WaitForSeconds(1.0f);
        albusObject.gameObject.SetActive(true);
        albusObject.GetComponent<TutorialHero>().SpawnEffect();
        albusObject.GetComponent<TutorialHero>().SpriteAlphaSetting(0);
        albusObject.GetComponent<Animator>().SetInteger("weaponType", 4);
        albusObject.GetComponent<TutorialHero>().isLeftorRight = true;
        albusObject.GetComponent<TutorialHero>().RedirectCharacter();
        yield return new WaitForSeconds(1.5f);
        albusObject.GetComponent<TutorialHero>().isLeftorRight = true;
        albusObject.GetComponent<TutorialHero>().RedirectCharacter();
        albusObject.GetComponent<TutorialHero>().SpriteAlphaSetting(1);
        tutorialHero.Chat("?!",0,1);
        yield return new WaitForSeconds(5.0f);
        albusObject.GetComponent<TutorialHero>().Chat(LocalizationManager.GetText("TutorialHeroChat4"), 1,1);
        yield return new WaitForSeconds(3.0f);
        tutorialHero.Chat(LocalizationManager.GetText("TutorialHeroChat5"), 1,1);
        yield return new WaitForSeconds(1.0f);
        tutorialHero.SpawnEffect();
        albusObject.GetComponent<TutorialHero>().SpawnEffect();
        yield return new WaitForSeconds(1.0f);
        tutorialHero.gameObject.SetActive(false);
        albusObject.SetActive(false);
        yield return new WaitForSeconds(2.0f);
        User.tutorialSequence=1;
        GoogleSignManager.SaveData();
        yield return new WaitForSeconds(1.5f);
        UI_TutorialManager.instance.OpenCompletedPanel();
    }

    public bool IsSkillAble(int skillenergy)
    {
        if (energy - skillenergy < 0)
            return false;
        else
        {
            return true;
        }
    }

    public void UseSkill(int skillenergy)
    {
        energy -= skillenergy;
    }
}
