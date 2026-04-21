using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BahayKuboSequentialSpawner : MonoBehaviour
{
    [Header("Vegetable Pool")]
    public GameObject[] vegetablePrefabs;

    [Header("Song Structure")]
    private int[] batchSizes = { 4, 3, 4, 2, 4, 1 };
    private int currentPhase = 0;
    private int nextExpectedIndexInBeatmap = 0;
    private int difficultyCycle = 0; // 0=Easy, 1=Medium, 2=Hard

    [Header("Grid Settings")]
    public int columns = 3;
    public int rows = 3;
    public float cellSize = 2f;

    [Header("Rhythm Settings")]
    public AudioSource bahayKuboAudio;
    public float hitWindow = 0.5f;

    [Header("Game State & UI")]
    public GameObject pausePanel;
    public GameObject feedbackPopup;
    public Text feedbackText;
    public Vector3 popupOffset = new Vector3(0, 50f, 0);

    private bool isGameOver = false;
    private List<GameObject> activeVegetables = new List<GameObject>();
    private int globalVegetableOffset = 0;

    [Header("Health UI References")]
    public GameObject[] heartIcons; // Drag your 3 Heart GameObjects here in the Inspector

    [Header("Grace Period")]
    private bool isGracePeriodActive = false;
    private bool allowGracePeriod = true;

void Start()
{
    // 1. Set starting point based on user choice
    int startingCycle = 0;
    if (GameSettings.CurrentDifficulty == Difficulty.Medium) startingCycle = 1;
    else if (GameSettings.CurrentDifficulty == Difficulty.Hard) startingCycle = 2;

    // 2. Initialize Rules (This now handles the Hearts automatically)
    UpdateRulesForCycle(startingCycle);

    // 3. UI Setup
    if (pausePanel != null) pausePanel.SetActive(false);
    if (feedbackPopup != null) feedbackPopup.SetActive(false);

    SpawnCurrentBatch();
}
private void UpdateRulesForCycle(int cycle)
{
    difficultyCycle = cycle;
    
    // --- NEW: HANDLE HEART VISIBILITY MID-GAME ---
    if (heartIcons != null && heartIcons.Length > 0)
    {
        if (cycle == 0) // Easy Mode
        {
            foreach (GameObject heart in heartIcons) if (heart != null) heart.SetActive(false);
            hitWindow = 0.8f;
            allowGracePeriod = true;
            Debug.Log("<color=green>BK: Easy Mode - Hearts Hidden</color>");
        }
        else if (cycle == 1) // Medium Mode
        {
            // Turn hearts BACK ON when moving from Easy to Medium
            foreach (GameObject heart in heartIcons) if (heart != null) heart.SetActive(true);
            hitWindow = 0.5f;
            allowGracePeriod = false;
            Debug.Log("<color=yellow>BK: Medium Mode - Hearts Restored</color>");
        }
        else // Hard Mode
        {
            // Show only the first heart
            for (int i = 0; i < heartIcons.Length; i++)
            {
                if (heartIcons[i] != null) heartIcons[i].SetActive(i == 0);
            }
            hitWindow = 0.3f;
            allowGracePeriod = false;
            Debug.Log("<color=red>BK: Hard Mode - One Heart Only</color>");
        }
    }

    isGracePeriodActive = false; 
}

    void Update()
    {
        if (isGameOver || bahayKuboAudio == null || !bahayKuboAudio.isPlaying) return;

        if (SongManager.Instance != null && nextExpectedIndexInBeatmap < SongManager.Instance.beatmap.Count)
        {
            float currentTime = bahayKuboAudio.time;
            float targetTime = SongManager.Instance.beatmap[nextExpectedIndexInBeatmap].timestamp;

            if (currentTime > (targetTime + hitWindow))
            {
                RemoveMissedVegetable(nextExpectedIndexInBeatmap);
                ApplyPenalty();
                HandleProgress();
            }
        }
    }

    public void TryHarvest(int clickedID, GameObject vegetableObj)
    {
        if (isGameOver) return;

        float clickTime = bahayKuboAudio.time;
        if (SongManager.Instance == null || clickedID >= SongManager.Instance.beatmap.Count) return;

        float targetTimestamp = SongManager.Instance.beatmap[clickedID].timestamp;
        float timeDifference = clickTime - targetTimestamp;

        bool isCorrectVeggie = (clickedID == nextExpectedIndexInBeatmap);
        bool isOnBeat = Mathf.Abs(timeDifference) <= hitWindow;

        if (isCorrectVeggie && isOnBeat)
        {
            activeVegetables.Remove(vegetableObj);
            Destroy(vegetableObj);
            HandleProgress();
        }
        else
        {
            string msg = !isCorrectVeggie ? "Mali!" : (timeDifference < 0 ? "Too Early!" : "Too Late!");
            ShowFeedback(vegetableObj, msg);
            ApplyPenalty();
        }
    }

private void ApplyPenalty()
{
    if (isGameOver) return;

    // 1. Check current cycle (0 = Easy, 1 = Medium, 2 = Hard)
    // We use difficultyCycle because it updates mid-song!
    if (difficultyCycle == 0) 
    {
        // EASY MODE: The "Infinite Health" logic
        if (!isGracePeriodActive)
        {
            isGracePeriodActive = true;
            if (feedbackText != null) feedbackText.text = "Ingat!";
            Debug.Log("Easy Cycle: Grace active, no health lost.");
        }
        else
        {
            isGracePeriodActive = false;
            Debug.Log("Easy Cycle: Damage blocked (Infinite).");
        }
        return; // Exit here so HealthManager is never touched
    }

    // 2. MEDIUM & HARD MODE: Actually take damage
    if (HealthManager.Instance != null)
    {
        if (difficultyCycle == 2) // Hard Mode (Cycle 2)
        {
            Debug.Log("<color=red>Hard Cycle: Instant Death!</color>");
            HealthManager.Instance.TakeDamage(HealthManager.Instance.maxHealth);
            
            if (heartIcons != null && heartIcons.Length > 0 && heartIcons[0] != null)
            {
                heartIcons[0].SetActive(false);
            }
        }
        else // Medium Mode (Cycle 1)
        {
            HealthManager.Instance.TakeDamage(1);
            Debug.Log("Medium Cycle: 1 Life Lost.");
        }

        // 3. Check for Death
        if (HealthManager.Instance.currentHealth <= 0)
        {
            TriggerGameOver();
        }
    }
}

    private void TriggerGameOver()
    {
        isGameOver = true;
        if (bahayKuboAudio != null) bahayKuboAudio.Stop();
        ClearGarden();
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void RestartGame()
    {
        isGameOver = false;

        // Reset based on initial Menu choice
        int startingCycle = 0;
        if (GameSettings.CurrentDifficulty == Difficulty.Medium) startingCycle = 1;
        else if (GameSettings.CurrentDifficulty == Difficulty.Hard) startingCycle = 2;

        currentPhase = 0;
        globalVegetableOffset = 0;
        nextExpectedIndexInBeatmap = 0;

        UpdateRulesForCycle(startingCycle);

        if (pausePanel != null) pausePanel.SetActive(false);
        if (HealthManager.Instance != null) HealthManager.Instance.ResetHealth();

        if (bahayKuboAudio != null)
        {
            bahayKuboAudio.Stop();
            bahayKuboAudio.time = 0;
            bahayKuboAudio.Play();
        }

        SpawnCurrentBatch();
    }

    public void SpawnCurrentBatch()
    {
        ClearGarden();
        if (difficultyCycle > 2) return;

        int countToSpawn = batchSizes[currentPhase];
        List<Vector2Int> allCells = GetShuffledCells();
        Vector2 gridOffset = new Vector2((columns * cellSize) / 2, (rows * cellSize) / 2);

        for (int i = 0; i < countToSpawn; i++)
        {
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
                clickScript.vegetableID = nextExpectedIndexInBeatmap + i;
            }
            activeVegetables.Add(newVeg);
        }
        UpdateEasyHint();
    }

    private void UpdateEasyHint()
    {
        // Hint only shows if the CURRENT mode is Easy
        bool showHint = (difficultyCycle == 0);
        foreach (GameObject veg in activeVegetables)
        {
            if (veg == null) continue;
            VegetableClick script = veg.GetComponent<VegetableClick>();
            if (script != null)
            {
                script.SetHint(showHint && script.vegetableID == nextExpectedIndexInBeatmap);
            }
        }
    }

    private void HandleProgress()
    {
        nextExpectedIndexInBeatmap++;

        int currentBatchStart = 0;
        for (int i = 0; i < currentPhase; i++) currentBatchStart += batchSizes[i];
        currentBatchStart += (difficultyCycle * 18);

        if (nextExpectedIndexInBeatmap >= currentBatchStart + batchSizes[currentPhase])
        {
            currentPhase++;

            if (currentPhase >= batchSizes.Length)
            {
                currentPhase = 0;
                globalVegetableOffset = 0;

                int nextCycle = difficultyCycle + 1;
                if (nextCycle <= 2)
                {
                    UpdateRulesForCycle(nextCycle);
                    Invoke("SpawnCurrentBatch", 0.5f);
                }
                else { TriggerGameOver(); } // Won game
            }
            else
            {
                globalVegetableOffset += batchSizes[currentPhase - 1];
                Invoke("SpawnCurrentBatch", 0.5f);
            }
        }
        else { UpdateEasyHint(); }
    }

    private void ShowFeedback(GameObject vegetableObj, string message)
    {
        VegetableClick clickScript = vegetableObj.GetComponent<VegetableClick>();
        if (clickScript != null) clickScript.FlashRed();

        if (feedbackPopup != null)
        {
            if (feedbackText != null) feedbackText.text = message;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(vegetableObj.transform.position);
            feedbackPopup.transform.position = screenPos + popupOffset;
            feedbackPopup.SetActive(true);
            CancelInvoke("HideFeedback");
            Invoke("HideFeedback", 0.6f);
        }
    }

    private void HideFeedback() => feedbackPopup.SetActive(false);

    private void RemoveMissedVegetable(int missedID)
    {
        for (int i = activeVegetables.Count - 1; i >= 0; i--)
        {
            GameObject veg = activeVegetables[i];
            if (veg != null && veg.GetComponent<VegetableClick>().vegetableID == missedID)
            {
                Destroy(veg);
                activeVegetables.RemoveAt(i);
                break;
            }
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