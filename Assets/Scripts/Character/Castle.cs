using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    public int id;
    public int hp;
    [HideInInspector]
    public int maxHp;
    public int defence;
    public bool isDead;
    int spawnCount;
    float spawnTime =1.0f;
    public float spawnDelay;
    float drainEnergyTime = 0.0f;
    float curSpawnTime = 0;
    public List<SpawnEnemy> spawnEnemys = new List<SpawnEnemy>();
    Transform enemySpawnPoint;
    bool isUnBeat;
    bool isShake;
    public bool isGod;
    Vector3 firstPos;
    private void Awake()
    {
        maxHp = hp;
        enemySpawnPoint = GameObject.Find("EnemysHero").transform;
        isGod = false;
    }
    void Start()
    {
        Common.hitTargetObject = this.gameObject;
        GUI_Manager.instance.OpenHpUI(this.gameObject);
        firstPos = this.transform.position;
        SetSpawnMonster();
        FirstSpawn();
        SetEffect();
    }
    void SetEffect()
    {
        GameObject shieldEffect = EffectPool.Instance.PopFromPool("ShieldYellow", this.transform);
        shieldEffect.transform.position = transform.position;
        shieldEffect.gameObject.SetActive(true);
        GameObject auraEffect = EffectPool.Instance.PopFromPool("MagicAuraYellow", this.transform);
        auraEffect.transform.position = transform.position;
        auraEffect.gameObject.SetActive(true);
    }
    void SetSpawnMonster()
    {
        SpawnEnemy spawnEnemy4 = new SpawnEnemy(PrefabsDatabaseManager.instance.GetMonsterPrefab(504), 1);
        spawnEnemys.Add(spawnEnemy4);
        SpawnEnemy spawnEnemy5 = new SpawnEnemy(PrefabsDatabaseManager.instance.GetMonsterPrefab(505), 1);
        spawnEnemys.Add(spawnEnemy5);
        SpawnEnemy spawnEnemy6 = new SpawnEnemy(PrefabsDatabaseManager.instance.GetMonsterPrefab(506), 1);
        spawnEnemys.Add(spawnEnemy6);
    }
    private void Update()
    {
        if(!isDead)
        {
            StateUpdate();
            SpawnUpdate();
        }
    }
    void StateUpdate()
    {
        if(hp<=0)
        {
            isDead = true;
            StopAllCoroutines();
            StartCoroutine("CastleExplosionEffect");
        }
        else
        {
            drainEnergyTime += Time.deltaTime;
            if(drainEnergyTime>5.0f)
            {
                StageManagement.instance.DrainStageEnergy();
                drainEnergyTime = 0;
            }
        }
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
                        e.transform.position = new Vector3(UnityEngine.Random.Range(10f,13f), e.transform.localScale.y);
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
        SpawnEffect();
        for (int i = 0; i < spawnEnemy.count; i++)
        {
            GameObject e = Instantiate(spawnEnemy.enemyPrefab, enemySpawnPoint);
            e.SetActive(false);
            e.GetComponent<Hero>().isPlayerHero = false;
            e.transform.position = this.transform.position;
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
    void Shake()
    {
        if(!isShake)
        {
            StartCoroutine("Shaking");
        }
    }
    IEnumerator Shaking()
    {
        isShake = true;
        float shakeTime = 0.1f;
        while(shakeTime>0)
        {
            this.transform.position = new Vector3((firstPos.x + Mathf.Sin(Time.time * 1) * 0.1f), (firstPos.y + (Mathf.Sin(Time.time * 1) * 0.1f)), 0);
            shakeTime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        isShake = false;
        this.transform.position = firstPos;
        yield return null;
    }
    public void Hitted(Collider2D collision, int dam, float kx, float ky)
    {
        CastleHitEffect();
        if(!isGod)
        {
            dam = Common.GetDamage(dam, defence);
            Shake();
            isUnBeat = true;
            if (collision.GetComponentInParent<Hero>() != null)
            {
                DamageUIShow(dam.ToString(), collision.GetComponentInParent<Hero>().isCriticalAttack);
            }
            else if (collision.GetComponent<arrowController>() != null)
            {
                DamageUIShow(dam.ToString(), collision.GetComponent<arrowController>().isCritical);
            }
            else if (collision.GetComponent<bulletController>() != null)
            {
                DamageUIShow(dam.ToString(), collision.GetComponent<bulletController>().isCritical);
            }
            hp = Common.looMinus(hp, dam);
            StartCoroutine(UnBeatTime(dam));
        }
    }
    public void HittedByObject(int dam, bool isCritical, Vector2 addforce)
    {
        CastleHitEffect();
        if (!isGod)
        {
            dam = Common.GetDamage(dam, defence);
            Shake();
            isUnBeat = true;
            DamageUIShow(dam.ToString(), isCritical);
            hp = Common.looMinus(hp, dam);
            StartCoroutine(UnBeatTime(dam));
        }


    }
    public void CastleHitEffect()
    {
        GameObject effect = EffectPool.Instance.PopFromPool("RoundHitRed");
        effect.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);
        effect.SetActive(true);
    }
    public void SpawnEffect()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.teleport);
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("SimplePortalRed");
            effect.transform.position = transform.position;
            effect.SetActive(true);
        }
    }
    public IEnumerator CastleExplosionEffect()
    {
        Camera.main.GetComponent<FollowCamera>().ChangeTarget(this.gameObject);
        Camera.main.GetComponent<CameraEffectHandler>().SetCameraSize(3.5f);
        yield return new WaitForSeconds(2.0f);
        for(int i = 0; i < 5; i++)
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
    IEnumerator UnBeatTime(int dam)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.damage2);
        while (isUnBeat)
        {
            yield return new WaitForFixedUpdate();
            isUnBeat = false;
        }
        isUnBeat = false;
        yield return null;
    }
    public void DamageUIShow(string font, bool isCritical = false)
    {
        GameObject damageUIprefab = ObjectPool.Instance.PopFromPool("damageUI", GameObject.Find("CanvasUI").transform) as GameObject;
        damageUIprefab.GetComponentInChildren<Text>().text = font.ToString();
        damageUIprefab.GetComponent<TextDamageController>().isLeft = false;
        damageUIprefab.GetComponent<TextDamageController>().isCritical = isCritical;
        damageUIprefab.GetComponent<TextDamageController>().isCC = false;
        damageUIprefab.transform.position = transform.position + new Vector3(0, 1);
        damageUIprefab.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isDead)
        {
            try
            {
                if (collision.gameObject.CompareTag("bullet") && !isUnBeat && collision.GetComponent<bulletController>() != null && collision.GetComponent<bulletController>().isAlly && collision.GetComponent<bulletController>().Target.GetInstanceID() == this.transform.GetInstanceID())
                {
                    collision.GetComponent<bulletController>().BulletStand(this.transform);
                }
                if (collision.gameObject.CompareTag("arrow") && !isUnBeat && collision.GetComponent<arrowController>() != null && collision.GetComponent<arrowController>().isAlly && !collision.GetComponent<arrowController>().isStand && collision.GetComponent<arrowController>().target.transform.GetInstanceID() == this.transform.GetInstanceID())
                {
                    collision.GetComponent<arrowController>().ArrowStand(this.transform);
                }
                if (collision.gameObject.layer == 9 && collision.GetComponent<Collider2D>()!=null&& collision.isTrigger && !isUnBeat && collision.GetComponentInParent<Hero>() != null && collision.GetComponentInParent<Hero>().isPlayerHero && collision.GetComponentInParent<Hero>().target.transform.GetInstanceID() == this.transform.GetInstanceID())
                {
                    Hitted(collision, collision.GetComponentInParent<Hero>().Damage(), 3f, 3f);
                }
            }
            catch(NullReferenceException e)
            {
                Debugging.LogWarning(e.ToString());
            }

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
