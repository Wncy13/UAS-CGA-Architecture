using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    private GameObject selectedObject; // Track the selected object for rotation
    private float currentRotation = 0f;
    private float rotationStep = 90f; // Rotation increment per right-click (90 degrees)

    private GameObject lastPrefab; // Track the last placed prefab

    void Update()
    {
        // If no object is selected, do nothing
        if (selectedObject == null)
        {
            return; // No selected object, so no rotation
        }

        // Right-click to update rotation
        if (Input.GetMouseButtonDown(1)) // Right mouse button (index 1)
        {
            // Update current rotation (add 90 degrees per click)
            currentRotation += rotationStep;

            // Ensure the rotation stays within 0 to 360 degrees
            if (currentRotation >= 360f)
            {
                currentRotation = 0f; // Reset back to 0 when it goes beyond 360 degrees
            }

            // Apply the updated rotation to the selected object immediately
            // selectedObject.transform.rotation = Quaternion.Euler(0f, currentRotation, 0f);
            // Debug.Log("Object rotated to: " + currentRotation + " degrees");
        }
    }

    // Raycast to select an object when clicked
    private void HandleObjectSelection()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button (index 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Create a ray from the camera to the mouse position
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                // Check if the clicked object is in the placed objects list
                if (placedGameObjects.Contains(clickedObject))
                {
                    selectedObject = clickedObject; // Set the selected object
                    Debug.Log("Selected Object: " + selectedObject.name);
                }
                else
                {
                    selectedObject = null; // Deselect the object if clicked outside the list
                    Debug.Log("Deselected object.");
                }
            }
        }
    }

    // Method to place object and apply a static initial rotation
    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        // Reset the rotation if the prefab is different from the last one
        if (lastPrefab != prefab)
        {
            currentRotation = 0f; // Reset rotation to 0 if the prefab changes
            lastPrefab = prefab; // Update last placed prefab
            Debug.Log(prefab.name);
            // newObject.transform.rotation = Quaternion.Euler(0f, currentRotation, 0f);
        }

        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;

        // Apply the initial rotation (based on currentRotation)
        newObject.transform.rotation = Quaternion.Euler(0f, currentRotation, 0f); // Apply the current rotation

        placedGameObjects.Add(newObject);

        // Immediately select the newly placed object
        selectedObject = newObject;
        Debug.Log("Placed and selected object: " + newObject.name);

        return placedGameObjects.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
            return;

        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}