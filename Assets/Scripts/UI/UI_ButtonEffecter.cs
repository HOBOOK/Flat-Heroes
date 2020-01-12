using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ButtonEffecter : MonoBehaviour
{
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
    public void Effect002(Transform target=null)
    {
        if (target == null) target = this.transform;
        GameObject effect = EffectPool.Instance.PopFromPool("BattleEffect", target);
        effect.transform.localScale = new Vector3(1, 1, 1);
        if(effect.GetComponent<RectTransform>()!=null)
            effect.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f);
        Vector3 pos = target.position;
        pos.z = 0;
        effect.transform.localPosition = pos;
        effect.gameObject.SetActive(true);
    }
}
