using UnityEngine;
using System.Collections.Generic;

public class SequentialSpawner : MonoBehaviour
{
    [Header("Vegetable Pool (Order of the Song)")]
    [Tooltip("Place sprites in order: Singkamas, Talong, Sigarilyas, etc.")]
    public GameObject[] vegetablePrefabs; 

    [Header("Grid Settings")]
    public int columns = 10;
    public int rows = 5;
    public float cellSize = 1.5f;
    public float jitterAmount = 0.3f;

    void Start()
    {
        if (vegetablePrefabs.Length == 0) return;
        SpawnInOrder();
    }

    void SpawnInOrder()
    {
        // 1. Create a list of all available grid positions
        List<Vector2Int> allCells = new List<Vector2Int>();
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                allCells.Add(new Vector2Int(x, y));
            }
        }

        // 2. Shuffle the positions so the order of the song looks scattered on the field
        for (int i = 0; i < allCells.Count; i++)
        {
            Vector2Int temp = allCells[i];
            int randomIndex = Random.Range(i, allCells.Count);
            allCells[i] = allCells[randomIndex];
            allCells[randomIndex] = temp;
        }

        // 3. Spawn each vegetable from the list exactly once
        Vector2 gridOffset = new Vector2((columns * cellSize) / 2, (rows * cellSize) / 2);

        for (int i = 0; i < vegetablePrefabs.Length; i++)
        {
            // Safety: Stop if we have more vegetables than grid squares
            if (i >= allCells.Count) break; 

            Vector2Int cell = allCells[i];

            // Centering logic
            float posX = (cell.x * cellSize) - gridOffset.x + (cellSize / 2);
            float posY = (cell.y * cellSize) - gridOffset.y + (cellSize / 2);

            // Jitter for natural look
            posX += Random.Range(-jitterAmount, jitterAmount);
            posY += Random.Range(-jitterAmount, jitterAmount);

            Vector3 finalPos = new Vector3(posX, posY, 0) + transform.position;

            // Spawn the specific vegetable for this index
            Instantiate(vegetablePrefabs[i], finalPos, Quaternion.identity, transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.5f); // Orange grid
        Vector2 offset = new Vector2((columns * cellSize) / 2, (rows * cellSize) / 2);
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 pos = new Vector3((x * cellSize) - offset.x + (cellSize / 2), (y * cellSize) - offset.y + (cellSize / 2), 0);
                Gizmos.DrawWireCube(pos + transform.position, new Vector3(cellSize, cellSize, 0));
            }
        }
    }
}