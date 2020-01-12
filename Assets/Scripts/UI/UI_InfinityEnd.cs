using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InfinityEnd : MonoBehaviour
{
    public Text pointText;
    void Start()
    {
        StartCoroutine(ShowCount(StageManagement.instance.stageInfo.stagePoint, 1.5f, pointText));
    }
    IEnumerator ShowCount(float target, float current, Text txt)
    {
        float duration = 1.5f; // 카운팅에 걸리는 시간 설정. 
        float offset = (target - current) / duration;
        while (current < target)
        {
            current += offset * Time.deltaTime;
            txt.text = string.Format("{0} <size='60'>pts</size>", Common.GetThousandCommaText((int)current));
            yield return null;
        }
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.coinGet);
        current = target;
        txt.text = string.Format("{0} <size='60'>pts</size>", Common.GetThousandCommaText((int)current));
    }
}
