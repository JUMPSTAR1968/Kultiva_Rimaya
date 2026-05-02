using System.Collections;
using UnityEngine;

public class DuckHealth : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Sprite damagedSprite;
    private Sprite originalSprite;

    [Header("Easy Mode Tutorial")]
    public float worldRewindDistance = 2.5f;
    public float rewindDuration = 0.5f;

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
                // Talk to both managers so nothing breaks!
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

        Time.timeScale = 0f;

        GameObject[] allObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        LoopingBackground[] allBackgrounds = FindObjectsOfType<LoopingBackground>();

        float elapsed = 0f;
        float speed = worldRewindDistance / rewindDuration;

        while (elapsed < rewindDuration)
        {
            float step = speed * Time.unscaledDeltaTime;

            foreach (GameObject obstacle in allObstacles)
            {
                if (obstacle != null) obstacle.transform.position += Vector3.right * step;
            }

            foreach (LoopingBackground bg in allBackgrounds)
            {
                if (bg != null) bg.transform.position += Vector3.right * step;
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        MTB_GameManager.Instance.ShowEasyTutorial();

        yield return new WaitUntil(() => Time.timeScale > 0);

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