using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ProgressCircle : MonoBehaviour
{
    public float progress=0.0f;
    private void OnEnable()
    {
        progress = 0.0f;
    }
    private void Update()
    {
        if(progress>100)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetProgress(float p)
    {
        progress = p;
    }
}
