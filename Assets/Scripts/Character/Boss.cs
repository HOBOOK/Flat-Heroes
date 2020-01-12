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
        }
        Common.hitTargetObject = this.gameObject;
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
            MissionSystem.AddClearPoint(MissionSystem.ClearType.BossKill);
            StartCoroutine("BossDead");
        }
    }

    public IEnumerator BossDead()
    {
        Camera.main.GetComponent<FollowCamera>().ChangeTarget(this.gameObject);
        Camera.main.GetComponent<CameraEffectHandler>().SetCameraSize(3.5f);
        yield return new WaitForSeconds(5.0f);
        StageManagement.instance.StageClear();
        this.gameObject.SetActive(false);
        yield return null;
    }
}
