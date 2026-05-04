using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI feedbackText;
    public Image missOverlay;

    [Header("Settings")]
    public float fadeSpeed = 2.0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (feedbackText != null) feedbackText.alpha = 0;
        if (missOverlay != null) missOverlay.color = new Color(1, 0, 0, 0);
    }

    public void ShowFeedback(string message, Color color)
    {
        Debug.Log("FeedbackManager: Showing " + message);

        // 1. Stop the previous fading animations
        StopAllCoroutines();

        // --- NEW FIX: FORCE RESET THE RED SCREEN ---
        // If an old red flash was interrupted, this instantly clears it 
        // so it never gets stuck on screen!
        if (missOverlay != null)
        {
            missOverlay.color = new Color(1, 0, 0, 0);
        }

        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.alpha = 1f;

        if (message == "MISS")
        {
            StartCoroutine(FlashRed());
        }

        StartCoroutine(FadeText());
    }

    IEnumerator FlashRed()
    {
        missOverlay.color = new Color(1, 0, 0, 0.6f);
        float elapsed = 0;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(0.6f, 0, elapsed / 0.5f);
            missOverlay.color = new Color(1, 0, 0, newAlpha);
            yield return null;
        }

        // --- NEW FIX: SAFETY NET ---
        // Force the alpha to exactly 0 at the very end of the animation
        missOverlay.color = new Color(1, 0, 0, 0);
    }

    IEnumerator FadeText()
    {
        yield return new WaitForSeconds(0.4f);
        while (feedbackText.alpha > 0)
        {
            feedbackText.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    void Update()
    {
        // Press the 'T' key while the game is running to test the Miss effect
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowFeedback("MISS", Color.red);
        }
    }
}