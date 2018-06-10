using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //TODO make this private
    public GameObject selectedObject;

    [SerializeField] private GameObject areaHighlightPrefab;
    private GameObject areaHighlight;
    private Vector2 highlightOffset = Vector3.zero; //Used to adjust where the highlight would go due to rotation

    private int[] rotationAngles = { 0, 90, 180, 270 };
    private int currentRotation = 0;
    
	// Use this for initialization
	void Start () {
        //TODO, need an initializer for new objects as the player chooses them
        areaHighlight = Instantiate(areaHighlightPrefab);
        updateHighlightScaleAndOffset();
	}
    
    // Update is called once per frame
    void Update () {
        int direction = 0;
        //Get the direction based on what button they're pressing
        direction += Input.GetKeyDown(KeyCode.LeftArrow) ? -1 : 0;
        direction += Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0;
        
        RotateObject(direction);
        moveHighlightToObject();

	}

    public void RotateObject(int direction) {
        if (Mathf.Abs(direction) < Mathf.Epsilon)
            return;
        updateCurrentRotation(direction);
        //Assign a new z-rotation quaternion
        Quaternion newRotation = Quaternion.Euler(0, 0, rotationAngles[currentRotation]);
        Quaternion oldRotation = selectedObject.transform.rotation;
        //selectedObject.GetComponent<Rigidbody>().MoveRotation(newRotation); //transform.rotation = newRotation;
        selectedObject.transform.rotation = newRotation;
        if (objectHasOverlaps()) {
            selectedObject.transform.rotation = oldRotation;
            updateCurrentRotation(-direction);
        }
        updateHighlightScaleAndOffset();
    }
    
    private void updateCurrentRotation(int direction) {
        direction /= Mathf.Abs(direction); //Make it either +1 or -1 if it's not already
        currentRotation += direction;
        if (currentRotation >= rotationAngles.Length) {
            currentRotation = 0;
        }
        else if (currentRotation < 0) {
            currentRotation = rotationAngles.Length - 1;
        }
    }

    private bool objectHasOverlaps() {
        Vector3 sizeOffset = Vector3.one * 0.05f; //Small number so boundary cases would still work
        foreach (Transform child in selectedObject.transform) {
            Collider[] overlaps = Physics.OverlapBox(child.position, child.lossyScale / 2.0f - sizeOffset, Quaternion.identity);
            if (overlaps.Length > 0)
                return true;
        }
        return false;
    }

    private void moveHighlightToObject() {
        Vector3 objectPos = selectedObject.transform.position;
        Vector3 yOffset = Vector3.up * areaHighlight.transform.localScale.y / 2;
        //if (areaHighlight.transform.localScale.x % 2 == 0) {
            objectPos.x += highlightOffset.x;
        //}
        objectPos.y += highlightOffset.y;
        areaHighlight.transform.position = objectPos - yOffset;
    }

    private void updateHighlightScaleAndOffset() {
        float lowestX = int.MaxValue, highestX = int.MinValue;
        foreach (Transform child in selectedObject.transform) {
            lowestX = Mathf.Min(lowestX, child.position.x);
            highestX = Mathf.Max(highestX, child.position.x);
        }
        //Get the difference between highest and lowest and add 1 for the new scale, since it uses center points
        int newScaleX = (int)Mathf.Round(highestX - lowestX) + 1;
        Vector3 newScale = areaHighlight.transform.localScale;
        newScale.x = newScaleX;
        areaHighlight.transform.localScale = newScale;
        highlightOffset.x = (highestX + lowestX) / 2 - selectedObject.transform.position.x;
        print("Highest X: " + highestX + " | Lowest X: " + lowestX + "Offset: " + highlightOffset.x);
        updateYOffset(newScaleX);
    }

    //Uses XOffset to calculate where the area should go to cover the lowest part where it covers the xoffset
    private void updateYOffset(int xScale) {
        float highestYPos = int.MinValue;
        float lowestYPos = int.MaxValue;
        foreach (Transform child in selectedObject.transform) {
            highestYPos = Mathf.Max(child.position.y, highestYPos);
            lowestYPos = Mathf.Min(child.position.y, lowestYPos);
        }
        int[] filledAmount = new int[Mathf.RoundToInt((highestYPos - lowestYPos) / selectedObject.transform.localScale.y) + 1];
        foreach (Transform child in selectedObject.transform) {
            //Get the index of how far child is away from the top child
            int currYIndex = Mathf.RoundToInt((highestYPos - child.position.y) / selectedObject.transform.localScale.y);
            filledAmount[currYIndex]++;
        }
        int highestIndexY = 0;
        for (int value = 0; value < filledAmount.Length; value++) {
            if (filledAmount[value] == xScale) {
                highestIndexY = Mathf.Max(value, highestIndexY);
            }
        }
        print(highestIndexY);
        float highDifference = highestYPos - selectedObject.transform.position.y;
        highlightOffset.y = highDifference - highestIndexY * selectedObject.transform.localScale.y;
    }
}
