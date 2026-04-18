using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BeatEvent
{
    public string vegetableName; // Just for your reference in Inspector
    public float timestamp;      // The exact second in the song
}

// nigga

public class SongManager : MonoBehaviour
{
    public static SongManager Instance; // This allows the Spawner to find this script

    private int _scoreCount = 0;

    public int ScoreCount { get { return _scoreCount; } set { _scoreCount = value; } }

    public void ResetScore()
    {
        ScoreCount = 0;
    }


    [Header("Song Data")]
    public List<BeatEvent> beatmap = new List<BeatEvent>();

    void Awake()
    {
        Debug.Log("Song Manager Instantiated");

        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
