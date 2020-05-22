using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDisabler : Photon.PunBehaviour
{
	[SerializeField]
	private GameObject refCam;

	void Start()
	{
		if (!photonView.isMine)
		{
			refCam.SetActive(false);
			refCam.transform.tag = "Untagged";
			return;
		}
	}
}
