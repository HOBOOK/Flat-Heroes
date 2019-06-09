using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
	public bool OnlyDeactivate;
    public string poolItemName = "BulletEffect";
    void OnEnable()
	{
		//StartCoroutine("CheckIfAlive");
	}

    private void Update()
    {
        if(!GetComponent<ParticleSystem>().IsAlive(true))
			{
				if(OnlyDeactivate)
				{
					#if UNITY_3_5
						this.gameObject.SetActiveRecursively(false);
					#else
						ObjectPool.Instance.PushToPool(poolItemName, gameObject);
					#endif
				}
				else
					GameObject.Destroy(this.gameObject);
			}
    }

    IEnumerator CheckIfAlive ()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.5f);
			if(!GetComponent<ParticleSystem>().IsAlive(true))
			{
				if(OnlyDeactivate)
				{
					#if UNITY_3_5
						this.gameObject.SetActiveRecursively(false);
					#else
						ObjectPool.Instance.PushToPool(poolItemName, gameObject);
					#endif
				}
				else
					GameObject.Destroy(this.gameObject);
				break;
			}
		}
	}
}
