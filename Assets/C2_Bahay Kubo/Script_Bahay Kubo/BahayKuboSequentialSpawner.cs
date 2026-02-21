using UnityEngine;
using System.Collections.Generic;

public class BahayKuboSequentialSpawner : MonoBehaviour
{
    [Header("Vegetable Pool (Order of Song)")]
    [Tooltip("Place prefabs in order: Singkamas, Talong, Sigarilyas, Mani, etc.")]
    public GameObject[] vegetablePrefabs; 

    [Header("Grid Settings")]
    public int columns = 10;
    public int rows = 5;
    public float cellSize = 1.5f;
    public float jitterAmount = 0.3f;

    // Logic Tracking
    private List<GameObject> activeVegetables = new List<GameObject>();
    private int nextExpectedIndex = 0; 

    void Start()
    {
        if (vegetablePrefabs.Length == 0)
        {
            Debug.LogError("No vegetables assigned to the Spawner!");
            return;
        }
        SpawnInOrder();
    }

    public void SpawnInOrder()
    {
        // 1. Define the possible grid spots
        List<Vector2Int> allCells = new List<Vector2Int>();
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                allCells.Add(new Vector2Int(x, y));
            }
        }

        // 2. Shuffle the grid spots
        for (int i = 0; i < allCells.Count; i++)
        {
            Vector2Int temp = allCells[i];
            int randomIndex = Random.Range(i, allCells.Count);
            allCells[i] = allCells[randomIndex];
            allCells[randomIndex] = temp;
        }

        // 3. Calculate grid centering offset
        Vector2 gridOffset = new Vector2((columns * cellSize) / 2, (rows * cellSize) / 2);

        // 4. Spawn vegetables sequentially based on the prefab list
        for (int i = 0; i < vegetablePrefabs.Length; i++)
        {
            if (i >= allCells.Count) break; 

            Vector2Int cell = allCells[i];

            // Math for centering and jitter
            float posX = (cell.x * cellSize) - gridOffset.x + (cellSize / 2);
            float posY = (cell.y * cellSize) - gridOffset.y + (cellSize / 2);
            posX += Random.Range(-jitterAmount, jitterAmount);
            posY += Random.Range(-jitterAmount, jitterAmount);

            Vector3 finalPos = new Vector3(posX, posY, 0) + transform.position;

            // Instantiate and set up references
            GameObject newVeg = Instantiate(vegetablePrefabs[i], finalPos, Quaternion.identity, transform);
            
            VegetableClick clickScript = newVeg.GetComponent<VegetableClick>();
            if (clickScript != null)
            {
                clickScript.spawner = this;
                clickScript.vegetableID = i; // Assigns the "Lyric Order" ID
            }
            else
            {
                Debug.LogWarning(newVeg.name + " is missing the VegetableClick script!");
            }

            activeVegetables.Add(newVeg);
        }
    }

    // This handles the "By Order" click logic
    public void TryHarvest(int clickedID, GameObject vegetableObj)
    {
        if (clickedID == nextExpectedIndex)
        {
            Debug.Log("Correct! Harvested: " + vegetablePrefabs[clickedID].name);
            
            Destroy(vegetableObj);
            nextExpectedIndex++;

            // If the whole song verse is finished, reset the garden
            if (nextExpectedIndex >= vegetablePrefabs.Length)
            {
                Debug.Log("Verse Complete! Refreshing...");
                RefreshGarden();
            }
        }
        else
        {
            Debug.Log("Wrong veggie! You need to find the: " + vegetablePrefabs[nextExpectedIndex].name);
        }
    }

    public void RefreshGarden()
    {
        nextExpectedIndex = 0; 
        foreach (GameObject veg in activeVegetables)
        {
            if (veg != null) Destroy(veg);
        }
        activeVegetables.Clear();
        SpawnInOrder();
    }

    // Visualize the grid in Scene View
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