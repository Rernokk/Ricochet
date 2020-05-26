using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : Photon.PunBehaviour
{
	protected int remainingBounces = ProjectileSettings.DEFAULT_PROJECTILE_BOUNCE_MAX;
	protected int damageToDeal = ProjectileSettings.DEFAULT_PROJECTILE_DAMAGE;
	protected float lifeTimer = ProjectileSettings.DEFAULT_PROJECTILE_LIFETIME;
	protected float projectileSpeed = ProjectileSettings.DEFAULT_PROJECTILE_SPEED;
	protected Rigidbody rgd;

	#region Protected Methods

	protected virtual void Start()
	{
		rgd = GetComponent<Rigidbody>();
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		remainingBounces--;
		PlayerManager other = collision.transform.root.GetComponent<PlayerManager>();
		if (other != null)
		{
			other.TakeDamage(damageToDeal);
			DestroyProjectile();
		}

		if (remainingBounces < 0)
		{
			DestroyProjectile();
		}
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

	protected virtual void DestroyProjectile()
	{
		if (PhotonNetwork.isMasterClient)
		{
			LeaderboardManager mgr = GameObject.FindGameObjectWithTag("LeaderboardManager").GetComponent<LeaderboardManager>();
			mgr.CallTestRPC();
		}

		transform.Find("VFX").GetComponent<ParticleSystem>().Stop();
		transform.Find("VFX").parent = null;

		// Owner of object deletes projectile.
		if (photonView.isMine)
		{
			PhotonNetwork.Destroy(gameObject);
		}
	}

	#endregion Protected Methods
}
