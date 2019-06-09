using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour {

    float standardTime = 5.0f;
    float animationTime = 0.0f;
    int randomStatus = 0;
    public float movePower = 1.0f;
    public int hp = 100;


    bool isLeftorRight = false;

    public GameObject attackPoint;
    Animator animator;
	void Start ()
    {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Normal();
	}

    private void FixedUpdate()
    {
        Running();
        Die();
    }


    void Normal()
    {
        animationTime += Time.deltaTime;
        if(animationTime>= standardTime)
        {
            animationTime = 0;
            standardTime = Random.Range(3.0f, 5.0f);
            randomStatus = Random.Range(0, 3);
            switch(randomStatus)
            {
                case 0:
                    Idle();
                    break;
                case 1:
                    Run();
                    break;
                case 2:
                    Idle();
                    break;
            }
        }
    }

    void Idle()
    {
        animator.SetBool("isRun", false);
        attackPoint.SetActive(false);
    }

    void Die()
    {
        if (hp <= 0)
        {
            this.gameObject.SetActive(false);
            Debug.Log("몹사망");
        }
        else
            return;
    }

    void Run()
    {
        animator.SetBool("isRun", true);
        isLeftorRight = Random.Range(0, 2) == 0 ? true : false;
        attackPoint.SetActive(true);
    }

    void Running()
    {
        if(!animator.GetBool("isRun"))
        {
            return;
        }
        else
        {
            Vector3 moveVelocity = Vector3.zero;

            if (isLeftorRight)
            {
                moveVelocity = Vector3.left;
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                moveVelocity = Vector3.right;
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            transform.position += moveVelocity * movePower * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bullet"))
        {
            hp = Common.looMinus(hp, Random.Range(5,15));
        }
    }
}
