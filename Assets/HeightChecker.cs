using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightChecker : MonoBehaviour {

    [SerializeField] float velocityThreshold = Vector3.kEpsilon;

    float bestHeight = 0f;
    float currentHeight = 0f;
	
	// Update is called once per frame
	void Update () {
        Rigidbody[] bodies = FindObjectsOfType<Rigidbody>();
        float height = 0f;
        foreach (Rigidbody body in bodies) {
            if (body.gameObject.layer == LayerMask.NameToLayer("Placed Block")) {
                float thisHeight = CheckHeight(body);
                if (thisHeight > height)
                    height = thisHeight;
            }
        }
        currentHeight = height;
        bestHeight = Mathf.Max(bestHeight, currentHeight);
	}

    float CheckHeight(Rigidbody body) {
        if (body.velocity.magnitude > velocityThreshold || body.angularVelocity.magnitude > velocityThreshold)
            return 0;
        return body.transform.position.y;
    }

    public float GetBestHeight() {
        return bestHeight;
    }

    public float GetCurrentHeight() {
        return currentHeight;
    }
}
