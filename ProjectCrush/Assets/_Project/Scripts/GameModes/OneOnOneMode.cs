using Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneOnOneMode : GameModeBase
{
	protected override void Awake()
	{
		matchTimer = GameModeSettings.OneVOneTimeLimit;
		base.Awake();
	}

	void Start()
	{
		Debug.Log("Added O3 Mode");
	}
}
