using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this script to the highlight prefab
public class HighlightBehaviour : MonoBehaviour {

    //Used to reposition based on which object is selected, and the orientation of that object
    private Vector2 highlightOffset = Vector3.zero;

    // Pass in the position of the object that this highlight wants to follow
    public void MoveHighlightTo(GameObject wantedObject) {
        Vector3 objectPos = wantedObject.transform.position;
        Vector3 yOffset = Vector3.up * transform.localScale.y / 2;
        //if (areaHighlight.transform.localScale.x % 2 == 0) {
        objectPos.x += highlightOffset.x;
        //}
        objectPos.y += highlightOffset.y;
        transform.position = objectPos - yOffset;
    }

    // Pass in the transform of the object that the highlight should adjust to
    public void AdjustHighlightScaleAndOffsetFor(GameObject selectedObject) {
        //X Offset must come first since Y Offset uses X Offset in it's calculations
        updateXOffset(selectedObject.transform); 
        updateYOffset(selectedObject.transform);
    }

    private void updateXOffset(Transform selectedObject) {
        float lowestX = int.MaxValue, highestX = int.MinValue;
        foreach (Transform child in selectedObject) {
            lowestX = Mathf.Min(lowestX, child.position.x);
            highestX = Mathf.Max(highestX, child.position.x);
        }
        //Get the difference between highest and lowest and add 1 for the new scale, since it uses center points
        int newScaleX = (int)Mathf.Round(highestX - lowestX) + 1;
        Vector3 newScale = transform.localScale;
        newScale.x = newScaleX;
        transform.localScale = newScale;
        highlightOffset.x = (highestX + lowestX) / 2 - selectedObject.transform.position.x;

    }

    //Uses XOffset to calculate where the area should go to cover the lowest part where it covers the xoffset
    private void updateYOffset(Transform selectedObject) {
        // Get the highest and lowest point of the objet
        float highestYPos = int.MinValue;
        float lowestYPos = int.MaxValue;
        foreach (Transform child in selectedObject) {
            highestYPos = Mathf.Max(child.position.y, highestYPos);
            lowestYPos = Mathf.Min(child.position.y, lowestYPos);
        }
        // Count how many blocks there are in each row 
        int[] filledAmount = new int[Mathf.RoundToInt((highestYPos - lowestYPos) / selectedObject.transform.localScale.y) + 1];
        foreach (Transform child in selectedObject.transform) {
            //Get the index of how far child is away from the top child
            int currYIndex = Mathf.RoundToInt((highestYPos - child.position.y) / selectedObject.transform.localScale.y);
            filledAmount[currYIndex]++;
        }
        // Get the lowest row where the number of blocks in that row matches the x scale
        // (May not work for objects which don't have a full row, but it should be good enough for our case)
        int highestIndexY = 0;
        for (int value = 0; value < filledAmount.Length; value++) {
            if (filledAmount[value] == transform.localScale.x) {
                highestIndexY = Mathf.Max(value, highestIndexY);
            }
        }
        float highDifference = highestYPos - selectedObject.transform.position.y;
        highlightOffset.y = highDifference - highestIndexY * selectedObject.transform.localScale.y;
    }

}
