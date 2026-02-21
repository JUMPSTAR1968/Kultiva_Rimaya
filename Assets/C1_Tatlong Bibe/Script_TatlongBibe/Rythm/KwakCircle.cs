using UnityEngine;
using UnityEngine.UI;

public class KwakCircle : MonoBehaviour
{
    private float targetHitTime;
    private AudioSource audioClock;
    private bool isHit = false;

    [Header("Visual Elements")]
    public RectTransform approachRing;
    public float startScale = 3.0f;

    public void Setup(float target, AudioSource source)
    {
        targetHitTime = target;
        audioClock = source;
    }

    void Update()
    {
        if (isHit || audioClock == null) return;

        float currentTime = audioClock.time;

        // This creates the closing ring "Information"
        float progress = 1.0f - (targetHitTime - currentTime);
        float currentScale = Mathf.Lerp(startScale, 1.0f, progress);

        if (approachRing != null)
            approachRing.localScale = new Vector3(currentScale, currentScale, 1f);

        // Auto-delete if they miss the timing window
        if (currentTime > targetHitTime + 0.25f)
        {
            Debug.Log("<color=red>MISS!</color>");
            Destroy(gameObject);
        }
    }

    public void OnPlayerClick()
    {
        if (isHit || audioClock == null) return;

        float accuracy = Mathf.Abs(audioClock.time - targetHitTime);

        // Feedback System based on timing
        if (accuracy <= 0.12f)
            Debug.Log("<color=cyan>PERFECTLY TIMED!</color>");
        else if (accuracy <= 0.25f)
            Debug.Log("<color=yellow>GOOD</color>");
        else
            Debug.Log("<color=orange>POORLY TIMED</color>");

        isHit = true;
        Destroy(gameObject);
    }
}