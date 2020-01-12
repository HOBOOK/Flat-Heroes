using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecompositionObject : MonoBehaviour
{
    bool isCloneEnd = false;
    bool isLeft = false;
    public bool isCloneObject = false;
    public bool isRolling = false;
    public string TargetTag = "";
    public int Damage;
    public bool isCritical;
    Vector3 scale;
    float mass;
    GameObject firstShape;
    private void Awake()
    {
        mass = GetComponent<Rigidbody2D>().mass;
        if(this.gameObject.transform.localScale.x < 1)
            scale = this.gameObject.transform.lossyScale* 1.5f;
        else
            scale = this.gameObject.transform.lossyScale * 0.5f;

        firstShape = this.gameObject;
    }
    private void Update()
    {
        if(isRolling)
        {
            this.transform.Rotate(0, 0, 1000 * Time.deltaTime);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.transform.CompareTag(TargetTag) ||collision.gameObject.layer==31) && !isCloneEnd && gameObject.transform.localScale.x > 0.1f)
        {
            if (collision.transform.parent.position.x > transform.position.x)
                isLeft = false;
            else
                isLeft = true;

            Collider2D[] rh2d = Physics2D.OverlapCircleAll(this.transform.position, this.GetComponent<SpriteRenderer>().bounds.size.x * 0.5f);
            foreach(var hero in rh2d)
            {
                if (hero.transform.GetComponentInParent<Hero>() != null&& hero.transform.GetComponentInParent<Hero>().gameObject.CompareTag(TargetTag) && !hero.transform.GetComponentInParent<Hero>().isUnBeat)
                {
                    hero.transform.GetComponentInParent<Hero>().HittedByObject(Damage, isCritical,Vector2.zero);
                    hero.transform.GetComponentInParent<Hero>().Stunned();
                }
                else if(hero.gameObject.layer==12&&hero.transform.GetComponent<Castle>()!=null)
                {
                    hero.transform.GetComponent<Castle>().HittedByObject(Damage, isCritical, Vector2.zero);
                }
            }
            CloneObject();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.transform.CompareTag(TargetTag) || collision.gameObject.layer == 31) && !isCloneEnd && gameObject.transform.localScale.x > 0.1f)
        {
            if (collision.transform.parent.position.x > transform.position.x)
                isLeft = false;
            else
                isLeft = true;

            Collider2D[] rh2d = Physics2D.OverlapCircleAll(this.transform.position, this.GetComponent<SpriteRenderer>().bounds.size.x * 0.5f);
            foreach (var hero in rh2d)
            {
                if (hero.transform.GetComponentInParent<Hero>() != null && hero.transform.GetComponentInParent<Hero>().gameObject.CompareTag(TargetTag) && !hero.transform.GetComponentInParent<Hero>().isUnBeat)
                {
                    hero.transform.GetComponentInParent<Hero>().HittedByObject(Damage, isCritical, Vector2.zero);
                    hero.transform.GetComponentInParent<Hero>().Stunned();
                }
                else if (hero.gameObject.layer == 12 && hero.transform.GetComponent<Castle>() != null)
                {
                    hero.transform.GetComponent<Castle>().HittedByObject(Damage, isCritical, Vector2.zero);
                }
            }
            CloneObject();
        }
    }
    public void CloneObject()
    {
        if(!isCloneEnd)
        {
            Sound_StoneCrack();
            StartCoroutine("CloningObject");
            isCloneEnd = true;
        }
    }
    public void Sound_StoneCrack()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.stoneCrack);
    }

    void CreateObject()
    {
        GameObject cloneObj = Instantiate(gameObject, gameObject.transform);
        Destroy(cloneObj.GetComponent<DecompositionObject>());
        cloneObj.transform.position = transform.position + new Vector3(Random.Range(-0.5f,0.5f), Random.Range(-0.5f, 0.5f),0);
        cloneObj.transform.localScale = scale;
        cloneObj.tag = "Untagged";
        cloneObj.GetComponent<DecompositionObject>().isCloneObject = true;
        if(cloneObj.GetComponent<BackObjectPool>()!=null)
            Destroy(cloneObj.GetComponent<BackObjectPool>());
        cloneObj.gameObject.SetActive(true);
        Vector3 addForce = new Vector3(isLeft ? 200 : -200, 600, isLeft ? 200 : -200);
        cloneObj.GetComponent<Rigidbody2D>().AddForce(addForce, ForceMode2D.Impulse);
        StartCoroutine("DisappearObj");
    }

    public IEnumerator CloningObject()
    {
        if (isRolling)
            isRolling = false;
        gameObject.transform.localScale *= 0.5f;
        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector3(isLeft ? 150 : -150, 600, isLeft ? 150 : -150), ForceMode2D.Impulse);
        int cnt = 0;
        while(cnt<2)
        {
            CreateObject();
            yield return new WaitForSeconds(0.05f);
            cnt++;
        }
        yield return null;
    }

    public IEnumerator DisappearObj()
    {
        Color thisColor = GetComponentInChildren<SpriteRenderer>().color;
        foreach (var i in this.GetComponentsInChildren<Collider2D>())
        {
            i.isTrigger = true;
        }
        float cnt = 1;
        while(cnt>0.1f)
        {
            foreach(var i in GetComponentsInChildren<SpriteRenderer>())
            {
                i.color = new Color(thisColor.r, thisColor.g, thisColor.b, cnt);
            }
            cnt -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        foreach (var i in GetComponentsInChildren<SpriteRenderer>())
        {
            i.color = new Color(thisColor.r, thisColor.g, thisColor.b, 0);
        }
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
        yield return null;
    }
}
