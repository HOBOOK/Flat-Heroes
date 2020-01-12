using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartManager : MonoBehaviour
{
    public static UI_StartManager instance = null;
    public GameObject VersionAlertUI;
    public GameObject DownloadUI;
    public GameObject StartAbleUI;
    public GameObject CoverUI;
    public GameObject TitleUI;
    public GameObject ErrorAlertUI;
    public GameObject ConsentAlertUI;

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
        versionText.text = string.Format("Ver. {0}",Application.version);
    }

    public void SetDownloadCount(int maxCount)
    {
        MaxDownloadCount = maxCount;
        CurrentDownloadCount = 0;
    }

    public void ShowStartUI(bool isServerLogin)
    {
        DownloadUI.gameObject.SetActive(false);
        StartAbleUI.gameObject.SetActive(true);

        if(isServerLogin)
        {
            StartAbleUI.transform.GetChild(0).gameObject.SetActive(true);
            StartAbleUI.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            StartAbleUI.transform.GetChild(0).gameObject.SetActive(false);
            StartAbleUI.transform.GetChild(1).gameObject.SetActive(true);
        }
        TitleUI.gameObject.SetActive(true);
    }
    public void ShowDownloadUI()
    {
        TitleUI.gameObject.SetActive(false);
        StartAbleUI.gameObject.SetActive(false);
        DownloadUI.gameObject.SetActive(true);
    }
    public void ShowErrorUI(string errorMsg)
    {
        if(ErrorAlertUI!=null)
        {
            Caching.ClearCache();
            ErrorAlertUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
            //ErrorAlertUI.gameObject.SetActive(true);
            ErrorAlertUI.transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>().text = errorMsg;
        }
    }
    public void ShowVersionUI(string latestVersion)
    {
        if (VersionAlertUI != null)
        {

            VersionAlertUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
            VersionAlertUI.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = string.Format("{0} : {1} \r\n {2} : {3}", LocalizationManager.GetText("CurrentVersion"), Application.version, LocalizationManager.GetText("LatestVersion"), latestVersion);
        }
    }

    public void ShowConsnetUI()
    {
        if(ConsentAlertUI!=null)
        {
            ConsentAlertUI.GetComponent<AiryUIAnimatedElement>().ShowElement();
        }
    }

    public void SetDownLoadUIProgressbarValue(float v, string downloadBundleName)
    {
        if(downloadProgressBar != null)
        {
            downloadProgressBar.value = v;
            downloadProgressBar.transform.GetComponentInChildren<Text>().text = string.Format("{0} {1}%",LocalizationManager.GetText("AssetBundleDownText") ,(v * 100f).ToString("N0"));
        }
    }
    public void SetAssetLoadUIProgressbarValue(float v, string assetName, int count)
    {
        if (downloadProgressBar != null)
        {
            downloadProgressBar.value = v;
            downloadProgressBar.transform.GetComponentInChildren<Text>().text = string.Format("{0} {1}%", LocalizationManager.GetText("AssetBundleLoadText"), ((float)count * 100f / (float)MaxDownloadCount).ToString("N0"));
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
