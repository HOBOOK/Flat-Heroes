using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class UI_StartManager : MonoBehaviour
{
    public static UI_StartManager instance = null;
    bool isReady = false;
    public GameObject DownloadUI;
    public GameObject StartAbleUI;
    public GameObject CoverUI;
    public GameObject TitleUI;
    public GameObject ErrorAlertUI;
    public Button startButton;
    public Text versionText;

    Slider downloadProgressBar;

    public int MaxDownloadCount;
    public int CurrentDownloadCount;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);

        DownloadUI.gameObject.SetActive(false);
        StartAbleUI.gameObject.SetActive(false);
        TitleUI.gameObject.SetActive(false);
        ErrorAlertUI.gameObject.SetActive(false);
        downloadProgressBar = DownloadUI.GetComponentInChildren<Slider>();
        startButton.interactable = false;
        versionText.text = Application.version;
    }

    public void SetDownloadCount(int maxCount)
    {
        MaxDownloadCount = maxCount;
        CurrentDownloadCount = 0;
    }
    public void ShowStartUI()
    {
        DownloadUI.gameObject.SetActive(false);
        StartAbleUI.gameObject.SetActive(true);
        startButton.interactable = true;
        TitleUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
    }
    public void ShowDownloadUI()
    {
        TitleUI.gameObject.SetActive(false);
        startButton.interactable = false;
        StartAbleUI.gameObject.SetActive(false);
        DownloadUI.gameObject.SetActive(true);
    }
    public void ShowErrorUI(string errorMsg)
    {
        if(ErrorAlertUI!=null)
        {
            ErrorAlertUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
            //ErrorAlertUI.gameObject.SetActive(true);
            ErrorAlertUI.transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>().text = errorMsg;
        }

    }

    public void SetDownLoadUIProgressbarValue(float v, string downloadBundleName)
    {
        if(downloadProgressBar != null)
        {
            downloadProgressBar.value = v;
            downloadProgressBar.transform.GetComponentInChildren<Text>().text = string.Format("실행에 필요한 {0} 서버로부터 다운로드중... {1}%", downloadBundleName, (v * 100f).ToString("N0"));
        }
    }
    public void SetAssetLoadUIProgressbarValue(float v, string assetName, int count)
    {
        if (downloadProgressBar != null)
        {
            downloadProgressBar.value = v;
            downloadProgressBar.transform.GetComponentInChildren<Text>().text = string.Format("다운받은 {0} 로컬에 로드중... ({1}/{2})", assetName, count, MaxDownloadCount);
        }
    }

    public void CoverFadeIn()
    {
        if (CoverUI != null)
            StartCoroutine(FadeIn(CoverUI.GetComponent<Image>()));
    }
    IEnumerator FadeIn(Image image)
    {
        if (image == null)
            yield return null;
        float cnt = 0;
        Color color = image.color;
        while (cnt < 1)
        {
            image.color = new Color(color.r, color.g, color.b, cnt);
            yield return new WaitForEndOfFrame();
            cnt += 0.01f;
        }
        yield return null;
    }
}
