using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GameModeBase : Photon.PunBehaviour, IPunObservable
{
	public static GameModeBase Instance;
	protected float matchTimer = 300f;
	protected float startMatchTimer = 5f;
	protected bool isGameOver = false;
	protected bool hasGameStarted = false;
	protected bool matchStarting = false;

	public float MatchTimer
	{
		get
		{
			return matchTimer;
		}
		set
		{
			matchTimer = value;
		}
	}

	public bool IsGameOver
	{
		get
		{
			return isGameOver;
		}
	}

	protected virtual void Awake()
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

	protected virtual void Update()
	{
		if (!photonView.isMine)
			return;

		TickTimer();
	}

	public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(MatchTimer);
			stream.SendNext(hasGameStarted);
			stream.SendNext(matchStarting);
		}
		else
		{
			MatchTimer = (float)stream.ReceiveNext();
			hasGameStarted = (bool)stream.ReceiveNext();
			matchStarting = (bool)stream.ReceiveNext();
		}
	}

	public void StartMatch()
	{
		matchStarting = true;
		photonView.RPC("MatchStartLockPlayer", PhotonTargets.AllBuffered);
	}

	protected void TickTimer()
	{
		if (matchStarting)
		{
			if (!hasGameStarted)
			{
				startMatchTimer -= Time.deltaTime;
				if (startMatchTimer <= 0f)
				{
					hasGameStarted = true;
					photonView.RPC("MatchStartUnlockPlayer", PhotonTargets.AllBuffered);
				}
			}

			if (!isGameOver && hasGameStarted)
			{
				matchTimer -= Time.deltaTime;
				if (matchTimer < 0f)
				{
					Debug.Log("Match Over, times up!");
					TriggerGameOver();
				}
			}
		}
	}

	public override string ToString()
	{
		return $"{Mathf.FloorToInt(matchTimer / 60f).ToString("00")}:{(matchTimer % 60f).ToString("00")}";
	}

	protected virtual void TriggerGameOver()
	{
		photonView.RPC("TriggerGameOverRPC", PhotonTargets.AllBuffered);
		photonView.RPC("DisablePlayerControls", PhotonTargets.AllBuffered, true);
	}

	[PunRPC]
	protected virtual void MatchStartLockPlayer()
	{
		UIController.Instance.PlayerReference.LockPlayerControls();
	}

	[PunRPC]
	protected virtual void MatchStartUnlockPlayer()
	{
		UIController.Instance.PlayerReference.UnlockPlayerControls();
	}

	[PunRPC]
	protected virtual void DisablePlayerControls(bool isGameOver = false)
	{
		UIController.Instance.PlayerReference.DisablePlayer(isGameOver);
	}

	[PunRPC]
	protected virtual void EnablePlayerControls()
	{
		UIController.Instance.PlayerReference.EnablePlayer();
	}

	[PunRPC]
	protected virtual void TriggerGameOverRPC()
	{
		isGameOver = true;
		matchTimer = 0f;
		UIController.Instance.FlipInterfaceDisplay(UIController.UserInterface.Settings, true);
		UIController.Instance.FlipInterfaceDisplay(UIController.UserInterface.Leaderboard, false);
		UIController.Instance.FlipInterfaceDisplay(UIController.UserInterface.GameOverScreen, true);
		UIController.Instance.FlipInterfaceDisplay(UIController.UserInterface.GameMode, false);
	}
}
