using Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : PunBehaviour
{
	private GameModeSettings.GameMode activeGameMode;
	private GameModeBase currentGameModeReference;
	public static GameModeManager Instance;

	public GameModeSettings.GameMode ActiveGameMode
	{
		get
		{
			return activeGameMode;
		}
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		} else if (Instance != this)
		{
			Destroy(this);
			return;
		}

		activeGameMode = (GameModeSettings.GameMode)PhotonNetwork.room.CustomProperties["GAMEMODE"];
		if (photonView.isMine)
		{
			photonView.RPC("SendGameModeRPC", PhotonTargets.AllBuffered, GameModeSettings.GameMode.OneVOne);
		}
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		base.OnPhotonPlayerConnected(newPlayer);
		if (PhotonNetwork.playerList.Length >= 2)
		{
			currentGameModeReference.StartMatch();
		}
	}

	[PunRPC]
	private void SendGameModeRPC(GameModeSettings.GameMode mode)
	{
		activeGameMode = mode;
		switch (activeGameMode)
		{
			case (GameModeSettings.GameMode.OneVOne):
				currentGameModeReference = gameObject.AddComponent<OneOnOneMode>();
				photonView.ObservedComponents.Add(currentGameModeReference);
				break;
			default:
				break;
		}
	}
}
