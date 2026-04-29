using UnityEngine;

public class PhysicalObstacle : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public float deadZone = -15f;

    void Update()
    {
        // NEW: Get the multiplier (defaults to 1 if Game Manager is missing)
        float currentMultiplier = (MTB_GameManager.Instance != null) ? MTB_GameManager.Instance.globalSpeedMultiplier : 1f;

        // Move left at a constant speed multiplied by the global speed increase!
        transform.Translate(Vector2.left * (scrollSpeed * currentMultiplier) * Time.deltaTime);

        // Destroy when off-screen to save memory
        if (transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }
    }
}