using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    public float scrollSpeed = 5f;

    // The width of your background image
    private float imageWidth = 18.8f;
    private int backgroundCount = 3;

    // Buffer keeps it on screen longer
    private float viewBuffer = 10.0f;

    // NEW: How many seconds to wait before recycling starts
    public float startDelay = 2.0f;
    private float timer = 0f;

    void Update()
    {
        // 1. Always Move Left (Scrolling never stops)
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        // 2. Count up the timer
        timer += Time.deltaTime;

        // 3. Only check for Teleporting AFTER the delay
        if (timer > startDelay)
        {
            // If the object is WAY off to the left...
            if (transform.position.x < -(imageWidth + viewBuffer))
            {
                // Teleport to the far right
                Vector3 jumpOffset = Vector3.right * (imageWidth * backgroundCount);
                transform.position += jumpOffset;
            }
        }
    }
}