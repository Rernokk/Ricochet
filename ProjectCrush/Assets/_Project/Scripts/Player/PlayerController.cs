using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerController : Photon.PunBehaviour, IPunObservable
{
	[SerializeField] private GameObject projectilePrefab;
	[SerializeField] private GameObject playerUI;
	[SerializeField] private GameObject fracturePrefab;
	[SerializeField] private GameObject playerModelRoot;

	private CapsuleFracture fractureReference;

	private float playerSpeed = PlayerCharacterSettings.PLAYER_SPEED;
	private float ammoRegenTimer = PlayerCharacterSettings.AMMO_RECHARGE;
	private float respawnTimerTrigger = 0;
	private int currentHealth = PlayerCharacterSettings.MAX_HEALTH;

	[SerializeField]
	private int currentAmmo = PlayerCharacterSettings.MAX_AMMO;
	private int currentLives = PlayerCharacterSettings.MAX_LIVES;
	private Boolean isDisabled = false;
	private bool isGameOver = false;
	private bool isLocked = false;


	[SerializeField]
	private MeshRenderer baseMesh;

	private Rigidbody rgd;

	#region Properties

	public bool IsLocked
	{
		get
		{
			return isLocked;
		}

		set
		{
			isLocked = value;
		}
	}

	public bool IsDisabled
	{
		get
		{
			return isDisabled;
		}
	}

	public float RespawnTimerTrigger
	{
		get
		{ 
			return respawnTimerTrigger;
		}
	}

	public int CurrentHealth
	{
		get
		{
			return currentHealth;
		}

		set
		{
			currentHealth = value;
		}
	}
	public int CurrentAmmo
	{
		get
		{
			return currentAmmo;
		}

		set
		{
			currentAmmo = Mathf.Min(value, MaxAmmo);
			if (currentAmmo == MaxAmmo)
			{
				ammoRegenTimer = PlayerCharacterSettings.AMMO_RECHARGE;
			}
		}
	}
	public int CurrentLives
	{
		get
		{
			return currentLives;
		}

		set
		{
			currentLives = value;
		}
	}
	public int MaxHealth
	{
		get
		{
			return PlayerCharacterSettings.MAX_HEALTH;
		}
	}
	public int MaxAmmo
	{
		get
		{
			return PlayerCharacterSettings.MAX_AMMO;
		}
	}
	public int MaxLives
	{
		get
		{
			return PlayerCharacterSettings.MAX_LIVES;
		}
	}
	#endregion Properties;

	#region Public Methods
	public void TakeDamage(int amount)
	{
		CurrentHealth -= amount;
		if (CurrentHealth <= 0)
		{
			fractureReference = PhotonNetwork.Instantiate(fracturePrefab.name, transform.position, Quaternion.identity, 0).GetComponent<CapsuleFracture>();
			fractureReference.TriggerDetonation();
			respawnTimerTrigger = PlayerCharacterSettings.RESPAWN_COUNTDOWN_SECONDS;
			KillPlayer();
		}
	}

	public void EnablePlayer()
	{
		isDisabled = false;
		photonView.RPC("EnablePlayerRPC", PhotonTargets.All);
		Debug.Log("Enable Player");
	}

	public void DisablePlayer()
	{
		isDisabled = true;
		photonView.RPC("DisablePlayerRPC", PhotonTargets.All);
		Debug.Log("Disable Player");
	}
	public void DisablePlayer(bool gameOver)
	{
		isDisabled = true;
		isGameOver = gameOver;
		photonView.RPC("DisablePlayerRPC", PhotonTargets.All);
		Debug.Log("Disable Player");
	}

	public void LockPlayerControls()
	{
		print("Locking player controls.");
		isLocked = true;
	}
	public void UnlockPlayerControls()
	{
		isLocked = false;
	}

	[PunRPC]
	private void DisablePlayerRPC()
	{
		playerModelRoot.SetActive(false);
	}

	[PunRPC]
	private void EnablePlayerRPC()
	{
		playerModelRoot.SetActive(true);
	}

	//TODO: Display interface for appropriate players
	public void KillPlayer()
	{
		// Disables the player, effectively 'killing' them
		DisablePlayer();

		// Decrement lives
		CurrentLives -= 1;

		// If out of lives, display GameOver interface 
		// (should probably cause event so that both players recieve user specified interfaces based on who won)
		if (CurrentLives <= 0)
		{
			Debug.Log($"GAME OVER - Lives: {CurrentLives}");
		}
		else
		{
			//RespawnPlayer();
		}
	}

	public void RespawnPlayer()
	{
		//TODO: Couple seconds of immunity, and can't attack
		if (isGameOver)
			return;

		// Wait for timer to run out
		if (RespawnTimer())
		{
			//Debug.Log("Waiting to respawn...");
		}
		else
		{
			// Reset values
			respawnTimerTrigger = 0f;
			CurrentHealth = PlayerCharacterSettings.MAX_HEALTH;
			CurrentAmmo = PlayerCharacterSettings.MAX_AMMO;
			isDisabled = false;

			// Move the player to a random position
			int minDist = 3;
			int maxDist = 8;

			//Re-enable Model
			photonView.RPC("EnablePlayerRPC", PhotonTargets.All);

			Vector3 anchorPos = Vector3.zero;
			Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
			float respawnPoint = Random.Range(minDist, maxDist);
			transform.position = anchorPos + randomDir * respawnPoint;

			if (fractureReference != null)
				fractureReference.ResetShards();
		}
	}


	[PunRPC]
	public void TakeDamageRPC(int amnt, Vector3 pos)
	{
		if (photonView.isMine)
		{
			CurrentHealth -= amnt;
			if (CurrentHealth <= 0)
			{
				fractureReference = PhotonNetwork.Instantiate(fracturePrefab.name, transform.position + Vector3.up, Quaternion.identity, 0).GetComponent<CapsuleFracture>();
				fractureReference.TriggerDetonation();
				respawnTimerTrigger = PlayerCharacterSettings.RESPAWN_COUNTDOWN_SECONDS;
				KillPlayer();
			}
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(CurrentHealth);
			stream.SendNext(respawnTimerTrigger);
			stream.SendNext(isDisabled);
			stream.SendNext(isLocked);
		}
		else
		{
			CurrentHealth = (int)stream.ReceiveNext();
			respawnTimerTrigger = (float)stream.ReceiveNext();
			isDisabled = (bool)stream.ReceiveNext();
			isLocked = (bool)stream.ReceiveNext();
		}
	}

	public void UpdatePlayerCustomizations()
	{
		photonView.RPC("UpdateCustomizationsRPC", PhotonTargets.AllBuffered, new object[]
		{ PlayerPrefs.GetFloat("PlayerRedChannel", 1), PlayerPrefs.GetFloat("PlayerGreenChannel", 1), PlayerPrefs.GetFloat("PlayerBlueChannel", 1), });
	}

	[PunRPC]
	private void UpdateCustomizationsRPC(float r, float g, float b)
	{
		baseMesh.material.color = new Color(r, g, b);
	}

	#endregion Public Methods

	#region Private Methods

	private void Start()
	{
		rgd = transform.GetComponent<Rigidbody>();

		if (photonView.isMine)
		{
			playerUI = Instantiate(playerUI, Vector3.zero, Quaternion.identity);
			playerUI.GetComponent<UIController>().PlayerReference = this;
		}
	}

	private void Update()
	{
		if (!photonView.isMine)
		{
			return;
		}


		if (isDisabled)
		{
			RespawnPlayer();
			return;
		}
		else if (!isLocked)
		{
			ProcessInputs();
			RegenerateAmmo();
		}
	}

	private void ProcessInputs()
	{
		switch (Application.platform)
		{
			case (RuntimePlatform.WindowsEditor):
			case (RuntimePlatform.WindowsPlayer):
				ProcessPCInput();
				break;
			case (RuntimePlatform.Android):
				ProcessAndroidInput();
				break;
		}
	}

	private void ProcessAndroidInput()
	{
		throw new NotImplementedException();
		Vector3 moveDir = Vector3.zero;
		moveDir.x = -Input.acceleration.y;
		moveDir.z = Input.acceleration.x;
		moveDir.Normalize();
		transform.root.position += moveDir * Time.deltaTime * playerSpeed;

		if (Input.touches[0].phase == TouchPhase.Began)
		{
			FireProjectile(Input.touches[0].position);
		}
	}

	private void ProcessPCInput()
	{
		Vector3 moveDir = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			moveDir += Camera.main.transform.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			moveDir -= Camera.main.transform.forward;
		}
		if (Input.GetKey(KeyCode.A))
		{
			moveDir -= Camera.main.transform.right;
		}
		if (Input.GetKey(KeyCode.D))
		{
			moveDir += Camera.main.transform.right;
		}

		moveDir.y = 0;
		moveDir.Normalize();
		rgd.velocity = moveDir * playerSpeed + new Vector3(0, rgd.velocity.y, 0);

		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			FireProjectile(Input.mousePosition);
		}

		if (Input.GetKeyDown(KeyCode.L))
		{
			GameManager.ReturnToMainMenu();
		}
	}

	private void FireProjectile(Vector3 inputPos)
	{
		if (currentAmmo > 0)
		{
			Ray rayToSurface = Camera.main.ScreenPointToRay(inputPos);
			RaycastHit info;
			Physics.Raycast(rayToSurface, out info, 200f, LayerMask.GetMask("MouseCollisionLayer"));
			if (info.transform != null)
			{
				Vector3 aimPos = info.point;
				//Debug.DrawRay(aimPos, Vector3.up, Color.red, 3f);
				Vector3 dir = info.point - transform.position;
				dir.y = 0;
				dir.Normalize();
				Debug.DrawRay(transform.position + Vector3.up * .5f, dir * 10f, Color.red, 1f);

				GameObject projectile = PhotonNetwork.Instantiate(this.projectilePrefab.name, transform.position + Vector3.up * 1f + dir * 1f, Quaternion.identity, 0);
				currentAmmo--;
				Rigidbody rgd = projectile.GetComponent<Rigidbody>();
				if (rgd != null)
				{
					rgd.velocity = dir.normalized * 30f;
				}
				else
				{
					Debug.Log("Projectile rigidbody is null...");
				}
			}
		}
		else
		{
			TriggerOutOfAmmoFX();
		}
	}

	private void RegenerateAmmo()
	{
		if (currentAmmo < MaxAmmo)
		{
			ammoRegenTimer -= Time.deltaTime;
			if (ammoRegenTimer <= 0)
			{
				ammoRegenTimer += PlayerCharacterSettings.AMMO_RECHARGE;
				CurrentAmmo++;
				Debug.Log($"Reloaded one shot!: {CurrentAmmo}/{MaxAmmo}");
			}
		}
	}

	private void TriggerOutOfAmmoFX()
	{
		//TODO: Signal out of Ammo;
		Debug.Log("Out of Ammo!");
	}

	private bool RespawnTimer()
	{
		respawnTimerTrigger -= Time.deltaTime;

		if (respawnTimerTrigger <= 0)
		{
			//Debug.Log("Respawning...");
			return false;
		}
		else
		{
			//Debug.Log($"Respawning in {respawnTimerTrigger} seconds");
			return true;
		}
	}

	#endregion Private Methods
}
