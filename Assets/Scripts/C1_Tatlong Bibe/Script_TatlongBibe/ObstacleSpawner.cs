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
    // THIS IS THE CRUCIAL LINE! It lets RythmManager find it.
    public static ObstacleSpawner Instance;

    [Header("Rhythm Zones (NO Spawning Here)")]
    public RhythmZone[] silenceZones;
    public float gracePeriod = 1.0f;

    [Header("Spawner Settings")]
    public GameObject obstaclePrefab;
    public AudioSource songSource;
    public float spawnRate;
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
        // THIS IS CRUCIAL! Without this, the RythmManager thinks the spawner doesn't exist.
        Instance = this;
    }

    void Start()
    {
        // 1. Check the difficulty the moment the scene loads
        switch (GameSettings.CurrentDifficulty)
        {
            case Difficulty.Easy:
                spawnRate = 2.0f;
                break;
            case Difficulty.Medium:
                spawnRate = 1.0f;
                break;
            case Difficulty.Hard:
                spawnRate = 0.6f;
                break;
            default: // Failsafe just in case difficulty isn't set yet
                spawnRate = 1.0f;
                break;
        }

        // 2. Save this specific rate so we can reset it later
        originalSpawnRate = spawnRate;
    }

    void Update()
    {
        if (songSource == null) return;

        if (IsInSilenceZone())
        {
            timer = 0f;
            return;
        }

        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

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

            // We should see this in the console alongside your Game Manager log!
            Debug.Log("Song Looped! New spawn rate is: " + spawnRate);
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