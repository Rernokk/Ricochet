using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerFireController : Photon.PunBehaviour
{
	[SerializeField]
	private GameObject projectilePrefab;

	private float playerSpeed = 12f;

	void Update()
	{
		if (!this.transform.root.GetComponent<PhotonView>().isMine)
		{
			return;
		}
		ProcessInputs();
	}

	private void ProcessInputs()
	{
		switch (Application.platform) {
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
			moveDir += transform.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			moveDir -= transform.forward;
		}
		if (Input.GetKey(KeyCode.A))
		{
			moveDir -= transform.right;
		}
		if (Input.GetKey(KeyCode.D))
		{
			moveDir += transform.right;
		}

		moveDir.y = 0;
		moveDir.Normalize();
		transform.root.position += moveDir * Time.deltaTime * playerSpeed;

		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			FireProjectile(Input.mousePosition);
		}
	}

	private void FireProjectile(Vector3 inputPos)
	{
		print("Attempting to fire projectile...");
		Ray rayToSurface = Camera.main.ScreenPointToRay(inputPos);
		RaycastHit info;
		Physics.Raycast(rayToSurface, out info, 200f, LayerMask.GetMask("MouseCollisionLayer"));
		if (info.transform != null)
		{
			Vector3 aimPos = info.point;
			//Debug.DrawRay(aimPos, Vector3.up, Color.red, 3f);
			Vector3 dir = info.point - transform.root.position;
			dir.y = 0;
			dir.Normalize();
			Debug.DrawRay(transform.root.position + Vector3.up * .5f, dir * 10f, Color.red, 1f);

			GameObject projectile =PhotonNetwork.Instantiate(this.projectilePrefab.name, transform.root.position + Vector3.up * .5f + dir * .5f, Quaternion.identity, 0);
			Destroy(projectile, 3f);
			Rigidbody rgd = projectile.GetComponent<Rigidbody>();
			if (rgd != null)
			{
				rgd.velocity = dir.normalized * 30f;
			} else
			{
				Debug.Log("Projectile rigidbody is null...");
			}
		}
	}
}
