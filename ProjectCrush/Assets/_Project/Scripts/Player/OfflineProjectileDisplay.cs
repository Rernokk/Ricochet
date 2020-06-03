using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineProjectileDisplay : MonoBehaviour
{
	[SerializeField]
	private float rotSpeed = 90f, bounceSpeed = 2f;

	void Update()
	{
		transform.Rotate(Vector3.up, Time.deltaTime * rotSpeed);
		transform.GetChild(0).localPosition = new Vector3(0, Mathf.Sin(Time.time * bounceSpeed) * .5f, transform.GetChild(0).localPosition.z);
	}
}
