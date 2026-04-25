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
    public GameObject[] heartIcons;

    [Header("Grace Period")]
    private bool isGracePeriodActive = false;

    [Header("Difficulty State")]
    private bool mediumGracePointUsed = false; // Tracks if the first mistake was made in Medium

    [Header("Hint State")]
    private bool hintTriggered = false;

    void Start()
    {
        int startingCycle = 0;
        if (GameSettings.CurrentDifficulty == Difficulty.Medium) startingCycle = 1;
        else if (GameSettings.CurrentDifficulty == Difficulty.Hard) startingCycle = 2;

        UpdateRulesForCycle(startingCycle);

        if (pausePanel != null) pausePanel.SetActive(false);
        if (feedbackPopup != null) feedbackPopup.SetActive(false);

        SpawnCurrentBatch();
    }

    private void UpdateRulesForCycle(int cycle)
    {
        difficultyCycle = cycle;
        mediumGracePointUsed = false; // Reset grace state on new cycle

        if (heartIcons != null && heartIcons.Length > 0)
        {
            if (cycle == 0) // Easy: Infinite Health
            {
                foreach (GameObject heart in heartIcons) if (heart != null) heart.SetActive(false);
                hitWindow = 0.8f;
            }
            else if (cycle == 1) // Medium: Grace Period (2 mistakes = 1 heart)
            {
                foreach (GameObject heart in heartIcons) if (heart != null) heart.SetActive(true);
                hitWindow = 0.5f;
            }
            else // Hard: One Life
            {
                for (int i = 0; i < heartIcons.Length; i++)
                {
                    if (heartIcons[i] != null) heartIcons[i].SetActive(i == 0);
                }
                hitWindow = 0.3f;
            }
        }
        isGracePeriodActive = false;
        hintTriggered = false;
    }

    void Update()
    {
        if (isGameOver || bahayKuboAudio == null || !bahayKuboAudio.isPlaying) return;

        if (SongManager.Instance != null && nextExpectedIndexInBeatmap < SongManager.Instance.beatmap.Count)
        {
            float currentTime = bahayKuboAudio.time;
            float targetTime = SongManager.Instance.beatmap[nextExpectedIndexInBeatmap].timestamp;

            // Only trigger auto-hint if the song has passed the hit window
            if (currentTime > (targetTime + hitWindow))
            {
                RemoveMissedVegetable(nextExpectedIndexInBeatmap);
                if (difficultyCycle == 0)
                {
                    hintTriggered = true;
                }
                ApplyPenalty();
                HandleProgress();
            }
        }
    }

    public void TryHarvest(int clickedID, GameObject vegetableObj)
    {
        if (isGameOver) return;

        float clickTime = bahayKuboAudio.time;
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
            // --- LOOSER ANTI-EXPLOIT ---
            // Only block the hint if they are clicking MORE than 1 second before the lyric.
            // If they click 0.1s early, we now allow the hint to trigger.
            bool isWayTooEarly = clickTime < (targetTimestamp - 1f);

            // Debugging: See why it's not pulsating
            Debug.Log($"Click: {clickTime} | Target: {targetTimestamp} | Diff: {timeDifference} | TooEarly: {isWayTooEarly}");

            if (difficultyCycle == 0 && !isWayTooEarly)
            {
                hintTriggered = true;
                UpdateEasyHint(); // Force refresh
            }

            string msg = !isCorrectVeggie ? "Mali!" : (timeDifference < 0 ? "Too Early!" : "Too Late!");
            ShowFeedback(vegetableObj, msg);
            ApplyPenalty();
        }
    }

    // Extracted logic for readability
    private void TriggerEasyHintLogic()
    {
        float currentTime = bahayKuboAudio.time;
        float targetTime = SongManager.Instance.beatmap[nextExpectedIndexInBeatmap].timestamp;

        if (currentTime >= (targetTime - 1.5f))
        {
            hintTriggered = true;
            UpdateEasyHint();
        }
    }

    private void ApplyPenalty()
    {
        if (isGameOver) return;

        // --- EASY MODE ---
        if (difficultyCycle == 0)
        {
            // Simply trigger the hint, no health deduction
            TriggerEasyHintLogic();
            return;
        }

        if (HealthManager.Instance != null)
        {
            // --- HARD MODE ---
            if (difficultyCycle == 2)
            {
                // Instant death
                HealthManager.Instance.TakeDamage(HealthManager.Instance.maxHealth);
                if (heartIcons != null && heartIcons.Length > 0 && heartIcons[0] != null)
                    heartIcons[0].SetActive(false);
            }
            // --- MEDIUM MODE ---
            else if (difficultyCycle == 1)
            {
                if (!mediumGracePointUsed)
                {
                    // First mistake: show warning but don't take health
                    mediumGracePointUsed = true;
                    if (feedbackText != null) feedbackText.text = "Ingat! (1/2)";
                }
                else
                {
                    // Second mistake: take health and reset grace counter
                    HealthManager.Instance.TakeDamage(1);
                    mediumGracePointUsed = false;
                }
            }

            // Check for Game Over after damage
            if (HealthManager.Instance.currentHealth <= 0)
            {
                TriggerGameOver();
            }
        }
    }

    public void RestartGame()
    {
        isGameOver = false;
        int startingCycle = 0;
        if (GameSettings.CurrentDifficulty == Difficulty.Medium) startingCycle = 1;
        else if (GameSettings.CurrentDifficulty == Difficulty.Hard) startingCycle = 2;

        currentPhase = 0;
        globalVegetableOffset = 0;
        nextExpectedIndexInBeatmap = 0;
        hintTriggered = false;

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

    private void OnDrawGizmos()
    {
        // Sets the color of the grid lines in the Scene view
        Gizmos.color = Color.yellow;

        // Calculate the same offset used in your SpawnCurrentBatch logic
        Vector2 gridOffset = new Vector2((columns * cellSize) / 2, (rows * cellSize) / 2);

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // Calculate the center of each cell
                float posX = (x * cellSize) - gridOffset.x + (cellSize / 2);
                float posY = (y * cellSize) - gridOffset.y + (cellSize / 2);
                Vector3 cellCenter = new Vector3(posX, posY, 0) + transform.position;

                // Draw a wire cube representing the spawn area of one vegetable
                Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, cellSize, 0.1f));
            }
        }
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
        bool isEasyMode = (difficultyCycle == 0);
        foreach (GameObject veg in activeVegetables)
        {
            if (veg == null) continue;
            VegetableClick script = veg.GetComponent<VegetableClick>();
            if (script != null)
            {
                bool isTarget = (script.vegetableID == nextExpectedIndexInBeatmap);
                script.SetHint(isEasyMode && isTarget && hintTriggered);
            }
        }
    }

    private void HandleProgress()
    {
        nextExpectedIndexInBeatmap++;
        hintTriggered = false;

        // Calculate how many vegetables are in all phases up to the current one
        int vegetablesInCompletedPhases = 0;
        for (int i = 0; i < currentPhase; i++)
        {
            vegetablesInCompletedPhases += batchSizes[i];
        }

        // The threshold is: (Vegetables in past phases) + (Vegetables in the current phase)
        int currentBatchThreshold = vegetablesInCompletedPhases + batchSizes[currentPhase];

        // If the player cleared the last vegetable of the current batch
        if (nextExpectedIndexInBeatmap >= currentBatchThreshold)
        {
            currentPhase++;

            // Check if we finished all batches in the current difficulty cycle
            if (currentPhase >= batchSizes.Length)
            {
                // Reset for the next cycle
                currentPhase = 0;
                // Since nextExpectedIndexInBeatmap keeps growing, we don't reset it,
                // but we ensure the prefab offset continues or wraps.

                int nextCycle = difficultyCycle + 1;

                if (nextCycle <= 2)
                {
                    UpdateRulesForCycle(nextCycle);
                    Invoke("SpawnCurrentBatch", 0.5f);
                }
                else
                {
                    // If Hard Mode (Cycle 2) is infinite, we just keep cycling Hard Mode
                    UpdateRulesForCycle(2);
                    Invoke("SpawnCurrentBatch", 0.5f);
                }
            }
            else
            {
                // Move to the next batch within the same cycle
                globalVegetableOffset = nextExpectedIndexInBeatmap;
                Invoke("SpawnCurrentBatch", 0.5f);
            }
        }
        else
        {
            UpdateEasyHint();
        }
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        if (bahayKuboAudio != null) bahayKuboAudio.Stop();
        ClearGarden();
        if (pausePanel != null) pausePanel.SetActive(true);
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