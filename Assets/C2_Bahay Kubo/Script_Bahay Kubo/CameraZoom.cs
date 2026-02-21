using UnityEngine;
using System.Collections; // <--- THIS WAS MISSING!

public class CameraIntroZoom : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCamera;
    public Transform playerTransform;
    
    [Header("Zoom Values")]
    public float startSize = 15f;     
    public float targetSize = 5f;      
    public float waitDuration = 5f;    
    public float zoomSpeed = 2f;       

    private bool isZooming = false;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        mainCamera.orthographicSize = startSize;

        StartCoroutine(StartZoomSequence());
    }

    IEnumerator StartZoomSequence()
    {
        yield return new WaitForSeconds(waitDuration);
        isZooming = true;
    }

    void Update()
{
    if (isZooming && playerTransform != null)
    {
        // Smoothly change the Camera Size
        mainCamera.orthographicSize = Mathf.MoveTowards(
            mainCamera.orthographicSize, 
            targetSize, 
            zoomSpeed * Time.deltaTime
        );

        // Calculate target: Player's X/Y but keep Camera's Z at -10
        Vector3 targetPos = new Vector3(playerTransform.position.x, playerTransform.position.y, -10f);
        
        // Use SmoothDamp or a faster Lerp to ensure the camera reaches the player 
        // at the same time the zoom finishes
        transform.position = Vector3.Lerp(transform.position, targetPos, zoomSpeed * Time.deltaTime);

        if (Mathf.Abs(mainCamera.orthographicSize - targetSize) < 0.01f)
        {
            // Final snap to ensure perfect alignment
            transform.position = targetPos;
            isZooming = false;
        }
    }
}
}