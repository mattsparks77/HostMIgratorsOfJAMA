using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
public class BlockSpawner : NetworkBehaviour {
	public GameObject block;
	public float movementSpeed = 5f;
	// Use this for initialization
	void Start() {
	}

	// Update is called once per frame
	void Update () {
//		float x_mov = Input.GetAxisRaw ("Horizontal");
//		float y_mov = Input.GetAxisRaw ("Vertical");
		Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= movementSpeed;
	
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (isLocalPlayer)
				Cmdtry_block_spawn ();
		}
	}
	[Command]
	void Cmdtry_block_spawn(){
	 {
			GameObject a_block = Instantiate (block, this.gameObject.transform);
			NetworkServer.Spawn (a_block);
		}

	}

}
