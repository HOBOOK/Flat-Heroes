using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadAssetBundleExample : MonoBehaviour
{
    // 번들 다운 받을 서버의 주소(필자는 임시방편으로 로컬 파일 경로 쓸 것임) 
    private string BundleManagerURL = "https://drive.google.com/uc?authuser=0&id=1_6oGhDxgHtfVxKlC7yDy3vWNSWYfkelS&export=download";
    private string BundleHeroURL = "https://drive.google.com/uc?authuser=0&id=1pqbaVXe7giw4GlqXr3SriuZU_zeSSTSZ&export=download";
    //private string BundleManagerURL = "file://" + Application.streamingAssetsPath + "/AssetBundles/manager";
    //private string BundleHeroURL = "file://" + Application.streamingAssetsPath + "/AssetBundles/hero";
    // 번들의 version 
    public int version;
    void Start()
    {
        StartCoroutine (LoadAssetBundle());
    }
    IEnumerator LoadAssetBundle ()
    {
        while (!Caching.ready)
            yield return null;
        Caching.ClearCache();
        using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(BundleManagerURL))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                AssetBundleRequest request = bundle.LoadAssetAsync("LoadSceneManager", typeof(GameObject));
                yield return request;
                GameObject obj = Instantiate(request.asset, this.transform) as GameObject;
                obj.transform.position = Vector3.zero;
                bundle.Unload(false);
            }
        }
        Debugging.Log("매니저 에셋 번들 로드성공");

        using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(BundleHeroURL))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                for(int i = 0; i<7; i++)
                {
                    AssetBundleRequest request = bundle.LoadAssetAsync("Hero00"+(i+1), typeof(GameObject));
                    yield return request;
                    PrefabsDatabaseManager.instance.AddPrefabToHeroList(request.asset as GameObject);
                }
                bundle.Unload(false);
            }
        }
        Debugging.Log("영웅 에셋 번들 로드성공");
        // using문은 File 및 Font 처럼 컴퓨터 에서 관리되는 리소스들을 쓰고 나서 쉽게 자원을 되돌려줄수 있도록 기능을 제공 
    }
}
