using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    
	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}
    public GameObject bullet;
    public void Shoot()
    {
        Instantiate(bullet, transform.position, Quaternion.identity);
    }
}
