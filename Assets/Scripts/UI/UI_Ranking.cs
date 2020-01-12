using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UI_Ranking : MonoBehaviour
{
    public int RankType; // 0:Infinity 1:HeroLevel
    public GameObject InfinityModeRankPanel;
    public GameObject InfinityModeRankTop10Panel;
    Transform rankerParent;
    Transform userParent;

    Text firstRankerScoreText;
    Text firstRankerNameText;
    Text secondRankerScoreText;
    Text secondRankerNameText;
    Text thirdRankerScoreText;
    Text thirdRankerNameText;
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
            firstRankerScoreText = rankerParent.transform.GetChild(1).GetChild(0).GetComponentInChildren<Text>();
            firstRankerNameText = rankerParent.transform.GetChild(1).GetChild(1).GetComponentInChildren<Text>();
            secondRankerScoreText = rankerParent.transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>();
            secondRankerNameText = rankerParent.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>();
            thirdRankerScoreText = rankerParent.transform.GetChild(2).GetChild(0).GetComponentInChildren<Text>();
            thirdRankerNameText = rankerParent.transform.GetChild(2).GetChild(1).GetComponentInChildren<Text>();


            userParent = InfinityModeRankPanel.transform.GetChild(2).transform;
            playerScoreText = userParent.transform.GetChild(0).GetComponentInChildren<Text>();
            playerNameText = userParent.transform.GetChild(1).GetComponentInChildren<Text>();
        }
    }

    private void Start()
    {
        StartCoroutine(GetRankData(RankType));
    }
    IEnumerator GetRankData(int type)
    {
        Top10Rankers = GoogleSignManager.Instance.GetRankData(type);
        Debugging.Log(Top10Rankers.Count + " >> 데이터 받아오는중 ");
        while (Top10Rankers.Count < 10)
            yield return null;
        RefreshUI();
        yield return null;
    }

    void RefreshUI()
    {
        if(Top10Rankers != null&& Top10Rankers.Count>9)
        {
            firstRankerScoreText.text = GetPointText(GetPoint(Top10Rankers[0].Value));
            firstRankerNameText.text = Top10Rankers[0].Key;
            ShowHero(rankerParent.transform.GetChild(1).GetChild(1).GetChild(1).transform, Top10Rankers[0].Value.Thumbnail);
            secondRankerScoreText.text = GetPointText(GetPoint(Top10Rankers[1].Value));
            secondRankerNameText.text = Top10Rankers[1].Key;
            ShowHero(rankerParent.transform.GetChild(0).GetChild(1).GetChild(1).transform, Top10Rankers[1].Value.Thumbnail);
            thirdRankerScoreText.text = GetPointText(GetPoint(Top10Rankers[2].Value));
            thirdRankerNameText.text = Top10Rankers[2].Key;
            ShowHero(rankerParent.transform.GetChild(2).GetChild(1).GetChild(1).transform, Top10Rankers[2].Value.Thumbnail);

            for(var i = 3; i < Top10Rankers.Count; i++)
            {
                top10ThumbnailImage = InfinityModeRankTop10Panel.transform.GetChild(i - 3).GetChild(1).GetChild(0).GetComponent<Image>();
                top10NameText = InfinityModeRankTop10Panel.transform.GetChild(i - 3).GetChild(2).GetComponent<Text>();
                top10PointText = InfinityModeRankTop10Panel.transform.GetChild(i - 3).GetChild(3).GetComponentInChildren<Text>();

                if (top10ThumbnailImage != null)
                    top10ThumbnailImage.sprite = HeroSystem.GetHeroThumbnail(Top10Rankers[i].Value.Thumbnail);
                if (top10NameText != null)
                    top10NameText.text = Top10Rankers[i].Key;
                if (top10PointText != null)
                    top10PointText.text = GetPointText(GetPoint(Top10Rankers[i].Value));
            }
        }
        playerScoreText.text = string.Format("{0} <color='yellow'>(Rank {1})</color>", GetPointText(GetUserPoint()), GetUserRankText());
        playerNameText.text = User.name;
        GoogleSignManager.ShowProgressCircle(100);
    }

    string GetPointText(int point)
    {
        if(RankType==0)
        {
            return Common.GetThousandCommaText(point) + " pts";
        }
        else if(RankType==1)
        {
            return "Lv " + Common.GetThousandCommaText(point);
        }
        else 
        {
            return point.ToString();
        }
    }
    int GetPoint(RankDataInfo data)
    {
        if (RankType == 0)
        {
            return data.InfinityRankPoint;
        }
        else if(RankType==1)
        {
            return data.HeroRankPoint;
        }
        else if(RankType==2)
        {
            return data.BattleRankPoint;
        }
        else
        {
            return data.AttackRankPoint;
        }
    }
    int GetUserPoint()
    {
        if(RankType==0)
        {
            return User.InfinityRankPoint;
        }
        else if(RankType==1)
        {
            return User.HeroRankPoint;
        }
        else if(RankType==2)
        {
            return User.battleRankPoint;
        }
        else
        {
            return User.attackRankPoint;
        }
    }
    string GetUserRankText()
    {
        if(RankType==0)
        {
            return Common.GetThousandCommaText(Common.RankOfInfinityMode);
        }
        else if(RankType==1)
        {
            return Common.GetThousandCommaText(Common.RankOfHeroLevel);
        }
        else if(RankType==2)
        {
            return Common.GetThousandCommaText(Common.RankOfPvp);
        }
        else
        {
            return Common.GetThousandCommaText(Common.RankOfAttackMode);
        }
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
