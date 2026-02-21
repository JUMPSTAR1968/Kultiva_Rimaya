using UnityEngine;

public class PhysicalObstacle : MonoBehaviour
{
    public float scrollSpeed = 5f; // Match this to your LoopingBackground speed
    public float deadZone = -15f;

    void Update()
    {
        // Move left at a constant speed
        transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime);

        // Destroy when off-screen to save memory
        if (transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }
    }
}