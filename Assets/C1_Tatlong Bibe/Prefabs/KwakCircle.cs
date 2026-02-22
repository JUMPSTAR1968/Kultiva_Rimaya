using UnityEngine;
using UnityEngine.UI;

public class KwakCircle : MonoBehaviour
{
    public RectTransform approachRing;
    private float targetHitTime;
    private float leadTime;
    private AudioSource audioClock;

    public void Setup(float target, AudioSource source, float lead)
    {
        targetHitTime = target;
        audioClock = source;
        leadTime = lead;
    }

    void Update()
    {
        if (audioClock == null) return;

        // Calculate progress from 0.0 (spawn) to 1.0 (hit time)
        float timeRemaining = targetHitTime - audioClock.time;
        float progress = 1.0f - (timeRemaining / leadTime);

        // Scale from 3x size down to 1x size
        float currentScale = Mathf.Lerp(3.0f, 1.0f, progress);

        if (approachRing != null)
            approachRing.localScale = new Vector3(currentScale, currentScale, 1f);

        // Auto-destroy if the player misses it (e.g., 0.2 seconds after the hit time)
        if (audioClock.time > targetHitTime + 0.2f)
        {
            Debug.Log("MISSED!");
            Destroy(gameObject);
        }
    }

    public void OnPlayerClick()
    {
        float accuracy = Mathf.Abs(audioClock.time - targetHitTime);
        if (accuracy <= 0.12f) Debug.Log("<color=cyan>PERFECTLY TIMED!</color>");
        else if (accuracy <= 0.25f) Debug.Log("<color=yellow>GOOD</color>");
        else Debug.Log("<color=orange>POORLY TIMED</color>");
        Destroy(gameObject);
    }
}