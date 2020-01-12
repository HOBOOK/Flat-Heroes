using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public string InputString;
    public string OutString;

    private void Start()
    {
        Debug.Log(DataSecurityManager.DecryptData(InputString));
        Debug.Log(DataSecurityManager.EncryptData(OutString));
    }
}
