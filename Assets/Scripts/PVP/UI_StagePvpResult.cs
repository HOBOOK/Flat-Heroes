using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StagePvpResult : MonoBehaviour
{
    bool isFindEnemy = false;

    public Text winText;
    public Text pointText;
    public Transform RankProfileImage;
    public GameObject ToLobbyButton;
    public Button ReFindButton;
    public GameObject FindMessage;

    private static UI_StageResult _instance = null;
    public static UI_StageResult Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(UI_StageResult)) as UI_StageResult;
                if (_instance == null)
                {
                    return null;
                }
            }
            return _instance;
        }
    }
    private void OnEnable()
    {
        ToLobbyButton.SetActive(false);
        ReFindButton.gameObject.SetActive(false);
    }
    public void ShowUI(bool isWin, int getPoint)
    {
        RankProfileImage.GetComponentInChildren<Text>().text = GetRankText(User.battleRankPoint - getPoint);
        StartCoroutine(ShowCount(User.battleRankPoint, User.battleRankPoint - getPoint,getPoint, pointText));
        if(isWin)
            winText.text = string.Format("<color='yellow'>{0}승</color> {1}패", User.battleWin, User.battleLose);
        else
            winText.text = string.Format("{0}승 <color='yellow'>{1}패</color>", User.battleWin, User.battleLose);
    }

    IEnumerator ShowCount(float target, float current,int gap,Text txt)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.pickup);
        float duration = 2f; // 카운팅에 걸리는 시간 설정. 

        if(gap>=0)
        {
            float offset = (target - current) / duration;
            while (current < target)
            {
                current += offset * Time.deltaTime;
                txt.text = string.Format("{0} <size='50'>(<color='yellow'>+{1}</color>)</size>", (int)current, gap);
                yield return null;
            }
            current = target;
            txt.text = string.Format("{0} <size='50'>(<color='yellow'>+{1}</color>)</size>", (int)current, gap);
        }
        else
        {
            float offset = (target - current) / duration;
            while (current > target)
            {
                current += offset * Time.deltaTime;
                txt.text = string.Format("{0} <size='50'>(<color='yellow'>{1}</color>)</size>", (int)current, gap);
                yield return null;
            }

            current = target;
            txt.text = string.Format("{0} <size='50'>(<color='yellow'>{1}</color>)</size>", (int)current, gap);
        }

        if(IsChangeRank(gap))
        {
            yield return StartCoroutine("RankChangeAnimation");
        }

        ToLobbyButton.SetActive(true);
        ToLobbyButton.GetComponent<AiryUIAnimatedElement>().ShowElement();
        ReFindButton.gameObject.SetActive(true);
        ReFindButton.GetComponent<AiryUIAnimatedElement>().ShowElement();

    }

    IEnumerator RankChangeAnimation()
    {
        Effect001(RankProfileImage);
        RankProfileImage.GetComponentInChildren<Text>().text = GetRankText(User.battleRankPoint);
        float size = 2.5f;
        while(size>1.0f)
        {
            RankProfileImage.localScale = new Vector3(size, size, size);
            yield return new WaitForEndOfFrame();
            size -= 1.5f*Time.deltaTime;
        }
        yield return null;
    }

    public void Effect001(Transform target = null)
    {
        if (target == null) target = this.transform;
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
        GameObject effect = EffectPool.Instance.PopFromPool("SkillUpgradeEffect", target);
        effect.transform.localScale = new Vector3(1, 1, 1);
        effect.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f);
        Vector3 pos = target.position;
        pos.z = 0;
        effect.transform.localPosition = pos;
        effect.gameObject.SetActive(true);
    }

    bool IsChangeRank(int getPoint)
    {
        if(GetRank(User.battleRankPoint)!=GetRank(User.battleRankPoint-getPoint))
        {
            return true;
        }
        return false;
    }

    string GetRankText(int rankPoint)
    {
        if (rankPoint >= 0 && rankPoint <= 800)
            return "D";
        else if (rankPoint > 800 && rankPoint <= 1100)
            return "C";
        else if (rankPoint > 1100 && rankPoint <= 1500)
            return "B";
        else if (rankPoint > 1500 && rankPoint <= 1900)
            return "A";
        else if (rankPoint > 1900 && rankPoint <= 2300)
            return "S";
        else if (rankPoint > 2300 && rankPoint <= 2700)
            return "SS";
        else if (rankPoint > 2700 && rankPoint <= 3000)
            return "SSS";
        else
            return "Lenged";
    }
    int GetRank(int rankPoint)
    {
        if (rankPoint >= 0 && rankPoint <= 800)
            return 1;
        else if (rankPoint > 800 && rankPoint <= 1100)
            return 2;
        else if (rankPoint > 1100 && rankPoint <= 1500)
            return 3;
        else if (rankPoint > 1500 && rankPoint <= 1900)
            return 4;
        else if (rankPoint > 1900 && rankPoint <= 2300)
            return 5;
        else if (rankPoint > 2300 && rankPoint <= 2700)
            return 6;
        else if (rankPoint > 2700 && rankPoint <= 3000)
            return 7;
        else
            return 8;
    }

    public void ReFindEnemyPlayer()
    {
        if (!isFindEnemy)
        {
            isFindEnemy = true;
            StartCoroutine("ReFindingEnemy");
        }
    }

    IEnumerator ReFindingEnemy()
    {
        ReFindButton.interactable = false;
        GoogleSignManager.Instance.FindPvpData();
        FindMessage.SetActive(true);
        Text messageTxt = FindMessage.GetComponentInChildren<Text>();
        messageTxt.text = "대전 상대를 찾고있습니다...";
        bool isFind = false;
        while (!Common.isLoadCompleted)
        {
            yield return null;
        }
        if (!string.IsNullOrEmpty(Common.pvpEnemyLocalId))
        {
            messageTxt.text = "대전 상대를 찾았습니다!";
            isFind = true;
        }
        else
        {
            messageTxt.text = "대전 상대를 찾지 못했습니다.";
        }
        yield return new WaitForSeconds(1f);
        FindMessage.SetActive(false);
        if (isFind)
        {
            Debugging.Log(Common.pvpEnemyLocalId);
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
            StartPvP();
        }
        else
        {
            ReFindButton.interactable = true;
        }
        isFindEnemy = false;
        yield return null;
    }
    public void StartPvP()
    {
        int battleHeroCount = CharactersManager.instance.GetBattleHeroCount();
        if (Common.PaymentCheck(ref User.portalEnergy, battleHeroCount))
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
            Debugging.Log(battleHeroCount + " 에너지 소모. 전투씬 로드 시작 > " + User.portalEnergy);
            SaveSystem.SavePlayer();
            LoadSceneManager.instance.LoadStageScene(3);

        }
        else
        {
            UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.energy, battleHeroCount);
        }
    }
}
