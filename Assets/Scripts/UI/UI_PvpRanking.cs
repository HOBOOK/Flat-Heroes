using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Proyecto26;

public class UI_PvpRanking : MonoBehaviour
{
    public GameObject InfinityModeRankPanel;
    public GameObject InfinityModeRankTop10Panel;
    public Transform RankTitleTransform;
    Transform rankerParent;
    Transform userParent;

    Text firstRankerScoreText;
    Text firstRankerNameText;
    Text playerScoreText;
    Text playerNameText;

    Image top10ThumbnailImage;
    Text top10NameText;
    Text top10PointText;

    List<KeyValuePair<string, RankDataInfo>> Top10Rankers = new List<KeyValuePair<string, RankDataInfo>>();
    private void Awake()
    {
        if(InfinityModeRankPanel!=null)
        {
            rankerParent = InfinityModeRankPanel.transform.GetChild(0).transform;
            firstRankerScoreText = rankerParent.transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>();
            firstRankerNameText = rankerParent.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>();


            userParent = InfinityModeRankPanel.transform.GetChild(2).transform;
            playerScoreText = userParent.transform.GetChild(0).GetComponentInChildren<Text>();
            playerNameText = userParent.transform.GetChild(1).GetComponentInChildren<Text>();
        }
    }

    private void Start()
    {
        StartCoroutine(GetRankData(User.battleRankPoint));
    }
    IEnumerator GetRankData(int point)
    {
        Top10Rankers = GoogleSignManager.Instance.GetPvpRankData(Common.GetRank(point));
        Debugging.Log(Top10Rankers.Count + " >> 데이터 받아오는중 ");
        while (Top10Rankers.Count < 20)
            yield return null;
        RefreshUI();
        yield return null;
    }

    void RefreshUI()
    {
        if(RankTitleTransform!=null)
        {
            RankTitleTransform.GetComponentInChildren<Text>().text = string.Format("<size='50'>{0}</size> class {1}", Common.GetRankText(User.battleRankPoint), LocalizationManager.GetText("PvpRankTitle"));
        }
        if(Top10Rankers != null&& Top10Rankers.Count>9)
        {
            firstRankerScoreText.text = string.Format("{0} <size='30'>class</size>\r\n{1}", Common.GetRankText(Top10Rankers[0].Value.BattleRankPoint),LocalizationManager.GetText("Champion"));
            firstRankerNameText.text = Top10Rankers[0].Key;
            ShowHero(rankerParent.transform.GetChild(0).GetChild(1).GetChild(1).transform, Top10Rankers[0].Value.Thumbnail);

            for(var i = 0; i < Top10Rankers.Count; i++)
            {
                top10ThumbnailImage = InfinityModeRankTop10Panel.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Image>();
                top10NameText = InfinityModeRankTop10Panel.transform.GetChild(i).GetChild(2).GetComponent<Text>();
                top10PointText = InfinityModeRankTop10Panel.transform.GetChild(i).GetChild(3).GetComponentInChildren<Text>();

                if (top10ThumbnailImage != null)
                    top10ThumbnailImage.sprite = HeroSystem.GetHeroThumbnail(Top10Rankers[i].Value.Thumbnail);
                if (top10NameText != null)
                    top10NameText.text = Top10Rankers[i].Key;
                if (top10PointText != null)
                    top10PointText.text = string.Format("{0}{1} {2}{3}\r\n<color='yellow'>{4}rp</color>",Top10Rankers[i].Value.BattleWin, LocalizationManager.GetText("W") ,Top10Rankers[i].Value.BattleLose, LocalizationManager.GetText("L"), Top10Rankers[i].Value.BattleRankPoint.ToString());
            }
        }
        playerScoreText.text = string.Format("{0} <color='yellow'> ({1} class {2} {3})</color>", User.battleRankPoint.ToString(), Common.GetRankText(User.battleRankPoint),LocalizationManager.GetText("Rank"),Common.GetThousandCommaText(Common.RankOfPvp));
        playerNameText.text = User.name;
        GoogleSignManager.ShowProgressCircle(100);
    }

    public void ShowHero(Transform parent, int heroId)
    {
        if (parent.transform.childCount > 0)
        {
            foreach (Transform child in parent.transform)
                Destroy(child.gameObject);
        }
        if (heroId == 0) heroId = 101;
        GameObject hero = PrefabsDatabaseManager.instance.GetHeroPrefab(heroId);
        if (hero != null)
        {
            GameObject showHeroObj = Instantiate(hero, parent);
            showHeroObj.transform.localScale = new Vector3(100, 100, 100);
            showHeroObj.transform.localPosition = Vector3.zero;

            if (showHeroObj.GetComponent<Hero>() != null)
                Destroy(showHeroObj.GetComponent<Hero>());
            if (showHeroObj.GetComponent<Rigidbody2D>() != null)
                Destroy(showHeroObj.GetComponent<Rigidbody2D>());
            foreach (var sp in showHeroObj.GetComponentsInChildren<SpriteRenderer>())
            {
                sp.sortingLayerName = "ShowObject";
                sp.gameObject.layer = 16;
            }
            showHeroObj.gameObject.SetActive(true);
        }
    }

}
