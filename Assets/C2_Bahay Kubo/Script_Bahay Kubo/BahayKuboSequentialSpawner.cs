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
        // Safety checks to prevent errors
        if (isGameOver || bahayKuboAudio == null || !bahayKuboAudio.isPlaying) return;
        if (SongManager.Instance == null || SongManager.Instance.beatmap == null) return;

        // Ensure we don't look past the end of the beatmap list
        if (nextExpectedIndexInBatch < SongManager.Instance.beatmap.Count)
        {
            float currentTime = bahayKuboAudio.time;
            float targetTime = SongManager.Instance.beatmap[nextExpectedIndexInBatch].timestamp;

            // --- AUTO-SKIP LOGIC ---
            // If the song is past the target timestamp AND past the hit window, mark it as missed
            if (currentTime > (targetTime + hitWindow))
            {
                Debug.Log($"<color=orange>Missed {vegetablePrefabs[nextExpectedIndexInBatch].name}!</color> Auto-skipping...");

                if (HealthManager.Instance != null)
                {
                    HealthManager.Instance.TakeDamage(1);
                    if (HealthManager.Instance.currentHealth <= 0) TriggerGameOver();
                }

                HandleVegetableProgress();
            }
        }
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
    }

    public void TryHarvest(int clickedID, GameObject vegetableObj)
    {
        if (isGameOver) return;

        float clickTime = bahayKuboAudio.time;
        float targetTimestamp = SongManager.Instance.beatmap[clickedID].timestamp;

        // Check if the click is within the rhythm window
        if (Mathf.Abs(clickTime - targetTimestamp) <= hitWindow)
        {
            if (clickedID == nextExpectedIndexInBatch)
            {
                Destroy(vegetableObj);
                HandleVegetableProgress();
            }
            else
            {
                // Wrong sequence click
                Penalty();
            }
        }
        else
        {
            // Wrong timing click
            Penalty();
        }
    }

    private void HandleVegetableProgress()
    {
        nextExpectedIndexInBatch++;

        int currentBatchEnd = globalVegetableOffset + batchSizes[currentPhase];

        if (nextExpectedIndexInBatch >= currentBatchEnd)
        {
            globalVegetableOffset += batchSizes[currentPhase];
            currentPhase++;
            Invoke("SpawnCurrentBatch", 0.5f);
        }
    }

    private void Penalty()
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
}