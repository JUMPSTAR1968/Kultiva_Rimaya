using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    [Header("Settings")]
    public float scrollSpeed = 5f;
    public int backgroundCount = 3;

    private float width;
    private bool isReady = false; // Safety switch

    void Start()
    {
        // 1. Look for the sprite on THIS object first
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        // 2. If not found, look into the CHILDREN (the layers inside)
        if (sr == null)
        {
            sr = GetComponentInChildren<SpriteRenderer>();
        }

        // 3. If we found it, measure it!
        if (sr != null)
        {
            width = sr.bounds.size.x;
            isReady = true;
        }
        else
        {
            // If we still can't find it, show a clear error
            Debug.LogError("Help! I (" + gameObject.name + ") can't find a SpriteRenderer inside me or my children.");
        }
    }

    void Update()
    {
        // Only run if we successfully found the width
        if (!isReady) return;

        // Move to the left
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        // Check if we need to jump
        // We use the world position to check
        if (transform.position.x <= -width)
        {
            Vector3 jumpOffset = Vector3.right * (width * backgroundCount);
            transform.position += jumpOffset;
        }
    }
}