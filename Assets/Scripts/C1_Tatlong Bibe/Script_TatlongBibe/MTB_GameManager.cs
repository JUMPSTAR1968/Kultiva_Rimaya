using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class MTB_GameManager : MonoBehaviour
{
    public static MTB_GameManager Instance;

    [Header("Tutorial UI")]
    public GameObject tutorialHand;
    private bool waitingForTutorialClick = false;

    [Header("Global Stats")]
    public int sharedHealth = 3;
    private int maxHealth = 3;
    public bool isGameOver = false;

    [Header("Difficulty Multiplier")]
    public float globalSpeedMultiplier = 1f;
    public float loopSpeedIncrease = 0.15f;

    [Header("Score System")]
    public TextMeshProUGUI scoreText;
    public float currentScore = 0f;
    private float scoreTimer = 0f;
    public float timePerPoint = 0.5f;

    [Header("Score Multipliers")]
    public float passiveScoreMultiplier = 1.0f;
    public float boulderBonusPoints = 10f;
    public float boulderMultiplier = 1.5f;

    [Header("HUD UI Elements")]
    public GameObject healthBarUI;
    public Image[] hudHearts;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    [Header("Game Over UI Elements")]
    public Image[] gameOverHearts;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // --- THE FIX: Unfreeze time immediately on restart! ---
        Time.timeScale = 1f;
    }

    void Start()
    {
        globalSpeedMultiplier = 1f;
        Time.timeScale = 1f;
        if (tutorialHand != null) tutorialHand.SetActive(false);

        switch (GameSettings.CurrentDifficulty)
        {
            case Difficulty.Easy:
                maxHealth = 3;
                sharedHealth = 3;
                if (healthBarUI != null) healthBarUI.SetActive(false);

                // NEW: Hide score on Easy
                if (scoreText != null) scoreText.gameObject.SetActive(false);
                break;

            case Difficulty.Medium:
                maxHealth = 3;
                sharedHealth = 3;
                if (healthBarUI != null) healthBarUI.SetActive(true);

                // NEW: Hide score on Medium
                if (scoreText != null) scoreText.gameObject.SetActive(false);
                break;

            case Difficulty.Hard:
                maxHealth = 1;
                sharedHealth = 1;
                if (healthBarUI != null) healthBarUI.SetActive(true);

                // NEW: Show score on Hard!
                if (scoreText != null) scoreText.gameObject.SetActive(true);
                break;
        }

        UpdateHeartsUI();
    }

    public void IncreaseLoopSpeed()
    {
        if (GameSettings.CurrentDifficulty == Difficulty.Hard)
        {
            globalSpeedMultiplier += loopSpeedIncrease;
            Debug.Log("Song Looped! Speed increased to: " + globalSpeedMultiplier);
        }
    }

    void Update()
    {
        // --- NEW: THE HEALTH ENFORCER ---
        // If a rogue script or your restart button tries to give you 3 lives on Hard Mode, block it!
        if (GameSettings.CurrentDifficulty == Difficulty.Hard && sharedHealth > 1)
        {
            maxHealth = 1;
            sharedHealth = 1;
            UpdateHeartsUI();
        }

        // THE NEW WAY:
        // Check if there is a mouse click OR a screen tap using the new system
        bool tapOrClick = false;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            tapOrClick = true;
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            tapOrClick = true;
        }

        if (waitingForTutorialClick && tapOrClick)
        {
            ResumeFromTutorial();
        }

        if (isGameOver) return;

        scoreTimer += Time.deltaTime;

        if (scoreTimer >= timePerPoint)
        {
            currentScore += (1f * passiveScoreMultiplier);
            scoreTimer = 0f;
            UpdateScoreUI();
        }
    }

    public void PassedBoulder()
    {
        if (isGameOver) return;
        currentScore += (boulderBonusPoints * boulderMultiplier);
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.FloorToInt(currentScore).ToString();
        }
    }

    public void ShowEasyTutorial()
    {
        if (tutorialHand != null) tutorialHand.SetActive(true);
        waitingForTutorialClick = true;
        Time.timeScale = 0f;
    }

    public void ResumeFromTutorial()
    {
        if (tutorialHand != null) tutorialHand.SetActive(false);
        waitingForTutorialClick = false;
        Time.timeScale = 1f;
    }

    public void LoseHealth()
    {
        if (isGameOver) return;
        if (GameSettings.CurrentDifficulty == Difficulty.Easy) return;

        sharedHealth--;
        UpdateHeartsUI();

        if (sharedHealth <= 0)
        {
            EndGame();
        }
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < hudHearts.Length; i++)
        {
            if (i >= maxHealth) hudHearts[i].enabled = false;
            else
            {
                hudHearts[i].enabled = true;
                hudHearts[i].sprite = (i < sharedHealth) ? fullHeartSprite : emptyHeartSprite;
            }
        }

        for (int i = 0; i < gameOverHearts.Length; i++)
        {
            if (i >= maxHealth) gameOverHearts[i].gameObject.SetActive(false);
            else
            {
                gameOverHearts[i].gameObject.SetActive(true);
                gameOverHearts[i].sprite = (i < sharedHealth) ? fullHeartSprite : emptyHeartSprite;
            }
        }
    }

    void EndGame()
    {
        isGameOver = true;
        Debug.Log("GAME OVER!");
        Time.timeScale = 0f;
    }

    // --- NEW: BULLETPROOF RESTART METHOD ---
    // If your UI Restart Button isn't calling this yet, link it to this function!
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- NEW: Add Score Method ---
    public void AddScore(float pointsToAdd)
    {
        // Don't add score if the game is over
        if (isGameOver) return;

        currentScore += pointsToAdd;

        // Update the UI text if it exists
        if (scoreText != null)
        {
            // You can format this however your score text normally looks!
            scoreText.text = "Score: " + Mathf.FloorToInt(currentScore).ToString();
        }
    }
}