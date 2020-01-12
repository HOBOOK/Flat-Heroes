using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InfinityCastleStatsUp : MonoBehaviour
{
    public Castle InfinityCastle;
    public Transform buttonsParentTransform;
    public Text timeInformationText;
    public Transform infinityCastleInfoTransform;

    List<Castle.CastleStatsType> castleStatsType;
    bool isStart = false;
    bool isSelected = false;
    float autoSelectTime;

    private void OnEnable()
    {
        castleStatsType = null;
        isStart = false;
        isSelected = false;
        autoSelectTime = 0.0f;
    }

    public void ShowCastleStatSelectUI()
    {
        StartCoroutine("ShowingCastleStatSelectUI");
    }
    IEnumerator ShowingCastleStatSelectUI()
    {
        this.transform.localScale = Vector3.one;
        castleStatsType = new List<Castle.CastleStatsType>();
        for (int i = 0; i < 3; i++)
        {
            int type = UnityEngine.Random.Range(0, 9);
            castleStatsType.Add((Castle.CastleStatsType)type);
            buttonsParentTransform.GetChild(i).GetComponentInChildren<Text>().text = GetStatTypeText(type);
            buttonsParentTransform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Castle/CastleStats" + (type + 1));
            buttonsParentTransform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            buttonsParentTransform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate {
                OnClickSelectStatButton(type);
            });
            yield return new WaitForFixedUpdate();
        }
        buttonsParentTransform.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0.0f;
        isStart = true;
    }

    void Update()
    {
        if(isStart)
        {
            SetTimeInformationText((5 - autoSelectTime));
            autoSelectTime += Time.unscaledDeltaTime;
            if(autoSelectTime>5.0f)
            {
                SelectStat((int)castleStatsType[UnityEngine.Random.Range(0,3)]);
            }
        }
    }
    void OnClickSelectStatButton(int type)
    {
        SelectStat(type);
    }
    void SelectStat(int type)
    {
        if(!isSelected)
        {
            if (InfinityCastle != null && !InfinityCastle.isDead)
            {
                InfinityCastle.CastleLevelUp((Castle.CastleStatsType)type);
            }
        }
        if (infinityCastleInfoTransform != null)
        {
            if(!infinityCastleInfoTransform.gameObject.activeSelf)
            {
                infinityCastleInfoTransform.gameObject.SetActive(true);
                infinityCastleInfoTransform.GetComponent<AiryUIAnimatedElement>().ShowElement();
            }

            infinityCastleInfoTransform.GetChild(1).GetComponent<Text>().text = string.Format("공격력 : {0}\r\n방어력 : {1}\r\n공격스피드 : {2}초\r\n발사체 수 : {3}\r\n초당 회복량 : {4}", InfinityCastle.attack, InfinityCastle.defence, InfinityCastle.attackSpeed, InfinityCastle.shotUp + 1, (InfinityCastle.autoHpUp * 100)+50);
        }
        Time.timeScale = User.isSpeedGame ? 1.3f : 1.0f;
        isSelected = true;
        StartCoroutine("CloseAnimation");
    }
    IEnumerator CloseAnimation()
    {
        this.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);
        this.gameObject.SetActive(false);
    }

    void SetTimeInformationText(float time)
    {
        if(timeInformationText!=null)
        {
            timeInformationText.text = User.language=="ko"? string.Format("<size='70'><color='red'>{0}</color></size>초 후 자동으로 선택됩니다.", time.ToString("N0")):
                 string.Format("Automatically selected after <size='70'><color='red'>{0}</color></size>seconds", time.ToString("N0"));
        }

    }
    string GetStatTypeText(int type)
    {
        return LocalizationManager.GetText("CastleStats" + (type + 1));
    }
}
