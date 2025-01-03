using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlacedObject
{
    public Vector3Int position;
    public int objectID;
}

[System.Serializable]
public class PlacedObject
{
    public Vector3Int position;
    public int objectID;
}

public class PlacementSystem : MonoBehaviour
{
    public Camera mainCamera;
    public Camera rightCamera;
    public Camera backCamera;
    public Camera leftCamera;
    public Camera frontCamera;
    public Button nextButton;
    public Button previousButton;

    private List<Camera> cameras;
    private int currentCameraIndex = 0;

    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private AudioClip correctPlacementClip, wrongPlacementClip;
    [SerializeField]
    private AudioSource source;

    private GridData floorData, furnitureData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    IBuildingState buildingState;

    [SerializeField]
    private SoundFeedback soundFeedback;
    [SerializeField]
    private Camera mainCamera;

    private List<PlacedObject> placedObjects = new List<PlacedObject>();

    private List<PlacedObject> placedObjects = new List<PlacedObject>();

    [SerializeField]
    private float rotationSpeed = 90f; // Rotation speed (degrees per click)
    private float currentRotation = 0f; // Current rotation (in degrees)

    private void Start()
    {
        currentRotation = 0f;
        cameras = new List<Camera> { mainCamera, rightCamera, backCamera, leftCamera, frontCamera };
        nextButton.onClick.AddListener(SwitchToNextCamera);
        previousButton.onClick.AddListener(SwitchToPreviousCamera);
        gridVisualization.SetActive(false);
        floorData = new();
        furnitureData = new();
    }

    private void SwitchToPreviousCamera()
    {
        cameras[currentCameraIndex].gameObject.SetActive(false);
        currentCameraIndex = (currentCameraIndex - 1 + cameras.Count) % cameras.Count;
        cameras[currentCameraIndex].gameObject.SetActive(true);
        Debug.Log("Switched to previous camera");
    }

    private void SwitchToNextCamera()
    {
        cameras[currentCameraIndex].gameObject.SetActive(false);
        currentCameraIndex = (currentCameraIndex + 1) % cameras.Count;
        cameras[currentCameraIndex].gameObject.SetActive(true);
        Debug.Log("Switched to next camera");
    }

    private void nextCamera(Camera nextCamera)
    {
        nextCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);

        // Create the object placement state
        buildingState = new PlacementState(ID,
                                           grid,
                                           preview,
                                           database,
                                           floorData,
                                           furnitureData,
                                           objectPlacer,
                                           soundFeedback);

        // Set initial rotation of the object
        currentRotation = 0f;
        preview.transform.rotation = Quaternion.Euler(0f, currentRotation, 0f); // Rotate on Y-axis

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer, soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void donePlacement()
    {
        StopPlacement();
        gridVisualization.SetActive(false);
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI()) // Mengabaikan input jika pointer berada di atas UI
        {
            Debug.Log("Pointer berada di atas UI, mengabaikan klik.");
            return;
        }

        // Mendapatkan posisi grid berdasarkan posisi mouse
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        // Terapkan rotasi pada objek preview sesuai dengan currentRotation
        Debug.Log(currentRotation);
        preview.transform.rotation = Quaternion.Euler(0f, currentRotation, 0f); // Rotasi objek

        // Setelah rotasi diterapkan pada preview, lakukan aksi peletakan objek
        buildingState.OnAction(gridPosition); // Tempatkan objek di grid

        // Jika peletakan berhasil, bisa menambahkan objek yang ditempatkan ke dalam daftar
        placedObjects.Add(new PlacedObject
        {
            position = gridPosition,
        });

        Debug.Log($"Object placed at {gridPosition} with rotation {currentRotation}°");
    }

    private void RotateObject()
    {
        // Rotate the preview object 90 degrees
        currentRotation += rotationSpeed;
        if (currentRotation >= 360f) currentRotation = 0f; // Reset rotation if > 360

        // Call UpdateRotation to update the preview object's rotation
        preview.UpdateRotation(currentRotation); // Update the preview object rotation
    }

    private void StopPlacement()
    {
        soundFeedback.PlaySound(SoundType.Click);
        if (buildingState == null)
            return;
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        // Reset object rotation when placement is stopped
        preview.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Set rotation to 0

        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if (buildingState == null)
            return;

        // Deteksi klik kanan untuk rotasi
        if (Input.GetMouseButtonDown(1)) // Klik kanan untuk rotasi
        {
            Debug.Log("Klik kanan terdeteksi di Update!");
            RotateObject(); // Fungsi rotasi
            return;
        }

        // Dapatkan posisi mouse di layar (screen space)
        Vector3 mouseScreenPosition = Input.mousePosition;

        // Konversikan posisi mouse ke dunia (world space) menggunakan mainCamera
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        // Pastikan hanya posisi X dan Z yang diubah, posisi Y tetap pada ketinggian kamera
        Vector3 newCameraPosition = new Vector3(mouseWorldPosition.x, mainCamera.transform.position.y, mouseWorldPosition.z);

        // Update posisi kamera
        mainCamera.transform.position = newCameraPosition;

        // Dapatkan posisi grid berdasarkan posisi mouse
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        // Terapkan rotasi pada objek preview sesuai dengan currentRotation
        preview.transform.rotation = Quaternion.Euler(0f, currentRotation, 0f); // Rotasi objek

        // Perbarui state building berdasarkan posisi grid
        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }
}
