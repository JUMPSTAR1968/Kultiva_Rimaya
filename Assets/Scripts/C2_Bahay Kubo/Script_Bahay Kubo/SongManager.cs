using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BeatEvent
{
    public string vegetableName; // Just for your reference in Inspector
    public float timestamp;      // The exact second in the song
}

public class SongManager : MonoBehaviour
{
    public static SongManager Instance; // This allows the Spawner to find this script

    [Header("Song Data")]
    public List<BeatEvent> beatmap = new List<BeatEvent>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
