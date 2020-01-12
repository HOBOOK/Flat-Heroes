using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackObjectPool : MonoBehaviour
{
    public string poolItemName;
    public float lifeTime =1f;
    public float _elapsedTime = 0f;
    public POOLTYPE PoolType = POOLTYPE.ObjectPool;
    public enum POOLTYPE { ObjectPool,EffectPool};
    private void Awake()
    {
        if(this.name.Contains("(Clone)"))
            poolItemName = this.name.Replace("(Clone)","");
    }
    void Update ()
    {
        if (GetTimer() > lifeTime)
        {
            SetTimer();
            if(ObjectPool.Instance!=null&& PoolType==POOLTYPE.ObjectPool)
                ObjectPool.Instance.PushToPool(poolItemName, gameObject);
            else if(EffectPool.Instance!=null&&PoolType==POOLTYPE.EffectPool)
                EffectPool.Instance.PushToPool(poolItemName, gameObject);
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

}
