using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;

    [Header("Game Modes")]
    public bool isHardMode = false;

    [Header("Stats")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("UI References")]
    public Image[] heartImages;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (isHardMode) maxHealth = 1;
        currentHealth = maxHealth;
        UpdateHearts();
    }

    public void TakeDamage(int damageAmount)
    {
        // --- NEW: THE ENDGAME TRIGGER ---
        if (currentHealth <= 0)
        {
            // Get your score from wherever you are tracking it!
            // Example: int finalScore = FindFirstObjectByType<ScoreManager>().currentScore;
            int finalScore = 123456; // Replace this placeholder with your actual score variable

            // Fire the Game Over sequence (outOfLives = true)
            FindFirstObjectByType<ResultsManager>().TriggerEndGame(true, finalScore, isHardMode);
        }
    }



    // --- NEW: CALL THIS TO REFILL HEARTS ---
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHearts();
        Debug.Log("Health Reset to: " + currentHealth);
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i >= maxHealth)
            {
                heartImages[i].enabled = false;
            }
            else
            {
                heartImages[i].enabled = true;
                if (i < currentHealth) heartImages[i].sprite = fullHeartSprite;
                else heartImages[i].sprite = emptyHeartSprite;
            }
        }
    }
}