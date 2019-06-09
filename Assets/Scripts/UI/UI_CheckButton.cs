using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CheckButton : MonoBehaviour
{
    public bool isChecking = false;
    public bool isResult;

    public void ResultYES()
    {
        isResult = true;
        isChecking = true;
    }
    public void ResultNO()
    {
        isResult = false;
        isChecking = true;
    }

    private void OnEnable()
    {
        isChecking = false;
    }
}
