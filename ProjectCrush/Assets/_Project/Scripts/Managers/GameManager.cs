using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Photon.PunBehaviour, IPunObservable
{
	[SerializeField]
	private GameObject playerPrefab;
	int tick = 0;

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(tick);
		}
		else
		{
			tick = (int)stream.ReceiveNext();
		}
	}

	void Start()
	{
		PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)).normalized * 5f, Quaternion.identity, 0);
		tick++;
	}
}
