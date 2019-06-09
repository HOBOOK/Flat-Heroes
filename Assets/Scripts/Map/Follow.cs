using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public GameObject target;
    public float followingSpeed;
    public bool isFixY;
    Vector3 pos;
    Transform t;
    private void Start()
    {
        if (target != null && t == null)
            t = target.transform;
    }

    void Update()
    {
        if (isFixY)
        {
            transform.position = new Vector3(t.transform.position.x, transform.position.y);
        }
        else
        {
            transform.position = new Vector3(t.transform.position.x, t.transform.position.y);
        }
    }
}
