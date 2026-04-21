using UnityEngine;

public class ObstacleSpawn : MonoBehaviour
{
    [Header("Obstacle Setup")]
    public GameObject[] obstaclePrefabs;
    public float spawnRate = 1.0f;
    public float spawnX = 12f;

    [Header("River Boundaries")]
    public float riverTopY = 0.5f;
    public float riverBottomY = -1.5f;

    private float timer = 0f;

    void Start()
    {
        // Difficulty code remains untouched as requested
        switch (GameSettings.CurrentDifficulty)
        {
            case Difficulty.Easy:
<<<<<<< Updated upstream
                spawnRate = 3.0f; 
=======
                spawnRate = 3.0f;
>>>>>>> Stashed changes
                Debug.Log("Easy Mode: Boulders spawning every 2 seconds.");
                break;
            case Difficulty.Medium:
<<<<<<< Updated upstream
                spawnRate = 2.0f; 
=======
                spawnRate = 2.0f;
>>>>>>> Stashed changes
                Debug.Log("Medium Mode: Boulders spawning every 1 second.");
                break;
            case Difficulty.Hard:
<<<<<<< Updated upstream
                spawnRate = 2.0f; 
=======
                spawnRate = 2.0f;
>>>>>>> Stashed changes
                Debug.Log("Hard Mode: Boulders spawning FAST!");
                break;
        }
    }

    void Update()
    {
        if (MTB_GameManager.Instance != null && MTB_GameManager.Instance.isGameOver) return;

        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;

        // 1. Pick a lane index: 0 (Bottom), 1 (Middle), or 2 (Top)
        int lane = Random.Range(0, 3);
        float spawnY = 0f;

        // 2. Calculate the specific Y based on the lane
        if (lane == 0) // Bottom
        {
            spawnY = riverBottomY;
        }
        else if (lane == 1) // Middle
        {
            // The midpoint between Top and Bottom
            spawnY = (riverTopY + riverBottomY) / 2f;
        }
        else // Top
        {
            spawnY = riverTopY;
        }

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);

        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        Instantiate(obstaclePrefabs[randomIndex], spawnPos, Quaternion.identity);
    }
}