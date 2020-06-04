using Facepunch.Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConnectionManager : Photon.PunBehaviour
{
	public static PlayerConnectionManager Instance;

	public Dictionary<int, PhotonPlayer> PlayerDictionary = new Dictionary<int, PhotonPlayer>();

	private void Start()
	{
		if (Instance == null)
		{
			Instance = this;
		} else if (Instance != this)
		{
			Destroy(this);
			return;
		}

		foreach (PhotonPlayer player in PhotonNetwork.playerList)
		{
			PlayerDictionary.Add(player.ID, player);
		}
		LeaderboardManager.UpdateLeaderboard();
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		Console.WriteLine($"New player connection: {newPlayer.NickName}");
		PlayerDictionary.Add(newPlayer.ID, newPlayer);
		LeaderboardManager.UpdateLeaderboard();
	}
}
