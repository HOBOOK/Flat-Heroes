using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FollowMap : MonoBehaviour {

    public GameObject target;
    public float followingSpeed;
    public float defaultY;
    public bool isFixY;
    Vector3 pos;
    Transform t;
    private void Start()
    {
        if (target != null && t == null)
            t = target.transform;
        else if (t != target.transform)
            t = target.transform;
    }

    void Update() {
        if (isFixY)
        {
            transform.localPosition = new Vector3(t.transform.position.x * (-0.01f * followingSpeed), defaultY, followingSpeed);
        }
        else
        {
            transform.localPosition = new Vector3(t.transform.position.x * (-0.01f * followingSpeed), t.transform.position.y * (-0.01f * followingSpeed) + defaultY, followingSpeed);
        }
    }
}
