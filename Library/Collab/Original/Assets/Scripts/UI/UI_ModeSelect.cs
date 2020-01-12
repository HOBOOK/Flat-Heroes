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
                InfinityMode.transform.GetChild(2).gameObject.SetActive(false);
                InfinityMode.transform.GetChild(3).GetComponent<Text>().text = string.Format("{0} pts", Common.GetThousandCommaText(User.InfinityRankPoint));
            }
            else
            {
                InfinityMode.GetComponent<Button>().enabled = false;
                InfinityMode.transform.GetChild(2).gameObject.SetActive(true);
                InfinityMode.transform.GetChild(3).GetComponent<Text>().text = LocalizationManager.GetText("modeWarningMessage1");
            }
        }
        if (BossMode != null)
        {
            if (MapSystem.IsAbleBossMode())
            {
                BossMode.GetComponent<Button>().enabled = true;
                BossMode.transform.GetChild(2).gameObject.SetActive(false);
                BossMode.transform.GetChild(3).GetComponent<Text>().gameObject.SetActive(false);
            }
            else
            {
                BossMode.GetComponent<Button>().enabled = false;
                BossMode.transform.GetChild(2).gameObject.SetActive(true);
                BossMode.transform.GetChild(3).GetComponent<Text>().gameObject.SetActive(true);
            }
        }
    }
}
