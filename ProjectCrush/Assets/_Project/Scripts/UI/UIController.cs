using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public static UIController Instance;

	private PlayerController playerRef;
	private Dictionary<UserInterface, bool> isDisplayed = new Dictionary<UserInterface, bool>();

	public enum UserInterface { Settings, Leaderboard, GameOverScreen, GameMode };

	[SerializeField]
	private Text playerNickname;

	[SerializeField]
	private Text leaderboardUI, gameOverUI;

	[SerializeField]
	private Text currentAmmoUI, maxAmmoUI;

	[SerializeField]
	private Text respawnTimerUI;

	[SerializeField]
	private Text gameModeUI;

	[SerializeField]
	private Toggle isLocked, isDisabled;

	public PlayerController PlayerReference
	{
		get
		{
			return playerRef;
		}

		set
		{
			playerRef = value;
		}
	}

	public void ExitToMenu()
	{
		GameManager.ReturnToMainMenu();
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
	}

	private void Start()
	{
		foreach (UserInterface val in Enum.GetValues(typeof(UserInterface)))
		{
			isDisplayed.Add(val, true);
			FlipInterfaceDisplay(val);
		}

		FlipInterfaceDisplay(UserInterface.GameMode);
	}

	private void Update()
	{
		UpdateNickname();
		UpdateLeaderboard();
		UpdateAmmoCount();
		UpdateRespawnTimer();
		UpdateGameMode();
		UpdateToggle();
		ProcessInputs();
	}

	private void ProcessInputs()
	{
		if (GameModeBase.Instance.IsGameOver)
			return;

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			FlipInterfaceDisplay(UserInterface.Settings);
		}

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			FlipInterfaceDisplay(UserInterface.Leaderboard);
		}
	}

	public void FlipInterfaceDisplay(UserInterface uiElement)
	{
		isDisplayed[uiElement] = !isDisplayed[uiElement];
		CanvasGroup canvasGroup = transform.Find(uiElement.ToString()).GetComponent<CanvasGroup>();
		canvasGroup.blocksRaycasts = isDisplayed[uiElement];
		canvasGroup.interactable = isDisplayed[uiElement];
		canvasGroup.alpha = (isDisplayed[uiElement] ? 1 : 0);
	}

	public void FlipInterfaceDisplay(UserInterface uiElement, bool toState)
	{
		isDisplayed[uiElement] = toState;
		CanvasGroup canvasGroup = transform.Find(uiElement.ToString()).GetComponent<CanvasGroup>();
		canvasGroup.blocksRaycasts = isDisplayed[uiElement];
		canvasGroup.interactable = isDisplayed[uiElement];
		canvasGroup.alpha = (isDisplayed[uiElement] ? 1 : 0);
	}

	private void UpdateNickname()
	{
		playerNickname.text = PhotonNetwork.player.NickName;
	}

	private void UpdateLeaderboard()
	{
		leaderboardUI.text = "";
		gameOverUI.text = "";
		List<string> sortedKills = new List<string>();
		foreach (int key in LeaderboardManager.Instance.Leaderboard.Keys)
		{
			sortedKills.Add($"{LeaderboardManager.Instance.Leaderboard[key]},{key}");
		}
		sortedKills.Sort((q1, q2) => -q1.Split(',')[0].CompareTo(q2.Split(',')[0]));

		foreach (string entry in sortedKills)
		{
			int key = int.Parse(entry.Split(',')[1]);
			leaderboardUI.text += $"{PlayerConnectionManager.Instance.PlayerDictionary[key].NickName}: {LeaderboardManager.Instance.Leaderboard[key]} Kill(s)\n";
			gameOverUI.text += $"{PlayerConnectionManager.Instance.PlayerDictionary[key].NickName}: {LeaderboardManager.Instance.Leaderboard[key]} Kill(s)\n";
		}

	}

	private void UpdateAmmoCount()
	{
		maxAmmoUI.text = PlayerCharacterSettings.MAX_AMMO.ToString();

		if (playerRef != null)
		{
			currentAmmoUI.text = playerRef.CurrentAmmo.ToString();
		}
	}

	private void UpdateRespawnTimer()
	{
		if (playerRef.IsDisabled)
		{
			respawnTimerUI.GetComponent<CanvasGroup>().alpha = 1;
			respawnTimerUI.text = Mathf.CeilToInt(playerRef.RespawnTimerTrigger).ToString();
			Color refCol = respawnTimerUI.color;
			refCol.a = playerRef.RespawnTimerTrigger % 1;
			respawnTimerUI.color = refCol;
			respawnTimerUI.rectTransform.localScale = Vector3.one * (.5f + .5f * refCol.a);
		} else
		{
			respawnTimerUI.GetComponent<CanvasGroup>().alpha = 0;
		}
	}

	private void UpdateGameMode()
	{
		if (GameModeManager.Instance != null)
		{
			gameModeUI.text = $"{GameModeManager.Instance.ActiveGameMode.ToString()}";
			if (GameModeBase.Instance != null)
			{
				gameModeUI.text += $"\n{GameModeBase.Instance.ToString()}";
			}
		}
	}

	private void UpdateToggle()
	{
		isLocked.isOn = playerRef.IsLocked;
		isDisabled.isOn = playerRef.IsDisabled;
	}
}
