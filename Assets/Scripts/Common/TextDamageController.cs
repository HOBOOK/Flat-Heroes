using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDamageController : MonoBehaviour {

    GameObject textMesh;
    float popuptime;
    bool isOver;
    public bool isLeft = false;
    public bool isCritical = false;
    public bool isCC = false;
    Transform parent = null;
    Color initColor;
    private void Awake()
    {
        parent = GameObject.Find("CanvasUI").transform;
        textMesh = this.gameObject.transform.GetChild(1).gameObject;
        initColor = GetComponentInChildren<Text>().color;
    }

    private void OnEnable()
    {
        this.transform.localScale = new Vector3(1, 1, 1);

        DamagePop();
        isOver = false;
        textMesh.GetComponent<Text>().color = initColor;
    }
    void DamagePop()
    {
        if(isCritical)
        {
            this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            textMesh.SetActive(true);
            Vector2 popVelocity = isLeft ? new Vector2(Random.Range(-4, -6), Random.Range(5,8)) : new Vector2(Random.Range(4, 6), Random.Range(5, 8));
            GetComponent<Rigidbody2D>().AddForce(popVelocity, ForceMode2D.Impulse);
            this.GetComponent<Animator>().SetTrigger("criticalDamage");
        }
        else if(isCC)
        {
            this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            textMesh.SetActive(true);
            this.GetComponent<Animator>().SetTrigger("cc");
        }
        else
        {
            this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            textMesh.SetActive(true);
            Vector2 popVelocity = isLeft ? new Vector2(Random.Range(-2, -4), Random.Range(2, 4)) : new Vector2(Random.Range(2, 4), Random.Range(2, 4));
            GetComponent<Rigidbody2D>().AddForce(popVelocity, ForceMode2D.Impulse);
            this.GetComponent<Animator>().SetTrigger("damage");
        }

    }

    void Update ()
    {
        if (textMesh.GetComponent<Text>().color.a <= 0)
        {
            ObjectPool.Instance.PushToPool("damageUI", gameObject, parent);
        }
    }
}
