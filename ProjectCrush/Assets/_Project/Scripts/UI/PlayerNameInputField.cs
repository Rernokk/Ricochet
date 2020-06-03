using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
	private static string playerNamePrefKey = "PlayerName";

	// Start is called before the first frame update
	void Start()
	{
		InputField field = this.GetComponent<InputField>();
		string defaultName = "";
		
		if (Facepunch.Steamworks.Client.Instance != null)
		{
			field.enabled = false;
			defaultName = Facepunch.Steamworks.Client.Instance.Username;
			field.text = defaultName;
		}
		else
		{
			if (field != null)
			{
				if (PlayerPrefs.HasKey(playerNamePrefKey))
				{
					defaultName = PlayerPrefs.GetString(playerNamePrefKey);
					field.text = defaultName;
				}
			}
		}
		PhotonNetwork.playerName = defaultName;
	}

	public void SetPlayerName(string value)
	{
		PhotonNetwork.playerName = value + " ";

		PlayerPrefs.SetString(playerNamePrefKey, value);
	}
}