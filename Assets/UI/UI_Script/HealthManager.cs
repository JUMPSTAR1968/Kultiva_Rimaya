using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    // THIS LINE MAKES IT GLOBAL
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
        // Set up the global reference
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (isHardMode) maxHealth = 1;
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
            Debug.Log("GAME OVER! ZERO HEARTS!");
            // You can trigger your Game Over panel here later
        }
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