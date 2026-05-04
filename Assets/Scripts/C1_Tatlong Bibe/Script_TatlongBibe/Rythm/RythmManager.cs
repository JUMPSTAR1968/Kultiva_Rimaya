using UnityEngine;

public class RythmManager : MonoBehaviour
{
    public static RythmManager Instance;

    [Header("Setup")]
    public AudioSource songSource;
    public GameObject kwakPrefab;
    public RectTransform spawnZone;

    // NEW: How far apart circles must be from each other!
    // You might need to tweak this number in the Inspector based on how big your circles are.
    public float minSpawnDistance = 150f;

    [Header("Rhythm Data")]
    public float[] kwakTimings;
    public float leadTime = 1.0f;

    [Header("Debug")]
    public float currentSongTime;

    private int nextNoteIndex = 0;
    private float previousSongTime = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (songSource != null)
        {
            songSource.loop = true;
            songSource.Play();
        }
    }

    void Update()
    {
        if (songSource == null) return;

        currentSongTime = songSource.time;

        // LOOP DETECTION
        if (currentSongTime < previousSongTime)
        {
            nextNoteIndex = 0;

            if (MTB_GameManager.Instance != null)
            {
                MTB_GameManager.Instance.IncreaseLoopSpeed();
            }

            if (ObstacleSpawn.Instance != null)
            {
                ObstacleSpawn.Instance.DecreaseSpawnRate();
            }
        }
        previousSongTime = currentSongTime;

        if (nextNoteIndex >= kwakTimings.Length) return;

        // Spawn logic
        if (currentSongTime >= kwakTimings[nextNoteIndex] - leadTime)
        {
            SpawnKwak(kwakTimings[nextNoteIndex]);
            nextNoteIndex++;
        }
    }

    void SpawnKwak(float targetTime)
    {
        GameObject newNote = Instantiate(kwakPrefab, spawnZone);
        newNote.transform.localScale = Vector3.one;

        Vector2 finalPosition = Vector2.zero;
        bool foundValidPos = false;

        // NEW SMART RADAR: Start with your ideal distance
        float currentTryDistance = minSpawnDistance;

        // Phase loop: If it fails to find a spot, it shrinks the safe distance slightly and tries again!
        for (int phase = 0; phase < 5; phase++)
        {
            for (int i = 0; i < 30; i++) // Try 30 random spots per phase
            {
                float rx = Random.Range(-spawnZone.rect.width / 2, spawnZone.rect.width / 2);
                float ry = Random.Range(-spawnZone.rect.height / 2, spawnZone.rect.height / 2);
                finalPosition = new Vector2(rx, ry);
                foundValidPos = true;

                foreach (Transform child in spawnZone)
                {
                    if (child == newNote.transform) continue;

                    // Check distance using the CURRENT strictness
                    if (Vector2.Distance(child.GetComponent<RectTransform>().anchoredPosition, finalPosition) < currentTryDistance)
                    {
                        foundValidPos = false;
                        break;
                    }
                }

                if (foundValidPos) break; // Found a good spot, break the 30-try loop!
            }

            if (foundValidPos) break; // Found a good spot, break the Phase loop!

            // If we get here, it means the screen is too crowded for this distance.
            // Shrink the distance requirement by 25% and try again!
            currentTryDistance *= 0.75f;
            Debug.LogWarning("Spawn zone is crowded! Shrinking safe distance to: " + currentTryDistance);
        }

        // Apply the final best position it could find
        newNote.GetComponent<RectTransform>().anchoredPosition = finalPosition;
        newNote.transform.SetAsFirstSibling();
        newNote.GetComponent<KwakCircle>().Setup(targetTime, songSource, leadTime);
    }
}