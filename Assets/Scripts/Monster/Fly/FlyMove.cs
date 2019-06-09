using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMove : MonoBehaviour {

    public Vector3 firstPos;
    Vector3 currentPos;
    bool isLeft = false;
    bool isUp = false;
    public GameObject Target;
    public bool isUpandDown = false;
    public float restricX = 3;
    public float moveSpeed = 2;
	void Start ()
    {
        firstPos = this.transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        currentPos = this.transform.position;
        if (Target != null)
            firstPos = Target.transform.position;

        if(currentPos.x <firstPos.x - restricX)
        {
            isLeft = false;
        }
        else if(currentPos.x > firstPos.x + restricX)
        {
            isLeft = true;
        }

        if(isUpandDown)
        {
            if (currentPos.y < firstPos.y - 0.3f)
            {
                isUp = true;
            }
            else if(currentPos.y > firstPos.y + 0.3f)
            {
                isUp = false;
            }

            if(isUp)
            {
                currentPos += Vector3.up * moveSpeed * 0.3f * Time.deltaTime;
            }
            else
            {
                currentPos += Vector3.down * moveSpeed * 0.3f * Time.deltaTime;
            }
        }

        if (isLeft)
        {
            currentPos += Vector3.left * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPos += Vector3.right * moveSpeed * Time.deltaTime;
        }
        transform.rotation = Quaternion.Euler(0, isLeft ? 0 : 180, 0);
        this.transform.position = currentPos; ;
    }
}
