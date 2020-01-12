using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InfiniteSpawn : MonoBehaviour
{
    int waveNumber;
    int spawnCount;
    float spawnTime =1.0f;
    public float spawnDelay;
    float drainEnergyTime = 0.0f;
    float curSpawnTime = 0;
    public List<SpawnEnemy> spawnEnemys = new List<SpawnEnemy>();
    Transform enemySpawnPoint;
    public bool isGod;
    Vector3 firstPos;
    private void Awake()
    {
        enemySpawnPoint = GameObject.Find("EnemysHero").transform;
        firstPos = this.transform.position;
    }
    void Start()
    {
        FirstSpawn();
    }
    private void Update()
    {
        StateUpdate();
        SpawnUpdate();
    }
    void StateUpdate()
    {

    }
    void SpawnUpdate()
    {
        curSpawnTime += Time.deltaTime;
        if (curSpawnTime > spawnDelay)
        {
            Spawn();
            curSpawnTime = 0.0f;
        }
    }
    void FirstSpawn()
    {
        if (spawnEnemys != null && spawnEnemys.Count > 0 && spawnCount < 15)
        {
            Debugging.Log(this.name + " 소환 시작");
            for (int i = 0; i < spawnEnemys.Count; i++)
            {
                if (!spawnEnemys[i].isSpawnEnd && spawnCount < 15)
                {
                    spawnEnemys[i].isSpawnEnd = true;
                    for (int j = 0; j < spawnEnemys[i].count; j++)
                    {
                        GameObject e = Instantiate(spawnEnemys[i].enemyPrefab, enemySpawnPoint);
                        e.SetActive(false);
                        e.GetComponent<Hero>().isPlayerHero = false;
                        e.transform.position = new Vector3(UnityEngine.Random.Range(15,18), 0);
                        e.SetActive(true);
                        StageManagement.instance.AddMonsterCount();
                    }
                    spawnEnemys[i].isSpawnEnd = false;
                }
            }
        }
        spawnCount = Common.FindEnemysCount();
    }
    void Spawn()
    {
        if (spawnEnemys != null && spawnEnemys.Count > 0&&spawnCount<15)
        {
            Debugging.Log(this.name + " 소환");
            for (int i = 0; i < spawnEnemys.Count; i++)
            {
                if (!spawnEnemys[i].isSpawnEnd&&spawnCount<15)
                {
                    StartCoroutine(Spawning(spawnEnemys[i]));
                }
            }
        }
    }
    IEnumerator Spawning(SpawnEnemy spawnEnemy)
    {
        spawnEnemy.isSpawnEnd = true;

        for (int i = 0; i < spawnEnemy.count; i++)
        {
            GameObject e = Instantiate(spawnEnemy.enemyPrefab, enemySpawnPoint);
            e.SetActive(false);
            e.GetComponent<Hero>().isPlayerHero = false;
            e.transform.position = new Vector3(18, 0);
            yield return new WaitForEndOfFrame();
            e.SetActive(true);
            StageManagement.instance.AddMonsterCount();
            List<GameObject> allys = Common.FindAlly();
            foreach(var ally in allys)
            {
                if (ally.GetComponent<Hero>() != null)
                {
                    ally.GetComponent<Hero>().ResearchingEnemys(e);
                }
            }
            yield return new WaitForSeconds(spawnTime);
        }
        spawnEnemy.isSpawnEnd = false;
        spawnCount = Common.FindEnemysCount();
        yield return null;
    }
    [Serializable]
    public class SpawnEnemy
    {
        public GameObject enemyPrefab;
        public int count;
        public bool isSpawnEnd = false;
    }

}
