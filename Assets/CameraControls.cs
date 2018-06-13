using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour {

    [SerializeField] float moveSpeed = 3f;
	
	// Update is called once per frame
	void Update () {
        Vector3 camPosition = Vector3.zero;
        camPosition += Vector3.right * ((Input.GetKey(KeyCode.D)?1:0) + (Input.GetKey(KeyCode.A)?-1:0));
        camPosition += Vector3.up * ((Input.GetKey(KeyCode.W) ? 1 : 0) + (Input.GetKey(KeyCode.S) ? -1 : 0));
        camPosition *= Time.deltaTime * moveSpeed;
        transform.position += camPosition;
	}
}
