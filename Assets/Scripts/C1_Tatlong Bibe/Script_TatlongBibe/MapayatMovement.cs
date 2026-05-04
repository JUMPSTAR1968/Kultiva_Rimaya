using UnityEngine;
using UnityEngine.InputSystem;

public class MapayatMovement : MonoBehaviour
{
    private Vector3 _offset;
    private bool _isDragging = false;

    [Header("Movement Limits")]
    public float minY = -4.0f;
    public float maxY = 4.0f;

    [Header("Ice Physics")]
    public float maxSpeed = 20f;
    public float friction = 8f;
    public float followSharpness = 0.2f;

    private float _velocity;

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

        if (Pointer.current.press.wasReleasedThisFrame)
        {
            _isDragging = false;
        }

        if (_isDragging && Pointer.current.press.isPressed)
        {
            float targetY = worldPos.y + _offset.y;
            targetY = Mathf.Clamp(targetY, minY, maxY);

            float idealVelocity = (targetY - transform.position.y) / Time.deltaTime;
            _velocity = Mathf.Clamp(idealVelocity, -maxSpeed, maxSpeed);
        }
        else
        {
            _velocity = Mathf.Lerp(_velocity, 0, Time.deltaTime * friction);
        }

        float newY = transform.position.y + (_velocity * Time.deltaTime);

        if (newY >= maxY || newY <= minY)
        {
            _velocity = 0;
        }

        newY = Mathf.Clamp(newY, minY, maxY);

        // X is locked to transform.position.x so it ONLY moves up and down!
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}