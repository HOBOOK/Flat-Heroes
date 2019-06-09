using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class TriggerObject
{
    public GameObject prefabObject;
    [Range(0, 10)]
    public int count = 1;
    public float startTimeAtTargetPositionX;
    public Vector3 startPosition;
    [HideInInspector]
    public bool isTriggerEnd = false;
    public bool isOnceTrigger = false;

    public void Initialize(Transform parent = null)
    {
        for(int i = 0; i < count; i++)
            CreateItem(parent);
    }

    private GameObject CreateItem(Transform parent = null)
    {
        if (prefabObject != null)
        {
            GameObject item = Object.Instantiate(prefabObject) as GameObject;
            item.name = isOnceTrigger ? "Once" : prefabObject.name;
            item.transform.SetParent(parent);
            item.transform.position = startPosition;
            item.SetActive(true);
            isTriggerEnd = true;
            return item;
        }
        else
            return null;

    }
}


