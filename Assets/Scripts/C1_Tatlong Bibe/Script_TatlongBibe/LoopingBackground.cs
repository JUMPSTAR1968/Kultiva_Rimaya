using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    public float scrollSpeed = 5f;

    [Header("Difficulty Settings")]
    // NEW: Check this box in the Inspector for Grass and Water, uncheck it for Trees!
    public bool scalesWithDifficulty = true;

    // The width of your background image
    private float imageWidth = 18.8f;
    private int backgroundCount = 3;

    // Buffer keeps it on screen longer
    private float viewBuffer = 10.0f;

    public float startDelay = 2.0f;
    private float timer = 0f;

    void Update()
    {
        // NEW: Only apply the speed multiplier if the box is checked!
        float currentMultiplier = 1f;
        if (scalesWithDifficulty && MTB_GameManager.Instance != null)
        {
            currentMultiplier = MTB_GameManager.Instance.globalSpeedMultiplier;
        }

        // 1. Always Move Left using the new multiplier
        transform.Translate(Vector3.left * (scrollSpeed * currentMultiplier) * Time.deltaTime);

        // 2. Count up the timer
        timer += Time.deltaTime;

        // 3. Only check for Teleporting AFTER the delay
        if (timer > startDelay)
        {
            if (transform.position.x < -(imageWidth + viewBuffer))
            {
                Vector3 jumpOffset = Vector3.right * (imageWidth * backgroundCount);
                transform.position += jumpOffset;
            }
        }
    }
}