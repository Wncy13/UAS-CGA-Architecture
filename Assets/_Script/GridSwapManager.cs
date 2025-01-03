using UnityEngine;

public class GridSwapManager : MonoBehaviour
{
    public GameObject selectedItem; // The currently selected item
    public GameObject BuildPanel;   // Reference to the BuildPanel GameObject
    public GameObject InventoryManager; // Reference to the InventoryManager GameObject

    // Select an item or swap if already selected
    public void SelectItem(GameObject item)
    {
        if (selectedItem == null)
        {
            // First selection
            selectedItem = item;
            HighlightItem(item, true); // Highlight the selected item
        }
        else
        {
            // Second selection: Swap the items
            SwapItems(selectedItem, item);
            HighlightItem(selectedItem, false); // Remove highlight from the previous item
            selectedItem = null; // Deselect after swapping
        }
    }

    // Swap two items between grids
    public void SwapItems(GameObject itemA, GameObject itemB)
    {
        if (itemA == null || itemB == null)
        {
            Debug.LogWarning("Cannot swap. One or both items are null.");
            return;
        }

        Transform parentA = itemA.transform.parent;
        Transform parentB = itemB.transform.parent;

        // Swap parents to move items between grids
        itemA.transform.SetParent(parentB);
        itemB.transform.SetParent(parentA);

        // Update positions
        itemA.transform.localPosition = Vector3.zero;
        itemB.transform.localPosition = Vector3.zero;

        Debug.Log($"Swapped {itemA.name} with {itemB.name}");
    }

    // Highlight or unhighlight an item
    private void HighlightItem(GameObject item, bool highlight)
    {
        Renderer renderer = item.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = highlight ? Color.yellow : Color.white;
        }
    }
}
