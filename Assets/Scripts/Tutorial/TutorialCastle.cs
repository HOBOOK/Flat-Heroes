using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class TutorialCastle : MonoBehaviour
{
    public int id;
    public int hp;
    [HideInInspector]
    public int maxHp;
    public int defence;
    public bool isDead;
    int spawnCount;
    public float spawnDelay;
    float drainEnergyTime = 0.0f;
    public SpawnEnemy spawnEnemys;
    public GameObject enemyPrefab;
    Transform enemySpawnPoint;
    bool isUnBeat;
    bool isShake;
    public bool isGod;
    public bool isPlayerCastle;
    Vector3 firstPos;

    GameObject shieldEffect;
    GameObject auraEffect;
    GameObject hpUI;
    private void Awake()
    {
        maxHp = hp;
        enemySpawnPoint = GameObject.Find("EnemysHero").transform;
        isGod = false;
    }
    void Start()
    {
        firstPos = this.transform.position;
        if (isPlayerCastle)
        {
            Common.allyTargetObject = this.gameObject;
            SetEffect();
        }
        else
        {
            Common.hitTargetObject = this.gameObject;
            SetSpawnMonster();
            SetEffect();
        }
        //GUI_Manager.instance.OpenHpUI(this.gameObject);
        OpenHpBar(isPlayerCastle);
    }
    private void Update()
    {
        if (!isDead)
        {
            StateUpdate();
        }
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
        if (!isPlayerCastle)
        {
            if (hp > 0)
            {
                drainEnergyTime += Time.deltaTime;
                if (drainEnergyTime > 5.0f)
                {
                    drainEnergyTime = 0;
                }
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
        if (shieldEffect != null)
        {
            EffectPool.Instance.PushToPool("ShieldYellow", shieldEffect);
        }
        if (auraEffect != null)
        {
            EffectPool.Instance.PushToPool("MagicAuraYellow", auraEffect);
        }
    }
    void SetSpawnMonster()
    {
        SpawnEnemy spawnEnemy = new SpawnEnemy(enemyPrefab, 3);
        spawnEnemys = spawnEnemy;
    }
    void Shake()
    {
        if (!isShake)
        {
            StartCoroutine("Shaking");
        }
    }
    IEnumerator Shaking()
    {
        isShake = true;
        float shakeTime = 0.1f;
        while (shakeTime > 0)
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
        if (!isGod)
        {
            dam = Common.GetDamage(dam, defence);
            Shake();
            isUnBeat = true;
            if (collision.GetComponentInParent<TutorialHero>() != null)
            {
                DamageUIShow(dam.ToString(), collision.GetComponentInParent<TutorialHero>().isCriticalAttack);
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
            GameObject effect = EffectPool.Instance.PopFromPool("SoftPortalRed");
            effect.transform.position = transform.position;
            effect.SetActive(true);
        }
    }
    public void Spawn()
    {
        StartCoroutine(Spawning(spawnEnemys));
    }
    IEnumerator Spawning(SpawnEnemy spawnEnemy)
    {
        spawnEnemy.isSpawnEnd = true;
        for(var i = 0; i < spawnEnemy.count; i++)
        {
            SpawnEffect();
            GameObject e = Instantiate(spawnEnemy.enemyPrefab, enemySpawnPoint);
            e.SetActive(false);
            e.GetComponent<TutorialHero>().isPlayerHero = false;
            e.transform.position = this.transform.position;
            yield return new WaitForEndOfFrame();
            e.SetActive(true);
            List<GameObject> allys = Common.FindAlly(true);
            foreach (var ally in allys)
            {
                if (ally.GetComponent<TutorialHero>() != null)
                {
                    ally.GetComponent<TutorialHero>().ResearchingEnemys(e);
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
        spawnEnemy.isSpawnEnd = false;
        yield return null;
    }
    public IEnumerator CastleExplosionEffect()
    {
        if (isPlayerCastle)
            Common.allyTargetObject = null;
        else
            Common.hitTargetObject = null;
        for (int i = 0; i < 5; i++)
        {
            Common.isHitShake = true;
            GameObject effect = EffectPool.Instance.PopFromPool("ExplosionRoundFire");
            effect.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f), 0);
            effect.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }
        TutorialStageManager.instance.EndTutorial();
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
        damageUIprefab.transform.position = transform.position + new Vector3(0, 1);
        damageUIprefab.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDead)
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
                if (collision.gameObject.layer == 9 && collision.GetComponent<Collider2D>() != null && collision.isTrigger && !isUnBeat && collision.GetComponentInParent<TutorialHero>() != null && collision.GetComponentInParent<TutorialHero>().isPlayerHero != isPlayerCastle && collision.GetComponentInParent<TutorialHero>().target.transform.GetInstanceID() == this.transform.GetInstanceID())
                {
                    Hitted(collision, collision.GetComponentInParent<TutorialHero>().Damage(), 3f, 3f);
                }
            }
            catch (NullReferenceException e)
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
