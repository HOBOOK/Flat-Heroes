using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Configuration : MonoBehaviour
{
    public Text UserDeviceIdentifierText;

    private void Awake()
    {
        UserDeviceIdentifierText.text = string.Format("플레이어 ID : {0}\r\n게임버전 : {1}", SystemInfo.deviceUniqueIdentifier,Application.version);
    }
}
