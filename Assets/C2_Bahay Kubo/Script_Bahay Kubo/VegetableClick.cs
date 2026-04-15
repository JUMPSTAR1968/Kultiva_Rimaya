using UnityEngine;
using System.Collections;

public class VegetableClick : MonoBehaviour
{
    [HideInInspector] public BahayKuboSequentialSpawner spawner;
    [HideInInspector] public int vegetableID; // Assigned by the Spawner

    private SpriteRenderer spriteRenderer;
    private bool isFlashing = false;

    private void Awake()
    {
        // Get the renderer so we can change its color
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        // If we are currently flashing from a wrong click, 
        // we might want to ignore extra clicks until it's done
        if (isFlashing) return;

        if (spawner != null)
        {
            // Ask the spawner if this is the correct vegetable and timing
            spawner.TryHarvest(vegetableID, gameObject);
        }
    }

    /// <summary>
    /// Called by the Spawner when the user clicks the wrong vegetable.
    /// </summary>
    public void FlashRed()
    {
        if (!isFlashing && gameObject.activeInHierarchy)
        {
            StartCoroutine(FlashRoutine());
        }
    }

    private IEnumerator FlashRoutine()
    {
        isFlashing = true;
        Color originalColor = Color.white;

        if (spriteRenderer != null)
        {
            // Visual feedback: Turn Red and scale up slightly
            spriteRenderer.color = Color.red;
            Vector3 originalScale = transform.localScale;
            transform.localScale = originalScale * 1.1f;

            // Wait for a short duration
            yield return new WaitForSeconds(0.2f);

            // Return to normal
            spriteRenderer.color = originalColor;
            transform.localScale = originalScale;
        }

        isFlashing = false;
    }
}