using UnityEngine;
using System.Collections; // --- NEW: Needed for the one-frame delay ---

public class ScreenAligner : MonoBehaviour
{
    [Header("Screen Alignment")]
    [Tooltip("0 = far left, 0.5 = middle, 1.0 = far right")]
    [Range(0f, 1f)]
    public float screenPositionX = 0.15f;

    // --- THE FIX: Change Start to an IEnumerator ---
    IEnumerator Start()
    {
        // Wait for exactly one frame before doing the math.
        // This guarantees the Camera and Screen Resolution are 100% ready!
        yield return null;

        AlignToScreen();
    }

    void AlignToScreen()
    {
        if (Camera.main == null) return;

        // Find the true distance from the camera down to the 2D gameplay layer
        float distanceToCamera = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

        // Use that true distance to find the exact screen percentage
        Vector3 viewportPoint = new Vector3(screenPositionX, 0.5f, distanceToCamera);
        Vector3 newWorldPosition = Camera.main.ViewportToWorldPoint(viewportPoint);

        // Apply the newly calculated X, keeping Y and Z exactly the same
        transform.position = new Vector3(newWorldPosition.x, transform.position.y, transform.position.z);
    }
}