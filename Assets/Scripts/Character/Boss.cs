using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    int id = 0;
    bool isEnd = false;
    public Hero bossPrefabData;
    float bossSkillDelay = 15f;
    float skillDelayTime = 0.0f;
    public void StartBoss()
    {
        isEnd = false;
        if (bossPrefabData == null)
        {
            bossPrefabData = this.GetComponent<Hero>();
            id = bossPrefabData.id;
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
    bool IsSkillAbleBossId()
    {
        if (bossPrefabData.id == 1007 || bossPrefabData.id == 1006||bossPrefabData.id==1008)
            return true;
        return false;
    }

    void StateUpdate()
    {
        if (bossPrefabData.isDead)
        {
            isEnd = true;
            MissionSystem.AddClearPoint(MissionSystem.ClearType.BossKill);
            StartCoroutine("BossDead");
        }
        else
        {
            if(StageManagement.instance.isStageStart()&& IsSkillAbleBossId())
            {
                skillDelayTime += Time.deltaTime;
                if(skillDelayTime>bossSkillDelay)
                {
                    Debugging.Log(id + " 보스 스킬발동");
                    bossPrefabData.SkillAttack();
                    skillDelayTime = 0.0f;
                }
            }
        }
    }

    public IEnumerator BossDead()
    {
        SoundManager.instance.BgmSourceChange(null);
        Camera.main.GetComponent<FollowCamera>().ChangeTarget(this.gameObject);
        Camera.main.GetComponent<CameraEffectHandler>().SetCameraSize(3.5f);
        yield return new WaitForSeconds(3f);
        if (Common.GetSceneCompareTo(Common.SCENE.BOSS))
            StageManagement.instance.StageBoissEnd();
        else
            StageManagement.instance.StageClear();
        this.gameObject.SetActive(false);
        yield return null;
    }
}
