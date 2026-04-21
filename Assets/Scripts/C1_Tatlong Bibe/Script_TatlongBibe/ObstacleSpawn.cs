using UnityEngine;

public class ObstacleSpawn : MonoBehaviour
{
    [Header("Obstacle Setup")]
    public GameObject[] obstaclePrefabs; // Drag your Log, Boulder, and Branch prefabs here
    public float spawnRate = 1.0f;       // Seconds between each spawn
    public float spawnX = 12f;           // Position to the right of the screen

    [Header("River Boundaries")]
    public float riverTopY = 0.5f;
    public float riverBottomY = -1.5f;

    private float timer = 0f;

    void Start()
    {
        // Difficulty Logic (Remains Untouched)
        switch (GameSettings.CurrentDifficulty)
        {
            case Difficulty.Easy:
                spawnRate = 3.0f;
                Debug.Log("Easy Mode: Boulders spawning every 3 seconds.");
                break;

            case Difficulty.Medium:
                spawnRate = 2.0f;
                Debug.Log("Medium Mode: Boulders spawning every 2 seconds.");
                break;

            case Difficulty.Hard:
                spawnRate = 2.0f;
                Debug.Log("Hard Mode: Boulders spawning FAST!");
                break;
        }
    }

    void Update()
    {
        // Stop the timer if the game is over
        if (MTB_GameManager.Instance != null && MTB_GameManager.Instance.isGameOver) return;

        // Count up the timer
        timer += Time.deltaTime;

        // Check if it's time to spawn
        if (timer >= spawnRate)
        {
            SpawnObstacle();
            timer = 0f; // Reset the loop
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;

        // 1. Pick a lane index (0, 1, or 2)
        int lane = Random.Range(0, 3);
        float targetY = 0f;

        // 2. Set the "Center" height for the chosen lane
        if (lane == 0) // Bottom
        {
            targetY = riverBottomY;
        }
        else if (lane == 1) // Middle (shifted up slightly as requested)
        {
            targetY = ((riverTopY + riverBottomY) / 2f) + 0.25f;
        }
        else // Top
        {
            targetY = riverTopY;
        }

        // 3. Add random variance so they spawn "within" the section rather than on a line
        // Change 0.3f to a higher number for more spread, or lower for tighter lanes
        float finalY = targetY + Random.Range(-0.3f, 0.3f);

        Vector3 spawnPos = new Vector3(spawnX, finalY, 0);
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        Instantiate(obstaclePrefabs[randomIndex], spawnPos, Quaternion.identity);
    }
}