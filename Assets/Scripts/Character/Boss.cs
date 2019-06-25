using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int hp;
    [HideInInspector]
    public int maxHp;
    bool isEnd = false;
    public Hero bossPrefabData;
    public void StartBoss()
    {
        isEnd = false;
        if (bossPrefabData == null)
        {
            bossPrefabData = this.GetComponent<Hero>();
            maxHp = bossPrefabData.status.maxHp;
            hp = bossPrefabData.status.hp;
            GUI_Manager.instance.OpenHpUI(this.gameObject);
        }
    }
    private void Update()
    {
        if(bossPrefabData!=null&&!isEnd)
        {
            StateUpdate();
        }
    }

    void StateUpdate()
    {
        hp = bossPrefabData.status.hp;
        if (bossPrefabData.isDead)
        {
            isEnd = true;
            StartCoroutine("BossDead");
        }
    }

    public IEnumerator BossDead()
    {
        Camera.main.GetComponent<FollowCamera>().ChangeTarget(this.gameObject);
        Camera.main.GetComponent<CameraEffectHandler>().SetCameraSize(3.5f);
        yield return new WaitForSeconds(2.0f);
        for (int i = 0; i < 5; i++)
        {
            Common.isHitShake = true;
            GameObject effect = EffectPool.Instance.PopFromPool("ExplosionRoundFire");
            effect.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f), 0);
            effect.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }
        StageManagement.instance.StageClear();
        this.gameObject.SetActive(false);
        yield return null;
    }
}
