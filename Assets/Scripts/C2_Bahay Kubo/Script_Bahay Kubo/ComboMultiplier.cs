using UnityEngine;
using TMPro;

public class ComboMultiplier : MonoBehaviour
{
    [Header("Multiplier Settings")]
    public float multiplierStep = 0.1f;
    public float maxMultiplier = 5.0f;
    public float minMultiplier = 1.0f;
    
    [Header("Decay Settings")]
    public float waitBeforeDecay = 3.0f; // Seconds to wait
    public float decaySpeed = 0.5f;      // How much to lose per second
    
    [Header("UI Reference")]
    public TextMeshProUGUI comboText;

    private float _currentMultiplier = 1.0f;
    private float _idleTimer;

    void Start()
    {
        _idleTimer = waitBeforeDecay;
        UpdateUI();
    }

    void Update()
    {
        // 1. Reduce the idle timer every frame
        if (_idleTimer > 0)
        {
            _idleTimer -= Time.deltaTime;
        }
        else
        {
            // 2. If timer is 0, start decaying the multiplier
            ApplyDecay();
        }
    }

    private void OnMouseDown()
    {
        IncrementCombo();
    }

    private void IncrementCombo()
    {
        // Reset the timer back to 3 seconds on every click
        _idleTimer = waitBeforeDecay;

        _currentMultiplier += multiplierStep;
        _currentMultiplier = Mathf.Clamp(_currentMultiplier, minMultiplier, maxMultiplier);

        UpdateUI();
    }

    private void ApplyDecay()
    {
        // Only decay if we are above the minimum
        if (_currentMultiplier > minMultiplier)
        {
            // Reduce multiplier based on time passed
            _currentMultiplier -= decaySpeed * Time.deltaTime;
            
            // Clamp again so it doesn't go below 1.0
            _currentMultiplier = Mathf.Max(_currentMultiplier, minMultiplier);
            
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (comboText != null)
        {
            comboText.text = _currentMultiplier.ToString("F1") + "x";
        }
    }
}