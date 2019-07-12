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
        ShowGetExp();
        ShowGetGold(StageManagement.instance.stageInfo.stageCoin);
        StartCoroutine("ShowGetItem");
    }
    public void ShowGetExp()
    {
        userinfo = StageManagement.instance.GetUserInfo();
        levelText.text = userinfo.level.ToString();
        int initexp = userinfo.exp;
        if (initexp + StageManagement.instance.stageInfo.stageExp > Common.USER_EXP_TABLE[(userinfo.level - 1)])
        {
            userinfo.departExp = (initexp + StageManagement.instance.stageInfo.stageExp) - Common.USER_EXP_TABLE[(userinfo.level - 1)];
        }
        StartCoroutine(ShowCountExp((initexp+StageManagement.instance.stageInfo.stageExp),initexp , expSlider));
    }
    IEnumerator ShowCountExp(float target, float current, Slider slider)
    {
        float duration = 1.5f; // 카운팅에 걸리는 시간 설정. 
        float offset = (target - current) / duration;
        while (current < target)
        {
            current += offset * Time.deltaTime;
            userinfo.exp = ((int)current);
            userinfo.LevelUp();
            if(userinfo.isLevelUp)
            {
                current = 0;
                target = userinfo.departExp;
                StartCoroutine(LevelUpTextEffect(levelText));
                userinfo.isLevelUp = false;
            }
            slider.value = ((float)current) / (float)Common.USER_EXP_TABLE[userinfo.level-1];
            slider.GetComponentInChildren<Text>().text = string.Format("{0}/{1}({2}%)", ((int)current), Common.USER_EXP_TABLE[userinfo.level-1], (slider.value*100).ToString("N0"));
            yield return null;
        }
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.coinGet);
        current = target;
        slider.GetComponentInChildren<Text>().text = string.Format("{0}/{1}({2}%)", ((int)current), Common.USER_EXP_TABLE[userinfo.level-1], (slider.value * 100).ToString("N0"));
    }
    IEnumerator LevelUpTextEffect(Text txt)
    {
        float size = 1.5f;
        while (size > 1)
        {
            txt.text = userinfo.level.ToString();
            txt.transform.localScale = new Vector3(size, size, size);
            size -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        txt.transform.localScale = Vector3.one;
        yield return null;
    }

    public void ShowGetGold(int amount)
    {
        StartCoroutine(ShowCount(amount, 0,GoldInfoTextl));
    }
    IEnumerator ShowGetItem()
    {
        yield return new WaitForSeconds(1f);
        List<Item> getItemsIdList = StageManagement.instance.GetStageItems();
        if(getItemsIdList!=null&& getItemsIdList.Count>0)
        {
            foreach(var item in getItemsIdList)
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
        else
        {
            yield return null;
        }
    }

    IEnumerator ShowCount(float target, float current,Text txt)
    {
        float duration = 1.5f; // 카운팅에 걸리는 시간 설정. 
        float offset = (target - current) / duration;
        while (current < target)
        {
            current += offset * Time.deltaTime;
            txt.text = ((int)current).ToString();
            yield return null;
        }
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.coinGet);
        current = target;
        txt.text = ((int)current).ToString();
    }
}
