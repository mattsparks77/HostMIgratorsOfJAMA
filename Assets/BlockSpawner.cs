using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
public class BlockSpawner : NetworkBehaviour {
    public GameObject[] block_list;
	public float movementSpeed = 5f;
    public GameObject selectedObject;
    public GameObject toDrop;
    private bool is_instantiated = false;
    [SerializeField] private GameObject areaHighlightPrefab;
    private GameObject areaHighlight;
    private Vector2 highlightOffset = Vector3.zero; //Used to adjust where the highlight would go due to rotation

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
        if (!is_instantiated && timeCounter <= 0)
        {
            int rand = Random.Range(0, 6);
            selectedObject = block_list[rand];

            //Choose block to spawn and re-enable highlight 
            toDrop = Instantiate(selectedObject, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform);
            //Cmdtry_block_spawn();
            updateHighlightScaleAndOffset();
            areaHighlight.SetActive(true);
            resetCurrentRotation();
            is_instantiated = true;
        }
        if (toDrop) {
            RotateObject(direction);
            moveHighlightToObject();
            toDrop.transform.position = this.gameObject.transform.position;

            if (Input.GetKeyDown(KeyCode.Space) || timeCounter <= -3f) {//change value here to determine the amount of time it takes to auto drop block subtracting from 1.5f so at -3f it auto spawns in 4.5 seconds etc.
                if (isLocalPlayer) {
                    // Let the block drop and disable the highlight
                    Destroy(toDrop);
                    Cmdtry_block_spawn();
                    toDrop.GetComponent<Rigidbody>().useGravity = true;
                    is_instantiated = false;
                  
                    areaHighlight.SetActive(false);
                    timeCounter = timeTilNextBlock;
                    toDrop = null;
                    //next_selected_object();
                }
            }
        }
        
    }
    //[Command]
    //void Cmd_change_gravity()
    //{

    //}
	[Command]
	void Cmdtry_block_spawn(){
	 {
            //GameObject a_block = Instantiate (selectedObject, this.gameObject.transform.position, selectedObject.transform.rotation);
            toDrop = Instantiate(selectedObject, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform);
            NetworkServer.Spawn (toDrop);
		}

	}
    public void SetNewSelectedObjectRef(GameObject newReferencedObject)
    {
        //selectedObject = newReferencedObject;
        //selectedObject = selectedObject;
        updateHighlightScaleAndOffset();
    }
    public GameObject return_selected()
    {
        return selectedObject;
    }
    public void next_selected_object()
    {
        int rand = Random.Range(0, 6);
        selectedObject = Instantiate(block_list[rand], this.gameObject.transform.position, selectedObject.transform.rotation);
        updateHighlightScaleAndOffset();
        //SetNewSelectedObjectRef(sele);
     
    }
    public void instantiate_selected_object(GameObject toIns)
    {

        selectedObject = Instantiate(toIns, this.gameObject.transform.position, selectedObject.transform.rotation);
        selectedObject.GetComponent<Rigidbody>().useGravity = false;
    }

    // Use this for initialization
    void Start()
    {
        //TODO, need an initializer for new objects as the player chooses them
        //show_selected_object();
        areaHighlight = Instantiate(areaHighlightPrefab);
        areaHighlight.SetActive(false);
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
        updateHighlightScaleAndOffset();
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

    private void moveHighlightToObject()
    {
        Vector3 objectPos = toDrop.transform.position;
        Vector3 yOffset = Vector3.up * areaHighlight.transform.localScale.y / 2;
        //if (areaHighlight.transform.localScale.x % 2 == 0) {
        objectPos.x += highlightOffset.x;
        //}
        objectPos.y += highlightOffset.y;
        areaHighlight.transform.position = objectPos - yOffset;
    }

    private void updateHighlightScaleAndOffset()
    {
        float lowestX = int.MaxValue, highestX = int.MinValue;
        foreach (Transform child in toDrop.transform)
        {
            lowestX = Mathf.Min(lowestX, child.position.x);
            highestX = Mathf.Max(highestX, child.position.x);
        }
        //Get the difference between highest and lowest and add 1 for the new scale, since it uses center points
        int newScaleX = (int)Mathf.Round(highestX - lowestX) + 1;
        Vector3 newScale = areaHighlight.transform.localScale;
        newScale.x = newScaleX;
        areaHighlight.transform.localScale = newScale;
        highlightOffset.x = (highestX + lowestX) / 2 - toDrop.transform.position.x;
        print("Highest X: " + highestX + " | Lowest X: " + lowestX + "Offset: " + highlightOffset.x);
        updateYOffset(newScaleX);
    }

    //Uses XOffset to calculate where the area should go to cover the lowest part where it covers the xoffset
    private void updateYOffset(int xScale)
    {
        float highestYPos = int.MinValue;
        float lowestYPos = int.MaxValue;
        foreach (Transform child in toDrop.transform)
        {
            highestYPos = Mathf.Max(child.position.y, highestYPos);
            lowestYPos = Mathf.Min(child.position.y, lowestYPos);
        }
        int[] filledAmount = new int[Mathf.RoundToInt((highestYPos - lowestYPos) / toDrop.transform.localScale.y) + 1];
        foreach (Transform child in toDrop.transform)
        {
            //Get the index of how far child is away from the top child
            int currYIndex = Mathf.RoundToInt((highestYPos - child.position.y) / toDrop.transform.localScale.y);
            filledAmount[currYIndex]++;
        }
        int highestIndexY = 0;
        for (int value = 0; value < filledAmount.Length; value++)
        {
            if (filledAmount[value] == xScale)
            {
                highestIndexY = Mathf.Max(value, highestIndexY);
            }
        }
        print(highestIndexY);
        float highDifference = highestYPos - toDrop.transform.position.y;
        highlightOffset.y = highDifference - highestIndexY * toDrop.transform.localScale.y;
    }

    private void setColliders(bool active)
    {
        foreach (Transform child in toDrop.transform)
        {
            BoxCollider childCol = child.GetComponent<BoxCollider>();
            childCol.enabled = active;
        }
    }

}
