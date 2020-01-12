using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InfiniteSpawn : MonoBehaviour
{
    public GameObject SpawnNoticeUI;
    public float spawnDelay;
    public List<SpawnEnemy> spawnEnemys = new List<SpawnEnemy>();
    public Transform enemySpawnPointRight;
    public Transform enemySpawnPointLeft;
    public bool isGod;
    Vector3 firstPos;
    bool isBossSpawn = false;
    List<GameObject> tempEnemys = new List<GameObject>();
    private void Awake()
    {
        firstPos = this.transform.position;
    }
    public void Spawn(List<HeroData> monsterDataList)
    {
        spawnEnemys.Clear();

        foreach (var mon in monsterDataList)
        {
            SpawnEnemy spawnEnemy = new SpawnEnemy(PrefabsDatabaseManager.instance.GetMonsterPrefab(mon.id), Mathf.Clamp(1,1,5));
            spawnEnemys.Add(spawnEnemy);
        }
        if (spawnEnemys != null && spawnEnemys.Count > 0)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.teleport);
            SpawnEffect(enemySpawnPointRight.position);
            SpawnEffect(enemySpawnPointLeft.position);
            for (int i = 0; i < spawnEnemys.Count; i++)
            {
                if(StageManagement.instance.GetDPoint()<20)
                {
                    if (!spawnEnemys[i].isSpawnEnd)
                    {
                        Transform spawnTransform = i % 2 == 0 ? enemySpawnPointLeft : enemySpawnPointRight;
                        StartCoroutine(Spawning(spawnEnemys[i], spawnTransform));
                    }
                }
                else
                {
                    break;
                }
            }

        }
    }
    IEnumerator Spawning(SpawnEnemy spawnEnemy, Transform spawnPoint)
    {
        spawnEnemy.isSpawnEnd = true;
        isBossSpawn = false;
        tempEnemys.Clear();
        for (int i = 0; i < spawnEnemy.count; i++)
        {
            if(StageManagement.instance.GetDPoint()<20)
            {
                GameObject e = Instantiate(spawnEnemy.enemyPrefab, spawnPoint);
                e.SetActive(false);
                e.GetComponent<Hero>().isPlayerHero = false;
                e.GetComponent<Hero>().SpriteAlphaSetting(0);
                e.transform.localPosition = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0);
                yield return new WaitForSeconds(1.0f);
                e.SetActive(true);
                StageManagement.instance.SetDPoint(1);
                e.GetComponent<Hero>().SpriteAlphaSetting(1);
                yield return new WaitForSeconds(0.2f);
                e.GetComponent<Hero>().WaveBuff(StageManagement.instance.stageInfo.stageWave);
                if (e.GetComponent<Hero>().id > 1000)
                    isBossSpawn = true;
                if (isBossSpawn)
                    SpawnNotice();
                tempEnemys.Add(e);
                StageManagement.instance.AddMonsterCount();
                yield return new WaitForEndOfFrame();
            }
            else
            {
                break;
            }
        }
        if (tempEnemys != null && tempEnemys.Count > 0)
        {
            List<GameObject> allys = Common.FindAlly(true);
            foreach (var ally in allys)
            {
                if (ally.GetComponent<Hero>() != null)
                {
                    ally.GetComponent<Hero>().ResearchingEnemysAll(tempEnemys);
                }
            }
        }
        spawnEnemy.isSpawnEnd = false;
        yield return null;
    }
    public void SpawnEffect(Vector3 pos)
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("SoftPortalRed");
            effect.transform.position = pos;
            effect.SetActive(true);
        }
    }

    void SpawnNotice()
    {
        if(SpawnNoticeUI!=null)
        {
            SpawnNoticeUI.SetActive(true);
            SpawnNoticeUI.GetComponent<Animation>().Play();
        }
    }

    [Serializable]
    public class SpawnEnemy
    {
        public GameObject enemyPrefab;
        public int count;
        public bool isSpawnEnd = false;

        SpawnEnemy() { }
        public SpawnEnemy(GameObject prefab, int cnt)
        {
            enemyPrefab = prefab;
            count = cnt;
            isSpawnEnd = false;
        }
    }
}
