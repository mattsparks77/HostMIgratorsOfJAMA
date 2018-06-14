using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BlockSpawnerTwo : NetworkBehaviour {

    //settings
    [SerializeField] GameObject[] blockPrefabs;
    [SerializeField] int rotationPerClick = 90;
    [SerializeField] KeyCode rotateKey = KeyCode.E;
    [SerializeField] KeyCode spawnKey = KeyCode.Space;
    [SerializeField] float cooldownTime = 1.5f;

    //instance variables
    GameObject indicator;
    int blockIndex = 0;
    int rotation = 0;
    float spawnCooldown = 0;

    //================================================================================
    // Code Control
    //================================================================================

    // Use this for initialization
    void Start () {
        if (!isLocalPlayer)
            return;
        CreateIndicator();
    }
    
    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer)
            return;
        spawnCooldown = Mathf.Max(spawnCooldown - Time.deltaTime, 0);
        TakeUserInput();
        PositionIndicator(transform.position);
        RotateIndicator(rotation);
    }

    //================================================================================
    // User input
    //================================================================================

    void TakeUserInput() {
        if (Input.GetKeyDown(rotateKey))
            ChangeRotation();
        if (Input.GetKeyDown(spawnKey))
            TrySpawnBlock();
    }

    //rotates the indicator 90 degrees.
    void ChangeRotation() {
        rotation += rotationPerClick;
        rotation %= 360;
    }


    //================================================================================
    // Indicator functions
    //================================================================================


    //moves the indicator object to the spot on the grid in front of the camera.
    void PositionIndicator(Vector3 cameraPosition) {
        if (indicator == null)
            return;
        Vector3 newPosition = new Vector3((int)cameraPosition.x, (int)cameraPosition.y, GameConstants.playPlaneZ);
        indicator.transform.position = newPosition;
    }

    //sets the indicator's rotation
    void RotateIndicator(float angle) {
        if (indicator == null)
            return;
        indicator.transform.rotation = Quaternion.AngleAxis(angle, transform.forward);
    }

    //instantiates an indicator using the prefab for the block it is supposed to represent,
    //and then modifying its behavior.
    void CreateIndicator() {
        indicator = Instantiate(blockPrefabs[blockIndex]);
        indicator.layer = LayerMask.NameToLayer("Indicator");
        Rigidbody body = indicator.GetComponent<Rigidbody>();
        if(body != null)
            Destroy(body);
        Collider col = indicator.GetComponent<Collider>();
        if(col != null)
            Destroy(col);
        foreach (Collider child in indicator.GetComponentsInChildren<Collider>()) {
            child.gameObject.layer = LayerMask.NameToLayer("Indicator");
            if (child != null)
                Destroy(child);
        }
    }

    //destroys the current indicator
    void DestroyIndicator() {
        Destroy(indicator);
        indicator = null;
    }

    //================================================================================
    // Spawning functions
    //================================================================================

    //sets up the indicator and block index for the next block
    void NextBlock() {
        SetBlockToRandom();
        DestroyIndicator();
        CreateIndicator();
    }

    //picks a random valid number for the block index
    void SetBlockToRandom() {
        blockIndex = Random.Range(0, blockPrefabs.Length);
    }

    //spawns the new block, if able
    void TrySpawnBlock() {
        if (spawnCooldown > 0f || indicator == null || IsIndicatorOverlapping())
            return;
        spawnCooldown = cooldownTime;
        CmdSpawnBlock(blockIndex,
                      indicator.transform.position,
                      indicator.transform.rotation);
        NextBlock();
    }

    //spawns the block on the server
    [Command]
    void CmdSpawnBlock(int blockIndex, Vector3 position, Quaternion rotation) {
        GameObject newBlock = Instantiate(blockPrefabs[blockIndex], position, rotation);
        NetworkServer.Spawn(newBlock);
    }

    //checks if the indicator is overlapping any already-spawned block by checking if each
    //of its children are overlapping
    bool IsIndicatorOverlapping() {
        if (indicator == null)
            return false;
        int mask = LayerMask.NameToLayer("Placed Block");
        Vector3 overlapTolerance = Vector3.kEpsilon * Vector3.one;
        foreach (Transform child in indicator.transform.GetComponentsInChildren<Transform>()) {
            Collider[] overlaps = Physics.OverlapBox(child.position,
                                                     (child.lossyScale / 2) + overlapTolerance,
                                                     child.rotation);
            if (overlaps.Length > 0)
                return true;
        }
        return false;
    }
}
