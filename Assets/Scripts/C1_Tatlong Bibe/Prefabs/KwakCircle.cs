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

        // Updated these lines to point to FeedbackManager
        if (absDiff <= perfectWindow)
        {
            FeedbackManager.Instance.ShowFeedback("PERFECT!", Color.cyan);
        }
        else if (diff > perfectWindow && diff <= okWindow)
        {
            FeedbackManager.Instance.ShowFeedback("LATE", Color.yellow);
        }
        else if (diff < -perfectWindow && diff >= -okWindow)
        {
            FeedbackManager.Instance.ShowFeedback("EARLY", Color.orange);
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
        // Updated this line to point to FeedbackManager
        FeedbackManager.Instance.ShowFeedback("MISS", Color.red);
        FinishNote();
    }

    private void FinishNote()
    {
        isProcessed = true;
        Destroy(gameObject);
    }
}