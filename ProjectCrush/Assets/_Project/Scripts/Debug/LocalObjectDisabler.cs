using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalObjectDisabler : Photon.PunBehaviour
{
	[SerializeField]
	private List<GameObject> objectsToDisable;

	void Start()
	{
		if (!photonView.isMine)
		{
			foreach (GameObject obj in objectsToDisable)
			{
				obj.SetActive(false);
				obj.transform.tag = "Untagged";
			}
			return;
		}
	}
}
