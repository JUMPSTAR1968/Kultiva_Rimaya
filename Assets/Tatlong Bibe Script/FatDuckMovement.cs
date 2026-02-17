using UnityEngine;
using UnityEngine.InputSystem;

public class FatDuckMovement : MonoBehaviour
{
    private Vector3 _offset;
    private bool _isDragging = false;

    [Header("Movement Limits")]
    public float minY = -4.0f; // Adjust these in the Inspector
    public float maxY = 4.0f;

    void Update()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        // The 10f here is the distance from the camera to the duck
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
            // 1. Calculate the intended new Y position
            float newY = mouseWorldPos.y + _offset.y;

            // 2. Clamp the value so it stays between min and max
            newY = Mathf.Clamp(newY, minY, maxY);

            // 3. Apply the clamped position
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _isDragging = false;
        }
    }
}