using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudDataButton : MonoBehaviour
{
    public void OnCloudSaveClick()
    {
        App.Instance.gpgsManager.SaveData();
        UI_Manager.instance.ShowAlert("", "클라우드 저장에 성공하였습니다.");
    }
    public void OnCloudLoadClick()
    {
        App.Instance.gpgsManager.LoadData(true);
        UI_Manager.instance.ShowAlert("", "클라우드 로드에 성공하였습니다.");
    }
}
