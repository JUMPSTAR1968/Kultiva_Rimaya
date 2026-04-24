using UnityEngine;
using System.Collections.Generic;

public class BeatRecorder : MonoBehaviour
{
    [Header("Setup")]
    public AudioSource songSource;
    public KeyCode tapKey = KeyCode.Space; // The key you will tap to the beat
    public KeyCode printKey = KeyCode.P;   // Press this when you are done to get the list

    [Header("Recorded Data")]
    // This list will automatically fill up as you tap!
    public List<float> recordedTimings = new List<float>();

    void Update()
    {
        // 1. Check if the song is playing and we hit our tap key
        if (Input.GetKeyDown(tapKey) && songSource != null && songSource.isPlaying)
        {
            // Record the exact current time of the song
            float currentTime = songSource.time;
            recordedTimings.Add(currentTime);

            Debug.Log($"Beat mapped at: {currentTime}");
        }

        // 2. When the song is over, press P to print out the array format
        if (Input.GetKeyDown(printKey))
        {
            PrintTimingsToConsole();
        }
    }

    private void PrintTimingsToConsole()
    {
        if (recordedTimings.Count == 0)
        {
            Debug.LogWarning("No beats recorded yet!");
            return;
        }

        // Format it nicely so you can copy/paste it directly into your RythmManager code if needed
        string output = "Copy these timings:\n";
        foreach (float time in recordedTimings)
        {
            // Rounds it to 2 decimal places to keep it clean (e.g., 1.45f)
            output += Mathf.Round(time * 100f) / 100f + "f, ";
        }

        Debug.Log(output);
    }
}