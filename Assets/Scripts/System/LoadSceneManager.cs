using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager instance = null;
    public GameObject loadingUIPrefab;
    GameObject Canvas;
    private int nextSceneNumber;
    bool isLoadStart = false;
    float loadTime = 0.0f;
    string[] tipTexts = { "첫번째 팁입니다.", "두번째 팁입니다.", "세번째 팁입니다." };
    private void Awake()
    {
        if (instance == null)
            instance = this;
        Canvas = GameObject.Find("Canvas");
    }
    private void Update()
    {
        if(isLoadStart)
        {
            loadTime += Time.deltaTime;
            if(loadTime>=1.0f)
            {
                loadTime = 0.0f;
            }
        }
    }
    public string GetTip()
    {
        int tipNum = UnityEngine.Random.Range(0, tipTexts.Length);
        return string.Format("<size='50'>TIP #{0}. </size> {1}", (tipNum+1), tipTexts[tipNum]);
    }
    public string GetLoadingPercent(float percent)
    {
        return string.Format("Loading... {0} %", (int)(percent * 100));
    }

    public void LoadScene(int sceneNumber)
    {
        if(!isLoadStart)
        {
            Debugging.Log(sceneNumber + " 로드 시작");
            nextSceneNumber = sceneNumber;
            StartCoroutine("LoadingScene");
        }
    }

    public void LoadStageScene()
    {
        if (!isLoadStart)
        {
            nextSceneNumber = 2;
            StartCoroutine("LoadingScene");
        }
    }

    public void LoadSceneAddtive(int sceneNumber)
    {
        if (!isLoadStart)
        {
            Debugging.Log(sceneNumber + " 로드 시작");
            nextSceneNumber = sceneNumber;
            StartCoroutine("LoadingSceneAddtive");
        }
    }

    IEnumerator LoadingScene()
    {
        isLoadStart = true;
        UI_Manager.instance.CoverFadeIn();
        yield return new WaitForSeconds(2.0f);
        if (Canvas==null)
            Canvas = GameObject.Find("CanvasShow");
        GameObject loadingPrefab = Instantiate(loadingUIPrefab, Canvas.transform);
        loadingPrefab.SetActive(true);
        loadingPrefab.SetActive(true);
        loadingPrefab.transform.GetChild(0).GetComponent<Text>().text = GetTip();
        AsyncOperation async = SceneManager.LoadSceneAsync(nextSceneNumber);
        async.allowSceneActivation = false;
        float progress = 0.0f;
        while(!async.isDone)
        {
            progress = async.progress;
            loadingUIPrefab.GetComponentInChildren<Slider>().value = progress;
            loadingUIPrefab.GetComponentInChildren<Slider>().transform.GetComponentInChildren<Text>().text = GetLoadingPercent(progress);
            Debugging.Log("현재 로딩 " + progress*100f);
            yield return true;

            async.allowSceneActivation = true;
        }
        
        isLoadStart = false;
        Destroy(loadingPrefab);
        yield return null;
    }
    IEnumerator LoadingSceneAddtive()
    {
        isLoadStart = true;
        AsyncOperation async = SceneManager.LoadSceneAsync(nextSceneNumber,LoadSceneMode.Additive);
        async.allowSceneActivation = false;
        while (!async.isDone)
        {
            yield return true;
            async.allowSceneActivation = true;
        }
        isLoadStart = false;
        yield return null;
    }
}
