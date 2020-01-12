using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingMap : MonoBehaviour
{
    public Sprite sprite;

    public bool isShake = false;
    public float shakeAmount = 1.0f;
    Vector3 firstPos;
    Color firstColor;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
        firstPos = transform.position;
        firstColor = transform.GetComponent<SpriteRenderer>().color;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }
    private void Update()
    {
        if(Common.triggerObjectInitialize)
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            GetComponent<Collider2D>().isTrigger = false;
            shakeAmount = 1.0f;
            isShake = false;

            transform.position = firstPos;
            transform.GetComponent<SpriteRenderer>().color = firstColor;
        }

        if(isShake&&shakeAmount>0)
        {
            Vector2 shakePos = Random.insideUnitCircle * 0.07f;
            this.transform.position = new Vector3(firstPos.x + shakePos.x, firstPos.y + shakePos.y, transform.position.z);
            shakeAmount -= Time.deltaTime;
        }
        if(shakeAmount<=0&&isShake)
        {
            isShake = false;
            StartCoroutine("Falling");
        }

    }

    public IEnumerator Falling()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        GetComponent<Collider2D>().isTrigger = true;
        groundEffect();
        yield return new WaitForSeconds(0.3f);

        float cnt = 1;
        Color curretColor = GetComponent<SpriteRenderer>().color;
        while(cnt>0)
        {
            if (GetComponent<SpriteRenderer>() != null)
                GetComponent<SpriteRenderer>().color = new Color(curretColor.r, curretColor.g, curretColor.b, cnt);

            cnt -= 0.05f;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Common.triggerObjectInitialize)
            return;
        if (collision.transform.CompareTag("Player") && !isShake && (collision.transform.position.y - 0.5f)>(transform.position.y+ transform.GetComponent<SpriteRenderer>().bounds.size.y * 0.4f))
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.rumble);
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            isShake = true;
        }
    }

    public void Sound_StoneCrack()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.stoneCrack);
    }

    public void groundEffect()
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("KnockBack_Smoke");
        effect.transform.position = transform.position + new Vector3(0, transform.GetComponent<SpriteRenderer>().bounds.size.y * 0.4f);

        effect.SetActive(true);
    }

}
