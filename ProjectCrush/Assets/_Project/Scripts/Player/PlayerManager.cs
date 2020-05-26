using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerManager : Photon.PunBehaviour, IPunObservable
{
	[SerializeField]
	private GameObject projectilePrefab;

	private float playerSpeed = PlayerCharacterSettings.PLAYER_SPEED;
	private float ammoRegenTimer = PlayerCharacterSettings.AMMO_RECHARGE;
	private int currentHealth = PlayerCharacterSettings.MAX_HEALTH;

	[SerializeField]
	private int currentAmmo = PlayerCharacterSettings.MAX_AMMO;

	[SerializeField]
	private Text textWindow;

	private Rigidbody rgd;

	#region Properties
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
	#endregion Properties;

	#region Public Methods

	public void TakeDamage(int amount)
	{
		CurrentHealth -= amount;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(CurrentHealth);
		}
		else
		{
			CurrentHealth = (int)stream.ReceiveNext();
		}
	}

	#endregion Public Methods

	#region Private Methods

	private void Start()
	{
		rgd = transform.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (!photonView.isMine)
		{
			return;
		}

		ProcessInputs();
		RegenerateAmmo();
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

		if (Input.GetKeyDown(KeyCode.J))
		{
			LeaderboardManager.Instance.CallTestRPC();
		}

		if (Input.GetKeyDown(KeyCode.K))
		{
			textWindow.text = LeaderboardManager.Instance.GetDictKeys();
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

	#endregion Private Methods
}
