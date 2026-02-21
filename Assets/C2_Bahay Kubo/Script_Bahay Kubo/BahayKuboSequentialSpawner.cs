using UnityEngine;
using System.Collections.Generic;

public class BahayKuboSequentialSpawner : MonoBehaviour
{
    [Header("Vegetable Pool (All 18 in Order)")]
    public GameObject[] vegetablePrefabs;

    [Header("Song Structure")]
    // The number of vegetables in each line: 4, 3, 4, 2, 4, 1
    private int[] batchSizes = { 4, 3, 4, 2, 4, 1 };
    private int currentPhase = 0; // Which line of the song we are on
    private int nextExpectedIndexInBatch = 0; // Progress within the current batch

    [Header("Grid Settings")]
    public int columns = 6;
    public int rows = 4;
    public float cellSize = 1.8f;
    public float jitterAmount = 0.3f;

    private List<GameObject> activeVegetables = new List<GameObject>();
    private int globalVegetableOffset = 0; // Starting point in the prefab array for the current batch

    void Start()
    {
        if (vegetablePrefabs.Length < 18)
        {
            Debug.LogWarning("The song usually has 18 vegetables. Check your prefab list!");
        }
        SpawnCurrentBatch();
    }

    public void SpawnCurrentBatch()
    {
        // 1. Clear previous batch
        ClearGarden();

        if (currentPhase >= batchSizes.Length)
        {
            Debug.Log("Song Finished! Re-starting from the beginning.");
            currentPhase = 0;
            globalVegetableOffset = 0;
        }

        int countToSpawn = batchSizes[currentPhase];
        List<Vector2Int> allCells = GetShuffledCells();
        Vector2 gridOffset = new Vector2((columns * cellSize) / 2, (rows * cellSize) / 2);

        // 2. Spawn only the vegetables for this specific line of the song
        for (int i = 0; i < countToSpawn; i++)
        {
            int prefabIndex = globalVegetableOffset + i;
            if (prefabIndex >= vegetablePrefabs.Length) break;

            Vector2Int cell = allCells[i];
            float posX = (cell.x * cellSize) - gridOffset.x + (cellSize / 2) + Random.Range(-jitterAmount, jitterAmount);
            float posY = (cell.y * cellSize) - gridOffset.y + (cellSize / 2) + Random.Range(-jitterAmount, jitterAmount);
            Vector3 finalPos = new Vector3(posX, posY, 0) + transform.position;

            GameObject newVeg = Instantiate(vegetablePrefabs[prefabIndex], finalPos, Quaternion.identity, transform);

            // Set up click logic
            VegetableClick clickScript = newVeg.GetComponent<VegetableClick>();
            if (clickScript != null)
            {
                clickScript.spawner = this;
                clickScript.vegetableID = prefabIndex;
            }
            activeVegetables.Add(newVeg);
        }

        nextExpectedIndexInBatch = globalVegetableOffset;
    }

    public void TryHarvest(int clickedID, GameObject vegetableObj)
    {
        // 1. Check if the clicked vegetable matches the next one in the song sequence
        if (clickedID == nextExpectedIndexInBatch)
        {
            nextExpectedIndexInBatch++;

            // Determine the next vegetable's name for the log
            int currentBatchEnd = globalVegetableOffset + batchSizes[currentPhase];

            if (nextExpectedIndexInBatch < currentBatchEnd)
            {
                // There are still veggies left in this specific line of the song
                string nextVegName = vegetablePrefabs[nextExpectedIndexInBatch].name;
                Debug.Log($"<color=green>Correct veggie!</color> Next veggie to find is: <b>{nextVegName}</b>");
            }
            else
            {
                // The batch is finished
                Debug.Log("<color=cyan>Correct veggie!</color> Line complete! Moving to next batch...");
            }

            Destroy(vegetableObj);

            // 2. Check if the current line (batch) of the song is finished
            if (nextExpectedIndexInBatch >= currentBatchEnd)
            {
                globalVegetableOffset += batchSizes[currentPhase];
                currentPhase++;
                Invoke("SpawnCurrentBatch", 0.5f);
            }
        }
        else
        {
            // 3. Wrong vegetable clicked logic
            string targetVegName = vegetablePrefabs[nextExpectedIndexInBatch].name;
            Debug.Log($"<color=red>Wrong veggie!</color> You should have clicked: <b>{targetVegName}</b>");
        }
    }

    private List<Vector2Int> GetShuffledCells()
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        for (int x = 0; x < columns; x++)
            for (int y = 0; y < rows; y++)
                cells.Add(new Vector2Int(x, y));

        for (int i = 0; i < cells.Count; i++)
        {
            Vector2Int temp = cells[i];
            int randomIndex = Random.Range(i, cells.Count);
            cells[i] = cells[randomIndex];
            cells[randomIndex] = temp;
        }
        return cells;
    }

    private void ClearGarden()
    {
        foreach (GameObject veg in activeVegetables) if (veg != null) Destroy(veg);
        activeVegetables.Clear();
    }
}