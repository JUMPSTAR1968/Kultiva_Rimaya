using UnityEngine;
using UnityEngine.InputSystem;

public class FatDuckMovement : MonoBehaviour
{
    private Vector3 _offset;
    private bool _isDragging = false;

    [Header("Movement Limits")]
    public float minY = -4.0f;
    public float maxY = 4.0f;

    [Header("Weight Settings")]
    [Tooltip("Higher = Heavier/Fatter. Try 0.5 for heavy, 1.0 for very fat.")]
    [Range(0.01f, 2.0f)]
    public float smoothTime = 0.5f;

    private float _yVelocity = 0.0f;

    void Update()
    {
        if (MTB_GameManager.Instance != null && MTB_GameManager.Instance.isGameOver) return;

        // Safety check to ensure a mouse or touchscreen exists
        if (Pointer.current == null) return;

        // --- NEW: Using Pointer instead of Mouse for Mobile Support! ---
        Vector2 screenPos = Pointer.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));

        if (Pointer.current.press.wasPressedThisFrame)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                _isDragging = true;
                _offset = transform.position - worldPos;
            }
        }

        if (_isDragging && Pointer.current.press.isPressed)
        {
            float targetY = worldPos.y + _offset.y;
            targetY = Mathf.Clamp(targetY, minY, maxY);

            float sluggishY = Mathf.SmoothDamp(
                transform.position.y,
                targetY,
                ref _yVelocity,
                smoothTime
            );

            // X is locked to transform.position.x so it ONLY moves up and down!
            transform.position = new Vector3(transform.position.x, sluggishY, transform.position.z);
        }

        if (Pointer.current.press.wasReleasedThisFrame)
        {
            _isDragging = false;
            _yVelocity = 0;
        }
    }
}