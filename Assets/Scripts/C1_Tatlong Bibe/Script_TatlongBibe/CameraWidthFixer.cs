using UnityEngine;

public class CameraWidthFixer : MonoBehaviour
{
    [Header("Your Camera Settings")]
    [Tooltip("What is the Orthographic Size of your camera right now?")]
    public float defaultOrthoSize = 5f;

    // We assume you designed the game for a standard phone screen (16:9)
    private float targetAspect = 16f / 9f;

    void Start()
    {
        // Calculate the shape of the device the player is using
        float currentAspect = (float)Screen.width / (float)Screen.height;

        // If the screen is squarer than a phone (like an iPad)...
        if (currentAspect < targetAspect)
        {
            // Zoom the camera OUT vertically so the left and right sides are pushed back into view!
            Camera.main.orthographicSize = defaultOrthoSize * (targetAspect / currentAspect);
        }
        else
        {
            // If they are on a normal wide phone, keep the normal camera size
            Camera.main.orthographicSize = defaultOrthoSize;
        }
    }
}