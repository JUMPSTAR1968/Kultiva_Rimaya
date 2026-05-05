using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem; // --- NEW: Required to read mobile touches! ---

public class VegetableClick : MonoBehaviour
{
    [HideInInspector] public BahayKuboSequentialSpawner spawner;
    [HideInInspector] public int vegetableID; // Assigned by the Spawner

    private SpriteRenderer spriteRenderer;
    private bool isFlashing = false;
    private Vector3 originalScale;

    [Header("Toddler Hint Settings")]
    private bool isHintActive = false;
    public float bounceSpeed = 6f;   // How fast it pulses
    public float bounceAmount = 0.2f; // How much it grows (0.2 = 20%)

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale; // Capture the base scale
    }

    private void Update()
    {
        // If this vegetable is currently the "Hint", make it pulse
        if (isHintActive && !isFlashing)
        {
            float pulse = 1f + Mathf.Sin(Time.time * bounceSpeed) * bounceAmount;
            transform.localScale = originalScale * pulse;
        }

        // --- THE FIX: Mobile Touch & Mouse Click Detection ---
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            // Find exactly where the player touched the screen
            Vector2 screenPos = Pointer.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));

            // Shoot a tiny laser to see what they tapped on
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            // If the laser hits THIS specific vegetable's collider, harvest it!
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                AttemptHarvest();
            }
        }
    }

    // --- REPLACED: We swapped OnMouseDown() for this custom method ---
    private void AttemptHarvest()
    {
        if (isFlashing) return;

        if (spawner != null)
        {
            spawner.TryHarvest(vegetableID, gameObject);
        }
    }

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
            spriteRenderer.color = Color.red;
            // Use originalScale so we don't accidentally capture a "pulsed" scale
            transform.localScale = originalScale * 1.1f;

            yield return new WaitForSeconds(0.2f);

            spriteRenderer.color = originalColor;
            transform.localScale = originalScale;
        }

        isFlashing = false;
    }

    public void SetHint(bool active)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        isHintActive = active;

        if (active)
        {
            // Optional: Keep the yellow tint if it helps, or remove for pure white
            spriteRenderer.color = new Color(1f, 1f, 0.7f);
        }
        else
        {
            // Reset everything when hint is turned off
            spriteRenderer.color = Color.white;
            transform.localScale = originalScale;
        }
    }
}