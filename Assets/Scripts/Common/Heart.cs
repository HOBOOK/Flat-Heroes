using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    public GameObject MagnetTarget;
    private int heartAmount;
    Vector3 heartPos;
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

    void Update()
    {
        if (MagnetTarget != null)
        {
            if (!isAwake)
            {
                StartCoroutine("StartHeart");
                isAwake = true;
            }
            else if (!isGet && isStart)
            {
                heartPos = Vector2.Lerp(this.transform.position, MagnetTarget.transform.position, 0.1f);
                heartPos.z = 0;
                this.transform.position = heartPos;
                distance = Vector2.Distance(heartPos, MagnetTarget.transform.position);
                if (distance < 0.3f)
                {
                    GetHeart(heartAmount, MagnetTarget.GetComponent<Hero>());
                    isGet = true;
                    ObjectPool.Instance.PushToPool("Heart", gameObject);
                }
            }
        }
    }
    IEnumerator StartHeart()
    {
        yield return new WaitForSeconds(2.0f);
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        this.GetComponent<Collider2D>().isTrigger = true;
        isStart = true;
        yield return null;
    }
    public void SetHeart(int amount)
    {
        heartAmount = amount;
    }

    void GetHeart(int amount, Hero hero)
    {
        hero.status.hp = Common.looHpPlus(hero.status.hp, hero.status.maxHp, amount);
        //Sound_Heart();
    }
    public void Sound_Heart()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.heal);
    }
}
