using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public bool isBoom = false;
    GameObject sprite;
    GameObject on;
    private float scale = 1.0f;
    private float endTime = 0.0f;
	// Use this for initialization
	void Start ()
    {
        on = transform.GetChild(0).gameObject;
        sprite = transform.GetChild(0).GetChild(0).gameObject;
	}
    private void OnEnable()
    {
        Camera.main.GetComponent<CameraEffectHandler>().HitShake(0.2f, 0.04f);
    }

    // Update is called once per frame
    void FixedUpdate () {
        Booming();
    }

    public void Booming()
    {
        if(isBoom)
        {
            on.gameObject.SetActive(true);

            scale += 1;

            if(scale<10)
            {
                sprite.transform.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                endTime += Time.deltaTime;
                if(endTime > 2.0f)
                {
                    isBoom = false;
                    scale = 1.0f;
                    endTime = 0.0f;
                    on.SetActive(false);
                }

            }
        }
    }
}
