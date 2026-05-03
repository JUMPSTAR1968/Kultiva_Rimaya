using UnityEngine;

public class RythmManager : MonoBehaviour
{
    public static RythmManager Instance;

    [Header("Setup")]
    public AudioSource songSource;
    public GameObject kwakPrefab;
    public RectTransform spawnZone;

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
        // 1. Only stop if the song source is entirely missing
        if (songSource == null) return;

        // 2. ALWAYS update the time so the Inspector keeps ticking
        currentSongTime = songSource.time;

        // 3. ALWAYS check for loops so the notes can reset when the song restarts!
        // LOOP DETECTION: Resets the sequence when the song restarts
        // LOOP DETECTION: Resets the sequence when the song restarts
        if (currentSongTime < previousSongTime)
        {
            nextNoteIndex = 0;

            // 1. Tell the Game Manager to speed up movement
            if (MTB_GameManager.Instance != null)
            {
                MTB_GameManager.Instance.IncreaseLoopSpeed();
            }

            // 2. THIS IS FIXED: It now talks to ObstacleSpawn without the "er"
            if (ObstacleSpawn.Instance != null)
            {
                ObstacleSpawn.Instance.DecreaseSpawnRate();
            }
        }
        previousSongTime = currentSongTime;

        // 4. NOW we check if we've run out of notes. If we have, stop here before spawning.
        if (nextNoteIndex >= kwakTimings.Length) return;

        // 5. Spawn logic
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

        float rx = Random.Range(-spawnZone.rect.width / 2, spawnZone.rect.width / 2);
        float ry = Random.Range(-spawnZone.rect.height / 2, spawnZone.rect.height / 2);
        newNote.GetComponent<RectTransform>().anchoredPosition = new Vector2(rx, ry);

        newNote.GetComponent<KwakCircle>().Setup(targetTime, songSource, leadTime);
    }
}