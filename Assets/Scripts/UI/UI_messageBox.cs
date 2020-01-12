using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_messageBox : MonoBehaviour
{
    bool isStart = false;
    Text messageText;
    private void Awake()
    {
        messageText = GetComponentInChildren<Text>();
    }
    private void OnEnable()
    {
        isStart = false;
    }
    public void StartMessage(string text, Transform tran)
    {
        if(!isStart)
        {
            isStart = true;
            messageText.text = text;
            StartCoroutine(Messaging(text, tran));
        }
    }

    IEnumerator Messaging(string txt, Transform tran)
    {
        Vector3 initPos = tran.transform.position + new Vector3(0, 1, 0);
        float width = this.GetComponent<RectTransform>().rect.width / 2;
        if (initPos.x - width < -960)
            initPos.x = -960;
        else if (initPos.x + width > 960)
            initPos.x = 960 - width;
        this.transform.position = initPos;
        float time = 0.0f;
        while(time<0.2f)
        {
            this.transform.localScale = new Vector3(1, time * 5f, 1);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(2.0f);
        ObjectPool.Instance.PushToPool("messageBox", this.gameObject, this.transform.parent);
        isStart = false;
    }
}
