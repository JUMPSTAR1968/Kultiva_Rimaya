using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;

    // We don't need the local 'isHardMode' boolean anymore, 
    // because we are using the global GameSettings!

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
        // FIXED: Now it reads from the exact same rules as the Game Manager!
        if (GameSettings.CurrentDifficulty == Difficulty.Hard)
        {
            maxHealth = 1;
        }
        else
        {
            maxHealth = 3;
        }

        currentHealth = maxHealth;
        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHearts();

        if (currentHealth == 0)
        {

            Debug.Log("HealthManager: GAME OVER! ZERO HEARTS!");

        }
    }

    // --- NEW: CALL THIS TO REFILL HEARTS ---
    public void ResetHealth()
    {
        // Make sure it enforces difficulty when resetting!
        if (GameSettings.CurrentDifficulty == Difficulty.Hard) maxHealth = 1;
        else maxHealth = 3;

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