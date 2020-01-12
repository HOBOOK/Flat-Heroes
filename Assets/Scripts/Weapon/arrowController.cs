using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowController : MonoBehaviour
{

    public string poolItemName;
    public bool isAlly = false;
    public bool isStand = false;
    public bool isCritical;
    public int damage = 10;
    public float pent;
    float arrowTimer;
    public Transform target;
    Vector3 prevPos;
    Vector3 targetPos;
    Vector3 startPos;
    float vx, vy, vz;
    float angle;
    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        isStand = false;
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        targetPos = Vector3.zero;
        startPos = this.transform.position;
        prevPos = transform.position;

        arrowTimer = 0;
        vx = 0; vy = 0; vz = 0;
    }
    private void FixedUpdate()
    {
        ArrowMoving();
    }
    public void ArrowMoving()
    {
        if (!isStand && target != null)
        {
            if (targetPos == Vector3.zero)
            {
                startPos = transform.position;
                targetPos = target.transform.position;
                if(targetPos.x>startPos.x)
                    targetPos.x = Random.Range(targetPos.x+0.5f, targetPos.x+1.5f);
                else
                    targetPos.x = Random.Range(targetPos.x - 0.5f, targetPos.x - 1.5f);
                if (Mathf.Sqrt((Mathf.Pow(targetPos.x, 2)-Mathf.Pow(startPos.x,2)))<10)
                {
                    if(targetPos.x>startPos.x)
                        targetPos.x = startPos.x + 10;
                    else
                        targetPos.x = startPos.x - 10;
                }
                targetPos.y = Random.Range(targetPos.y - 1f, targetPos.y +1.5f);
                vx = (targetPos.x - startPos.x) / 2f;
                vy = (targetPos.y - startPos.y + 9.8f) / 2f;
                vz = (targetPos.z - startPos.z) / 2f;
                if(vx<1.5f&&vx>0)
                {
                    vx = 1.5f;
                    vy = 2.5f;
                }
                else if(vx<0&&vx>-1.5f)
                {
                    vx = -1.5f;
                    vy = 2.5f;
                }
            }
            else
            {
                arrowTimer += Time.deltaTime;
                float sx = startPos.x + vx * arrowTimer * 4f;
                float sy = startPos.y + vy * arrowTimer - 1f * 9.8f * arrowTimer * arrowTimer;
                float sz = startPos.z + vz * arrowTimer;
                if (sy > target.transform.position.y + 2)
                    sy = target.transform.position.y + 2;
                transform.position = new Vector3(sx, sy, sz);
            }
            SetArrowAngle();
        }

    }
    public void SetArrowAngle()
    {
        Vector3 deltaPos = transform.position - prevPos;
        angle = Mathf.Atan2(deltaPos.y, deltaPos.x) * Mathf.Rad2Deg;
        if (0 != angle)
        {
            transform.rotation = Quaternion.Euler(0, 0, angle);
            prevPos = transform.position;
        }
    }
    public void ArrowStand(Transform parent)
    {
        if (!isStand)
        {
            this.transform.parent = parent.transform;
            if(parent.GetComponentInParent<Hero>()!=null)
            {
                parent.GetComponentInParent<Hero>().HittedByObject(damage, isCritical, new Vector2(2, 1),pent);
            }
            else if (parent.GetComponent<Castle>() != null)
            {
                parent.GetComponent<Castle>().HittedByObject(damage, isCritical, new Vector2(2, 1));
            }

            isStand = true;
            StartCoroutine("ArrowStanding");
        }
    }
    IEnumerator ArrowStanding()
    {
        TriggerEffet();
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        rigid.velocity = Vector2.zero;
        yield return new WaitForSeconds(5.0f);
        ObjectPool.Instance.PushToPool(poolItemName, gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 31)
        {
            ArrowStand(collision.transform);
        }
    }

    private void TriggerEffet()
    {
        GameObject effect = EffectPool.Instance.PopFromPool("Arrow_Hit");
        effect.transform.position = this.transform.position+this.transform.right*0.5f;
        effect.SetActive(true);
    }

}
