using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_damage : MonoBehaviour {

	// Use this for initialization
	void Start () {
        scale = this.transform.localScale;
	}
    Vector3 scale;
    float damageTime = 0.0f;
    private void OnEnable()
    {
        damageTime = 0.0f;
    }
    // Update is called once per frame
    void Update ()
    {
        damageTime += Time.deltaTime;

        this.transform.localScale= new Vector3(scale.x - damageTime, scale.y - damageTime, scale.z - damageTime);
        if (damageTime > 1.0f)
            this.gameObject.SetActive(false);
	}
}
