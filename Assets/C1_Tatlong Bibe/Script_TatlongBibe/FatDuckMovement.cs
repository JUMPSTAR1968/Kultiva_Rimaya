using UnityEngine;
using UnityEngine.InputSystem;

public class FatDuckMovement : MonoBehaviour
{
    private Vector3 _offset;
    private bool _isDragging = false;

    [Header("Movement Limits")]
    public float minY = -4.0f;
    public float maxY = 4.0f;

    // --- NEW SECTIONS ADDED BELOW ---
    [Header("Weight Settings")]
    [Tooltip("Higher = Heavier/Fatter. Try 0.5 for heavy, 1.0 for very fat.")]
    [Range(0.01f, 2.0f)]
    public float smoothTime = 0.5f;

    private float _yVelocity = 0.0f; // Required to track speed for SmoothDamp
    // --------------------------------

    void Update()
    {
        // If the game is stopped, stop all movement calculations immediately
        if (MTB_GameManager.Instance != null && MTB_GameManager.Instance.isGameOver)
        {
            return;
        }

        // ... your existing movement code here ...


        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f));

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                _isDragging = true;
                _offset = transform.position - mouseWorldPos;
            }
        }

        if (_isDragging && Mouse.current.leftButton.isPressed)
        {
            // 1. Calculate the intended target position
            float targetY = mouseWorldPos.y + _offset.y;

            // 2. Clamp the value so it stays between min and max
            targetY = Mathf.Clamp(targetY, minY, maxY);

            // 3. APPLY HEAVINESS:
            // This replaces the old instant movement. 
            // It makes the duck "trail" behind the mouse vertically.
            float sluggishY = Mathf.SmoothDamp(
                transform.position.y,
                targetY,
                ref _yVelocity,
                smoothTime
            );

            // 4. Apply the position
            transform.position = new Vector3(transform.position.x, sluggishY, transform.position.z);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _isDragging = false;
            _yVelocity = 0; // Stop the movement instantly when released
        }
    }
}