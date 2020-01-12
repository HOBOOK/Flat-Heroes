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
        //Caching.ClearCache();
        UI_StartManager.instance.ShowDownloadUI();
        yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetBundle(BundleManagerURL, version, false,0.0f,"게임매니저"));
        UI_StartManager.instance.SetDownloadCount(4);

        if (AssetBundleManager.Instance.isVersionAdded(BundleManagerURL, version))
        {
            yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleManagerURL, version, "LoadSceneManager"));
            GameObject obj = AssetBundleManager.Instance.GetLoadedAsset(BundleManagerURL, version, "LoadSceneManager") as GameObject;
            InstantiateAsset(obj, this.transform);

            yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleManagerURL, version, "PrefabDatabase"));
            GameObject obj2 = AssetBundleManager.Instance.GetLoadedAsset(BundleManagerURL, version, "PrefabDatabase") as GameObject;
            InstantiateAsset(obj2, this.transform);

            yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleManagerURL, version, "SoundManager"));
            GameObject obj3 = AssetBundleManager.Instance.GetLoadedAsset(BundleManagerURL, version, "SoundManager") as GameObject;
            InstantiateAsset(obj3, this.transform);

            yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleManagerURL, version, "EffectPoolManager"));
            GameObject obj4 = AssetBundleManager.Instance.GetLoadedAsset(BundleManagerURL, version, "EffectPoolManager") as GameObject;
            InstantiateAsset(obj4, this.transform);
        }
        AssetBundleManager.Instance.RemoveAssetBundle(BundleManagerURL, version);
        Debugging.Log("매니저다운 끝");
        version =1;
        yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetBundle(BundleHeroURL, version, false, 0.0f, "영웅데이터"));
        UI_StartManager.instance.SetDownloadCount(17);
        if (AssetBundleManager.Instance.isVersionAdded(BundleHeroURL, version))
        {
            // 영웅 프리팹
            for (int i = 0; i < 9; i++)
            {
                yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleHeroURL, version, string.Format("Hero{0:D3}", (i+1))));
                PrefabsDatabaseManager.instance.AddPrefabToHeroList(AssetBundleManager.Instance.GetLoadedAsset(BundleHeroURL, version, string.Format("Hero{0:D3}", (i + 1))) as GameObject);
            }

            // 몬스터 프리팹
            for(int i = 0; i < 6; i++)
            {
                yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleHeroURL, version, string.Format("Monster{0:D3}", (i + 1))));
                PrefabsDatabaseManager.instance.AddPrefabToMonsterList(AssetBundleManager.Instance.GetLoadedAsset(BundleHeroURL, version, string.Format("Monster{0:D3}", (i + 1))) as GameObject);
            }
            for (int i = 0; i < 2; i++)
            {
                yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleHeroURL, version, string.Format("Boss{0:D3}", (i + 1))));
                PrefabsDatabaseManager.instance.AddPrefabToMonsterList(AssetBundleManager.Instance.GetLoadedAsset(BundleHeroURL, version, string.Format("Boss{0:D3}", (i + 1))) as GameObject);
            }
            // 캐슬
            for (int i = 0; i < 1; i++)
            {
                yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleHeroURL, version, string.Format("Castle{0:D3}", (i + 1))));
                PrefabsDatabaseManager.instance.AddPrefabToCastleList(AssetBundleManager.Instance.GetLoadedAsset(BundleHeroURL, version, string.Format("Castle{0:D3}", (i + 1))) as GameObject);
            }
            yield return new WaitForSeconds(0.1f);
            PrefabsDatabaseManager.instance.GetPrefabList();
        }
        Debugging.Log("영웅다운 끝");
        AssetBundleManager.Instance.RemoveAllAssetBundles();
        // 로그인 시도
        if(PlayerPrefs.GetInt("ServiceConsent")!=1)
        {
            UI_StartManager.instance.ShowConsnetUI();
        }
        else
        {
            GoogleSignManager.Instance.Init();

        }
    }

    void InstantiateAsset(GameObject obj, Transform parent)
    {
        if (obj != null)
            Instantiate(obj, parent);
    }
}
