using UnityEngine;
using UnityEngine.InputSystem; // ✅ Required for new Input System

public class CameraZoom : MonoBehaviour
{
    private float zoom;
    private float zoomMultiplier = 4f;
    private float minZoom = 2f;
    private float maxZoom = 8f;
    private float velocity = 0f;
    private float smoothTime = 0.25f;

    [SerializeField] private Camera cam;

    private void Start()
    {
        zoom = cam.orthographicSize;
    }

    private void Update()
    {
        if (Mouse.current != null)
        {
            // Read mouse scroll value (Y axis)
            float scroll = Mouse.current.scroll.ReadValue().y;

            // Adjust zoom
            zoom -= scroll * zoomMultiplier * Time.deltaTime;

            // Clamp zoom range
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

            // Smooth transition
            cam.orthographicSize = Mathf.SmoothDamp(
                cam.orthographicSize,
                zoom,
                ref velocity,
                smoothTime
            );
        }
    }
}
