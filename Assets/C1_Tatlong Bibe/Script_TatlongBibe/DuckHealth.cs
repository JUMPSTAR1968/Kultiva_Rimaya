using System.Collections;
using UnityEngine;

public class DuckHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("I-Frames Settings")]
    public float iFrameDuration = 2f;    // How long the duck is invincible (seconds)
    public int numberOfFlashes = 5;      // How many times it flashes

    private SpriteRenderer spriteRenderer;
    private bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // This detects when the duck hits a trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if we hit an obstacle AND we are not currently invincible
        if (collision.CompareTag("Obstacle") && !isInvincible)
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        currentHealth--;
        Debug.Log("Ouch! Health remaining: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Game Over!");
            // We will add Game Over logic here later!
        }
        else
        {
            // Start the flashing and invincibility process
            StartCoroutine(FlashRoutine());
        }
    }

    // A Coroutine that handles the flashing effect over time
    IEnumerator FlashRoutine()
    {
        isInvincible = true; // Turn on shield

        // Loop to make the duck flash
        for (int i = 0; i < numberOfFlashes; i++)
        {
            // Make duck slightly transparent (red=1, green=1, blue=1, alpha=0.5)
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlashes * 2));

            // Make duck fully solid again
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlashes * 2));
        }

        isInvincible = false; // Turn off shield
    }
}