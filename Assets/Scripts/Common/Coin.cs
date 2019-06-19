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

    private void OnEnable()
    {
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        this.GetComponent<Collider2D>().isTrigger = false;
        MagnetTarget = GameObject.FindGameObjectWithTag("CoinUI").transform;
        StartCoroutine("StartCoin");
    }
    void FixedUpdate()
    {
        if (dropCoinUI != null)
            dropCoinUI.transform.position = this.transform.position + new Vector3(0, 0.5f);
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
        StartCoroutine("GetCoin");
    }
    IEnumerator GetCoin()
    {
        targetUIPos = MagnetTarget.transform.position;
        coinPos = this.transform.position;
        distance = Vector2.Distance(coinPos, targetUIPos);
        Debugging.Log(distance + " 코인 시작 거리");
        while (distance > 0.2f)
        {
            targetUIPos = MagnetTarget.transform.position;
            coinPos = Vector3.Lerp(this.transform.position, targetUIPos, 0.1f);
            coinPos.z = 0;
            this.transform.position = coinPos;
            distance = Vector2.Distance(coinPos, targetUIPos);
            yield return new WaitForEndOfFrame();
        }
        ObjectPool.Instance.PushToPool("dropItemUI", dropCoinUI, Common.CanvasUI());
        GetCoin(coinAmount);
        ObjectPool.Instance.PushToPool("Coin", gameObject);
        MagnetTarget.GetComponentInParent<UI_StageCoin>().GetEffect();
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
