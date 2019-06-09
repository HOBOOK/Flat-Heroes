using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMove : MonoBehaviour {

    public float CircleRangeRadius = 7.0f;
    public bool isFlock = false;
    bool isFind = false;
    bool isLeftorRight = false;
    public bool isBackgroundFly = false;
    float scale;
    Transform Target;
    Transform Enemy;
    Animator animator;
    Vector3 Tpos;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    // Update is called once per frame
    void Update ()
    {
        if(isBackgroundFly)
        {
            this.transform.Translate(new Vector3(-0.1f * Time.deltaTime, 0, 0));
            if (scale < 0.5f)
                scale += Time.deltaTime * 0.001f;
            this.transform.localScale = new Vector3(0.3f - scale, 0.3f - scale, 0);
            animator.SetBool("isFly", true);
        }
        else
        {
            Flock();
        }

	}

    void Flock()
    {
        if(!isFlock)
        {
            string[] exceptLayer = {"mapObject", "Ignore Raycast", "Item", "UI", "BackField","InBuilding" };
            int layerMask = ~(LayerMask.GetMask(exceptLayer));
            RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, 2.0f, Vector2.up, 2.0f, layerMask);
            foreach(var i in hit)
            {
                if (i.transform.CompareTag("Player") || i.transform.CompareTag("Npc"))
                {
                    isFlock = true;
                    Enemy = i.transform;
                    break;
                }
            }
        }
        if(isFlock&&!isFind)
        {
            int layerMask = 1 << 29;
            RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, CircleRangeRadius, Vector2.up, CircleRangeRadius, layerMask);
            foreach (var i in hit)
            {
                if(Vector2.Distance(i.transform.position,Enemy.transform.position)>3)
                {
                    if ((Enemy.transform.position.x > transform.position.x && i.transform.position.x < transform.position.x)|| (Enemy.transform.position.x < transform.position.x && i.transform.position.x > transform.position.x))
                    {
                        Target = i.transform;
                        Tpos = Target.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0);
                        isFind = true;
                        animator.SetBool("isFly", true);

                        Debugging.Log("타겟발견 : " + Target.name);
                        break;
                    }
                }
            }

            if (!isFind)
            {
                foreach (var i in hit)
                {
                    if (Vector2.Distance(i.transform.position, Enemy.transform.position) > 5)
                    {
                        Target = i.transform;
                        Tpos = Target.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0.1f);
                        isFind = true;
                        animator.SetBool("isFly", true);

                        Debugging.Log("타겟발견 : " + Target.name);
                        break;
                    }
                }
                isFlock = false;
            }
        }
        if (isFind)
        {
            
            this.transform.position = Vector3.Lerp(this.transform.position, Tpos, 0.5f*Time.deltaTime);
            if (this.transform.position.x > Target.transform.position.x)
                isLeftorRight = true;
            else
                isLeftorRight = false;

            this.transform.rotation = Quaternion.Euler(new Vector3(0, isLeftorRight ? 0 : 180, 0));
            if (Vector2.Distance(this.transform.position, Tpos) < 0.05f)
            {
                isFind = false;
                isFlock = false;
                animator.SetBool("isFly", false);
            }
        }
    }
}
