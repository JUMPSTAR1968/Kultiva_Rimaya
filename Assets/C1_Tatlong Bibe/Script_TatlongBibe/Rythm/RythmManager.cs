using UnityEngine;

public class RythmManager : MonoBehaviour
{
    public AudioSource songSource;
    public GameObject kwakPrefab;
    public RectTransform spawnZone;
    public float[] kwakTimings;

    public float leadTime = 1.0f; // Notes appear 1 second before the beat
    private int nextNoteIndex = 0;

    void Start()
    {
        if (songSource != null) songSource.Play();
    }

    void Update()
    {
        if (songSource == null || nextNoteIndex >= kwakTimings.Length) return;

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
        newNote.transform.localScale = Vector3.one; // Fix scale issues

        // Random position inside the spawnZone
        float rx = Random.Range(-spawnZone.rect.width / 2, spawnZone.rect.width / 2);
        float ry = Random.Range(-spawnZone.rect.height / 2, spawnZone.rect.height / 2);
        newNote.GetComponent<RectTransform>().anchoredPosition = new Vector2(rx, ry);

        // Pass data to the circle
        newNote.GetComponent<KwakCircle>().Setup(targetTime, songSource, leadTime);
    }
}