using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SpeedButton : MonoBehaviour
{
    public Image onImage;
    bool isInterval = false;
    private void Start()
    {
        RefreshUI();
    }
    void RefreshUI()
    {
        if (User.isSpeedGame)
        {
            onImage.gameObject.SetActive(true);
            SetStageSpeed(true);
        }
        else
        {
            onImage.gameObject.SetActive(false);
            SetStageSpeed(false);
        }
    }

    public void OnClickSpeedButton()
    {
        if(!isInterval)
        {
            StartCoroutine("OnSpeedClicking");
        }
    }

    public IEnumerator OnSpeedClicking()
    {
        isInterval = true;
        User.isSpeedGame = !User.isSpeedGame;
        RefreshUI();
        yield return new WaitForSeconds(0.2f);
        isInterval = false;
    }

    public void SetStageSpeed(bool on)
    {
        if (!Common.GetSceneCompareTo(Common.SCENE.MAIN))
        {
            if (on)
                Time.timeScale = 1.3f;
            else
                Time.timeScale = 1.0f;
        }
    }

}
