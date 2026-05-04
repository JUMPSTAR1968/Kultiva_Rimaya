using UnityEngine;
using UnityEngine.UI;

public class KwakCircle : MonoBehaviour
{
    public RectTransform approachRing;

    [Header("Visual Settings")]
    public float startingRingScale = 4.0f; // For the outer ring
    public float overallNoteSize = 2.0f;

    private float targetHitTime;
    private float leadTime;
    private AudioSource audioClock;
    private bool isProcessed = false;

    private float perfectWindow = 0.12f;
    private float okWindow = 0.25f;

    void Start()
    {
        transform.localScale = new Vector3(overallNoteSize, overallNoteSize, 1f);
    }

    public void Setup(float target, AudioSource source, float lead)
    {
        targetHitTime = target;
        audioClock = source;
        leadTime = lead;
    }

    void Update()
    {
        if (audioClock == null || isProcessed) return;

        float timeRemaining = targetHitTime - audioClock.time;
        float progress = 1.0f - (timeRemaining / leadTime);
        // Updated to use your new startingRingScale variable!
        float currentScale = Mathf.Lerp(startingRingScale, 1.0f, progress);

        if (approachRing != null)
            approachRing.localScale = new Vector3(currentScale, currentScale, 1f);

        if (audioClock.time > targetHitTime + okWindow)
        {
            TriggerMiss();
        }
    }

    public void OnPlayerClick()
    {
        if (isProcessed) return;

        float diff = audioClock.time - targetHitTime;
        float absDiff = Mathf.Abs(diff);

        if (absDiff <= perfectWindow)
        {
            FeedbackManager.Instance.ShowFeedback("PERFECT!", Color.cyan);

            // --- NEW: Add 10 Points ---
            if (MTB_GameManager.Instance != null)
            {
                MTB_GameManager.Instance.AddScore(10f);
            }
        }
        else if (diff > perfectWindow && diff <= okWindow)
        {
            FeedbackManager.Instance.ShowFeedback("LATE", Color.yellow);

            // --- NEW: Add 5 Points ---
            if (MTB_GameManager.Instance != null)
            {
                MTB_GameManager.Instance.AddScore(5f);
            }
        }
        else if (diff < -perfectWindow && diff >= -okWindow)
        {
            FeedbackManager.Instance.ShowFeedback("EARLY", Color.orange);

            // --- NEW: Add 5 Points ---
            if (MTB_GameManager.Instance != null)
            {
                MTB_GameManager.Instance.AddScore(5f);
            }
        }
        else
        {
            TriggerMiss();
            return;
        }

        FinishNote();
    }

    private void TriggerMiss()
    {
        if (isProcessed) return;

        // Show the red MISS text and flash
        FeedbackManager.Instance.ShowFeedback("MISS", Color.red);

        // --- NEW: DEAL DAMAGE! ---
        // We use the same logic from DuckHealth to hurt the player
        if (HealthManager.Instance != null)
        {
            HealthManager.Instance.TakeDamage(1);
        }

        if (MTB_GameManager.Instance != null)
        {
            // Only trigger LoseHealth if the game isn't already over
            if (!MTB_GameManager.Instance.isGameOver)
            {
                MTB_GameManager.Instance.LoseHealth();
            }
        }

        FinishNote();
    }

    private void FinishNote()
    {
        isProcessed = true;
        Destroy(gameObject);
    }
}