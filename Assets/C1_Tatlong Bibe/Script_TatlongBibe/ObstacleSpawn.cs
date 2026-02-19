using UnityEngine;

public class ObstacleSpawn : MonoBehaviour
{
    [Header("Obstacle Setup")]
    public GameObject[] obstaclePrefabs; // Drag your Log, Boulder, and Branch prefabs here
    public float spawnRate = 0.5f;       // Seconds between each spawn
    public float spawnX = 12f;          // Position to the right of the screen

    [Header("River Boundaries")]
    // Adjust these based on where your blue water sprite is located
    public float riverTopY = 0.5f;
    public float riverBottomY = -1.5f;

    private float timer = 0f;

    void Update()
    {
        // 1. The Loop: Increment timer every frame
        timer += Time.deltaTime;

        // 2. Check if it's time to spawn
        if (timer >= spawnRate)
        {
            SpawnObstacle();
            timer = 0f; // Reset the loop
        }
        // If the game is over, don't run the timer or spawn anything
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        timer += Time.deltaTime;
        // ... rest of your code ...
    }

    void SpawnObstacle()
    {
        // Safety check: Make sure you've assigned prefabs in the Inspector
        if (obstaclePrefabs.Length == 0) return;

        // 3. Random Location: Pick a random height within the river
        float randomY = Random.Range(riverBottomY, riverTopY);
        Vector3 spawnPos = new Vector3(spawnX, randomY, 0);

        // 4. Random Selection: Pick a random prefab (Log, Boulder, or Branch)
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);

        Instantiate(obstaclePrefabs[randomIndex], spawnPos, Quaternion.identity);
    }
}