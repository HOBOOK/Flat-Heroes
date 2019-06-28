using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static void SkillUpgradeEffect(Transform parent=null)
    {
        GameObject effect = EffectPool.Instance.PopFromPool("SkillUpgradeEffect", parent);
        effect.transform.localScale = new Vector3(1, 1, 1);
        effect.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f);
        Vector3 pos = parent.transform.position;
        pos.z = 0;
        effect.transform.localPosition = pos;
        effect.gameObject.SetActive(true);
    }
}
