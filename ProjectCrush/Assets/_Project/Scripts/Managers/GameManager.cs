using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Photon.PunBehaviour
{
	[SerializeField]
	private GameObject playerPrefab;
	void Start()
	{
		PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0, 0, 0f), Quaternion.identity, 0);
	}
}
