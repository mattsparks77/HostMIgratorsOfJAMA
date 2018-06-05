using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Autoclicker : NetworkBehaviour {

	[SerializeField] float rate = 1f;

	CookieJar jar;
	Player owner;

	float timer = 0;

	// Use this for initialization
	void Start () {
		jar = FindObjectOfType<CookieJar>();
	}
	
	// Update is called once per frame
	void Update () {
		Run();
	}

	void Run(){
		timer += Time.deltaTime;
		if (timer >= 1 / rate){
			Click();
			timer -= 1 / rate;
		}
	}

	void Click(){
		if (isServer)
			jar.AddCookie();
	}
}
