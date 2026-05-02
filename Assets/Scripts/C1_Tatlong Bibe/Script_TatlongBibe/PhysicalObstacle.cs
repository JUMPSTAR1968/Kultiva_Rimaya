using UnityEngine;

public class PhysicalObstacle : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public float deadZone = -15f;

    [Header("Scoring")]
    // NEW: Type the X position of your duck here in the Inspector!
    public float duckXPosition = -6f;
    private bool hasPassedDuck = false; // Prevents giving infinite points

    void Update()
    {
        float currentMultiplier = (MTB_GameManager.Instance != null) ? MTB_GameManager.Instance.globalSpeedMultiplier : 1f;

        // Move left at a constant speed
        transform.Translate(Vector2.left * (scrollSpeed * currentMultiplier) * Time.deltaTime);

        // --- NEW SCORE LOGIC ---
        // If we haven't given points yet, AND we just passed behind the duck...
        if (!hasPassedDuck && transform.position.x < duckXPosition)
        {
            hasPassedDuck = true; // Lock it so it never gives points again!

            if (MTB_GameManager.Instance != null)
            {
                MTB_GameManager.Instance.PassedBoulder();
            }
        }

        // Destroy when off-screen to save memory
        if (transform.position.x < deadZone)
        {
            // (We removed the score trigger from here, so it only destroys itself now)
            Destroy(gameObject);
        }
    }
}