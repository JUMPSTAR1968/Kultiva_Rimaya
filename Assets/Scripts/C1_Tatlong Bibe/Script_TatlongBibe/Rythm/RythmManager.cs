using UnityEngine;

public class RythmManager : MonoBehaviour
{
    // --- RESTORED: The click script needs this to find the manager! ---
    public static RythmManager Instance;

    public AudioSource songSource;
    public GameObject kwakPrefab;
    public RectTransform spawnZone;
    public float[] kwakTimings;

    public float leadTime = 1.0f; // Notes appear 1 second before the beat
    private int nextNoteIndex = 0;

    private float previousSongTime = 0f;

    // --- RESTORED: The click script needs this to know what the current time is! ---
    public float currentSongTime;

    void Awake()
    {
        // Set the instance when the game starts
        Instance = this;
    }

    void Start()
    {
        if (songSource != null)
        {
            songSource.loop = true; // Make sure the AudioSource is set to loop!
            songSource.Play();
        }
    }

    void Update()
    {
        if (songSource == null) return;

        // Keep our public variable updated for the dots to read
        currentSongTime = songSource.time;

        // --- Loop Detection ---
        if (currentSongTime < previousSongTime)
        {
            nextNoteIndex = 0; // Reset our timing array back to the very first dot
        }

        // Save the time so we can check it again next frame
        previousSongTime = currentSongTime;

        // If we've spawned all the notes for this loop, just wait
        if (nextNoteIndex >= kwakTimings.Length) return;

        // Spawn logic: Check if song time has reached (Target - LeadTime)
        if (currentSongTime >= kwakTimings[nextNoteIndex] - leadTime)
        {
            SpawnKwak(kwakTimings[nextNoteIndex]);
            nextNoteIndex++;
        }
    }

    void SpawnKwak(float targetTime)
    {
        GameObject newNote = Instantiate(kwakPrefab, spawnZone);
        newNote.transform.localScale = Vector3.one; // Fix scale issues

        // Random position inside the spawnZone
        float rx = Random.Range(-spawnZone.rect.width / 2, spawnZone.rect.width / 2);
        float ry = Random.Range(-spawnZone.rect.height / 2, spawnZone.rect.height / 2);
        newNote.GetComponent<RectTransform>().anchoredPosition = new Vector2(rx, ry);

        // Pass data to the circle
        newNote.GetComponent<KwakCircle>().Setup(targetTime, songSource, leadTime);
    }
}