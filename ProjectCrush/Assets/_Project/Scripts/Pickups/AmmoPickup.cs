using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Photon.PunBehaviour
{
	protected int ammoToGive = PickupSettings.DEFAULT_PICKUP_AMMOGAIN;

	protected virtual void OnCollisionEnter(Collision collision)
	{
		PlayerManager other = collision.transform.root.GetComponent<PlayerManager>();
		if (other != null)
		{
			other.GainAmmo(ammoToGive);
			Debug.Log("giving " + ammoToGive.ToString() + " ammo");
			DestroyPickup();
		}
	}

	protected virtual void DestroyPickup()
	{
		if (photonView.isMine)
			PhotonNetwork.Destroy(gameObject);
	}
}
