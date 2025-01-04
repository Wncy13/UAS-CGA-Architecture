using UnityEngine;
using UnityEngine.UI;

public class ButtonSwitcher : MonoBehaviour
{
    public Transform buildPanel; // Reference to BuildPanel
    public Transform inventoryManager; // Reference to InventoryManager
    public Button backPackButton; // Reference to the BackPack button

    private bool isBackPackActive = false; // Tracks whether BackPack is active
    private GameObject firstSelectedItem; // Tracks the first selected item during swap

    // Toggles the BackPack state
    public void ToggleBackPack()
    {
        isBackPackActive = !isBackPackActive;
        Debug.Log("BackPack is now " + (isBackPackActive ? "active" : "inactive"));

        if (!isBackPackActive)
        {
            // Reset the first selected item when BackPack is deactivated
            firstSelectedItem = null;
        }
    }

    private void SwapButtonContents(GameObject button1, GameObject button2)
{
    // Simpan referensi konten (misalnya, gambar, teks, atau data lain)
    Image image1 = button1.GetComponent<Image>();
    Image image2 = button2.GetComponent<Image>();

    if (image1 != null && image2 != null)
    {
        // Tukar sprite gambar
        Sprite tempSprite = image1.sprite;
        image1.sprite = image2.sprite;
        image2.sprite = tempSprite;
    }

    // Tambahkan log untuk debug
    Debug.Log($"Swapped contents of {button1.name} with {button2.name}");
}


    // Function to swap buttons
    public void SwapButtons(GameObject clickedButton)
    {
        if (!isBackPackActive)
        {
            Debug.LogError("Swap is not allowed. BackPack is not active.");
            return;
        }

        if (clickedButton == null)
        {
            Debug.LogError("clickedButton is null. Please ensure the correct object is passed.");
            return;
        }

        // Ensure the clicked object has a Button component
        Button button = clickedButton.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError($"The clicked object ({clickedButton.name}) does not have a Button component.");
            return;
        }

        // Prevent BackPack button from being swapped
        if (clickedButton == backPackButton.gameObject)
        {
            Debug.LogError("BackPack button cannot be swapped.");
            return;
        }

        // Handle first and second item selection for swapping
        if (firstSelectedItem == null)
        {
            // First selection must be from InventoryManager
            if (clickedButton.transform.parent != inventoryManager)
            {
                Debug.LogError("First selection must be from InventoryManager.");
                return;
            }

            firstSelectedItem = clickedButton;
            Debug.Log($"First item selected: {clickedButton.name}");
        }
        else
        {
            // Second selection must be from BuildPanel
            if (clickedButton.transform.parent != buildPanel)
            {
                Debug.LogError("Second selection must be from BuildPanel.");
                return;
            }

            // Perform the swap
            Transform firstParent = firstSelectedItem.transform.parent;
            Transform secondParent = clickedButton.transform.parent;

            firstSelectedItem.transform.SetParent(secondParent);
            clickedButton.transform.SetParent(firstParent);

            Debug.Log($"Swapped {firstSelectedItem.name} with {clickedButton.name}");

            // Reset firstSelectedItem
            firstSelectedItem = null;
        }
    }

    // Called when a button is clicked
    public void OnClick()
    {
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (clickedButton == null)
        {
            Debug.LogError("No button clicked.");
            return;
        }

        Debug.Log($"Button clicked: {clickedButton.name}");
        SwapButtons(clickedButton);
    }

    // Function to disable placement during swapping
    public bool CanPlaceItems()
    {
        // Disallow item placement if BackPack is active
        return !isBackPackActive || firstSelectedItem == null;
    }
}
