using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickerSpawner : MonoBehaviour {
	
	[HideInInspector] public ClickerSpawnGateway gateway;

	public void OnClick(){
		gateway.CmdSpawnClicker();
	}

	void SpawnClicker(){

	}
}
