using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletController : MonoBehaviour
{
    public string poolItemName;
    public bool isAlly = false;
    public bool isCritical;
    public int damage = 10;
    public float lifeTime = 0.5f;
    public float _elapsedTime = 0f;
    public float speed;
    public Transform Target;
    private Vector3 targetPos;
    private Vector3 thisPos;
    private float angle;
    public float offset;
    private float bulletZ;
    bool isReset = false;
    public int penetrateCnt;
    public static int penetrateCount;
    private int[] penetrateTargetDistinctID;
    private void Awake()
    {
        if (penetrateCnt < 1)
            penetrateCnt = 1;
    }
    private void OnEnable()
    {
        penetrateCount = penetrateCnt;
        penetrateTargetDistinctID = new int[penetrateCount];
    }
    private void Update()
    {
        if (Target&&!isReset)
        {
            isReset = true;
            if (poolItemName.Equals("knife(throw)"))
            {
                targetPos = Target.position;
                thisPos = transform.position;
                targetPos.x = targetPos.x - thisPos.x;
                targetPos.y = targetPos.y - thisPos.y;
                angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
                bulletZ = angle + offset;
            }
        }
        if (poolItemName.Equals("knife(throw)"))
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, bulletZ));
        }
        transform.position += transform.right * speed * Time.deltaTime;
        if(GetTimer() > lifeTime)
        {
            SetTimer();
            isReset = false;
            ObjectPool.Instance.PushToPool(poolItemName, gameObject);
        }
        if (penetrateCount == 0)
        {
            ObjectPool.Instance.PushToPool(poolItemName, gameObject);
            SetTimer();
        }
    }
    float GetTimer()
    {
        return (_elapsedTime += Time.deltaTime);
    }
    void SetTimer()
    {
        _elapsedTime = 0f;
    }

    public void BulletStand(Transform parent)
    {
        if(penetrateCount>0&& penetrateCount <= penetrateCnt)
        {
            bool isStandAble = false;
            for (var i = 0; i < penetrateTargetDistinctID.Length; i++)
            {
                if (penetrateTargetDistinctID[i] == parent.GetInstanceID())
                {
                    isStandAble = true;
                }
            }
            if (!isStandAble)
            {
                if (parent.GetComponentInParent<Hero>() != null)
                {
                    parent.GetComponentInParent<Hero>().HittedByObject(damage, isCritical, new Vector2(2, 3));
                }
                else if (parent.GetComponent<Castle>() != null)
                {
                    parent.GetComponent<Castle>().HittedByObject(damage, isCritical, new Vector2(2, 3));
                }
                TriggerEffet(poolItemName);
                penetrateCount--;
                if (penetrateCount >= 0 && penetrateCount < penetrateCnt)
                    penetrateTargetDistinctID[penetrateCount] = parent.GetInstanceID();
            }
        }
    }
    private void TriggerEffet(string strPoolItemName)
    {
        if(strPoolItemName.Equals("Bullet"))
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.fireShot);
            GameObject bulletEffect = EffectPool.Instance.PopFromPool("bulletEffect");
            bulletEffect.transform.position = this.transform.position + this.transform.right * 0.5f;
            bulletEffect.SetActive(true);
        }
        else if (strPoolItemName.Equals("knife(throw)"))
        {

            GameObject effect = EffectPool.Instance.PopFromPool("Arrow_Hit");
            effect.transform.position = this.transform.position + this.transform.right * 0.3f;
            effect.SetActive(true);
        }
            
    }

}
