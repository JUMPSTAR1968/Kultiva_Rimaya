using UnityEngine;

public class ObstacleSpawn : MonoBehaviour
{
    [System.Serializable]
    public struct RhythmZone
    {
        public string sectionName; // Just to help you organize in Inspector
        public float startTime;    // Start of "Kwak Kwak"
        public float endTime;      // End of "Kwak Kwak"
    }

    [Header("Rhythm Zones (Silence Boulders Here)")]
    public RhythmZone[] kwakZones;
    public float gracePeriod = 2.0f; // Seconds to clear boulders before/after

    [Header("Obstacle Settings")]
    public GameObject obstaclePrefab;
    public AudioSource songSource;
    public float spawnRate = 2f;
    public float minY = -2.6f;
    public float maxY = 1.5f;
    public float spawnX = 10f;

    private float timer = 0f;

    void Update()
    {
        if (this == null || songSource == null) return;

        // **FIXED**: Complete silence during kwak zones
        if (IsInsideRhythmZone())
        {
            timer = 0f;  // Reset → resumes immediately after
            return;
        }

        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if (obstaclePrefab != null)
            {
                SpawnObstacle();
                timer = 0;
            }
        }
    }

    // This is the "Self-Sensing" logic
    bool IsInsideRhythmZone()
    {
        // Get the current time of the music
        float currentTime = songSource.time;

        foreach (RhythmZone zone in kwakZones)
        {
            // Check if current time is within the zone (with grace period)
            if (currentTime >= (zone.startTime - gracePeriod) &&
                currentTime <= (zone.endTime + gracePeriod))
            {
                return true; // Silence mode ACTIVE
            }
        }
        return false; // Normal mode ACTIVE
    }

    void SpawnObstacle()
    {
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(spawnX, randomY, 0);

        // Create the boulder
        Instantiate(obstaclePrefab, spawnPosition, transform.rotation);
    }
}