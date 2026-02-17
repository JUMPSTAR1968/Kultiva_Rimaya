using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonEffects : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Snappy Settings (No Interpolation)")]
    [SerializeField] float shrinkScale = 0.9f;

    [Header("Ghost Settings (Smooth)")]
    [SerializeField] float rippleExpansionSize = 1.8f;
    [SerializeField] float rippleFadeTime = 0.4f;

    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    // --- 1. INSTANT SHRINK (Frame 1 -> Frame 2) ---
    public void OnPointerDown(PointerEventData eventData)
    {
        // No Coroutine. No Lerp. Just SNAP to the size.
        // This creates that heavy "Guilty Gear" impact feel.
        transform.localScale = originalScale * shrinkScale;
    }

    // --- 2. INSTANT RELEASE (Frame 19 -> Frame 20) ---
    public void OnPointerUp(PointerEventData eventData)
    {
        // SNAP back to original size immediately
        transform.localScale = originalScale;

        // Trigger the smooth "Afterimage" ghost
        CreateGhostRipple();
    }

    // --- 3. THE GHOST (Smooth Animation) ---
    void CreateGhostRipple()
    {
        GameObject ghost = Instantiate(gameObject, transform.position, transform.rotation, transform.parent);

        // 1. CLEANUP SCRIPT
        Destroy(ghost.GetComponent<ButtonEffects>());
        Destroy(ghost.GetComponent<UnityEngine.UI.Button>());

        // 2. CLEANUP TEXT (Optional: Adds polish!)
        // This looks for TextMeshPro inside the ghost and deletes it
        // so you only see the green shape expanding.
        var textComponent = ghost.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (textComponent != null) Destroy(textComponent.gameObject);

        // 3. FIX HIERARCHY (The fix from above)
        ghost.transform.SetSiblingIndex(transform.GetSiblingIndex());

        StartCoroutine(AnimateGhost(ghost));
    }

    IEnumerator AnimateGhost(GameObject ghost)
    {
        // Add CanvasGroup to handle the transparency fade
        CanvasGroup group = ghost.GetComponent<CanvasGroup>();
        if (group == null) group = ghost.AddComponent<CanvasGroup>();

        float timer = 0;
        Vector3 startScale = originalScale;
        Vector3 targetScale = originalScale * rippleExpansionSize;

        while (timer < rippleFadeTime)
        {
            float progress = timer / rippleFadeTime;

            // The Ghost EXPANDS smoothly
            ghost.transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

            // The Ghost FADES out smoothly
            group.alpha = Mathf.Lerp(0.8f, 0f, progress); // Starts at 0.8 alpha for style

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        Destroy(ghost);
    }
}