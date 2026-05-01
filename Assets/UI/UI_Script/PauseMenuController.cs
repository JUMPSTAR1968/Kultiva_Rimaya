using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pausePanel;

    // --- NEW: The Instant Pause ---
    public void PauseGame()
    {
        // 1. Play the universal click sound instantly
        if (UniversalUIManager.Instance != null)
        {
            UniversalUIManager.Instance.PlayClickSound();
        }

        // 2. Freeze the game and show the panel IMMEDIATELY
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // --- The Delayed Un-Pauses ---
    public void ResumeGame()
    {
        UniversalUIManager.Instance.TriggerCustomAction(() =>
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f; // Unfreeze the game!
        });
    }

    public void RestartGame()
    {
        UniversalUIManager.Instance.TriggerCustomAction(() =>
        {
            Time.timeScale = 1f; // Always unfreeze before loading a scene!
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    public void BackToMainMenu()
    {
        UniversalUIManager.Instance.TriggerCustomAction(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("A1_Main Menu"); // Replace with your actual menu scene name
        });
    }
}