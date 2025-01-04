using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Camera playerCamera;
    public Camera buildCamera;
    public Camera leftCamera;
    public Camera rightCamera;
    public Camera behindCamera;
    public GameObject Panel;
    public GameObject InventoryManager;
    public GameObject ButtonKhusus;
    public Button buildButton;
    public Button saveButton;
    public Button backButton;
    public Button removeButton;
    public Button nextButton;
    public Button previousButton;
    private bool isPlacing = false;
    private bool isBuilding = false;

    public float mouseSensitivity = 25f; // Reduced for slower mouse rotation
    public float movementSpeed = 1f; // Reduced for slower movement

    public LayerMask placementLayerMask; // Layer mask for placement

    public void Start()
    {
        // Menonaktifkan semua elemen UI selain BuildButton
        ButtonKhusus.SetActive(false);
        InventoryManager.SetActive(false);
        Panel.SetActive(false);
        saveButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        buildCamera.gameObject.SetActive(false);
        leftCamera.gameObject.SetActive(false);
        rightCamera.gameObject.SetActive(false);
        behindCamera.gameObject.SetActive(false);

        // Menambahkan listener untuk tombol
        buildButton.onClick.AddListener(StartBuildMode);
    }

    public void StartBuildMode()
    {
        Debug.Log("StartBuildMode called.");
        isBuilding = true;
        isPlacing = true;

        // Alihkan ke Build Camera
        playerCamera.gameObject.SetActive(false);
        buildCamera.gameObject.SetActive(true);
        rightCamera.gameObject.SetActive(false);
        leftCamera.gameObject.SetActive(false);
        behindCamera.gameObject.SetActive(false);
        InventoryManager.SetActive(false);

        // Tampilkan elemen Build Mode
        ButtonKhusus.SetActive(true);
        Panel.SetActive(true);
        buildButton.gameObject.SetActive(false);
        saveButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        removeButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        previousButton.gameObject.SetActive(true);
    }

    public void InventoryChange()
    {
        InventoryManager.SetActive(!InventoryManager.activeSelf);
    }

    public void EndBuildMode()
    {
        Debug.Log("EndBuildMode called.");
        isBuilding = false;
        isPlacing = false;

        // Alihkan ke kamera utama
        playerCamera.gameObject.SetActive(true);

        buildCamera.gameObject.SetActive(false);

        // Sembunyikan elemen Build Mode
        ButtonKhusus.SetActive(false);
        Panel.SetActive(false);
        buildButton.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        removeButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        previousButton.gameObject.SetActive(false);

        // Bebaskan kursor untuk interaksi UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Update()
    {
        if (!isBuilding)
        {
            HandleCameraMovement();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                isBuilding = false;
                EndBuildMode();
            }

            if (isPlacing)
            {
                HandlePlacement();
            }
        }
    }

    void HandleCameraMovement()
    {
        Rigidbody rb = playerCamera.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody is not attached to the playerCamera!");
            return;
        }

        // Get movement inputs
        float horizontalMovement = Input.GetAxis("Horizontal") * movementSpeed;
        float verticalMovement = Input.GetAxis("Vertical") * movementSpeed;

        // Calculate movement direction
        Vector3 forwardMovement = playerCamera.transform.forward * verticalMovement;
        Vector3 rightMovement = playerCamera.transform.right * horizontalMovement;

        Vector3 movement = (forwardMovement + rightMovement).normalized * movementSpeed * Time.fixedDeltaTime;

        // Move the Rigidbody
        rb.MovePosition(rb.position + movement);

        // Rotate with mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        playerCamera.transform.Rotate(Vector3.up * mouseX, Space.World);

        Vector3 currentRotation = playerCamera.transform.localEulerAngles;
        float pitch = currentRotation.x;
        if (pitch > 180) pitch -= 360;

        float newPitch = Mathf.Clamp(pitch - mouseY, -89f, 89f);
        playerCamera.transform.localEulerAngles = new Vector3(newPitch, currentRotation.y, 0);
    }

    void HandlePlacement()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button for placing objects
        {
            Ray ray = buildCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayerMask))
            {
                Debug.Log("Placement location valid at: " + hit.point);

                // Logika untuk menempatkan objek di lokasi yang valid
                GameObject newObject = Instantiate(ButtonKhusus, hit.point, Quaternion.identity);
                newObject.transform.position = hit.point;
            }
            else
            {
                Debug.LogWarning("Placement location invalid!");
            }
        }
    }
}
