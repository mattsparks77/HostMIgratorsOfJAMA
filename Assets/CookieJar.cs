using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CookieJar : NetworkBehaviour {

	[SyncVar][SerializeField] int cookies = 0;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	[Server]
	public void AddCookie(){
		cookies++;
	}
}
