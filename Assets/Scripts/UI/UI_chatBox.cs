using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_chatBox : MonoBehaviour
{
    public Transform Target = null;
    public string chatText;
    public int correctionY = 1;
    Vector3 correctionPos;
    Vector3 initScale;
    int textCount;
    float _elapsedTime;
    float lifeTime = 3;
    float scale;
    bool scaleFlag = false;
    bool isCounting = false;
    
    Transform canvasUI;
    private void Awake()
    {
        canvasUI = GameObject.Find("CanvasUI").transform;
    }
    private void OnEnable()
    {
        initScale = this.transform.localScale;
        
        if (transform.rotation.y != 0)
        {
            correctionPos = new Vector3(-0.5f, correctionY * Random.Range(0.6f,0.8f), 0);
        }
        else
        {
            correctionPos = new Vector3(0.5f, correctionY * Random.Range(0.6f, 0.8f), 0);
        }
        this.GetComponentInChildren<Text>().text = "";
        textCount = chatText.Length;
        StartCoroutine("OpenChat");
        StartCoroutine("TypingChat");
    }
    private void OnDisable()
    {
        this.transform.localScale = initScale;
    }
    void LateUpdate ()
    {
        if(Target)
            this.transform.position = Target.position + correctionPos;
        if (isCounting)
        {
            if (scaleFlag)
            {
                scale += Time.deltaTime * 0.05f;
                this.transform.localScale = initScale + (initScale * scale);
                if (this.transform.localScale.x > initScale.x * 1.02f)
                    scaleFlag = false;
            }
            else
            {
                scale -= Time.deltaTime * 0.05f;
                this.transform.localScale = initScale + (initScale * scale);
                if (this.transform.localScale.x <= initScale.x)
                    scaleFlag = true;
            }

            if (GetTimer()>lifeTime)
            {
                SetTimer();
                isCounting = false;
                StartCoroutine("CloseChat");
            }
        }
    }
    IEnumerator OpenChat()
    {
        this.transform.localScale = Vector3.zero;
        float cnt = 0;
        while(!isCounting && cnt<initScale.x)
        {
            this.transform.localScale = new Vector3(cnt, cnt, cnt);
            cnt += initScale.x * 0.2f;
            yield return new WaitForEndOfFrame();
        }
        this.transform.localScale = initScale;
        yield return null;
    }
    IEnumerator CloseChat()
    {
        float cnt = this.transform.localScale.x;
        while (cnt > 0)
        {
            this.transform.localScale = new Vector3(cnt, cnt, cnt);
            cnt -= initScale.x * 0.2f;
            yield return new WaitForEndOfFrame();
        }
        this.transform.localScale = Vector3.zero;
        ObjectPool.Instance.PushToPool("chatBox", gameObject, canvasUI);
        yield return null;
    }
    IEnumerator TypingChat()
    {
        int cnt = 0;
        while(cnt<textCount)
        {
            this.GetComponentInChildren<Text>().text += chatText[cnt];
           yield return new WaitForSeconds(0.05f);
            if (cnt % 10 == 0 && cnt > 1&&textCount>10)
                this.GetComponentInChildren<Text>().text += "\r\n";
            cnt++;
            if (cnt >= textCount)
                isCounting = true;
        }

        yield return null;
    }
    float GetTimer()
    {
        return (_elapsedTime += Time.deltaTime);
    }
    void SetTimer()
    {
        _elapsedTime = 0f;
    }
}
