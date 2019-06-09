﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    Transform target;
    Vector3 cameraPos = Vector3.zero;
    // 마우스 버튼 이동 //
    public float dragSpeed = 2;
    private Vector3 dragOriginPos;
    // 마우스 줌 //
    float zoom;
    float otrhSize =4.5f;
    private void Start()
    {
        GetComponent<Camera>().orthographicSize = otrhSize;
        this.transform.position = new Vector3(-restricX(), 1.5f, -100);
    }

    public void ChangeTarget(GameObject t)
    {
        target = t.transform;
    }

    public float restricX()
    {
        float oSize = GetComponent<Camera>().orthographicSize;
        float size = 10 - (2 * (oSize - 5));
        return size;
    }

	void FixedUpdate ()
    {
        if (this.transform.position.x < -restricX())
            transform.position = new Vector3(-restricX(), 1.5f, -100);
        else if (this.transform.position.x > restricX())
            transform.position = new Vector3(restricX(), 1.5f, - 100);

        if (target != null)
        {
            cameraPos = Camera.main.WorldToViewportPoint(target.position);
            MoveCamera();
        }
        else
        {
            // 줌//
            zoom = Input.GetAxis("Mouse ScrollWheel") * -20 * Time.deltaTime;
            otrhSize += zoom;
            otrhSize = Mathf.Clamp(otrhSize, 4, 5);
            Camera.main.orthographicSize = otrhSize;
            // 줌 끝//

            // 스크롤 //
            if (Input.GetMouseButtonDown(0))
            {
                dragOriginPos = Input.mousePosition;
                return;
            }
            if (!Input.GetMouseButton(0)) return;
            cameraPos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOriginPos);
            if((this.transform.position.x - cameraPos.x * dragSpeed) >= -restricX() && (this.transform.position.x - cameraPos.x * dragSpeed ) <= restricX())
                transform.Translate(new Vector3(-cameraPos.x * dragSpeed, 0, 0 ), Space.World);
            // 스크롤 끝//
        }
    }
    private void MoveCamera()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(cameraX(), cameraY()), 2f * Time.smoothDeltaTime);
        transform.Translate(0, 0, -5);
    }

    private float cameraY()
    {
        if (target.position.y + 1.5f < 1.5f)
        {
            return target.position.y + 1.5f;
        }
        else
        {
            return 1.5f;
        }
    }

    private float cameraX()
    {
       return target.position.x;
    }
}
