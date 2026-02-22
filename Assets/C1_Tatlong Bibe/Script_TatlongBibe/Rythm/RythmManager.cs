using UnityEngine;
using System.Collections.Generic;

public class RythmManager : MonoBehaviour
{
    public AudioSource songSource;
    public GameObject kwakPrefab;
    public RectTransform spawnZone;
    public float[] kwakTimings;

    public float leadTime = 1.0f; // How many seconds before the beat the circle appears
    private int nextNoteIndex = 0;

    void Start()
    {
        if (songSource != null) songSource.Play();
    }

    void Update()
    {
        if (songSource == null || nextNoteIndex >= kwakTimings.Length) return;

        // Check if it's time to spawn the next note (Target Time minus Lead Time)
        if (songSource.time >= kwakTimings[nextNoteIndex] - leadTime)
        {
            SpawnKwak(kwakTimings[nextNoteIndex]);
            nextNoteIndex++; // Move to the next timestamp in the array
        }
    }

    void SpawnKwak(float targetTime)
    {
        GameObject newNote = Instantiate(kwakPrefab, spawnZone);

        // Ensure the scale is correct (UI instantiation often messes this up)
        newNote.transform.localScale = Vector3.one;

        // Moves the circle to a random spot inside your NoteContainer
        float rx = Random.Range(-spawnZone.rect.width / 2, spawnZone.rect.width / 2);
        float ry = Random.Range(-spawnZone.rect.height / 2, spawnZone.rect.height / 2);

        newNote.GetComponent<RectTransform>().anchoredPosition = new Vector2(rx, ry);
        newNote.GetComponent<KwakCircle>().Setup(targetTime, songSource, leadTime);

        Debug.Log("Spawned Kwak for time: " + targetTime);
    }
}