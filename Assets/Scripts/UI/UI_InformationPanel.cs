using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InformationPanel : MonoBehaviour
{
    float time;
    bool isActive;
    Transform t;
    private void OnEnable()
    {
        isActive = true;
        time = 0.0f;
    }

    public void ShowInformation(Transform target, string txt)
    {
        t = target;
        this.transform.position = target.position+new Vector3(0,-0.5f,0);
        this.GetComponentInChildren<Text>().text = txt;
        isActive = true;
        time = 0.0f;
    }
    public void ShowInformation(Vector3 pos, string txt)
    {
        this.transform.position = pos;
        this.GetComponentInChildren<Text>().text = txt;
        isActive = true;
        time = 0.0f;
    }

    private void Update()
    {
        if(isActive)
        {
            time += Time.deltaTime;
            if(time>5.0f||(t!=null&&!t.gameObject.activeSelf))
            {
                time = 0.0f;
                isActive = false;
                this.gameObject.SetActive(false);
            }
        }
    }
}
