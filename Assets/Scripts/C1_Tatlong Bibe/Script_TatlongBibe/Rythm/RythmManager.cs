using UnityEngine;

public class RythmManager : MonoBehaviour
{
    public static RythmManager Instance;

    [Header("Setup")]
    public AudioSource songSource;
    public GameObject kwakPrefab;
    public RectTransform spawnZone;

    [Header("Rhythm Data")]
    public float[] kwakTimings; // Every individual "Kwak" timestamp
    public float leadTime = 1.0f; // Seconds before the beat the circle appears

    private int nextNoteIndex = 0;
    private float previousSongTime = 0f;

    void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        if (songSource != null) songSource.Play();
    }

    void Update()
    {
        if (songSource == null || nextNoteIndex >= kwakTimings.Length) return;

        float currentSongTime = songSource.time;

        // LOOP DETECTION: Resets the sequence when the song restarts
        if (currentSongTime < previousSongTime)
        {
            nextNoteIndex = 0;
        }
        previousSongTime = currentSongTime;

        // SPAWN CLICKABLES: Check if it's time for the next "Kwak"
        if (nextNoteIndex < kwakTimings.Length && currentSongTime >= kwakTimings[nextNoteIndex] - leadTime)
        // Spawn logic: Check if song time has reached (Target - LeadTime)
        if (songSource.time >= kwakTimings[nextNoteIndex] - leadTime)
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

        // Pass the timing data to the circle script
        newNote.GetComponent<KwakCircle>().Setup(targetTime, songSource, leadTime);
    }
}