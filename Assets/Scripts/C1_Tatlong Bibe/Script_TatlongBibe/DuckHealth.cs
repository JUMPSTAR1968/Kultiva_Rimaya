using System.Collections;
using UnityEngine;

public class DuckHealth : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Sprite damagedSprite;     // Drag your "Ouch" sprite here in the Inspector
    private Sprite originalSprite;   // The script will remember the normal sprite here

    [Header("I-Frames Settings")]
    public float iFrameDuration = 2f;

    private SpriteRenderer spriteRenderer;
    private bool isInvincible = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Save the normal sprite at the start of the game so we can switch back to it later
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If we hit an obstacle and aren't invincible and the game isn't over
        if (collision.CompareTag("Obstacle") && !isInvincible && !MTB_GameManager.Instance.isGameOver)
        {
            // Tell the global HealthManager to remove 1 heart!
            HealthManager.Instance.TakeDamage(1);

            MTB_GameManager.Instance.LoseHealth(); // Tell the manager we got hit!
            StartCoroutine(FlashRoutine());
        }
    }

    IEnumerator FlashRoutine()
    {
        isInvincible = true;

        // 1. Instantly change to the damaged face!
        if (damagedSprite != null)
        {
            spriteRenderer.sprite = damagedSprite;
        }

        // 2. Do the flashing effect
        float elapsed = 0;
        while (elapsed < iFrameDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // Toggle visibility to flash
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        // 3. Ensure sprite is visible at the end
        spriteRenderer.enabled = true;

        // 4. Change back to the normal face!
        if (originalSprite != null)
        {
            spriteRenderer.sprite = originalSprite;
        }

        isInvincible = false;
    }
}