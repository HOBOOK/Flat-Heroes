using Firebase.Extensions;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadAssetBundle : MonoBehaviour
{
    private string BundleManifestURL = "https://drive.google.com/uc?authuser=0&id=1Egg5v7F5j-D6_9i_79hPcaK0QOhQB3i_&export=download";
    private string BundleManagerURL = "https://drive.google.com/uc?authuser=0&id=1_6oGhDxgHtfVxKlC7yDy3vWNSWYfkelS&export=download";
    private string BundleHeroURL = "https://drive.google.com/uc?authuser=0&id=1pqbaVXe7giw4GlqXr3SriuZU_zeSSTSZ&export=download";
    private string TestBundleHeroURL = "https://drive.google.com/uc?authuser=0&id=1agtmg-sq9EkAVZEUIufFF7QttsSk5Jin&export=download";
    private string TestBundleManagerURL = "https://drive.google.com/uc?authuser=0&id=19_q9WiYaZV1Ezx-uc-Ia-sjwhhy7iVet&export=download";
    public GameObject ProgressViewUI;
    public GameObject BundleUI;
    public int version = 0;
    bool isLoadManagerDataSuccess = false;
    bool isLoadHeroDataSuccess = false;
    public bool isOneStore = false;
    public bool isTest = false;

    IEnumerator Start()
    {
        if(isTest)
        {
            BundleManagerURL = TestBundleManagerURL;
            BundleHeroURL = TestBundleHeroURL;
        }
        if(!isOneStore)
            yield return CheckingVersion();
        isLoadManagerDataSuccess = false;
        isLoadHeroDataSuccess = false;
        yield return CheckingAlert();
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

            isLoadManagerDataSuccess = true;
        }
        AssetBundleManager.Instance.RemoveAssetBundle(BundleManagerURL, version);
        Debugging.Log("매니저다운 끝");
        version += 1;
        yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetBundle(BundleHeroURL, version, false, 0.0f, "영웅데이터"));
        PlayerPrefs.SetInt("AssetBundle", version-1);
        UI_StartManager.instance.SetDownloadCount(45);
        if (AssetBundleManager.Instance.isVersionAdded(BundleHeroURL, version))
        {
            // 영웅 프리팹
            for (int i = 0; i < 12; i++)
            {
                yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleHeroURL, version, string.Format("Hero{0:D3}", (i+1))));
                PrefabsDatabaseManager.instance.AddPrefabToHeroList(AssetBundleManager.Instance.GetLoadedAsset(BundleHeroURL, version, string.Format("Hero{0:D3}", (i + 1))) as GameObject);
            }

            // 몬스터 프리팹
            for(int i = 0; i < 24; i++)
            {
                yield return StartCoroutine(AssetBundleManager.Instance.LoadAssetFromABAsync(BundleHeroURL, version, string.Format("Monster{0:D3}", (i + 1))));
                PrefabsDatabaseManager.instance.AddPrefabToMonsterList(AssetBundleManager.Instance.GetLoadedAsset(BundleHeroURL, version, string.Format("Monster{0:D3}", (i + 1))) as GameObject);
            }
            for (int i = 0; i < 8; i++)
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

            isLoadHeroDataSuccess = true;
        }
        Debugging.Log("영웅다운  끝");
        AssetBundleManager.Instance.RemoveAllAssetBundles();
        // 로그인 시도
        if(isLoadManagerDataSuccess&&isLoadHeroDataSuccess)
        {
            if (PlayerPrefs.GetInt("ServiceConsent") != 1)
            {
                UI_StartManager.instance.ShowConsnetUI();
            }
            else
            {
                GoogleSignManager.Instance.Init();
            }
        }
        else
        {
            Debugging.LogWarning("에셋번들 로드에 실패하였습니다.");
        }
    }

    void InstantiateAsset(GameObject obj, Transform parent)
    {
        if (obj != null)
            Instantiate(obj, parent);
    }

    IEnumerator CheckingVersion()
    {
        bool isCheckingVersionCompleted = false;
        string latestVersion = "";
        RestClient.Get("https://api-project-81117173.firebaseio.com/version/.json").Done(x =>
        {
            if (x != null)
            {
                Debugging.Log(x.Text);
                var data = x.Text.Replace("\"", "");
                if (!string.IsNullOrEmpty(data) && !data.ToLower().Equals("null"))
                {
                    latestVersion = data;
                    isCheckingVersionCompleted = true;
                }
                else
                {
                    latestVersion = "";
                    isCheckingVersionCompleted = true;
                }
            }
            else
            {
                latestVersion = "";
                isCheckingVersionCompleted = true;
            }
        }, e =>
        {
            Debug.Log(e.StackTrace);
            latestVersion = "";
            isCheckingVersionCompleted = true;
        });
        while(!isCheckingVersionCompleted)
        {
            yield return null;
        }
        if(!string.IsNullOrEmpty(latestVersion))
        {
            if (float.Parse(latestVersion) > float.Parse(Application.version))
            {
                UI_StartManager.instance.ShowVersionUI(latestVersion);
                while (true)
                    yield return null;
            }
        }
        else
        {
            yield return null;
        }
    }

    IEnumerator CheckingAlert()
    {
        if(PlayerPrefs.GetInt("AssetBundle")!=version||isTest)
        {
            Caching.ClearCache();
            long filesize = 11808426;
            yield return StartCoroutine(GetFileSize(BundleHeroURL,
            (size) =>
            {
                filesize += size;
            }));
            Debug.Log("File Size: " + filesize);
            BundleUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = string.Format("{0}\r\n\r\n({1}MB)", LocalizationManager.GetText("BundleInformation"), (filesize/1000000f).ToString("N2"));
            BundleUI.SetActive(true);
            BundleUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
            while (!BundleUI.GetComponentInChildren<UI_CheckButton>().isChecking)
            {
                yield return new WaitForFixedUpdate();
            }
            if (BundleUI.GetComponentInChildren<UI_CheckButton>().isResult)
            {
                BundleUI.GetComponent<AiryUIAnimatedElement>().HideElement();
                yield return null;
            }
            else
            {
                Application.Quit();
            }
        }
        yield return null;
    }

    IEnumerator GetFileSize(string url, Action<long> resut)
    {
        UnityWebRequest uwr = UnityWebRequest.Head(url);
        uwr.chunkedTransfer = true;
        yield return uwr.SendWebRequest();
        string size = uwr.GetResponseHeader("Content-Length");
        Debugging.Log(size);
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log("Error While Getting Length: " + uwr.error);
            if (resut != null)
                resut(-1);
        }
        else
        {
            if (resut != null)
                resut(Convert.ToInt64(size));
        }
    }
}
