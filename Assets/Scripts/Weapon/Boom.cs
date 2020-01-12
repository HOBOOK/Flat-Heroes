using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public bool isAlly;
    public int damage;
    public bool isCritical;
    bool isBoom = false;
    bool isGround = false;
    bool isThrowStart;
    int maxTargetCount;
    int targetCount;
    float arrowTimer;
    public Transform target;
    Vector3 prevPos;
    Vector3 targetPos;
    Vector3 startPos;
    GameObject AttackPoint;
    float vx, vy, vz;
    private void Awake()
    {
        AttackPoint = this.transform.GetChild(0).gameObject;
    }
    private void OnEnable()
    {
        arrowTimer = 0;
        maxTargetCount = 1;
        targetCount = 0;
        isGround = false;
        isBoom = false;
        isThrowStart = false;

        this.GetComponent<Rigidbody2D>().isKinematic = false;
    }
    public void StartBomb(int maxCount,bool isAlias, int dam, bool critical, GameObject targetObject)
    {
        maxTargetCount = maxCount;
        isAlly = isAlias;
        damage = dam;
        isCritical = critical;

        target = targetObject.transform;
        startPos = this.transform.position;
        targetPos = target.transform.position;
        vx = (targetPos.x - startPos.x) / 2f;
        vy = (targetPos.y - startPos.y + 9.8f) / 2f;
        vz = (targetPos.z - startPos.z) / 2f;
        isThrowStart = true;
        StartCoroutine("Booming");
    }
    IEnumerator Booming()
    {
        yield return new WaitForSecondsRealtime(3.0f);
        isBoom = true;
        BoomEffect();
        BoomAttack();
        yield return null;
    }

    private void FixedUpdate()
    {
        BombMoving();
    }
    public void BombMoving()
    {
        if (target != null&&!isBoom&&!isGround&& isThrowStart)
        {
            arrowTimer += Time.deltaTime;
            float sx = startPos.x + vx * arrowTimer*1.5f;
            float sy = startPos.y + vy * arrowTimer - 4.9f * arrowTimer * arrowTimer;
            float sz = startPos.z + vz * arrowTimer;
            transform.position = new Vector3(sx, sy, sz);
            this.transform.Rotate(0, 0, 1000 * Time.deltaTime);
        }
    }

    void BoomEffect()
    {
        GameObject effect = EffectPool.Instance.PopFromPool("ExplosionRoundFire");
        effect.transform.position = transform.position;
        effect.SetActive(true);
    }
    void BoomAttack()
    {
        //점프 레이어 (필드 오브젝트)
        Collider2D[] enemys = Physics2D.OverlapCircleAll(this.transform.position, 3);
        if(enemys!=null&&enemys.Length>0)
        {
            foreach (var enemy in enemys)
            {
                Debugging.Log(enemy.gameObject.name);
                if (enemy.gameObject.layer == 10 && enemy.GetComponentInParent<Hero>() != null && enemy.GetComponentInParent<Hero>().isPlayerHero != isAlly && !enemy.GetComponentInParent<Hero>().isDead)
                {
                    if(targetCount<maxTargetCount)
                    {
                        enemy.GetComponentInParent<Hero>().HittedByObject(damage, isCritical, new Vector3(15, 15));
                        targetCount += 1;
                    }
                }
            }
        }
        ObjectPool.Instance.PushToPool("Bomb", this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Hero>()!=null)
        {
            isGround = true;
        }
        if(collision.gameObject.layer==31)
        {
            isGround = true;
        }
    }
}
