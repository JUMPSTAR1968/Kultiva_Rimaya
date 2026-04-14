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
        // THIS IS THE BRAIN: It reads the difficulty and sets the spawn rate!
        switch (GameSettings.CurrentDifficulty)
        {
            case Difficulty.Easy:
                spawnRate = 3.0f; // Spawns every 2 seconds (Very slow, lots of breathing room)
                Debug.Log("Easy Mode: Boulders spawning every 2 seconds.");
                break;

            case Difficulty.Medium:
                spawnRate = 2.0f; // Spawns every 1 second (Standard challenge)
                Debug.Log("Medium Mode: Boulders spawning every 1 second.");
                break;

            case Difficulty.Hard:
                spawnRate = 2.0f; // Spawns every 0.45 seconds (Super fast, chaotic!)
                Debug.Log("Hard Mode: Boulders spawning FAST!");
                break;
        }
    }

    void Update()
    {
        // Stop the timer if the game is over so boulders stop appearing
        if (MTB_GameManager.Instance != null && MTB_GameManager.Instance.isGameOver) return;

        // Count up the timer
        timer += Time.deltaTime;

        // Check if it's time to spawn a boulder
        if (timer >= spawnRate)
        {
            SpawnObstacle();
            timer = 0f; // Reset the loop
        }
    }

    void SpawnObstacle()
    {
        // Safety check
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;

        // Pick a random height within the river
        float randomY = Random.Range(riverBottomY, riverTopY);
        Vector3 spawnPos = new Vector3(spawnX, randomY, 0);

        // Pick a random prefab (Boulder, branch, etc.)
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);

        // Spawn it!
        Instantiate(obstaclePrefabs[randomIndex], spawnPos, Quaternion.identity);
    }
}