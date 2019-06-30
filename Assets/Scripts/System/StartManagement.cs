using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManagement : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
        SaveSystem.LoadPlayer();
    }
}
