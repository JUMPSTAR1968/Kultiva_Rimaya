using UnityEngine;

public class ObstacleSpawn : MonoBehaviour
{
    // 1. ADD THE INSTANCE SO RYTHMMANAGER CAN FIND IT!
    public static ObstacleSpawn Instance;

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
    public float spawnRate; // Removed default number so difficulty sets it
    private float originalSpawnRate;

    [Header("Difficulty Scaling (Hard Mode)")]
    public float decreasePerLoop = 0.2f;
    public float absoluteFastestSpawn = 0.3f;

    public float minY = -2.6f;
    public float maxY = 1.5f;
    public float spawnX = 10f;

    private float timer = 0f;

    void Awake()
    {
        // Set the instance!
        Instance = this;
    }

    void Start()
    {
        // 1. Check the difficulty the moment the scene loads
        switch (GameSettings.CurrentDifficulty)
        {
            case Difficulty.Easy:
                spawnRate = 5.0f;
                break;
            case Difficulty.Medium:
                spawnRate = 5.0f;
                break;
            case Difficulty.Hard:
                spawnRate = 5.0f;
                break;
            default:
                spawnRate = 1.0f;
                break;
        }

        // 2. Save this specific rate so we can reset it later
        originalSpawnRate = spawnRate;
    }

    void Update()
    {
        if (this == null || songSource == null) return;

        // Complete silence during kwak zones
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

    // NEW: Function to lower the spawn rate!
    public void DecreaseSpawnRate()
    {
        // Only increase the chaos if they are playing on Hard Mode!
        if (GameSettings.CurrentDifficulty == Difficulty.Hard)
        {
            spawnRate -= decreasePerLoop;

            // Don't let it go crazy fast
            if (spawnRate < absoluteFastestSpawn)
            {
                spawnRate = absoluteFastestSpawn;
            }

            Debug.Log("Song Looped! New spawn rate is: " + spawnRate);
        }
    }

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