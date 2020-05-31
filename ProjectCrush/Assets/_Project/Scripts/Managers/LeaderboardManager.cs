using Facepunch.Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardManager : Photon.PunBehaviour, IPunObservable
{
	public static LeaderboardManager Instance;

	[SerializeField]
	private int Counter;

	//ID: Kills
	public Dictionary<int, int> Leaderboard = new Dictionary<int, int>();

	public void Start()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(this);
			return;
		}
	}

	private void Update()
	{
		// Duplicate objects instantiated from both clients without check.
		if (!PhotonNetwork.isMasterClient)
			return;
	}


	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(Counter);
			stream.SendNext(Leaderboard);
		}
		else
		{
			Counter = (int)stream.ReceiveNext();
			Leaderboard = (Dictionary<int, int>)stream.ReceiveNext();
		}
	}

	public void GrantKillToPlayer(int id)
	{
		photonView.RPC("GrantKillRPC", PhotonTargets.AllViaServer, id);
	}

	[PunRPC]
	private void GrantKillRPC(int id)
	{
		Leaderboard[id]++;
	}

	public static void UpdateLeaderboard()
	{
		foreach (PhotonPlayer player in PhotonNetwork.playerList)
		{
			if (!Instance.Leaderboard.ContainsKey(player.ID))
			{
				Instance.Leaderboard.Add(player.ID, 0);
			}
		}
	}
}
