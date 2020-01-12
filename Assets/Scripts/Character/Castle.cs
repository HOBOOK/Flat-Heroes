using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    #region 변수
    public int id;
    public int hp;
    public int maxHp;
    public int defence;
    public int attack;
    public float attackSpeed;
    public bool isDead;
    public bool isPunchingBag = false;
    int spawnCount;
    int spawnLimitCount;
    float spawnTime;
    float drainEnergyTime = 0.0f;
    public List<SpawnEnemy> spawnEnemys = new List<SpawnEnemy>();
    Transform enemySpawnPoint;
    bool isUnBeat;
    bool isShake;
    public bool isGod;
    public bool isPlayerCastle;
    public bool isInfinityCastle;
    bool isAttack;
    int stageNumber;
    int mapNumber;
    float attackDelay;
    
    Vector3 firstPos;

    GameObject shieldEffect;
    GameObject auraEffect;
    GameObject hpUI;


    #endregion
    private void Awake()
    {
        enemySpawnPoint = GameObject.Find("EnemysHero").transform;
        isGod = false;
    }
    void Start()
    {
        firstPos = this.transform.position;
        if(!isPunchingBag)
        {
            SetCastle();
            if (isPlayerCastle)
            {
                Common.allyTargetObject = this.gameObject;
                //SetEffect();
            }
            else
            {
                Common.hitTargetObject = this.gameObject;
                SetSpawnMonster();
                //FirstSpawn();
                //SetEffect();
                StartCoroutine("Spawn");
            }
            OpenHpBar(isPlayerCastle);
        }
        else
        {
            Common.hitTargetObject = this.gameObject;
            isGod = true;
        }

    }
    private void Update()
    {
        if (!isDead)
        {
            StateUpdate();
        }
    }
    void SetCastle()
    {
        if(isInfinityCastle)
        {
            InitInfinityCastle();
            SetCastleSprite(true);
        }
        else
        {
            if (!isPlayerCastle)
            {
                mapNumber = StageManagement.instance.stageInfo.mapNumber;
                stageNumber = StageManagement.instance.stageInfo.stageNumber;
                spawnLimitCount = 2 + (int)((mapNumber - (stageNumber - 1) * 10) * 0.5f) + stageNumber;
                spawnTime = 2;
                maxHp = 500 + (int)(1000 * stageNumber * stageNumber * 0.5f);
                hp = maxHp;
                defence = 100 + (int)(100 * stageNumber * stageNumber * 0.2f);
            }
            else
            {
                maxHp = 1000 + 50 * User.level;
                defence = 300 + 15 * User.level;
                hp = maxHp;
            }
            SetCastleSprite(isPlayerCastle);
        }
    }
    #region 인피티니 모드
    public enum CastleStatsType { MaxHpUp, AutoHpUp, DefUp, AtkUp, AspeedUp, AllKill, God, ShotUp, Revive };
    int maxHpUp;
    public int autoHpUp;
    int defUp;
    int atkUp;
    int aSpeedUp;
    public int shotUp;

    float hpUpTime;

    void InitInfinityCastle()
    {
        maxHp = 5000;
        defence = 800;
        attack = 500;
        attackSpeed = 2.5f;
        hp = maxHp;
        maxHpUp = 0;
        autoHpUp = 0;
        defUp = 0;
        atkUp = 0;
        aSpeedUp = 0;
        shotUp = 0;
    }
    void UpdateInfinityCastle()
    {
        if(StageManagement.instance.isStageStart())
        {
            InfinityCastleHpUp();
            Attack();
        }
    }
    void InfinityCastleHpUp()
    {
        hpUpTime += Time.deltaTime;
        if(hpUpTime>1.0f)
        {
            this.hp = Mathf.Clamp(hp + ((autoHpUp * 100)+50), 0, this.maxHp);
            hpUpTime = 0.0f;
        }
    }
    public void CastleLevelUp(CastleStatsType type)
    {
        switch(type)
        {
            case CastleStatsType.MaxHpUp:
                maxHpUp += 1;
                maxHp = 5000 + maxHpUp*maxHpUp*2000;
                hp = maxHp;
                break;
            case CastleStatsType.AutoHpUp:
                autoHpUp += 1;
                break;
            case CastleStatsType.DefUp:
                defUp += 1;
                defence = 800 + 500 * defUp*defUp;
                break;
            case CastleStatsType.AtkUp:
                atkUp += 1;
                attack = 500 + (atkUp * atkUp * (atkUp*300));
                break;
            case CastleStatsType.AspeedUp:
                aSpeedUp += 1;
                attackSpeed = 2.5f - (aSpeedUp * 0.3f);
                break;
            case CastleStatsType.AllKill:
                List<GameObject> enemyList = Common.FindEnemy(true);
                foreach(var h in enemyList)
                {
                    if(h.GetComponent<Hero>()!=null&&!h.GetComponent<Hero>().isDead)
                    {
                        h.GetComponent<Hero>().status.hp = 0;
                    }
                }
                break;
            case CastleStatsType.God:
                StartCoroutine("GodMode");
                break;
            case CastleStatsType.ShotUp:
                shotUp += 1;
                break;
            case CastleStatsType.Revive:
                var deadAllyList = Common.FindDeadAlly();
                foreach(var h in deadAllyList)
                {
                    CharactersManager.instance.ResurrectionHero(h.GetComponent<Hero>().id);
                }
                break;
        }
    }
    IEnumerator GodMode()
    {
        isGod = true;
        SetEffect();
        yield return new WaitForSeconds(15.0f);
        isGod = false;
        OffEffect();
    }

    public void Attack()
    {
        attackDelay += Time.deltaTime;
        if (attackDelay > attackSpeed && !isAttack)
        {
            StartCoroutine("Attacking");
        }
    }
    IEnumerator Attacking()
    {
        isAttack = true;
        for(var i = 0; i < shotUp+1; i++)
        {
            var targets = Common.FindEnemy(true);
            if (targets != null && targets.Count > 0)
            {
                GameObject target = targets[UnityEngine.Random.Range(0, targets.Count)];
                if (target != null && target.GetComponent<Hero>() != null)
                {
                    StartCoroutine(AttackingMissile(target));
                    yield return null;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
        attackDelay = 0.0f;
        isAttack = false;
    }
    #endregion
    IEnumerator AttackingMissile(GameObject target)
    {
        Hero targetData = target.GetComponent<Hero>();
        GameObject missile = EffectPool.Instance.PopFromPool("NovaMissileBlue");
        missile.transform.position = this.transform.position;
        missile.gameObject.SetActive(true);
        Vector3 missilePos = missile.transform.position;
        Vector3 targetPos = target.transform.position;

        while (missile.transform.position.y < this.transform.position.y + 1.5f && target != null)
        {
            missilePos = Vector2.Lerp(missile.transform.position, this.transform.position + new Vector3(0, 2, 0), 0.08f);
            missilePos.z = 100;
            missile.transform.position = missilePos;
            yield return new WaitForEndOfFrame();
        }
        float distance = Vector2.Distance(missile.transform.position, targetPos);
        while (distance > 0.2f)
        {
            if (target != null && targetData != null && !targetData.isDead)
                targetPos = target.transform.position;
            distance = Vector2.Distance(missile.transform.position, targetPos);
            missilePos = Vector2.MoveTowards(missile.transform.position, targetPos, 0.5f);
            missilePos.z = 100;
            missile.transform.position = missilePos;
            yield return new WaitForEndOfFrame();
        }
        if (targetData != null)
        {
            targetData.HittedByObject((int)attack, false, new Vector2(30, 10), 0.8f);
        }
        if (missile != null&&missile.GetComponent<BackObjectPool>()!=null)
            missile.GetComponent<BackObjectPool>().PushToPool();
    }

    void SetCastleSprite(bool isAlly)
    {
        this.GetComponent<SpriteRenderer>().sprite = isAlly ?  Resources.Load<Sprite>("Castle/CastleAlly") : Resources.Load<Sprite>("Castle/CastleEnemy");
    }

    private void OnDisable()
    {
        OffEffect();
    }
    void StateUpdate()
    {
        if (hp <= 0)
        {
            isDead = true;
            OffEffect();
            StopAllCoroutines();
            StartCoroutine("CastleExplosionEffect");
        }
        else
        {
            if(isInfinityCastle)
            {
                UpdateInfinityCastle();
            }
        }
    }
    void SetEffect()
    {
        shieldEffect = EffectPool.Instance.PopFromPool("ShieldYellow", this.transform);
        shieldEffect.transform.position = transform.position;
        shieldEffect.gameObject.SetActive(true);
        auraEffect = EffectPool.Instance.PopFromPool("MagicAuraYellow", this.transform);
        auraEffect.transform.position = transform.position;
        auraEffect.gameObject.SetActive(true);
    }
    void OffEffect()
    {
        if(shieldEffect!=null)
        {
            EffectPool.Instance.PushToPool("ShieldYellow",shieldEffect);
        }
        if (auraEffect != null)
        {
            EffectPool.Instance.PushToPool("MagicAuraYellow", auraEffect);
        }
    }
    void SetSpawnMonster()
    {
        int mapID = StageManagement.instance.stageInfo.mapNumber;
        int stageNumber = StageManagement.instance.stageInfo.stageNumber;
        foreach (var mon in HeroSystem.GetStageMonster(mapID, stageNumber))
        {
            SpawnEnemy spawnEnemy = new SpawnEnemy(PrefabsDatabaseManager.instance.GetMonsterPrefab(mon.id), 1);
            spawnEnemys.Add(spawnEnemy);
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
    IEnumerator Spawn()
    {
        while(!StageManagement.instance.isStageStart())
        {
            yield return null;
        }
        int r = 0;
        while(spawnEnemys != null && spawnLimitCount > 0)
        {
            Debugging.Log(this.name + " 소환");
            for (int i = 0; i < spawnLimitCount; i++)
            {
                r = UnityEngine.Random.Range(0, spawnEnemys.Count);
                if (!spawnEnemys[r].isSpawnEnd&&spawnCount<15)
                {
                    yield return StartCoroutine(Spawning(spawnEnemys[r]));
                }
            }
            yield return new WaitForSeconds(15);
        }
        yield return null;
    }
    IEnumerator Spawning(SpawnEnemy spawnEnemy)
    {

        spawnEnemy.isSpawnEnd = true;
        SpawnEffect();
        GameObject e = Instantiate(spawnEnemy.enemyPrefab, enemySpawnPoint);
        e.SetActive(false);
        e.GetComponent<Hero>().isPlayerHero = false;
        e.transform.position = this.transform.position;
        yield return new WaitForEndOfFrame();
        e.SetActive(true);
        StageManagement.instance.AddMonsterCount();
        List<GameObject> allys = Common.FindAlly(true);
        foreach (var ally in allys)
        {
            if (ally.GetComponent<Hero>() != null)
            {
                ally.GetComponent<Hero>().ResearchingEnemys(e);
            }
        }
        yield return new WaitForSeconds(spawnTime);
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
        if(!isGod)
            hp = Common.looMinus(hp, dam);
        if (isPunchingBag)
            StageManagement.instance.AddAttackPoint(dam);
        StartCoroutine(UnBeatTime(dam));
    }
    public void HittedByObject(int dam, bool isCritical, Vector2 addforce)
    {
        CastleHitEffect();
        dam = Common.GetDamage(dam, defence);
        Shake();
        isUnBeat = true;
        DamageUIShow(dam.ToString(), isCritical);
        if(!isGod)
            hp = Common.looMinus(hp, dam);
        if (isPunchingBag)
            StageManagement.instance.AddAttackPoint(dam);
        StartCoroutine(UnBeatTime(dam));
    }
    public void CastleHitEffect()
    {
        GameObject effect = EffectPool.Instance.PopFromPool("RoundHitRed");
        effect.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);
        effect.SetActive(true);
    }
    public void SpawnEffect()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("SoftPortalRed");
            effect.transform.position = transform.position;
            effect.SetActive(true);
        }
    }
    public void BoomEffect()
    {
        GameObject effect = EffectPool.Instance.PopFromPool("ExplosionRoundFire");
        effect.transform.position = transform.position;
        effect.SetActive(true);
    }
    public IEnumerator CastleExplosionEffect()
    {
        SoundManager.instance.BgmSourceChange(null);

        if (isPlayerCastle)
            Common.allyTargetObject = null;
        else
            Common.hitTargetObject = null;
        Camera.main.GetComponent<FollowCamera>().ChangeTarget(this.gameObject);
        //Camera.main.GetComponent<CameraEffectHandler>().SetCameraSize(3.5f);
        yield return new WaitForSeconds(2.0f);
        for(int i = 0; i < 5; i++)
        {
            Common.isHitShake = true;
            GameObject effect = EffectPool.Instance.PopFromPool("ExplosionRoundFire");
            effect.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f), 0);
            effect.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }
        if (isPlayerCastle)
            StageManagement.instance.StageFail();
        else
            StageManagement.instance.StageClear();
        this.gameObject.SetActive(false);
        yield return null;
    }
    IEnumerator UnBeatTime(int dam)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.damage2);
        ShowHpBar(dam);
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
        damageUIprefab.GetComponent<TextDamageController>().isFixed = false;
        damageUIprefab.transform.position = transform.position + new Vector3(0, 1);
        damageUIprefab.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isDead)
        {
            try
            {
                if (collision.gameObject.CompareTag("bullet") && !isUnBeat && collision.GetComponent<bulletController>() != null && collision.GetComponent<bulletController>().isAlly != isPlayerCastle && collision.GetComponent<bulletController>().Target.GetInstanceID() == this.transform.GetInstanceID())
                {
                    collision.GetComponent<bulletController>().BulletStand(this.transform);
                }
                if (collision.gameObject.CompareTag("arrow") && !isUnBeat && collision.GetComponent<arrowController>() != null && collision.GetComponent<arrowController>().isAlly != isPlayerCastle && !collision.GetComponent<arrowController>().isStand && collision.GetComponent<arrowController>().target.transform.GetInstanceID() == this.transform.GetInstanceID())
                {
                    collision.GetComponent<arrowController>().ArrowStand(this.transform);
                }
                if (collision.gameObject.layer == 9 && collision.GetComponent<Collider2D>()!=null&& collision.isTrigger && !isUnBeat && collision.GetComponentInParent<Hero>() != null && collision.GetComponentInParent<Hero>().isPlayerHero!=isPlayerCastle && collision.GetComponentInParent<Hero>().target.transform.GetInstanceID() == this.transform.GetInstanceID())
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



    #region UI
    private void OpenHpBar(bool isBlue = false)
    {
        hpUI = ObjectPool.Instance.PopFromPool("hpCastleUI");
        hpUI.GetComponent<UI_castleHp>().OpenHpUI(this.gameObject, isBlue);
        hpUI.gameObject.SetActive(true);
    }
    private void ShowHpBar(int dam = 0)
    {
        if (!isDead && hp > 0 && hpUI != null)
        {
            if (!hpUI.gameObject.activeSelf)
            {
                hpUI.GetComponent<UI_castleHp>().panelHpTime = 0;
                hpUI.gameObject.SetActive(true);
            }
            if (dam > 0)
                hpUI.GetComponent<UI_castleHp>().GetDamage(dam);
        }
    }
    #endregion

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
