using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FallenBlockDestroyer : NetworkBehaviour {

    [SerializeField] float groundHeight = -3f;
	
	// Update is called once per frame
	void Update () {
        if (!isServer)
            return;
        Rigidbody[] bodies = FindObjectsOfType<Rigidbody>();
        List<Rigidbody> toDelete = new List<Rigidbody>();
        foreach (Rigidbody body in bodies) {
            if (body.gameObject.layer == LayerMask.NameToLayer("Placed Block")) {
                if (body.transform.position.z < groundHeight) {
                    toDelete.Add(body);
                }
            }
        }
        bodies = toDelete.ToArray();
        for (int i = 0; i < toDelete.Count; ++i) {
            NetworkServer.UnSpawn(bodies[i].gameObject);
            Destroy(bodies[i].gameObject);
        }
	}
}
