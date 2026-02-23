using UnityEngine;
using System.Collections.Generic;

public class BahayKuboSequentialSpawner : MonoBehaviour
{
    [Header("Vegetable Pool (All 18 in Project Order)")]
    public GameObject[] vegetablePrefabs;

    [Header("Song Structure")]
    private int[] batchSizes = { 4, 3, 4, 2, 4, 1 };
    private int currentPhase = 0;
    private int nextExpectedIndexInBeatmap = 0;

    [Header("Grid Settings")]
    public int columns = 3;
    public int rows = 3;
    public float cellSize = 2f;
    public float jitterAmount = 0f;

    [Header("Rhythm Settings")]
    public AudioSource bahayKuboAudio;
    public float hitWindow = 0.5f;

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

    void Update()
    {
        if (isGameOver || bahayKuboAudio == null || !bahayKuboAudio.isPlaying) return;

        // --- AUTO-SKIP MISSES ---
        // Range Check: Ensure we don't go past the list size
        if (SongManager.Instance != null && nextExpectedIndexInBeatmap < SongManager.Instance.beatmap.Count)
        {
            float currentTime = bahayKuboAudio.time;
            float targetTime = SongManager.Instance.beatmap[nextExpectedIndexInBeatmap].timestamp;

            // If the song has moved past the 'hit window' for the current veggie
            if (currentTime > (targetTime + hitWindow))
            {
                Debug.Log($"<color=orange>Missed {SongManager.Instance.beatmap[nextExpectedIndexInBeatmap].vegetableName}!</color>");
                ApplyPenalty();
                HandleProgress();
            }
        }
    }

    public void SpawnCurrentBatch()
    {
        ClearGarden();

        // Safety check for phase looping
        if (currentPhase >= batchSizes.Length) currentPhase = 0;

        int countToSpawn = batchSizes[currentPhase];
        List<Vector2Int> allCells = GetShuffledCells();
        Vector2 gridOffset = new Vector2((columns * cellSize) / 2, (rows * cellSize) / 2);

        for (int i = 0; i < countToSpawn; i++)
        {
            // Use % 18 so it always picks from your 18 prefabs even in Cycle 2 (indices 18-35)
            int prefabIndex = (globalVegetableOffset + i) % 18;
            if (prefabIndex >= vegetablePrefabs.Length) break;

            Vector2Int cell = allCells[i];
            float posX = (cell.x * cellSize) - gridOffset.x + (cellSize / 2);
            float posY = (cell.y * cellSize) - gridOffset.y + (cellSize / 2);
            Vector3 finalPos = new Vector3(posX, posY, 0) + transform.position;

            GameObject newVeg = Instantiate(vegetablePrefabs[prefabIndex], finalPos, Quaternion.identity, transform);

            VegetableClick clickScript = newVeg.GetComponent<VegetableClick>();
            if (clickScript != null)
            {
                clickScript.spawner = this;
                // Assign the ID to match the global Beatmap index (0, 1, 2... 35)
                clickScript.vegetableID = globalVegetableOffset + i;
            }
            activeVegetables.Add(newVeg);
        }
    }

    public void TryHarvest(int clickedID, GameObject vegetableObj)
    {
        if (isGameOver) return;

        float clickTime = bahayKuboAudio.time;
        if (SongManager.Instance == null || clickedID >= SongManager.Instance.beatmap.Count) return;

        float targetTimestamp = SongManager.Instance.beatmap[clickedID].timestamp;

        // Check if the click is within the rhythm window
        if (Mathf.Abs(clickTime - targetTimestamp) <= hitWindow)
        {
            // Must be the exact vegetable expected in the long beatmap sequence
            if (clickedID == nextExpectedIndexInBeatmap)
            {
                Destroy(vegetableObj);
                HandleProgress();
            }
            else
            {
                Debug.Log("Wrong Veggie for this part of the song!");
                ApplyPenalty();
            }
        }
        else
        {
            Debug.Log("Off-beat click!");
            ApplyPenalty();
        }
    }

    private void HandleProgress()
    {
        // Keep increasing the index to move through the 36+ element Beatmap list
        nextExpectedIndexInBeatmap++;

        int currentBatchEnd = globalVegetableOffset + batchSizes[currentPhase];

        // Check if the current visual batch is finished
        if (nextExpectedIndexInBeatmap >= currentBatchEnd)
        {
            globalVegetableOffset += batchSizes[currentPhase];
            currentPhase++;

            // Loop the phases (visual batches) but NOT the global offset
            if (currentPhase >= batchSizes.Length)
            {
                currentPhase = 0;
            }

            Invoke("SpawnCurrentBatch", 0.5f);
        }
    }

    private void ApplyPenalty()
    {
        if (HealthManager.Instance != null)
        {
            HealthManager.Instance.TakeDamage(1);
            if (HealthManager.Instance.currentHealth <= 0) TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void RestartGame()
    {
        isGameOver = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (HealthManager.Instance != null) HealthManager.Instance.ResetHealth();

        bahayKuboAudio.Stop();
        bahayKuboAudio.Play();

        currentPhase = 0;
        globalVegetableOffset = 0;
        nextExpectedIndexInBeatmap = 0;
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
}

