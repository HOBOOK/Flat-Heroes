using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UI_version : MonoBehaviour
{
    void Awake()
    {
        this.GetComponent<Text>().text = Application.version.ToString();
    }
}
