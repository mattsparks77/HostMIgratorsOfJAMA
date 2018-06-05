using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClickerSpawnGateway : NetworkBehaviour {

	[SerializeField] Autoclicker clickerPrefab;

	void Start(){
		if (isLocalPlayer){
			ClickerSpawner[] spawners = FindObjectsOfType<ClickerSpawner>();
			foreach (ClickerSpawner spawner in spawners)
				spawner.gateway = this;
		}
	}

	[Command]
	public void CmdSpawnClicker(){
		Autoclicker spawned = Instantiate(clickerPrefab);
		NetworkServer.Spawn(spawned.gameObject);
	}

}
