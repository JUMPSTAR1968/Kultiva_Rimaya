using UnityEngine;
using System.Collections.Generic;

public class SimpleRandomizer : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject spritePrefab;
    public int spawnCount = 10;        // Total sprites to spawn
    public Vector2Int gridSize = new Vector2Int(5, 5); // Number of columns and rows
    public float cellSize = 1.0f;      // Size of each grid square

    // To keep track of taken spots
    private HashSet<Vector2> occupiedPositions = new HashSet<Vector2>();

    void Start()
    {
        SpawnInGrid();
    }

    void SpawnInGrid()
    {
        // Safety check: Don't try to spawn more than the grid can hold
        int maxPossible = gridSize.x * gridSize.y;
        if (spawnCount > maxPossible)
        {
            Debug.LogWarning("Spawn count is higher than available grid slots! Capping to max.");
            spawnCount = maxPossible;
        }

        int spawned = 0;
        int attempts = 0;
        int maxAttempts = 200; // Total attempts to fill the count

        while (spawned < spawnCount && attempts < maxAttempts)
        {
            attempts++;

            // 1. Pick a random X and Y integer based on grid dimensions
            int randomX = Random.Range(0, gridSize.x);
            int randomY = Random.Range(0, gridSize.y);

            // 2. Calculate the actual world position based on cell size
            // We subtract half the grid size to center the spawner on the GameObject
            Vector2 gridPos = new Vector2(
                (randomX - gridSize.x / 2f) * cellSize + (cellSize / 2f),
                (randomY - gridSize.y / 2f) * cellSize + (cellSize / 2f)
            );

            // 3. Check if this specific grid coordinate is already in our HashSet
            if (!occupiedPositions.Contains(gridPos))
            {
                Instantiate(spritePrefab, gridPos, Quaternion.identity, transform);
                occupiedPositions.Add(gridPos); // Mark this spot as "Taken"
                spawned++;
            }
        }
    }

    // Visualizes the grid in the Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 pos = new Vector3(
                    (x - gridSize.x / 2f) * cellSize + (cellSize / 2f),
                    (y - gridSize.y / 2f) * cellSize + (cellSize / 2f),
                    0
                ) + transform.position;
                
                Gizmos.DrawWireCube(pos, new Vector3(cellSize * 0.9f, cellSize * 0.9f, 0));
            }
        }
    }
}