using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryPanel; // Assign the inventory panel (Grid Layout) here
    public GameObject itemPrefab; // A prefab representing an inventory item slot
    public int numberOfItems; // Number of items to populate the grid with
    [SerializeField]
    private ObjectsDatabaseSO database;
    public GameObject BuildPanel;

    private GameObject selectedItem;

    public GridSwapManager swapManager;
    IBuildingState buildingState;
    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private GameObject InputManager;
    [SerializeField]
    private GameObject GridParent;
    [SerializeField]
    private GameObject PreviewSystem;
    [SerializeField]
    private GameObject PlacementSystem;
    private bool isInventoryActive = false;

    void Start()
    {
        database = FindObjectOfType<ObjectsDatabaseSO>();
    }

    public void OnItemClicked(GameObject item)
    {
        if (swapManager != null)
        {
            swapManager.SelectItem(item);
        }

        if (selectedItem != null)
        {
            SwapItemToBuildPanel(selectedItem, item);
        }
        else
        {
            selectedItem = item;
        }
    }

    private void SwapItemToBuildPanel(GameObject inventoryItem, GameObject buildPanelItem)
    {
        Renderer inventoryItemRenderer = inventoryItem.GetComponent<Renderer>();
        Renderer buildPanelItemRenderer = buildPanelItem.GetComponent<Renderer>();

        if (inventoryItemRenderer != null && buildPanelItemRenderer != null)
        {
            // Swap the materials
            Material tempMaterial = inventoryItemRenderer.material;
            inventoryItemRenderer.material = buildPanelItemRenderer.material;
            buildPanelItemRenderer.material = tempMaterial;

        }

        selectedItem = null; // Deselect the item after swapping
    }

public void ToggleInventory()
{
    if (inventoryPanel != null)
    {
        isInventoryActive = !isInventoryActive;
        inventoryPanel.SetActive(isInventoryActive);

        // Enable/disable systems accordingly
        PreviewSystem.SetActive(!isInventoryActive);
        PlacementSystem.SetActive(!isInventoryActive);

        // Populate the grid with items only when opening
        if (isInventoryActive)
        {
            PopulateInventory();
        }
    }
}

    private void PopulateInventory()
    {
        Material buildPanelMaterial = BuildPanel.GetComponent<Renderer>().material;

        for (int i = 0; i < numberOfItems; i++)
        {
            GameObject item = Instantiate(itemPrefab, inventoryPanel.transform);
            Renderer itemRenderer = item.GetComponent<Renderer>();
            if (itemRenderer != null)
            {
                itemRenderer.material = buildPanelMaterial;
            }
        }
    }

    private void SelectItem(GameObject item)
    {
        if (selectedItem != null)
        {
            // Check if the selected item has the BuildPanel material
            Renderer selectedItemRenderer = selectedItem.GetComponent<Renderer>();
            Material buildPanelMaterial = BuildPanel.GetComponent<Renderer>().material;

            if (selectedItemRenderer != null && selectedItemRenderer.material == buildPanelMaterial)
            {
                // Replace the selected item with the new item
                ReplaceItem(selectedItem, item);
            }
        }
        else
        {
            // Select the item
            selectedItem = item;
        }
    }

    private void ReplaceItem(GameObject oldItem, GameObject newItem)
    {
        Renderer oldItemRenderer = oldItem.GetComponent<Renderer>();
        Renderer newItemRenderer = newItem.GetComponent<Renderer>();

        if (oldItemRenderer != null && newItemRenderer != null)
        {
            oldItemRenderer.material = newItemRenderer.material;
        }

        selectedItem = null; // Deselect the item after replacement
    }
}