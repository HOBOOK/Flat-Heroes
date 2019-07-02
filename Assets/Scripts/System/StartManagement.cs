using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManagement : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);
        if(Common.GetSceneCompareTo(Common.SCENE.MAIN))
        {
            if (this.transform.childCount < 1)
                Destroy(this.gameObject);
        }
    }
}
