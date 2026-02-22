using UnityEngine;
using System.Collections.Generic;

public class BahayKuboSequentialSpawner : MonoBehaviour
{
    [Header("Vegetable Pool (All 18 in Order)")]
    public GameObject[] vegetablePrefabs;

    [Header("Song Structure")]
    private int[] batchSizes = { 4, 3, 4, 2, 4, 1 };
    private int currentPhase = 0; 
    private int nextExpectedIndexInBatch = 0; 

    [Header("Grid Settings")]
    public int columns = 6;
    public int rows = 4;
    public float cellSize = 1.8f;
    public float jitterAmount = 0.3f;

    [Header("Game State & UI")]
    public GameObject pausePanel;    
    public GameObject restartButton; 
    private bool isGameOver = false; 
    private List<GameObject> activeVegetables = new List<GameObject>();
    private int globalVegetableOffset = 0; 

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (restartButton != null) restartButton.SetActive(true);

        SpawnCurrentBatch();
    }

    public void SpawnCurrentBatch()
    {
        ClearGarden();

        if (currentPhase >= batchSizes.Length)
        {
            currentPhase = 0;
            globalVegetableOffset = 0;
        }

        int countToSpawn = batchSizes[currentPhase];
        List<Vector2Int> allCells = GetShuffledCells();
        Vector2 gridOffset = new Vector2((columns * cellSize) / 2, (rows * cellSize) / 2);

        for (int i = 0; i < countToSpawn; i++)
        {
            int prefabIndex = globalVegetableOffset + i;
            if (prefabIndex >= vegetablePrefabs.Length) break;

            Vector2Int cell = allCells[i];
            float posX = (cell.x * cellSize) - gridOffset.x + (cellSize / 2) + Random.Range(-jitterAmount, jitterAmount);
            float posY = (cell.y * cellSize) - gridOffset.y + (cellSize / 2) + Random.Range(-jitterAmount, jitterAmount);
            Vector3 finalPos = new Vector3(posX, posY, 0) + transform.position;

            GameObject newVeg = Instantiate(vegetablePrefabs[prefabIndex], finalPos, Quaternion.identity, transform);

            VegetableClick clickScript = newVeg.GetComponent<VegetableClick>();
            if (clickScript != null)
            {
                clickScript.spawner = this;
                clickScript.vegetableID = prefabIndex;
            }
            activeVegetables.Add(newVeg);
        }

        nextExpectedIndexInBatch = globalVegetableOffset;

        // --- DEBUG: SHOW THE FIRST VEGETABLE OF THE NEW BATCH ---
        string firstVeg = vegetablePrefabs[nextExpectedIndexInBatch].name;
        Debug.Log($"<color=white>New Batch Started!</color> First vegetable to find: <b>{firstVeg}</b>");
    }

    public void TryHarvest(int clickedID, GameObject vegetableObj)
    {
        if (isGameOver) return;

        // --- LOGIC: CHECK IF CLICKED ID MATCHES THE SEQUENCE ---
        if (clickedID == nextExpectedIndexInBatch)
        {
            nextExpectedIndexInBatch++;
            int currentBatchEnd = globalVegetableOffset + batchSizes[currentPhase];
            
            Destroy(vegetableObj);

            if (nextExpectedIndexInBatch < currentBatchEnd)
            {
                // --- DEBUG: SHOW THE NEXT TARGET AFTER A CORRECT CLICK ---
                string nextVegName = vegetablePrefabs[nextExpectedIndexInBatch].name;
                Debug.Log($"<color=green>Correct!</color> Next vegetable to harvest: <b>{nextVegName}</b>");
            }
            else
            {
                Debug.Log("<color=cyan>Batch Complete!</color> Preparing next line of the song...");
                
                globalVegetableOffset += batchSizes[currentPhase];
                currentPhase++;
                Invoke("SpawnCurrentBatch", 0.5f);
            }
        }
        else
        {
            // --- DEBUG: SHOW THE CORRECT TARGET AFTER A MISTAKE ---
            string targetVegName = vegetablePrefabs[nextExpectedIndexInBatch].name;
            Debug.Log($"<color=red>Wrong Veggie!</color> You should be looking for: <b>{targetVegName}</b>");

            if(HealthManager.Instance != null)
            {
                HealthManager.Instance.TakeDamage(1);
                if (HealthManager.Instance.currentHealth <= 0) TriggerGameOver();
            }
        }
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("<color=black><b>GAME OVER!</b></color> Use the Retry button to restart.");
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void RestartGame()
    {
        isGameOver = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (HealthManager.Instance != null) HealthManager.Instance.ResetHealth();

        currentPhase = 0;
        globalVegetableOffset = 0;
        nextExpectedIndexInBatch = 0;
        SpawnCurrentBatch();
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