using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactFabricator : Photon.PunBehaviour
{
	[SerializeField]
	private float lifeRemains = 3f;

	[SerializeField]
	private GameObject puffObject;

	void Start()
	{
		if (PhotonNetwork.isMasterClient)
		{
			for (int i = 0; i < 3; i++)
			{
				GameObject obj = Instantiate(puffObject, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * .25f, Quaternion.identity);
				obj.transform.parent = transform;
			}
		}
	}

	private void Update()
	{
		if (lifeRemains <= 0)
		{
			Destroy(gameObject);
		} else
		{
			lifeRemains -= Time.deltaTime;
		}
	}
}
