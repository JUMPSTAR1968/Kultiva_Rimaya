using UnityEngine;

public class AutoVerticalStretch : MonoBehaviour
{
    [Header("Stretch Settings")]
    [Tooltip("1 = Stretch fully to cover gaps. 0 = Don't stretch at all.")]
    [Range(0f, 1f)]
    public float stretchIntensity = 1f; // This adds a nice slider in Unity!

    // We assume the game is built for 16:9 phones
    private float targetAspect = 16f / 9f;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        float currentAspect = (float)Screen.width / (float)Screen.height;

        // If we are on a squarish screen like an iPad...
        if (currentAspect < targetAspect)
        {
            // Calculate exactly how much the camera zoomed out vertically
            float fullStretchFactor = targetAspect / currentAspect;

            // Blend the stretch based on your slider
            float actualStretch = Mathf.Lerp(1f, fullStretchFactor, stretchIntensity);

            // Apply the customized stretch to the Y-axis
            transform.localScale = new Vector3(originalScale.x, originalScale.y * actualStretch, originalScale.z);
        }
    }
}