using UnityEngine;
using System.Collections.Generic;

public class SimpleRandomizer : MonoBehaviour
{
    [Header("Pool of Sprites")]
    public GameObject[] spritePrefabs;

    [Header("Grid Settings")]
    public int columns = 10;        // Number of chunks horizontally
    public int rows = 10;           // Number of chunks vertically
    public float cellSize = 2.0f;   // Size of each individual chunk/cell

    [Header("Randomization")]
    [Range(0, 100)]
    public float spawnChance = 70f; // Percent chance a chunk will contain a sprite
    public float jitterAmount = 0.5f; // How much to move the sprite from the cell center

    void Start()
    {
        if (spritePrefabs.Length == 0) return;
        GenerateGrid();
    }

    void GenerateGrid()
    {
        // Calculate the starting point so the grid is centered on the Spawner object
        Vector2 offset = new Vector2((columns * cellSize) / 2, (rows * cellSize) / 2);

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // 1. Roll the dice to see if we spawn anything in this chunk
                if (Random.Range(0f, 100f) <= spawnChance)
                {
                    // 2. Calculate the center of the current cell
                    float posX = (x * cellSize) - offset.x + (cellSize / 2);
                    float posY = (y * cellSize) - offset.y + (cellSize / 2);

                    // 3. Add Jitter so it's not a perfect, robotic grid
                    posX += Random.Range(-jitterAmount, jitterAmount);
                    posY += Random.Range(-jitterAmount, jitterAmount);

                    Vector3 finalPos = new Vector3(posX, posY, 0);

                    // 4. Pick a random sprite and spawn it
                    int randomIndex = Random.Range(0, spritePrefabs.Length);
                    Instantiate(spritePrefabs[randomIndex], finalPos, Quaternion.identity, transform);
                }
            }
        }
    }

    // Visualizes the chunks in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
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