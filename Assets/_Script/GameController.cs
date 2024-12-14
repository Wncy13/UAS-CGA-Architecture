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
    public GameObject ButtonKhusus;
    public Button buildButton;
    public Button saveButton; 
    public Button backButton;
    public Button removeButton; 
    public Button nextButton;
    public Button previousButton;
    private bool isPlacing = false;
    private bool isBuilding = false;

    public float mouseSensitivity = 50f; 
    public float movementSpeed = 10f; 

    public void Start()
{
    // Menonaktifkan semua elemen UI selain BuildButton
    ButtonKhusus.SetActive(false);
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

    public void EndBuildMode()
{
    Debug.Log("EndBuildMode called.");
    isBuilding = false;
    isPlacing = false;
    
    // Alihkan ke Build Camera
    playerCamera.gameObject.SetActive(true);

    buildCamera.gameObject.SetActive(false);

    // Tampilkan elemen Build Mode
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
        // Kontrol kamera hanya saat tidak dalam mode build
        if (!isBuilding)
        {
            HandleCameraMovement();
        }else
    {
        // Cek jika tombol Home ditekan saat dalam mode Build
        if (Input.GetKeyDown(KeyCode.Home))
        {
            isBuilding = false;
            EndBuildMode(); // Mengembalikan ke mode kamera utama
        }

        if (isPlacing)
        {
            HandlePlacement(); // Menangani proses placement objek
        }
    }
    }

    void HandleCameraMovement()
    {
        // Pergerakan kamera di sumbu X (maju mundur)
        float moveDirection = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;

        // Menggerakkan kamera dengan menggunakan input W dan S
        Vector3 movement = playerCamera.transform.forward * moveDirection;
        movement.y = 0; // Menghilangkan pergerakan di sumbu Y

        playerCamera.transform.position += movement; // Menggerakkan kamera
    }

    void HandlePlacement()
{
    // Cek input untuk menempatkan objek
    if (Input.GetMouseButtonDown(0)) // Tombol kiri mouse untuk menempatkan objek
    {
        // Logika placement objek, bisa menambahkan objek ke dunia atau memulai proses lainnya
        Debug.Log("Placement done!");
    }
}
}