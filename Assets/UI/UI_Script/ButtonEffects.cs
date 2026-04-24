using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Unity.VisualScripting;

public class ButtonEffects : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Snappy Settings (No Interpolation)")]
    [SerializeField] float shrinkScale = 0.9f;

    [Header("Ghost Settings (Smooth)")]
    [SerializeField] float rippleExpansionSize = 1.8f;
    [SerializeField] public float rippleFadeTime = 0.4f;

    private Vector3 originalScale;

    [SerializeField] private bool overrideGhostRipple = false;

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
        if (!overrideGhostRipple) CreateGhostRipple();
    }

    // --- 3. THE GHOST (Smooth Animation) ---
    void CreateGhostRipple()
    {
        GameObject ghost = Instantiate(gameObject, transform);

        // 1. CLEANUP SCRIPT
        Destroy(ghost.GetComponent<ButtonEffects>());
        Destroy(ghost.GetComponent<UnityEngine.UI.Button>());

        // 2. CLEANUP TEXT (Optional: Adds polish!)
        // This looks for TextMeshPro inside the ghost and deletes it
        // so you only see the green shape expanding.
        var textComponent = ghost.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (textComponent != null) Destroy(textComponent.gameObject);

        /* 
         Centering the anchors and pivot of ghost clones,
         because the original pause button's anchors and pivot are originally set to top left,
         and the ghost animation has to ripple from the middle of original button, not top left -Arjan
         */
        RectTransform ghostRectTransform = ghost.GetComponent<RectTransform>();
        RectTransform originalRectTransform = GetComponent<RectTransform>();

        if (ghostRectTransform != null) {
            ghostRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            ghostRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            ghostRectTransform.pivot = new Vector2(0.5f, 0.5f);

            ghostRectTransform.anchoredPosition = Vector2.zero;

            ghostRectTransform.SetParent(transform.parent);

            ghostRectTransform.SetSiblingIndex(transform.GetSiblingIndex());
        }

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