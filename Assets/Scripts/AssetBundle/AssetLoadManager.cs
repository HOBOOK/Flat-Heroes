using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetLoadManager : MonoBehaviour
{
    static private AssetBundleManager abManager;
    static private AssetBundleTimeManager timeManager;

    static private Dictionary<string, Dictionary<string, object>> dicAsset;

    private void Awake()
    {
        dicAsset = new Dictionary<string, Dictionary<string, object>>();
        abManager = this.transform.parent.GetComponent<AssetBundleManager>();
        timeManager = this.transform.parent.Find("_AssetBundleTimeManager").GetComponent<AssetBundleTimeManager>();
    }

    public void LoadAssetFromAB(string url, int version, string assetName, bool resetLife = true)
    {
        string keyName = abManager.MakeKeyName(url, version);

        if (!AssetBundleManager.Instance.isVersionAdded(url, version) || this.IsAssetLoaded(url, version,assetName))
        {
            //yield return null;
        }
        else
        {
            AssetBundle ab = abManager.GetAssetBundle(url, version);

            if (resetLife && timeManager.isHaveLife(keyName))
                timeManager.ResetLifeTime(keyName);

            this.LoadAsset(url, version, ab, assetName);
        }
    }

    private void LoadAsset(string url, int version, AssetBundle AB, string assetName)
    {
        string keyName = abManager.MakeKeyName(url, version);
        if(!dicAsset.ContainsKey(keyName))
        {
            Dictionary<string, object> ab = new Dictionary<string, object>();
            dicAsset.Add(keyName, ab);
        }

        if(!dicAsset[keyName].ContainsKey(assetName))
        {
            object obj = AB.LoadAsset(assetName);
            dicAsset[keyName].Add(assetName, obj);
        }
    }

    public IEnumerator LoadAssetFromABAsync(string url, int version, string assetName, bool resetLife = true)
    {
        string keyName = abManager.MakeKeyName(url, version);
        if(!abManager.isVersionAdded(url,version)||this.IsAssetLoaded(url,version,assetName))
        {
            UI_StartManager.instance.ShowErrorUI("이미 로드 되어있습니다.");
            yield return null;
        }
        else
        {
            AssetBundle ab = abManager.GetAssetBundle(url, version);
            if (resetLife && timeManager.isHaveLife(keyName))
                timeManager.ResetLifeTime(keyName);

            yield return StartCoroutine(LoadAssetAsync(url, version, ab, assetName));
        }
    }

    private IEnumerator LoadAssetAsync(string url, int version, AssetBundle AB, string assetName)
    {
        string keyName = abManager.MakeKeyName(url, version);
        if(!dicAsset.ContainsKey(keyName))
        {
            Dictionary<string, object> ab = new Dictionary<string, object>();
            dicAsset.Add(keyName, ab);
        }
        if(!dicAsset[keyName].ContainsKey(assetName))
        {
            AssetBundleRequest abReq = AB.LoadAssetAsync(assetName);
            while (!abReq.isDone)
            {
                yield return null;
                UI_StartManager.instance.SetAssetLoadUIProgressbarValue(abReq.progress, assetName,UI_StartManager.instance.CurrentDownloadCount);
            }
            if (abReq.asset!=null)
            {
                dicAsset[keyName].Add(assetName, abReq.asset);
                UI_StartManager.instance.CurrentDownloadCount += 1;
            }
            else
            {
                UI_StartManager.instance.ShowErrorUI(assetName + " 로드에 실패하였습니다.");
            }
        }
    }

    public object GetAsset(string url, int version, string assetName, bool remove=true)
    {
        string keyName = abManager.MakeKeyName(url, version);
        object obj = null;
        if(abManager.isVersionAdded(url,version)&&this.IsAssetLoaded(url,version,assetName))
        {
            obj = dicAsset[keyName][assetName];
            if (remove)
            {
                dicAsset[keyName].Remove(assetName);
            }
        }
        if(obj==null)
        {
            Debugging.Log(assetName + "이 Null 입니다.");
            UI_StartManager.instance.ShowErrorUI(assetName + " 로드에 실패하였습니다.");
        }


        return obj;
    }

    public bool IsAssetLoaded(string url, int version, string assetName)
    {
        string keyName = abManager.MakeKeyName(url, version);
        if(dicAsset.ContainsKey(keyName))
        {
            return dicAsset[keyName].ContainsKey(assetName);
        }
        return false;
    }

    public bool RemoveAsset(string url, int version, string assetName)
    {
        string keyName = abManager.MakeKeyName(url, version);

        if(this.IsAssetLoaded(url,version,assetName))
        {
            dicAsset[keyName].Remove(assetName);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveIncludedAssets(string keyName)
    {
        if(dicAsset.ContainsKey(keyName))
        {
            dicAsset.Remove(keyName);
        }
    }
    public void RemoveAllAssets()
    {
        dicAsset.Clear();
    }
}
