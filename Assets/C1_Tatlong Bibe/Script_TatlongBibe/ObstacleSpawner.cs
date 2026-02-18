using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float spawnRate = 2f;      // How often to spawn (seconds)
    public float minY = -2.6f;         // Bottom of your "limited area"
    public float maxY = 1.5f;          // Top of your "limited area"
    public float spawnX = 10f;       // Fixed X position (off-screen right)

    private float timer = 0f;

    void Update()
    {
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

    void SpawnObstacle()
    {
        // Calculate a random height within your limited area
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(spawnX, randomY, 0);

        // Create the obstacle
        Instantiate(obstaclePrefab, spawnPosition, transform.rotation);
    }
}