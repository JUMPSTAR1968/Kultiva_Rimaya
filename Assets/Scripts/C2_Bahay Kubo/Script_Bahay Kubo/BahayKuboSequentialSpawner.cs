using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BahayKuboSequentialSpawner : MonoBehaviour
{
    [Header("Vegetable Pool")]
    public GameObject[] vegetablePrefabs;

    [Header("Song Structure")]
    private int[] batchSizes = { 4, 3, 4, 2, 4, 1 };
    private int currentPhase = 0;
    private int nextExpectedIndexInBeatmap = 0;
    private int difficultyCycle = 0; // 0=Easy, 1=Medium, 2=Hard
    private int globalVegetableOffset = 0;

    [Header("Grid Settings")]
    public int columns = 3;
    public int rows = 3;
    public float cellSize = 2f;

    [Header("Rhythm Settings")]
    public AudioSource bahayKuboAudio;
    public float hitWindow = 0.5f;

    [Header("Game State & UI")]
    public GameObject pausePanel;
    public GameObject restartButton;
    public TextMeshProUGUI _scoreLabel;
    public GameObject feedbackPopup;
    public Text feedbackText;
    public Vector3 popupOffset = new Vector3(0, 50f, 0);

    [Header("Health UI References")]
    public GameObject[] heartIcons;

    private bool isGameOver = false;
    private List<GameObject> activeVegetables = new List<GameObject>();
    private bool mediumGracePointUsed = false;
    private bool hintTriggered = false;

    void Start()
    {
        // Initialize Difficulty based on GameSettings
        int startingCycle = 0;
        if (GameSettings.CurrentDifficulty == Difficulty.Medium) startingCycle = 1;
        else if (GameSettings.CurrentDifficulty == Difficulty.Hard) startingCycle = 2;

        UpdateRulesForCycle(startingCycle);

        if (pausePanel != null) pausePanel.SetActive(false);
        if (restartButton != null) restartButton.SetActive(true);
        if (feedbackPopup != null) feedbackPopup.SetActive(false);

        SpawnCurrentBatch();
    }

    void Update()
    {
        if (isGameOver || bahayKuboAudio == null || !bahayKuboAudio.isPlaying) return;

        // Update Score UI
        if (SongManager.Instance != null && _scoreLabel != null)
        {
            _scoreLabel.text = SongManager.Instance.ScoreCount.ToString();
        }

        // --- AUTO-SKIP MISSES ---
        if (SongManager.Instance != null && nextExpectedIndexInBeatmap < SongManager.Instance.beatmap.Count)
        {
            float currentTime = bahayKuboAudio.time;
            float targetTime = SongManager.Instance.beatmap[nextExpectedIndexInBeatmap].timestamp;

            if (currentTime > (targetTime + hitWindow))
            {
                Debug.Log($"Missed {SongManager.Instance.beatmap[nextExpectedIndexInBeatmap].vegetableName}!");
                RemoveMissedVegetable(nextExpectedIndexInBeatmap);

                if (difficultyCycle == 0) hintTriggered = true;

                ApplyPenalty();
                HandleProgress();
            }
        }
    }

    private void UpdateRulesForCycle(int cycle)
    {
        difficultyCycle = cycle;
        mediumGracePointUsed = false;

        if (heartIcons != null && heartIcons.Length > 0)
        {
            if (cycle == 0) // Easy
            {
                foreach (GameObject heart in heartIcons) if (heart != null) heart.SetActive(false);
                hitWindow = 0.8f;
            }
            else if (cycle == 1) // Medium
            {
                foreach (GameObject heart in heartIcons) if (heart != null) heart.SetActive(true);
                hitWindow = 0.5f;
            }
            else // Hard
            {
                for (int i = 0; i < heartIcons.Length; i++)
                {
                    if (heartIcons[i] != null) heartIcons[i].SetActive(i == 0);
                }
                hitWindow = 0.3f;
            }
        }
        hintTriggered = false;
    }

    public void SpawnCurrentBatch()
    {
        // 1. Force clear any lingering vegetables before spawning new ones
        ClearGarden();

        // Safety check: Don't spawn if we've exceeded the beatmap
        if (SongManager.Instance != null && nextExpectedIndexInBeatmap >= SongManager.Instance.beatmap.Count)
        {
            return;
        }

        int countToSpawn = batchSizes[currentPhase];
        List<Vector2Int> allCells = GetShuffledCells();
        Vector2 gridOffset = new Vector2((columns * cellSize) / 2, (rows * cellSize) / 2);

        for (int i = 0; i < countToSpawn; i++)
        {
            // Use the global offset to pick the prefab so they appear in song order
            int prefabIndex = (globalVegetableOffset + i) % vegetablePrefabs.Length;

            Vector2Int cell = allCells[i];
            float posX = (cell.x * cellSize) - gridOffset.x + (cellSize / 2);
            float posY = (cell.y * cellSize) - gridOffset.y + (cellSize / 2);
            Vector3 finalPos = new Vector3(posX, posY, 0) + transform.position;

            GameObject newVeg = Instantiate(vegetablePrefabs[prefabIndex], finalPos, Quaternion.identity, transform);
            VegetableClick clickScript = newVeg.GetComponent<VegetableClick>();

            if (clickScript != null)
            {
                clickScript.spawner = this;
                // CRITICAL: ID must be relative to the global song index
                clickScript.vegetableID = globalVegetableOffset + i;
            }
            activeVegetables.Add(newVeg);
        }
        UpdateEasyHint();
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

            if (SongManager.Instance != null) SongManager.Instance.ScoreCount++;

            HandleProgress();
        }
        else
        {
            // Easy mode hint logic
            bool isWayTooEarly = clickTime < (targetTimestamp - 1f);
            if (difficultyCycle == 0 && !isWayTooEarly)
            {
                hintTriggered = true;
                UpdateEasyHint();
            }

            string msg = !isCorrectVeggie ? "Mali!" : (timeDifference < 0 ? "Too Early!" : "Too Late!");
            ShowFeedback(vegetableObj, msg);
            ApplyPenalty();
        }
    }

    private void HandleProgress()
    {
        nextExpectedIndexInBeatmap++;
        hintTriggered = false;

        // Calculate the end of the current visual batch
        int currentBatchEnd = globalVegetableOffset + batchSizes[currentPhase];

        // If the player cleared the last vegetable of the current visual batch
        if (nextExpectedIndexInBeatmap >= currentBatchEnd)
        {
            // Update the offset to the start of the NEW batch
            globalVegetableOffset = nextExpectedIndexInBeatmap;
            currentPhase++;

            // Check if we finished the whole song/cycle
            if (currentPhase >= batchSizes.Length)
            {
                currentPhase = 0;
                int nextCycle = difficultyCycle + 1;

                // Advance difficulty or loop Hard mode
                UpdateRulesForCycle(Mathf.Min(nextCycle, 2));
            }

            // Small delay to let the last vegetable "poof" before new ones appear
            CancelInvoke("SpawnCurrentBatch");
            Invoke("SpawnCurrentBatch", 0.3f);
        }
        else
        {
            UpdateEasyHint();
        }
    }

    private void ApplyPenalty()
    {
        if (isGameOver) return;

        if (SongManager.Instance != null) SongManager.Instance.ResetScore();

        if (difficultyCycle == 0) return; // Easy mode: no damage

        if (HealthManager.Instance != null)
        {
            if (difficultyCycle == 2) // Hard: Instant Death
            {
                HealthManager.Instance.TakeDamage(HealthManager.Instance.maxHealth);
                if (heartIcons.Length > 0 && heartIcons[0] != null) heartIcons[0].SetActive(false);
            }
            else if (difficultyCycle == 1) // Medium: 2-hit grace
            {
                if (!mediumGracePointUsed)
                {
                    mediumGracePointUsed = true;
                    if (feedbackText != null) feedbackText.text = "Ingat! (1/2)";
                }
                else
                {
                    HealthManager.Instance.TakeDamage(1);
                    mediumGracePointUsed = false;
                }
            }

            if (HealthManager.Instance.currentHealth <= 0) TriggerGameOver();
        }
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
        if (SongManager.Instance != null) SongManager.Instance.ResetScore();

        if (bahayKuboAudio != null)
        {
            bahayKuboAudio.Stop();
            bahayKuboAudio.time = 0;
            bahayKuboAudio.Play();
        }

        SpawnCurrentBatch();
    }

    private void ShowFeedback(GameObject vegetableObj, string message)
    {
        if (vegetableObj == null) return;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 gridOffset = new Vector2((columns * cellSize) / 2, (rows * cellSize) / 2);
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                float posX = (x * cellSize) - gridOffset.x + (cellSize / 2);
                float posY = (y * cellSize) - gridOffset.y + (cellSize / 2);
                Vector3 cellCenter = new Vector3(posX, posY, 0) + transform.position;
                Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, cellSize, 0.1f));
            }
        }
    }
}