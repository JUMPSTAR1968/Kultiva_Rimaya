using System.Collections;
using UnityEngine;

public class DuckHealth : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isInvincible = false;
    public float iFrameDuration = 2f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If we hit an obstacle and aren't invincible and the game isn't over
        if (collision.CompareTag("Obstacle") && !isInvincible && !GameManager.Instance.isGameOver)
        {
            GameManager.Instance.LoseHealth(); // Tell the manager we got hit!
            StartCoroutine(FlashRoutine());
        }
    }

    IEnumerator FlashRoutine()
    {
        isInvincible = true;
        float elapsed = 0;
        while (elapsed < iFrameDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // Toggle visibility to flash
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }
        spriteRenderer.enabled = true; // Ensure sprite is visible at the end
        isInvincible = false;
    }
}