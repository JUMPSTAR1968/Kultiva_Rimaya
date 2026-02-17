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
    [Tooltip("How fast the duck can slide. Try 15 or 20.")]
    public float maxSpeed = 20f;
    [Tooltip("Higher = Stops faster. Try 5 or 10 for Minecraft feel.")]
    public float friction = 8f;
    [Tooltip("How snappy the initial grab is.")]
    public float followSharpness = 0.2f;

    private float _velocity;

    void Update()
    {
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

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _isDragging = false;
        }

        if (_isDragging && Mouse.current.leftButton.isPressed)
        {
            float targetY = mouseWorldPos.y + _offset.y;
            targetY = Mathf.Clamp(targetY, minY, maxY);

            // Instead of multiplying, we calculate the movement needed
            // and cap the velocity so it doesn't "rocket" away.
            float idealVelocity = (targetY - transform.position.y) / Time.deltaTime;
            _velocity = Mathf.Clamp(idealVelocity, -maxSpeed, maxSpeed);
        }
        else
        {
            // This is the Minecraft "Ice" slowing down part
            // It reduces velocity toward zero over time
            _velocity = Mathf.Lerp(_velocity, 0, Time.deltaTime * friction);
        }

        // Apply movement
        float newY = transform.position.y + (_velocity * Time.deltaTime);

        // Stop if it hits the borders
        if (newY >= maxY || newY <= minY)
        {
            _velocity = 0;
        }

        newY = Mathf.Clamp(newY, minY, maxY);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}