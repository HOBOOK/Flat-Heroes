using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ModeSelect : MonoBehaviour
{
    public GameObject InfinityMode;
    public GameObject MainMode;
    public GameObject BossMode;
    private void OnEnable()
    {
        RefreshUI();
    }
    void RefreshUI()
    {
        if(InfinityMode != null)
        {
            if(MapSystem.IsAbleInfinityMode())
            {
                InfinityMode.GetComponent<Button>().enabled = true;
                InfinityMode.transform.GetChild(2).GetComponent<Image>().enabled = false;
            }
            else
            {
                InfinityMode.GetComponent<Button>().enabled = false;
                InfinityMode.transform.GetChild(2).GetComponent<Image>().enabled = true;

            }
        }
        if (BossMode != null)
        {
            if (MapSystem.IsAbleBossMode())
            {
                BossMode.GetComponent<Button>().enabled = true;
                BossMode.transform.GetChild(2).GetComponent<Image>().enabled = false;
            }
            else
            {
                BossMode.GetComponent<Button>().enabled = false;
                BossMode.transform.GetChild(2).GetComponent<Image>().enabled = true;

            }
        }
    }
}
