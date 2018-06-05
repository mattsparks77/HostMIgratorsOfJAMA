using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookieClick : MonoBehaviour {

	[HideInInspector] public CookieClickGateway gateway;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClick(){
		gateway.CmdClickCookie();
	}
}
