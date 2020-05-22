using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkLauncher : Photon.PunBehaviour
{
	bool isConnecting;
	public void ExitGame()
	{
		Application.Quit();
	}

	public void ConnectToMatch()
	{
		Connect();
	}

	private void Connect()
	{
		print("Attempting to connect to match...");
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.JoinRandomRoom();
		} else
		{
			print("Connecting to Master");
			isConnecting = PhotonNetwork.ConnectUsingSettings(NetworkSettings.GAME_VERSION);
		}
	}

	public override void OnConnectedToMaster()
	{
		print("Connected to Master");
		if (isConnecting)
		{
			print($"Total rooms: {PhotonNetwork.GetRoomList().Length}");
			print($"Joining Room: {PhotonNetwork.JoinRandomRoom()}");
			isConnecting = false;
		}
		print("Exit OnConnectedToMaster");
	}

	public override void OnJoinedRoom()
	{
		print("Joined Room");
		if (PhotonNetwork.room.PlayerCount <= 1)
		{
			print("First player in room, load arena.");
			PhotonNetwork.LoadLevel("SimpleArena");
		}
	}

	public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
	{
		print("Failed to join room... Creating new match.");
		PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = NetworkSettings.MAX_PLAYER_COUNT }, null);
	}

	private void Awake()
	{
		PhotonNetwork.automaticallySyncScene = true;
	}
}
