using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    private static EffectPool _instance = null;

    public static EffectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(EffectPool)) as EffectPool;
                if (_instance == null)
                {
                    return null;
                }
            }
            return _instance;
        }
    }
    public List<PooledObject> effectPool = new List<PooledObject>();
    
    void Awake()
    {
        for (int ix = 0; ix < effectPool.Count; ++ix)
        {
            if (effectPool[ix].parent)
                effectPool[ix].Initialize(effectPool[ix].parent.transform);
            else
                effectPool[ix].Initialize(transform);
        }
    }
    public bool PushToPool(string itemName, GameObject item, Transform parent = null)
    {
        PooledObject pool = GetPoolItem(itemName);
        if (pool == null)
            return false;

        pool.PushToPool(item, parent == null ? transform : parent);
        return true;
    }
    public GameObject PopFromPool(string itemName, Transform parent = null)
    {
        PooledObject pool = GetPoolItem(itemName);
        if (pool == null)
            return null;
        
        return pool.PopFromPool(parent);
    }

    PooledObject GetPoolItem(string itemName)
    {
        for (int ix = 0; ix < effectPool.Count; ++ix)
        {
            if (effectPool[ix].poolItemName.Equals(itemName))
                return effectPool[ix];
        }

        Debug.Log("There's no matched pool list. \r\n" + effectPool[0].poolItemName + "== " +itemName);
        return null;
    }

}
