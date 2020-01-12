using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PVP : MonoBehaviour
{
    public Transform PlayerProfileTransform;
    public Transform PvpSettingTransform;
    public Transform MedalTransform;
    public GameObject HeroSelectPanel;
    public GameObject FindMessage;
    public Transform PvpRankTransform;
    public GameObject SlotRankPrefab;
    public Button StartButton;


    Text winLoseText;
    Text winRateText;
    Text winRankingText;
    Text medalText;

    bool isFindEnemy = false;

    private void Awake()
    {
        if(PlayerProfileTransform!=null)
        {
            winLoseText = PlayerProfileTransform.GetChild(0).GetChild(1).GetComponent<Text>();
            winRateText = PlayerProfileTransform.GetChild(1).GetChild(1).GetComponent<Text>();
            winRankingText = PlayerProfileTransform.GetChild(2).GetChild(1).GetComponent<Text>();
        }
        if(MedalTransform!=null)
        {
            medalText = MedalTransform.GetChild(0).GetComponentInChildren<Text>();
        }
    }
    private void Start()
    {
        StartCoroutine("GetPlayerRankInfo");
    }

    private void OnDisable()
    {
        if (FindMessage != null)
            FindMessage.gameObject.SetActive(false);
    }

    IEnumerator GetPlayerRankInfo()
    {
        GoogleSignManager.Instance.GetPvpRankPoint();
        while (!Common.isLoadCompleted)
            yield return null;
        RefreshUI();
        GetRank();
    }

    public void RefreshUI()
    {
        FindMessage.SetActive(false);
        if (CharactersManager.instance.GetBattleHeroCount() < 1)
        {
            StartButton.GetComponentInChildren<Text>().text = string.Format("<color='red'>{0}</color>", LocalizationManager.GetText("mainPvpTeamFail"));
            StartButton.interactable = false;
        }
        else
        {
            StartButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("mainPvpTeamStart");
            StartButton.interactable = true;
        }

        PlayerProfileTransformUIRefresh();
        MedalTransformRefresh();
    }
    void MedalTransformRefresh()
    {
        if(medalText!=null)
        {
            medalText.text = ItemSystem.GetUserMedalCount().ToString("N0");
        }
    }
    

    void PlayerProfileTransformUIRefresh()
    {
        if(winLoseText != null)
        {
            winLoseText.text = string.Format("<size='35'>{0}</size>{1} <size='35'>{2}</size>{3}", User.battleWin, LocalizationManager.GetText("Win"), User.battleLose, LocalizationManager.GetText("Lose"));
        }
        if(winRateText!=null)
        {
            winRateText.text = string.Format("<size='35'>{0}</size> %", ((float)User.battleWin*100 / (float)Mathf.Clamp((User.battleLose+User.battleWin),1f,5000f)).ToString("N0"));
        }
        if(winRankingText!=null)
        {
            winRankingText.text = string.Format("<size='35'>{0}</size> {1}", GetRankText(User.battleRankPoint), LocalizationManager.GetText("Rank"));
        }
    }

    public void FindEnemyPlayer()
    {
        if(!isFindEnemy)
        {
            isFindEnemy = true;
            StartCoroutine("FindingEnemy");
        }
    }

    IEnumerator FindingEnemy()
    {
        StartButton.interactable = false;
        GoogleSignManager.Instance.FindPvpData();
        FindMessage.SetActive(true);
        Text messageTxt = FindMessage.GetComponentInChildren<Text>();
        messageTxt.text = "대전 상대를 찾고있습니다...";
        bool isFind = false;
        while (!Common.isLoadCompleted)
        {
            yield return null;
        }
        if(!string.IsNullOrEmpty(Common.pvpEnemyLocalId))
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
        if(isFind)
        {
            Debugging.Log(Common.pvpEnemyLocalId);
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
            HeroSelectPanel.gameObject.SetActive(true);
        }
        else
        {
            StartButton.interactable = true;
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

    public string GetRankText(int rankPoint)
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

    #region 랭크데이터, UI
    List<KeyValuePair<string, RankDataInfo>> Top10Rankers = new List<KeyValuePair<string, RankDataInfo>>();
    void GetRank()
    {
        StartCoroutine(GetRankData(User.battleRankPoint));
    }
    IEnumerator GetRankData(int point)
    {
        Top10Rankers = GoogleSignManager.Instance.GetPvpRankData(Common.GetRank(point));
        Debugging.Log(Top10Rankers.Count + " >> 데이터 받아오는중 ");
        while (Top10Rankers.Count < 20)
            yield return null;
        RefreshRankUI();
        yield return null;
    }

    void RefreshRankUI()
    {
        if (PvpRankTransform != null)
        {
            PvpRankTransform.transform.GetChild(1).GetComponentInChildren<Text>().text = string.Format("<size='50'>{0}</size> class {1}", Common.GetRankText(User.battleRankPoint), LocalizationManager.GetText("PvpRankTitle"));
        }
        if (Top10Rankers != null && Top10Rankers.Count > 9)
        {
            Image top10ThumbnailImage;
            Text rankText;
            Text top10NameText;
            Text top10PointText;
            for (var i = 0; i < Top10Rankers.Count; i++)
            {
                GameObject slotRank = Instantiate(SlotRankPrefab, PvpRankTransform.GetComponentInChildren<VerticalLayoutGroup>().transform);
                rankText = slotRank.transform.GetChild(0).GetComponent<Text>();
                top10ThumbnailImage = slotRank.transform.GetChild(1).GetChild(0).GetComponent<Image>();
                top10NameText = slotRank.transform.GetChild(2).GetComponent<Text>();
                top10PointText = slotRank.transform.GetChild(3).GetComponentInChildren<Text>();

                if (Top10Rankers[i].Key.Equals(User.name))
                {
                    top10NameText.color = Color.magenta;
                    rankText.color = Color.magenta;
                }
                else
                {
                    top10NameText.color = Color.black;
                    rankText.color = Color.black;
                }

                if (rankText != null)
                    rankText.text = (i + 1).ToString();
                if (top10ThumbnailImage != null)
                    top10ThumbnailImage.sprite = HeroSystem.GetHeroThumbnail(Top10Rankers[i].Value.Thumbnail);
                if (top10NameText != null)
                {
                    top10NameText.text = Top10Rankers[i].Key;
                }

                if (top10PointText != null)
                    top10PointText.text = string.Format("{0}{1} {2}{3}\r\n<color='yellow'>{4}rp</color>", Top10Rankers[i].Value.BattleWin, LocalizationManager.GetText("W"), Top10Rankers[i].Value.BattleLose, LocalizationManager.GetText("L"), Top10Rankers[i].Value.BattleRankPoint.ToString());

            }
        }
        GoogleSignManager.ShowProgressCircle(100);
    }
    #endregion
}
