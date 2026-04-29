using UnityEngine;

[System.Serializable]
public class RhythmZone
{
    public string sectionName;
    public float startTime;
    public float endTime;
}

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Rhythm Zones (NO Spawning Here)")]
    public RhythmZone[] silenceZones;
    public float gracePeriod = 1.0f;  // Extra buffer before/after zones

    [Header("Spawner Settings")]
    public GameObject obstaclePrefab;
    public AudioSource songSource;    // Assign your song AudioSource
    public float spawnRate = 2f;
    public float minY = -2.6f;
    public float maxY = 1.5f;
    public float spawnX = 10f;

    private float timer = 0f;

    void Update()
    {
        // Safety check
        if (songSource == null) return;

        // **NEW**: Stop spawning during silence zones
        if (IsInSilenceZone())
        {
            timer = 0f; // Reset so it resumes immediately after
            return;
        }

        // Normal spawning logic
        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            SpawnObstacle();
            timer = 0;
        }
    }

    bool IsInSilenceZone()
    {
        float currentTime = songSource.time;

        foreach (RhythmZone zone in silenceZones)
        {
            if (currentTime >= (zone.startTime - gracePeriod) &&
                currentTime <= (zone.endTime + gracePeriod))
            {
                return true;
            }
        }
        return false;
    }

    void SpawnObstacle()
    {
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(spawnX, randomY, 0);
        Instantiate(obstaclePrefab, spawnPosition, transform.rotation);
    }
}