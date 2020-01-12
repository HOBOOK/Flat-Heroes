using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    Camera cam;
    public Camera ShowCamera;
    Transform target;
    Vector3 cameraPos = Vector3.zero;
    // 마우스 버튼 이동 //
    public float dragSpeed = 2;
    private Vector3 dragOriginPos;
    // 마우스 줌 //
    float zoom;
    public float otrhSize =4.5f;
    public float limitX = 5.0f;
    public float startX = 0.0f;
    //public bool isInControl = false;
    //float controlTime;
    public int stageType = 0; // 0 : Main 1 : Infinity
    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = otrhSize;
        this.transform.position = new Vector3(startX, 1f, -100);
    }

    public void ChangeTarget(GameObject t)
    {
        target = t.transform;
    }
    public void TargetOff()
    {
        target = null;
    }

    public float restricX()
    {
        float oSize = GetComponent<Camera>().orthographicSize;
        if (limitX > 0)
            return limitX - (2 * (oSize - 5));
        else
            return 0;
    }
    private float restricY()
    {
        if(stageType == 0)
        {
            return 1f;
        }
        else
        {
            return 1 + ((cam.orthographicSize - 4.5f) * 0.667f);
        }
    }
    void Zoom(float increment)
    {
        if(stageType==0)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, 4.25f, 4.75f);
        }
        else
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, 4.5f, 7.5f);
        }
        if (ShowCamera != null)
            ShowCamera.orthographicSize = Camera.main.orthographicSize;
    }
    private void MoveCamera()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(cameraX(), cameraY()), cam.orthographicSize * Time.smoothDeltaTime);
        transform.Translate(0, 0, -5);
    }
    private void AutoCamera()
    {
        if (target == null && User.isAutoCam)
        {
            float centerX = Common.GetCenterPositionXwithHeros();
            transform.position = Vector3.Lerp(transform.position, new Vector3(centerX, 0.7f, 0), 2f * Time.smoothDeltaTime);
            transform.Translate(0, 0, -5);

        }
    }

    private float cameraY()
    {
        if(stageType==0)
        {
            if (target != null && target.position.y + 17f < 1f)
            {
                return target.position.y + 1f;
            }
            else
            {
                return 1f;
            }
        }
        else
        {
            if (target != null && target.position.y + 17f < restricY())
            {
                return target.position.y + restricY();
            }
            else
            {
                return restricY();
            }
        }
    }


    private float cameraX()
    {
        return target.position.x;
    }

    void FixedUpdate ()
    {
        AutoCamera();

        if (this.transform.position.x < -restricX())
            transform.position = new Vector3(-restricX(), restricY(), -100);
        else if (this.transform.position.x > restricX())
            transform.position = new Vector3(restricX(), restricY(), - 100);
        else
            transform.position = new Vector3(transform.position.x, restricY(), -100);

        if (target != null)
        {
            cameraPos = Camera.main.WorldToViewportPoint(target.position);
            MoveCamera();
        }
        else
        {
            // 줌//
            if(Input.touchCount == 2)
            {
 
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;
                Zoom(difference * 0.01f);
            }
            else
            {
                zoom = Input.GetAxis("Mouse ScrollWheel") * 25 * Time.deltaTime;
                Zoom(zoom);
            }
            // 줌 끝//

            // 스크롤 //
            if(!User.isAutoCam)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    dragOriginPos = Input.mousePosition;
                    return;
                }
                if (!Input.GetMouseButton(0)) return;
                cameraPos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOriginPos);
                if ((this.transform.position.x - cameraPos.x * dragSpeed) >= -restricX() && (this.transform.position.x - cameraPos.x * dragSpeed) <= restricX())
                    transform.Translate(new Vector3(-cameraPos.x * dragSpeed, 0, 0), Space.World);
            }

            // 스크롤 끝//
        }
    }

}
