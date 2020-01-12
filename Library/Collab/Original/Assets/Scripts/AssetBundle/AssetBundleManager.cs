using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AssetBundleManager : MonoBehaviour
{
    static private AssetBundleManager instance = null;

    static public AssetBundleManager Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject inst = new GameObject("_AssetBundleManager");
                inst.isStatic = true;
                instance = inst.AddComponent<AssetBundleManager>();

                GameObject inst2 = new GameObject("_AssetBundleTimeManager");
                inst2.transform.parent = inst.transform;
                inst2.transform.localPosition = Vector3.zero;
                timeManager = inst2.AddComponent<AssetBundleTimeManager>();

                GameObject inst3 = new GameObject("_AssetBundleLoadManager");
                inst3.transform.parent = inst.transform;
                inst3.transform.localPosition = Vector3.zero;
                loadManager = inst3.AddComponent<AssetLoadManager>();

                dicAssetBundle = new Dictionary<string, AssetBundleNode>();
                lstKeyName = new List<string>();
            }
            return instance;
        }
    }

    public class AssetBundleNode
    {
        public AssetBundle assetBundle;
        public string url;
        public int version;
        public bool removeAll;

        public AssetBundleNode(string urlIn, int versionIn,bool removeAllIn, AssetBundle bundleIn)
        {
            this.url = urlIn;
            this.version = versionIn;
            this.removeAll = removeAllIn;
            this.assetBundle = bundleIn;
        }

        public void UnloadAssetBundle()
        {
            this.assetBundle.Unload(this.removeAll);
        }
    }
    static private AssetLoadManager loadManager;
    static private AssetBundleTimeManager timeManager;
    static private Dictionary<string, AssetBundleNode> dicAssetBundle;
    static private List<string> lstKeyName;

    public IEnumerator LoadAssetBundle(string url, int version, bool removeAll, float lifeTime = 0.0f, string bundleName=null)
    {
        string keyName = this.MakeKeyName(url, version);
        while(!Caching.ready)
        {
            yield return null;
        }
        if(this.isVersionAdded(url,version))
        {
            timeManager.ResetLifeTime(keyName);

        }
        else
        {
            using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, (uint)version, 0))
            {
                var operation = uwr.SendWebRequest();
                
                while (!operation.isDone)
                {
                    yield return null;
                    UI_StartManager.instance.SetDownLoadUIProgressbarValue(operation.progress, bundleName);
                }
                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    UI_StartManager.instance.ShowErrorUI("네트워크 오류로 인해 다운로드에 실패하였습니다.");
                }
                else
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                    //var materials = bundle.LoadAllAssets<Material>();
                    //foreach (Material m in materials)
                    //{
                    //    var shaderName = m.shader.name;
                    //    var newShader = Shader.Find(shaderName);
                    //    if (newShader != null)
                    //    {
                    //        m.shader = newShader;
                    //    }
                    //    else
                    //    {
                    //        Debug.LogWarning("unable to refresh shader: " + shaderName + " in material " + m.name);
                    //    }
                    //}
                    AssetBundleNode node = new AssetBundleNode(url, version, removeAll, bundle);
                    dicAssetBundle.Add(keyName, node);
                    lstKeyName.Add(keyName);
                    if (lifeTime > 0.0f)
                    {
                        timeManager.SetLifeTime(keyName, lifeTime);
                    }
                }
            }
        }
        yield return null;
    }

    public IEnumerator LoadAssetFromABAsync(string url, int version, string assetName, bool resetLife = true)
    {
        yield return StartCoroutine(loadManager.LoadAssetFromABAsync(url, version, assetName, resetLife));
    }

    public void LoadAssetFormAB(string url, int version, string assetName, bool resetLife=true)
    {
        loadManager.LoadAssetFromAB(url, version, assetName, resetLife);
    }

    public object GetLoadedAsset(string url, int version, string assetName, bool removeAfter = true)
    {
        object obj;
        obj = loadManager.GetAsset(url, version, assetName, removeAfter);
        return obj;
    }

    public AssetBundle GetAssetBundle(string url, int version)
    {
        string keyName = this.MakeKeyName(url, version);
        AssetBundle ab = dicAssetBundle[keyName].assetBundle;
        return ab;
    }

    public bool isVersionAdded(string url, int version)
    {
        string keyName = this.MakeKeyName(url, version);
        if (dicAssetBundle.ContainsKey(keyName))
            return true;
        else
            return false;
    }

    public string MakeKeyName(string url, int version)
    {
        string keyName = url + version;
        return keyName;
    }

    public void RemoveAssetBundle(string url, int version)
    {
        string keyName = this.MakeKeyName(url, version);
        if (dicAssetBundle.ContainsKey(keyName))
        {
            if(dicAssetBundle[keyName].removeAll ==true)
            {
                loadManager.RemoveIncludedAssets(keyName);
            }
            if(timeManager.isHaveLife(keyName))
            {
                timeManager.RemoveLifeTime(keyName);
            }
            dicAssetBundle[keyName].UnloadAssetBundle();
            dicAssetBundle.Remove(keyName);
            lstKeyName.Remove(keyName);

        }
    }

    public void RemoveAssetBundle(string keyName)
    {
        if(dicAssetBundle.ContainsKey(keyName))
        {
            if(dicAssetBundle[keyName].removeAll ==true)
            {
                loadManager.RemoveIncludedAssets(keyName);
            }
            if(timeManager.isHaveLife(keyName))
            {
                timeManager.RemoveLifeTime(keyName);
            }
            dicAssetBundle[keyName].UnloadAssetBundle();
            dicAssetBundle.Remove(keyName);
            lstKeyName.Remove(keyName);
        }
    }

    public int GetNumberOfABs()
    {
        return dicAssetBundle.Count;
    }

    public void RemoveAllAssetBundles()
    {
        for(int i = 0; i  <dicAssetBundle.Count; i++)
        {
            dicAssetBundle[lstKeyName[i]].UnloadAssetBundle();
            if(dicAssetBundle[lstKeyName[i]].removeAll==true)
            {
                loadManager.RemoveIncludedAssets(lstKeyName[i]);
            }
        }
        dicAssetBundle.Clear();
        lstKeyName.Clear();
        timeManager.RemoveAllLifeTime();
    }

    public void RemoveAllAssets()
    {
        loadManager.RemoveAllAssets();
    }
}
