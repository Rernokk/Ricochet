using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleFracture : Photon.PunBehaviour
{
	Dictionary<Transform, OriginalTransformData> originalPositionDicts = new Dictionary<Transform, OriginalTransformData>();

	[SerializeField]
	Vector3 localOffset = Vector3.zero;

	void Awake()
	{
		TriggerDetonation();
	}

	public void TriggerDetonation()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform childObj = transform.GetChild(i);
			Rigidbody rgd = childObj.gameObject.GetComponent<Rigidbody>();
			rgd.useGravity = true;
			rgd.AddExplosionForce(1500f, transform.position + localOffset, 3f);
			rgd.angularVelocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(-30f, 30f);
		}
	}

	public void TriggerDetonation(Vector3 detPos)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform childObj = transform.GetChild(i);
			Rigidbody rgd = childObj.gameObject.GetComponent<Rigidbody>();
			rgd.useGravity = true;
			rgd.AddExplosionForce(500f, detPos, 10f);
			rgd.angularVelocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(-30f, 30f);
		}
	}

	public void ResetShards()
	{
		PhotonNetwork.Destroy(gameObject);
		//for (int i = 0; i < transform.childCount; i++)
		//{
		//	Transform childObj = transform.GetChild(i);
		//	childObj.localPosition = originalPositionDicts[childObj].localPos;
		//	childObj.localRotation = originalPositionDicts[childObj].localRot;
		//	Rigidbody rgd = childObj.GetComponent<Rigidbody>();
		//	rgd.useGravity = false;
		//	rgd.velocity = Vector3.zero;
		//	rgd.angularVelocity = Vector3.zero;
		//}
	}
}

public class OriginalTransformData
{
	public Vector3 localPos;
	public Quaternion localRot;
}
