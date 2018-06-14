using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FallenBlockDestroyer : NetworkBehaviour {

    [SerializeField] float groundHeight = -3f;
    [SerializeField] float checkInterval = 1.5f;

    float timer = 0;
	
	// Update is called once per frame
	void Update () {
        if (!isServer)
            return;
        timer += Time.deltaTime;
        if (timer < checkInterval)
            return;
        timer = 0f;
        Rigidbody[] bodies = FindObjectsOfType<Rigidbody>();
        List<Rigidbody> toDelete = new List<Rigidbody>();
        foreach (Rigidbody body in bodies) {
            if (body.gameObject.layer == LayerMask.NameToLayer("Placed Block")) {
                if (body.transform.position.y < groundHeight) {
                    toDelete.Add(body);
                }
            }
        }
        bodies = toDelete.ToArray();
        print(bodies.Length);
        for (int i = 0; i < toDelete.Count; ++i) {
            NetworkServer.UnSpawn(bodies[i].gameObject);
            Destroy(bodies[i].gameObject);
        }
	}
}
