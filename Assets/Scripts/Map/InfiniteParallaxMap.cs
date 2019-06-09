using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(InfiniteMap))]
public class InfiniteParallaxMap : MonoBehaviour
{
    public Transform target;
    public Vector2 factor;

    InfiniteMap spriteCicle;

    void Awake()
    {
        spriteCicle = GetComponent<InfiniteMap>();
    }

    void Start()
    {
        if (!target)
        {
            if (Camera.main)
            {
                target = Camera.main.transform;
            }
        }
    }

    void Update()
    {
        if (target && spriteCicle)
        {
            spriteCicle.position = target.position.x * factor.x;

            Vector3 localPosition = transform.localPosition;
            localPosition.y = target.position.y * factor.y;
            transform.localPosition = localPosition;
        }
    }
}
