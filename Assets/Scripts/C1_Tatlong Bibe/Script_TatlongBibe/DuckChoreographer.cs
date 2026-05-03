using UnityEngine;
using System.Collections;

public class DuckChoreographer : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource songSource;

    [Header("Fat Duck Setup")]
    public SpriteRenderer fatDuck;
    public Sprite fatDuckNormal;
    public Sprite fatDuckOutline; // Drag the yellow outline sprite here!

    [Header("Thin Duck Setup")]
    public SpriteRenderer thinDuck;
    public Sprite thinDuckNormal;
    public Sprite thinDuckOutline; // Drag the yellow outline sprite here!

    [Header("Timings (in seconds)")]
    public float[] matabaTimings;
    public float[] mapayatTimings;

    [Header("Highlight Settings")]
    public float highlightDuration = 0.3f; // How long the yellow outline stays

    private int nextFatIndex = 0;
    private int nextThinIndex = 0;
    private float previousTime = 0f;

    void Start()
    {
        // Make sure they start with their normal sprites
        if (fatDuck != null && fatDuckNormal != null)
            fatDuck.sprite = fatDuckNormal;

        if (thinDuck != null && thinDuckNormal != null)
            thinDuck.sprite = thinDuckNormal;
    }

    void Update()
    {
        if (songSource == null) return;

        float currentTime = songSource.time;

        // LOOP DETECTION: Resets the sequence when the song restarts!
        if (currentTime < previousTime)
        {
            nextFatIndex = 0;
            nextThinIndex = 0;
        }
        previousTime = currentTime;

        // Check for Mataba (Fat Duck)
        if (nextFatIndex < matabaTimings.Length && currentTime >= matabaTimings[nextFatIndex])
        {
            StartCoroutine(SwapSprite(fatDuck, fatDuckOutline, fatDuckNormal));
            nextFatIndex++;
        }

        // Check for Mapayat (Thin Duck)
        if (nextThinIndex < mapayatTimings.Length && currentTime >= mapayatTimings[nextThinIndex])
        {
            StartCoroutine(SwapSprite(thinDuck, thinDuckOutline, thinDuckNormal));
            nextThinIndex++;
        }
    }

    IEnumerator SwapSprite(SpriteRenderer targetRenderer, Sprite highlightSprite, Sprite normalSprite)
    {
        if (targetRenderer == null || highlightSprite == null || normalSprite == null) yield break;

        // 1. Change to the yellow outline sprite
        targetRenderer.sprite = highlightSprite;

        // 2. Wait for a split second
        yield return new WaitForSeconds(highlightDuration);

        // 3. Change back to the normal sprite
        targetRenderer.sprite = normalSprite;
    }
}