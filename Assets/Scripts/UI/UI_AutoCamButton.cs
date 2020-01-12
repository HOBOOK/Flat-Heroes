using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AutoCamButton : MonoBehaviour
{
    public Image onImage;
    bool isInterval = false;
    private void OnEnable()
    {
        RefreshUI();
    }
    void RefreshUI()
    {
        if (User.isAutoCam)
            onImage.gameObject.SetActive(true);
        else
            onImage.gameObject.SetActive(false);
    }

    public void OnClickAutoCamButton()
    {
        if(!isInterval)
        {
            StartCoroutine("OnAutoCamClicking");
        }
    }

    public IEnumerator OnAutoCamClicking()
    {
        isInterval = true;
        User.isAutoCam = !User.isAutoCam;
        RefreshUI();
        yield return new WaitForSeconds(0.2f);
        isInterval = false;
    }
}
