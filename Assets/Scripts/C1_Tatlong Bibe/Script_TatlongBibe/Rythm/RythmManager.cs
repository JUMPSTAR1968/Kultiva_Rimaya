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

        // --- NEW: ANTI-OVERLAP RADAR ---
        Vector2 finalPosition = Vector2.zero;
        bool foundValidPos = false;
        int maxAttempts = 15; // Try 15 times to find a good empty spot

        for (int i = 0; i < maxAttempts; i++)
        {
            float rx = Random.Range(-spawnZone.rect.width / 2, spawnZone.rect.width / 2);
            float ry = Random.Range(-spawnZone.rect.height / 2, spawnZone.rect.height / 2);
            finalPosition = new Vector2(rx, ry);
            foundValidPos = true;

            // Look at every other circle currently on the screen
            foreach (Transform child in spawnZone)
            {
                // Don't measure distance against itself
                if (child == newNote.transform) continue;

                // If this spot is too close to another circle, reject it and try again!
                if (Vector2.Distance(child.GetComponent<RectTransform>().anchoredPosition, finalPosition) < minSpawnDistance)
                {
                    foundValidPos = false;
                    break;
                }
            }

            // If it survived the check without hitting anything, break the loop!
            if (foundValidPos) break;
        }

        // Apply the final safe position
        newNote.GetComponent<RectTransform>().anchoredPosition = finalPosition;

        // --- NEW: CLICK PRIORITY (Z-ORDER) FIX ---
        // This pushes the new circle to the very BACK layer. 
        // Now, older circles will naturally sit on top of it, so they get clicked first!
        newNote.transform.SetAsFirstSibling();

        // Finish setup
        newNote.GetComponent<KwakCircle>().Setup(targetTime, songSource, leadTime);
    }
}