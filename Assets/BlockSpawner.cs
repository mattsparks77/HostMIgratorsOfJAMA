﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class BlockSpawner : NetworkBehaviour {

    public GameObject[] block_list;
	public float movementSpeed = 5f;
    public GameObject selectedObject;
    public GameObject toDrop;
    private bool is_instantiated = false;
    [SerializeField] private HighlightBehaviour areaHighlightPrefab;
    private HighlightBehaviour areaHighlight;
    
    private int[] rotationAngles = { 0, 90, 180, 270 };
    private int currentRotation = 0;

    [SerializeField] private float timeTilNextBlock = 1.5f;
    private float timeCounter = 0;
    // Use this for initialization

    // Update is called once per frame
    void Update () {
        int direction = 0;
        //Get the direction based on what button they're pressing
        direction += Input.GetKeyDown(KeyCode.LeftArrow) ? -1 : 0;
        direction += Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0;

        timeCounter -= Time.deltaTime;
        if (!is_instantiated && timeCounter <= 0 &&isLocalPlayer)
        {
            int rand = Random.Range(0, 6);
            selectedObject = block_list[rand];

            //Choose block to spawn and re-enable highlight 
            toDrop = Instantiate(selectedObject, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform);
            //disables box colliders in indicator
            foreach (BoxCollider child in toDrop.GetComponentsInChildren<BoxCollider>())
            {
                child.enabled = false;
            }
            //Cmdtry_block_spawn();
            areaHighlight.AdjustHighlightScaleAndOffsetFor(toDrop);
            SetHighlightActive(true);

            resetCurrentRotation();
            is_instantiated = true;
        }
        if (toDrop) {
            RotateObject(direction);
            toDrop.transform.position = this.gameObject.transform.position;
            areaHighlight.MoveHighlightTo(toDrop);

            if (Input.GetKeyDown(KeyCode.Space) || timeCounter <= -3f) {//change value here to determine the amount of time it takes to auto drop block subtracting from 1.5f so at -3f it auto spawns in 4.5 seconds etc.
                if (isLocalPlayer) {
                    // Let the block drop and disable the highlight
                    Destroy(toDrop);
                    Cmdtry_block_spawn();
                    
                    is_instantiated = false;

                    SetHighlightActive(false);
                    timeCounter = timeTilNextBlock;
                    toDrop = null;
                    //next_selected_object();
                }
            }
        }
        
    }
    [Command]
    void Cmd_change_gravity()
    {

    }
	[Command]
	void Cmdtry_block_spawn(){
	 {
            //GameObject a_block = Instantiate (selectedObject, this.gameObject.transform.position, selectedObject.transform.rotation);
            toDrop = Instantiate(selectedObject, this.gameObject.transform.position, toDrop.transform.rotation);
            toDrop.GetComponent<Rigidbody>().useGravity = true;
            NetworkServer.Spawn (toDrop);
		}

	}

    public GameObject return_selected()
    {
        return selectedObject;
    }

     
    // Use this for initialization
    void Start()
    {
        //TODO, need an initializer for new objects as the player chooses them
        //show_selected_object();
        CreateHighlight();
        //next_selected_object();
    }

    // Update is called once per frame

    public void RotateObject(int direction)
    {
        if (Mathf.Abs(direction) < Mathf.Epsilon)
            return;
        setColliders(false); //Disable colliders to check for collision in script
        updateCurrentRotation(direction);
        //Assign a new z-rotation quaternion
        Quaternion newRotation = Quaternion.Euler(0, 0, rotationAngles[currentRotation]);
        Quaternion oldRotation = toDrop.transform.rotation;
        //toDrop.GetComponent<Rigidbody>().MoveRotation(newRotation); //transform.rotation = newRotation;
        toDrop.transform.rotation = newRotation;
        //if (objectHasOverlaps())
        //{
        //    print("Has overlaps");
        //    toDrop.transform.rotation = oldRotation;
        //    updateCurrentRotation(-direction);
        //}
        areaHighlight.AdjustHighlightScaleAndOffsetFor(toDrop);
        setColliders(true); //Reset colliders at the end
    }

    private void updateCurrentRotation(int direction)
    {
        direction /= Mathf.Abs(direction); //Make it either +1 or -1 if it's not already
        currentRotation += direction;
        if (currentRotation >= rotationAngles.Length)
        {
            currentRotation = 0;
        }
        else if (currentRotation < 0)
        {
            currentRotation = rotationAngles.Length - 1;
        }
    }

    private void resetCurrentRotation() {
        currentRotation = 0;
    }

    private bool objectHasOverlaps()
    {
        Vector3 sizeOffset = Vector3.one * 0.05f; //Small number so boundary cases would still work
        foreach (Transform child in toDrop.transform)
        {
            Collider[] overlaps = Physics.OverlapBox(child.position, child.lossyScale / 2.0f - sizeOffset, Quaternion.identity);
            if (overlaps.Length > 0)
                return true;
        }
        return false;
    }

    private void setColliders(bool active)
    {
        foreach (Transform child in toDrop.transform)
        {
            BoxCollider childCol = child.GetComponent<BoxCollider>();
            childCol.enabled = active;
        }
    }


    //-------------------------------------Highlight functions -----------------------------------------
    void CreateHighlight() {
        areaHighlight = Instantiate(areaHighlightPrefab.gameObject).GetComponent<HighlightBehaviour>();
        SetHighlightActive(false);
    }

    //Call this whenever the indicator goes "missing" and set it to false, then back to true when it appears
    void SetHighlightActive(bool active) {
        areaHighlight.gameObject.SetActive(active);
    }

}
