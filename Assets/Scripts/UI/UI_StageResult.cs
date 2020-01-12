using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageResult : MonoBehaviour
{
    public Text GoldInfoTextl;
    public Text levelText;
    public Slider expSlider;
    public GameObject GetItemInfoPanel;
    public GameObject GetItemSlotPrefab;
    public GameObject buttonParent;
    public GameObject premiumCoinInformation;
    UserInfo userinfo;
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
        if(buttonParent!=null)
            buttonParent.SetActive(false);
        GetItemInfoPanel.SetActive(false);
        if(premiumCoinInformation!=null)
        {
            if (SaveSystem.IsPremiumPassAble())
                premiumCoinInformation.SetActive(true);
            else
                premiumCoinInformation.SetActive(false);
        }

        ShowGetExp();
    }
    public void ShowGetExp()
    {
        userinfo = StageManagement.instance.GetUserInfo();
        levelText.text = string.Format("Lv. {0}",userinfo.level.ToString());
        int initexp = userinfo.exp;
        if (initexp + StageManagement.instance.stageInfo.stageExp >= GetUserNeedExp(userinfo.level))
        {
            userinfo.departExp = (initexp + StageManagement.instance.stageInfo.stageExp) - GetUserNeedExp(userinfo.level);
        }
        StartCoroutine(ShowCountExp((initexp+StageManagement.instance.stageInfo.stageExp),initexp , expSlider));
    }
    public int GetUserNeedExp(int level)
    {
        return 1000 + (int)(1000 * level * level * 0.1f);
    }
    IEnumerator ShowCountExp2(Slider slider)
    {
        Text expText = slider.transform.GetChild(5).GetComponent<Text>();
        expText.text = string.Format("+ {0} Exp", StageManagement.instance.stageInfo.stageExp);
        float time = 1.0f;
        while (time > 0.0f)
        {
            expText.color = new Color(1, 1, 1, time);
            expText.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, 50 - time * 50);
            time -= Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        expText.color = new Color(1, 1, 1, 0);
    }
    IEnumerator ShowCountExp(float target, float current, Slider slider)
    {
        StartCoroutine(ShowCountExp2(slider));
        float duration = 1.5f; // 카운팅에 걸리는 시간 설정. 
        float offset = (target - current) / duration;
        while (current < target)
        {
            duration -= Time.unscaledDeltaTime;
            current += offset * Time.unscaledDeltaTime;
            userinfo.exp = ((int)current);
            if (userinfo.exp >= GetUserNeedExp(userinfo.level))
            {
                userinfo.level += 1;
                userinfo.exp = 0;
                current = 0;
                target = userinfo.departExp;
                offset = (target - current) / duration;
                StartCoroutine(LevelUpTextEffect(levelText));
                userinfo.isLevelUp = false;
                slider.value = ((float)current) / (float)Common.GetUserNeedExp();
                slider.transform.GetChild(4).GetComponent<Text>().text = string.Format("{0}/{1}({2}%)", 0, GetUserNeedExp(userinfo.level), (slider.value * 100).ToString("N0"));
                yield return new WaitForFixedUpdate();
            }
            else
            {
                slider.value = ((float)current) / (float)Common.GetUserNeedExp();
                slider.GetComponentInChildren<Text>().text = string.Format("{0}/{1}({2}%)", ((int)current), GetUserNeedExp(userinfo.level), (slider.value * 100).ToString("N0"));
            }
            yield return null;
        }
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.coinGet);
        current = target;
        slider.GetComponentInChildren<Text>().text = string.Format("{0}/{1}({2}%)", ((int)current), GetUserNeedExp(userinfo.level), (slider.value * 100).ToString("N0"));
        ShowGetGold(StageManagement.instance.stageInfo.stageCoin);
    }
    IEnumerator LevelUpTextEffect(Text txt)
    {
        float size = 1.5f;
        while (size > 1)
        {
            txt.text = string.Format("Lv. {0}",userinfo.level.ToString());
            txt.transform.localScale = new Vector3(size, size, size);
            size -= Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        txt.transform.localScale = Vector3.one;
        yield return null;
    }

    public void ShowGetGold(int amount)
    {
        StartCoroutine(ShowGoldCount(amount, 0,GoldInfoTextl));
    }

    IEnumerator ShowGetItem()
    {
        List<Item> getItemsIdList = StageManagement.instance.GetStageItems();
        if(getItemsIdList!=null&& getItemsIdList.Count>0)
        {
            GetItemInfoPanel.SetActive(true);
            foreach (var item in getItemsIdList)
            {
                SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
                GameObject itemPrefab = Instantiate(GetItemSlotPrefab, GetItemInfoPanel.transform);

                itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemClassImage(item.id);
                itemPrefab.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemImage(item.id);
                itemPrefab.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = ItemColor.GetItemColor(item.itemClass);

                itemPrefab.transform.GetComponentInChildren<Text>().text = ItemSystem.GetItemName(item.id);
                if (itemPrefab.GetComponent<AiryUIAnimatedElement>() != null)
                    itemPrefab.GetComponent<AiryUIAnimatedElement>().ShowElement();
                yield return new WaitForSeconds(0.2f);
            }
        }
        if (buttonParent != null)
        {
            buttonParent.SetActive(true);
            buttonParent.GetComponent<AiryUIAnimatedElement>().ShowElement();
        }
    }

    IEnumerator ShowCount(float target, float current,Text txt)
    {
        float duration = 1.5f; // 카운팅에 걸리는 시간 설정. 
        float offset = (target - current) / duration;
        while (current < target)
        {
            current += offset * Time.unscaledDeltaTime;
            txt.text = Common.GetThousandCommaText((int)current);
            yield return null;
        }
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.coinGet);
        current = target;
        txt.text = Common.GetThousandCommaText((int)current);
    }

    IEnumerator ShowGoldCount(float target, float current, Text txt)
    {
        float duration = 1.2f; // 카운팅에 걸리는 시간 설정. 
        float offset = (target - current) / duration;
        while (current < target)
        {
            current += offset * Time.unscaledDeltaTime;
            txt.text = string.Format("+ {0}",Common.GetThousandCommaText((int)current));
            yield return null;
        }
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.coinGet);
        current = target;
        txt.text = string.Format("+ {0}", Common.GetThousandCommaText((int)current));
        StartCoroutine("ShowGetItem");
        yield return null;
    }


}
