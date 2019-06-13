using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageResult : MonoBehaviour
{
    public Text GoldInfoTextl;
    public GameObject GetItemInfoPanel;
    public GameObject GetItemSlotPrefab;

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
        ShowGetGold(StageManagement.instance.stageInfo.stageCoin);
        StartCoroutine("ShowGetItem");
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
                itemPrefab.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = ItemSystem.GetItemImage(item.id);
                itemPrefab.transform.GetComponentInChildren<Text>().text = item.name;
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
        float duration = 1f; // 카운팅에 걸리는 시간 설정. 
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
