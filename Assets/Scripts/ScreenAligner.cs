using UnityEngine;

public class ScreenAligner : MonoBehaviour
{
    [Header("Screen Alignment")]
    [Tooltip("0 = far left, 0.5 = middle, 1.0 = far right")]
    [Range(0f, 1f)]
    public float screenPositionX = 0.15f; // Defaults to 15% from the left edge

    void Start()
    {
        AlignToScreen();
    }

    // If you want to see the alignment update in real-time while editing, 
    // we can use OnDrawGizmos or Update, but doing it in Start is best for the actual game!
    void AlignToScreen()
    {
        if (Camera.main == null) return;

        // 1. Tell the camera to find the world coordinates of our chosen percentage (e.g., 0.15)
        // We put 0.5f for Y just to keep it vertically centered for the calculation
        Vector3 viewportPoint = new Vector3(screenPositionX, 0.5f, Camera.main.nearClipPlane);
        Vector3 newWorldPosition = Camera.main.ViewportToWorldPoint(viewportPoint);

        // 2. We ONLY want to change the Duck's X position. 
        // We leave Y and Z alone so we don't break your movement scripts!
        transform.position = new Vector3(newWorldPosition.x, transform.position.y, transform.position.z);
    }
}