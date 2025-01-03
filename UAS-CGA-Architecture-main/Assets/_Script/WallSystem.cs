using UnityEngine;

public class WallSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject wallPrefab; // Assign the wall prefab in the Unity Inspector

    private Grid grid;

    private void Start()
    {
        grid = FindObjectOfType<Grid>();
        if (grid == null)
        {
            Debug.LogError("Grid not found in the scene.");
        }
    }

    public void CreateWall(Vector3Int gridPosition)
    {
        if (wallPrefab == null)
        {
            Debug.LogError("Wall prefab is not assigned.");
            return;
        }

        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        GameObject newWall = Instantiate(wallPrefab, worldPosition, Quaternion.identity);
        newWall.transform.position = worldPosition;
    }

    public void RemoveWall(Vector3Int gridPosition)
    {
        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        Collider[] colliders = Physics.OverlapSphere(worldPosition, 0.1f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Wall"))
            {
                Destroy(collider.gameObject);
            }
        }
    }
}