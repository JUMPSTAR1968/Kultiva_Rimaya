using UnityEngine;
using UnityEngine.InputSystem; // ✅ Required for new Input System
using System.Collections;

public class CameraZoom : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCamera;
    public Transform playerTransform;
    
    [Header("Zoom Values")]
    public float startSize = 15f;     // Bird's eye view size
    public float targetSize = 5f;      // Gameplay zoom size
    public float waitDuration = 5f;    // Time before zoom starts
    public float zoomSpeed = 2f;       // How fast it zooms in

    private bool isZooming = false;

    void Start()
    {
        // 1. Set the initial zoomed-out state
        if (mainCamera == null) mainCamera = Camera.main;
        mainCamera.orthographicSize = startSize;

        // 2. Start the countdown
        StartCoroutine(StartZoomSequence());
    }

    IEnumerator StartZoomSequence()
    {
        // Wait for the specified time
        yield return new WaitForSeconds(waitDuration);
        isZooming = true;
    }

    void Update()
    {
        if (isZooming)
        {
            // 3. Smoothly interpolate the Camera Size
            mainCamera.orthographicSize = Mathf.MoveTowards(
                mainCamera.orthographicSize, 
                targetSize, 
                zoomSpeed * Time.deltaTime
            );

            // 4. Smoothly move the Camera Position to the player
            Vector3 targetPos = new Vector3(playerTransform.position.x, playerTransform.position.y, -10f);
            transform.position = Vector3.Lerp(transform.position, targetPos, zoomSpeed * Time.deltaTime);

            // Stop zooming once we are close enough
            if (Mathf.Abs(mainCamera.orthographicSize - targetSize) < 0.01f)
            {
                isZooming = false;
                Debug.Log("Zoom complete! Player start.");
            }
        }
    }
}