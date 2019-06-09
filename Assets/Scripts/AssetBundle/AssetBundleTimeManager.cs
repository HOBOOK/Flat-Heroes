using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleTimeManager : MonoBehaviour
{
    public class LifeTime
    {
        private float maxTime;
        public float currentTime;

        public LifeTime(float maxTimeIn)
        {
            this.maxTime = maxTimeIn;
            this.currentTime = this.maxTime;
        }

        public void SetCurrentToMax()
        {
            this.currentTime = this.maxTime;
        }

        public void SubtractElapsedTime()
        {
            this.currentTime -= Time.deltaTime;
        }
    }

    static private AssetBundleManager abManager;

    static private Dictionary<string, LifeTime> dicLifeTime;
    static private List<string> lstKeyName;

    private void Awake()
    {
        dicLifeTime = new Dictionary<string, LifeTime>();
        lstKeyName = new List<string>();
        abManager = this.transform.parent.GetComponent<AssetBundleManager>();
    }

    private void Update()
    {
        this.SubtractLifeTimes();
    }

    private void SubtractLifeTimes()
    {
        for(int i = 0; i < dicLifeTime.Count; i++)
        {
            if(dicLifeTime.ContainsKey(lstKeyName[i]))
            {
                dicLifeTime[lstKeyName[i]].SubtractElapsedTime();

                if(dicLifeTime[lstKeyName[i]].currentTime<=0.0f)
                {
                    this.RemoveAssetBundle(lstKeyName[i]);
                    --i;
                }
            }
        }
    }

    public bool isHaveLife(string keyName)
    {
        if(lstKeyName.Contains(keyName))
        {
            return true;
        }
        return false;
    }

    public void SetLifeTime(string keyName, float lifeTime)
    {
        LifeTime time = new LifeTime(lifeTime);
        dicLifeTime.Add(keyName, time);
        lstKeyName.Add(keyName);
    }

    public void ResetLifeTime(string keyName)
    {
        dicLifeTime[keyName].SetCurrentToMax();
    }

    public void RemoveLifeTime(string keyName)
    {
        if(lstKeyName.Contains(keyName))
        {
            dicLifeTime.Remove(keyName);
            lstKeyName.Remove(keyName);
        }
    }

    public void RemoveAllLifeTime()
    {
        dicLifeTime.Clear();
        lstKeyName.Clear();
    }

    private void RemoveAssetBundle(string keyName)
    {
        abManager.RemoveAssetBundle(keyName);
        dicLifeTime.Remove(keyName);
        lstKeyName.Remove(keyName);
    }
}
