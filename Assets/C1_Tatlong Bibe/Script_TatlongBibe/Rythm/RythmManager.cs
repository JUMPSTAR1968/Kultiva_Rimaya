using UnityEngine;

public class RythmManager : MonoBehaviour
{
    public AudioSource songSource;
    public GameObject kwakPrefab;
    public RectTransform spawnZone;
    public float[] kwakTimings;
    public float preemptionTime = 1.0f;

    private int nextNoteIndex = 0;

    void Start()
    {
        if (songSource != null) songSource.Play();
    }

    void Update()
    {
        if (nextNoteIndex < kwakTimings.Length)
        {
            if (songSource.time >= kwakTimings[nextNoteIndex] - preemptionTime)
            {
                SpawnKwak(kwakTimings[nextNoteIndex]);
                nextNoteIndex++;
            }
        }
    }

    void SpawnKwak(float targetTime)
    {
        // 1. Create the note as a child of the NoteContainer
        GameObject newNote = Instantiate(kwakPrefab, spawnZone);

        // 2. YOUR LOGIC START: Calculate random position based on the SpawnZone size
        float rx = Random.Range(-spawnZone.rect.width / 2, spawnZone.rect.width / 2);
        float ry = Random.Range(-spawnZone.rect.height / 2, spawnZone.rect.height / 2);

        // 3. YOUR LOGIC END: Apply that position to the new note
        newNote.GetComponent<RectTransform>().anchoredPosition = new Vector2(rx, ry);

        // 4. Connect to the KwakCircle script
        KwakCircle circleScript = newNote.GetComponent<KwakCircle>();
        if (circleScript != null)
        {
            circleScript.Setup(targetTime, songSource);
        }
    }
}