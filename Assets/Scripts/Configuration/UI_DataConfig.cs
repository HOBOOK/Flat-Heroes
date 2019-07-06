using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DataConfig : MonoBehaviour
{
    public GameObject SyncPanel;
    public GameObject NoSyncPanel;

    private void OnEnable()
    {
        if(GoogleSignManager.isServerLogin)
        {
            SyncPanel.gameObject.SetActive(true);
            NoSyncPanel.gameObject.SetActive(false);
            SyncPanel.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0} {1}",LocalizationManager.GetText("configLastSaveTimeText") , GoogleSignManager.lastSaveTime);
        }
        else
        {
            SyncPanel.gameObject.SetActive(false);
            NoSyncPanel.gameObject.SetActive(true);
        }
    }

    public void OnClickPassivSave()
    {
        GoogleSignManager.SaveData();
    }

    public void OnClickSyncInitData()
    {
        Debug.Log("싱크하기");
    }
}
