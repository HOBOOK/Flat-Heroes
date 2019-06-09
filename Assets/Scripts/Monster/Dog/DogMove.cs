using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogMove : MonoBehaviour {
    public void RigidPower()
    {
        Vector2 attackedVelocity = Vector2.zero;
        attackedVelocity = new Vector2(3, 2);
        GetComponent<Rigidbody2D>().AddForce(attackedVelocity, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            foreach (var i in GetComponentsInChildren<CapsuleCollider2D>())
            {
                i.isTrigger = true;
            }

        }
    }

    IEnumerator transparentSprite()
    {
        yield return new WaitForSeconds(1);
        var sprites = GetComponentsInChildren<SpriteRenderer>();

        float alpha = 1.0f;
        while (alpha >= 0)
        {
            foreach (var a in sprites)
            {
                if (!a.name.Contains("hair"))
                    a.color = new Color(1, 1, 1, alpha);
            }

            alpha -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        foreach (var a in sprites)
            a.color = new Color(1, 1, 1, 0);
        this.gameObject.SetActive(false);
        yield return null;

    }
}
