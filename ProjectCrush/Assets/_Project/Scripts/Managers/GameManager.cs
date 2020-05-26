using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour
{
	[SerializeField]
	private GameObject playerPrefab;

	[SerializeField]
	private GameObject leaderboardPrefab;
	private LeaderboardManager leaderboardManager;

	public void Start()
	{
		PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)).normalized * 5f, Quaternion.identity, 0);

		//Master, spawn controllers.
		if (PhotonNetwork.isMasterClient)
		{
			leaderboardManager = PhotonNetwork.Instantiate(this.leaderboardPrefab.name, Vector3.zero, Quaternion.identity, 0).GetComponent<LeaderboardManager>();
			print("Is Master");
		}
	}

	public static void ReturnToMainMenu()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.LeaveRoom();
		}
		SceneManager.LoadScene("MainMenu");
	}
}