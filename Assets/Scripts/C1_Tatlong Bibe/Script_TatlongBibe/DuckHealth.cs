using System.Collections;
using UnityEngine;

public class DuckHealth : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Sprite damagedSprite;
    private Sprite originalSprite;

    [Header("Easy Mode Tutorial")]
    public float worldRewindDistance = 2.5f;
    public float rewindDuration = 0.5f; // NEW: How many seconds the smooth slide takes!

    [Header("I-Frames Settings")]
    public float iFrameDuration = 2f;

    private SpriteRenderer spriteRenderer;
    private bool isInvincible = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle") && !isInvincible && !MTB_GameManager.Instance.isGameOver)
        {
            if (GameSettings.CurrentDifficulty == Difficulty.Easy)
            {
                StartCoroutine(EasyHitRoutine());
            }
            else
            {
                HealthManager.Instance.TakeDamage(1);
                MTB_GameManager.Instance.LoseHealth();
                StartCoroutine(FlashRoutine());
            }
        }
    }

    IEnumerator EasyHitRoutine()
    {
        isInvincible = true;
        if (damagedSprite != null) spriteRenderer.sprite = damagedSprite;

        // 1. FREEZE TIME IMMEDIATELY so the river stops flowing naturally
        Time.timeScale = 0f;

        // 2. Find everything we need to move
        GameObject[] allObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        LoopingBackground[] allBackgrounds = FindObjectsOfType<LoopingBackground>();

        // 3. THE SMOOTH TRANSITION LOOP
        float elapsed = 0f;
        float speed = worldRewindDistance / rewindDuration; // Calculate distance to cover per second

        while (elapsed < rewindDuration)
        {
            // We use unscaledDeltaTime because Time.timeScale is currently 0!
            float step = speed * Time.unscaledDeltaTime;

            // Slide all obstacles
            foreach (GameObject obstacle in allObstacles)
            {
                if (obstacle != null)
                {
                    obstacle.transform.position += Vector3.right * step;
                }
            }

            // Slide all backgrounds
            foreach (LoopingBackground bg in allBackgrounds)
            {
                if (bg != null)
                {
                    bg.transform.position += Vector3.right * step;
                }
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null; // Wait for the very next frame, then loop again
        }

        // 4. Now that the slide is done, show the hand!
        MTB_GameManager.Instance.ShowEasyTutorial();

        // 5. Wait here until the player taps the screen (Manager unfreezes time)
        yield return new WaitUntil(() => Time.timeScale > 0);

        // 6. Transition straight into I-frames
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        isInvincible = true;

        if (damagedSprite != null)
        {
            spriteRenderer.sprite = damagedSprite;
        }

        float elapsed = 0;
        while (elapsed < iFrameDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;

            // Back to normal WaitForSeconds because time is unfrozen!
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        spriteRenderer.enabled = true;

        if (originalSprite != null)
        {
            spriteRenderer.sprite = originalSprite;
        }

        isInvincible = false;
    }
}