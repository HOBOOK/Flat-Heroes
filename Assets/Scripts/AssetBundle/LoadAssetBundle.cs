using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAssetBundle : MonoBehaviour
{
    private string BundleManagerURL = "https://drive.google.com/uc?authuser=0&id=1_6oGhDxgHtfVxKlC7yDy3vWNSWYfkelS&export=download";
    private string BundleHeroURL = "https://drive.google.com/uc?authuser=0&id=1pqbaVXe7giw4GlqXr3SriuZU_zeSSTSZ&export=download";
    public GameObject ProgressViewUI;
    public int version = 0;

    IEnumerator Start()
    {
        AssetBundleManager.Instance.progressViewUI = ProgressViewUI;
        yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetBundle(BundleManagerURL, version, false, 2.0f,"게임매니저"));
        print(AssetBundleManager.Instance.isVersionAdded(BundleManagerURL, version));
        yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetBundle(BundleHeroURL, version, false,2.0f,"영웅데이터"));
        print(AssetBundleManager.Instance.isVersionAdded(BundleHeroURL, version));


        yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleManagerURL, version, "LoadSceneManager"));
        GameObject obj = AssetBundleManager.Instance.GetLoadedAsset(BundleManagerURL, version, "LoadSceneManager") as GameObject;
        Instantiate(obj, this.transform);

        yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleManagerURL, version, "PrefabDatabase"));
        GameObject obj2 = AssetBundleManager.Instance.GetLoadedAsset(BundleManagerURL, version, "PrefabDatabase") as GameObject;
        Instantiate(obj2, this.transform);

        yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleManagerURL, version, "SoundManager"));
        GameObject obj3 = AssetBundleManager.Instance.GetLoadedAsset(BundleManagerURL, version, "SoundManager") as GameObject;
        Instantiate(obj3, this.transform);

        yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleManagerURL, version, "EffectPoolManager"));
        GameObject obj4 = AssetBundleManager.Instance.GetLoadedAsset(BundleManagerURL, version, "EffectPoolManager") as GameObject;
        Instantiate(obj4, this.transform);



        for (int i = 0; i < 7; i++)
        {
            yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleHeroURL, version, "Hero00" + (i + 1)));
            PrefabsDatabaseManager.instance.AddPrefabToHeroList(AssetBundleManager.Instance.GetLoadedAsset(BundleHeroURL, version, "Hero00" + (i + 1)) as GameObject);
        }
        PrefabsDatabaseManager.instance.GetHeroList();

        //Caching.ClearCache();
    }
}
