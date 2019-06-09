using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpAndDownMap : MonoBehaviour {

    public float speed = 1;
    public bool isStart = false;
    public bool isAuto = true;
    private bool isUp = true;
    public bool isRolling = false;
    public float rollingSpeed = 90;
    public float LimitY = 10, LimitX;
    Vector3 initPos;
    Vector3 pos;
    private void Awake()
    {
        initPos = this.transform.position;
    }

    void Update ()
    {
        UpAndDown();
        Rolling();

    }
    void Rolling()
    {
        if(isRolling)
        {
            this.transform.Rotate(0, 0, rollingSpeed * Time.deltaTime);
        }
    }
    void UpAndDown()
    {
        if(isStart||isAuto)
        {
            if (pos.y > initPos.y + LimitY)
            {
                isStart = false;
                isUp = false;
            }
            else if(pos.y<initPos.y)
            {
                isStart = false;
                isUp = true;
            }
            if (isUp)
            {
                pos = this.transform.position;
                pos.y += speed * Time.deltaTime;
                this.transform.position = pos;
            }
            else
            {
                pos = this.transform.position;
                pos.y -= speed * Time.deltaTime;
                this.transform.position = pos;
            }

        }
    }
}
