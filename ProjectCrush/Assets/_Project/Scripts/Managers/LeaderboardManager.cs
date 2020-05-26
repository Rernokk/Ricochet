using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : Photon.PunBehaviour, IPunObservable
{
	public static LeaderboardManager Instance;

	[SerializeField]
	private int Counter;

	private Dictionary<int, int> TestDict = new Dictionary<int, int>();

	public void Start()
	{
		if (Instance == null)
		{
			Instance = this;
		} else if (Instance != this)
		{
			Destroy(this);
			return;
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(Counter);
			stream.SendNext(TestDict);
		}
		else
		{
			Counter = (int)stream.ReceiveNext();
			TestDict = (Dictionary<int, int>)stream.ReceiveNext();
		}
	}

	public void CallTestRPC()
	{
		photonView.RPC("BasicTestRPC", PhotonTargets.MasterClient);
	}

	[PunRPC]
	private void BasicTestRPC()
	{
		print("Writing from RPC!");
		Counter++;
		TestDict.Add(Random.Range(0, 10000), Random.Range(0, 10000));
	}

	public string GetDictKeys()
	{
		string ret = "";

		foreach (int k in TestDict.Keys)
		{
			ret += $"K:{k},V:{TestDict[k]}\n";
		}

		return ret;
	}
}
