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
    Transform enemySpawnPoint;
    public bool isGod;
    Vector3 firstPos;
    bool isBossSpawn = false;
    private void Awake()
    {
        enemySpawnPoint = GameObject.Find("EnemysHero").transform;
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
            Debugging.Log(this.name + " 소환");
            for (int i = 0; i < spawnEnemys.Count; i++)
            {
                if (!spawnEnemys[i].isSpawnEnd)
                {
                    StartCoroutine(Spawning(spawnEnemys[i]));
                }
            }
        }
    }
    IEnumerator Spawning(SpawnEnemy spawnEnemy)
    {
        spawnEnemy.isSpawnEnd = true;
        isBossSpawn = false;
        for (int i = 0; i < spawnEnemy.count; i++)
        {
            GameObject e = Instantiate(spawnEnemy.enemyPrefab, enemySpawnPoint);
            e.SetActive(false);
            e.GetComponent<Hero>().isPlayerHero = false;
            e.GetComponent<Hero>().SpriteAlphaSetting(0);
            e.transform.position = new Vector3(UnityEngine.Random.Range(10f,12f),0);
            yield return new WaitForSeconds(1.0f);
            e.SetActive(true);
            e.GetComponent<Hero>().SpriteAlphaSetting(1);
            yield return new WaitForSeconds(0.2f);
            e.GetComponent<Hero>().WaveBuff(StageManagement.instance.stageInfo.stageWave);
            if (e.GetComponent<Hero>().id > 1000)
                isBossSpawn = true;
            StageManagement.instance.AddMonsterCount();
            List<GameObject> allys = Common.FindAlly(true);
            foreach(var ally in allys)
            {
                if (ally.GetComponent<Hero>() != null)
                {
                    ally.GetComponent<Hero>().ResearchingEnemys(e);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        spawnEnemy.isSpawnEnd = false;
        yield return null;
    }

    void SpawnNotice()
    {
        if(SpawnNoticeUI!=null)
        {
            SpawnNoticeUI.SetActive(true);
            SpawnNoticeUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
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
