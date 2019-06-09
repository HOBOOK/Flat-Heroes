using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grass : MonoBehaviour {
    private void Start()
    {
        if (!GetComponent<Animation>().isPlaying)
            GetComponent<Animation>().Play();
    }
}
