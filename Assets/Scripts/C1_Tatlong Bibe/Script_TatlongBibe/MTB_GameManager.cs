using UnityEngine;
using UnityEngine.UI; // NEW: Required to change Images and Sprites!
using UnityEngine.SceneManagement;
using TMPro;

public class MTB_GameManager : MonoBehaviour
{
    public static MTB_GameManager Instance;

    [Header("Tutorial UI")]
    public GameObject tutorialHand; // The hand image
    private bool waitingForTutorialClick = false;

    [Header("Global Stats")]
    public int sharedHealth = 3;
    private int maxHealth = 3; // Remembers if the max is 3 (Medium) or 1 (Hard)
    public bool isGameOver = false;

    [Header("Difficulty Multiplier")]
    public float globalSpeedMultiplier = 1f;
    public float loopSpeedIncrease = 0.15f; // Increases speed by 15% every loop!

    [Header("Score System")]
    public TextMeshProUGUI scoreText;
    public int currentScore = 0;
    private float scoreTimer = 0f;
    public float timePerPoint = 0.5f; // 1 point every 0.5 seconds!

    [Header("HUD UI Elements")]
    public GameObject healthBarUI;
    public Image[] hudHearts; // Drag your 3 playing hearts here
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    [Header("Game Over UI Elements")]
    public Image[] gameOverHearts; // Drag the 3 hearts from the Game Over panel here

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        globalSpeedMultiplier = 1f;
        Time.timeScale = 1f;
        if (tutorialHand != null) tutorialHand.SetActive(false);
        // 1. Set the rules based on difficulty
        switch (GameSettings.CurrentDifficulty)
        {
            case Difficulty.Easy:
                maxHealth = 3;
                sharedHealth = 3;
                if (healthBarUI != null) healthBarUI.SetActive(false);
                break;

            case Difficulty.Medium:
                maxHealth = 3;
                sharedHealth = 3;
                if (healthBarUI != null) healthBarUI.SetActive(true);
                break;

            case Difficulty.Hard:
                maxHealth = 1;
                sharedHealth = 1;
                if (healthBarUI != null) healthBarUI.SetActive(true);
                break;
        }

        // 2. Refresh the UI immediately
        UpdateHeartsUI();
    }

    public void IncreaseLoopSpeed()
    {
        // Only increase speed if we are on Hard Mode!
        if (GameSettings.CurrentDifficulty == Difficulty.Hard)
        {
            globalSpeedMultiplier += loopSpeedIncrease;
            Debug.Log("Song Looped! Speed increased to: " + globalSpeedMultiplier);
        }
    }

    void Update()
    {
        // If we are paused for the tutorial, wait for a click
        if (waitingForTutorialClick && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            ResumeFromTutorial();
        }

        // 1. Stop counting if the game is over!
        if (isGameOver) return;

        // 2. Count up the timer
        scoreTimer += Time.deltaTime;

        // 3. When the timer hits 0.5 seconds...
        if (scoreTimer >= timePerPoint)
        {
            currentScore++;           // Add 1 point
            scoreTimer = 0f;          // Reset the timer back to 0
            UpdateScoreUI();          // Update the screen
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }

    public void ShowEasyTutorial()
    {
        if (tutorialHand != null) tutorialHand.SetActive(true);
        waitingForTutorialClick = true;
        Time.timeScale = 0f; // Freeze everything!
    }
    public void ResumeFromTutorial()
    {
        if (tutorialHand != null) tutorialHand.SetActive(false);
        waitingForTutorialClick = false;
        Time.timeScale = 1f; // Unfreeze!
    }

    public void LoseHealth()
    {
        if (isGameOver) return;

        // Ignore damage on Easy Mode
        if (GameSettings.CurrentDifficulty == Difficulty.Easy) return;

        // Take damage
        sharedHealth--;

        // Refresh the UI to show the empty hearts
        UpdateHeartsUI();

        if (sharedHealth <= 0)
        {
            EndGame();
        }
    }

    // --- NEW: THE MAGIC UI UPDATER ---
    private void UpdateHeartsUI()
    {
        // 1. Update the HUD Hearts
        for (int i = 0; i < hudHearts.Length; i++)
        {
            if (i >= maxHealth)
            {
                hudHearts[i].enabled = false; // Hide completely on Hard Mode
            }
            else
            {
                hudHearts[i].enabled = true;
                // Swap between Full and Empty sprite
                hudHearts[i].sprite = (i < sharedHealth) ? fullHeartSprite : emptyHeartSprite;
            }
        }

        // 2. Update the Game Over Hearts
        for (int i = 0; i < gameOverHearts.Length; i++)
        {
            if (i >= maxHealth)
            {
                gameOverHearts[i].gameObject.SetActive(false); // Hide extra ducks/hearts on Hard
            }
            else
            {
                gameOverHearts[i].gameObject.SetActive(true);
                // Swap between Full and Empty sprite
                gameOverHearts[i].sprite = (i < sharedHealth) ? fullHeartSprite : emptyHeartSprite;
            }
        }
    }

    void EndGame()
    {
        isGameOver = true;
        Debug.Log("GAME OVER!");
        Time.timeScale = 0f;

        // Note: Make sure another script (or this one) actually turns on the Game Over Panel!
    }
}