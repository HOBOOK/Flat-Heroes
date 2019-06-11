using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    GameObject dropCoinUI;
    Transform MagnetTarget;
    private int coinAmount;
    Vector3 coinPos;
    Vector3 targetUIPos;
    float distance;
    bool isGet = false;
    bool isAwake = false;
    bool isStart = false;

    private void OnEnable()
    {
        isAwake = false;
        isStart = false;
        isGet = false;
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        this.GetComponent<Collider2D>().isTrigger = false;
    }
    private void Awake()
    {
        foreach(var i in GameObject.FindGameObjectsWithTag("CoinUI"))
        {
            if(i.GetComponent<Image>()!=null)
            {
                MagnetTarget = i.transform;
            }
        }
    }
    void Update()
    {
        if(MagnetTarget != null)
        {
            if(!isAwake)
            {
                StartCoroutine("StartCoin");
                isAwake = true;
            }
            else if(!isGet && isStart)
            {

                targetUIPos = MagnetTarget.transform.position;
                coinPos = Vector3.Lerp(this.transform.position, targetUIPos, 0.1f);
                coinPos.z = 0;
                this.transform.position = coinPos;
                distance = Vector2.Distance(coinPos, targetUIPos);
                if (distance < 0.5f)
                {
                    ObjectPool.Instance.PushToPool("dropItemUI", dropCoinUI, Common.CanvasUI());
                    GetCoin(coinAmount);
                    isGet = true;
                    ObjectPool.Instance.PushToPool("Coin", gameObject);
                    MagnetTarget.GetComponentInParent<UI_StageCoin>().GetEffect();
                }
            }
            if (dropCoinUI != null)
                dropCoinUI.transform.position = this.transform.position+new Vector3(0,0.5f);
        }
    }
    IEnumerator StartCoin()
    {
        Sound_Coin();
        yield return new WaitForSeconds(1.0f);
        dropCoinUI = Instantiate(ObjectPool.Instance.PopFromPool("dropItemUI"), Common.CanvasUI());
        dropCoinUI.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        dropCoinUI.SetActive(true);
        dropCoinUI.GetComponentInChildren<Text>().text = coinAmount.ToString();
        yield return new WaitForSeconds(1.0f);
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        this.GetComponent<Collider2D>().isTrigger = true;
        isStart = true;
        yield return null;
    }
    public void SetCoin(int amount)
    {
        coinAmount = amount;
    }

    void GetCoin(int amount)
    {
        if (StageManagement.instance != null)
            StageManagement.instance.stageInfo.stageCoin += amount;
        else
            Debugging.Log("Coin.cs 에서 StageManagement instance 가 null임");
        Sound_CoinGet();
        Debugging.Log(amount + " 코인 획득했습니다.");
    }
    public void Sound_Coin()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.coin);
    }
    public void Sound_CoinGet()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.coinGet);
    }

}
