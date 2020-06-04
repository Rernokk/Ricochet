using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	private PlayerManager playerRef;
	private Dictionary<UserInterface, bool> isDisplayed = new Dictionary<UserInterface, bool>();

	private enum UserInterface { Settings, Leaderboard };

	[SerializeField]
	private Text playerNickname;

	[SerializeField]
	private Text leaderboardUI;

	[SerializeField]
	private Text currentAmmoUI, maxAmmoUI;

	public PlayerManager PlayerReference
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

	private void Start()
	{
		foreach (UserInterface val in Enum.GetValues(typeof(UserInterface)))
		{
			isDisplayed.Add(val, true);
			FlipInterfaceDisplay(val);
		}
	}

	private void Update()
	{
		UpdateNickname();
		UpdateLeaderboard();
		UpdateAmmoCount();
		ProcessInputs();
	}

	private void ProcessInputs()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			FlipInterfaceDisplay(UserInterface.Settings);
		}

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			FlipInterfaceDisplay(UserInterface.Leaderboard);
		}
	}

	private void FlipInterfaceDisplay(UserInterface uiElement)
	{
		isDisplayed[uiElement] = !isDisplayed[uiElement];
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
		foreach (int key in LeaderboardManager.Instance.Leaderboard.Keys)
		{
			leaderboardUI.text += $"{PlayerConnectionManager.Instance.PlayerDictionary[key].NickName} ({key}): {LeaderboardManager.Instance.Leaderboard[key]}\n";
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
}
