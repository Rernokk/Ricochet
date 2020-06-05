using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : Photon.PunBehaviour, IPunObservable
{
	protected int remainingBounces = ProjectileSettings.DEFAULT_PROJECTILE_BOUNCE_MAX;
	protected int damageToDeal = ProjectileSettings.DEFAULT_PROJECTILE_DAMAGE;
	protected float lifeTimer = ProjectileSettings.DEFAULT_PROJECTILE_LIFETIME;
	protected float projectileSpeed = ProjectileSettings.DEFAULT_PROJECTILE_SPEED;
	protected Rigidbody rgd;

	[SerializeField]
	private MeshRenderer myMesh;

	[SerializeField]
	private GameObject impactObject;

	#region Protected Methods

	protected virtual void Start()
	{
		rgd = GetComponent<Rigidbody>();

		if (photonView.isMine)
		{
			photonView.RPC("UpdateCustomizationsRPC", PhotonTargets.AllBuffered, new object[]
			{ PlayerPrefs.GetFloat("ProjectileRedChannel", 1), PlayerPrefs.GetFloat("ProjectileGreenChannel", 1), PlayerPrefs.GetFloat("ProjectileBlueChannel", 1), });
		}
	}

	[PunRPC]
	private void UpdateCustomizationsRPC(float r, float g, float b)
	{
		myMesh.material.color = new Color(r, g, b);
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		remainingBounces--;
		PlayerController other = collision.transform.root.GetComponent<PlayerController>();
		if (other != null)
		{
			if (photonView.isMine)
			{
				other.photonView.RPC("TakeDamageRPC", PhotonTargets.All, new object[] { damageToDeal, transform.position });
				//other.TakeDamage(damageToDeal);
				DestroyProjectile(other);
			}
		}
		else
		{
			if (photonView.isMine)
				photonView.RPC("SpawnFXRPC", PhotonTargets.All, new object[] { transform.position.x, transform.position.y, transform.position.z });
		}

		if (remainingBounces < 0)
		{
			DestroyProjectile();
		}
	}

	[PunRPC]
	protected virtual void SpawnFXRPC(float xPos, float yPos, float zPos)
	{
		Instantiate(impactObject, new Vector3(xPos, yPos, zPos), Quaternion.identity);
	}

	protected virtual void Update()
	{
		Vector3 vel = rgd.velocity;
		vel.y = 0;
		rgd.velocity = vel.normalized * projectileSpeed;
		if (lifeTimer <= 0f)
		{
			DestroyProjectile();
		}
		else
		{
			lifeTimer -= Time.deltaTime;
		}
	}

	protected virtual void DestroyProjectile(PlayerController other = null)
	{
		if (!photonView.isMine)
			return;

		LeaderboardManager mgr = GameObject.FindGameObjectWithTag("LeaderboardManager").GetComponent<LeaderboardManager>();
		if (other != null && other.CurrentHealth < damageToDeal)
		{
			mgr.GrantKillToPlayer(PhotonNetwork.player.ID);
		}

		// Owner of object deletes projectile.
		if (photonView.isMine)
		{
			PhotonNetwork.Destroy(gameObject);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(remainingBounces);
		}
		else
		{
			remainingBounces = (int)stream.ReceiveNext();
		}
	}

	#endregion Protected Methods
}
