using UnityEngine;
using TMPro;

public class ResultsManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject victoryPanel;
    public GameObject gameOverPanel;

    [Header("Victory Text")]
    public TextMeshProUGUI victoryScoreText;
    public TextMeshProUGUI scoreFeedbackText; // "New High Score!" etc.

    [Header("Game Over Text")]
    public TextMeshProUGUI gameOverScoreText;

    // Call this function when the player finishes the level or dies!
    public void TriggerEndGame(bool outOfLives, int finalScore, bool isHardMode)
    {
        // 1. Freeze the game
        Time.timeScale = 0f;

        // 2. The Director's Logic:
        // - If they survived Normal mode (!outOfLives) -> Victory
        // - If they died in Hard mode (Endless) -> Victory (Final Results)
        // - If they died in Normal mode -> Game Over
        bool showVictoryScreen = !outOfLives || isHardMode;

        if (showVictoryScreen)
        {
            victoryPanel.SetActive(true);
            // The "N0" formats the number with commas! (e.g. 64,128,512)
            victoryScoreText.text = finalScore.ToString("N0");

            // Note: You can add High Score checking logic here later to change the scoreFeedbackText!
        }
        else
        {
            gameOverPanel.SetActive(true);
            gameOverScoreText.text = finalScore.ToString("N0");
        }
    }
}