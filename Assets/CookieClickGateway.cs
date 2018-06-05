using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CookieClickGateway : NetworkBehaviour {

	CookieJar jar;

	// Use this for initialization
	void Start () {
		jar = FindObjectOfType<CookieJar>();
		if (isLocalPlayer){
			CookieClick[] clickers = FindObjectsOfType<CookieClick>();
			foreach (CookieClick clicker in clickers)
				clicker.gateway = this;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	[Command]
	public void CmdClickCookie(){
		jar.AddCookie();
	}
}
