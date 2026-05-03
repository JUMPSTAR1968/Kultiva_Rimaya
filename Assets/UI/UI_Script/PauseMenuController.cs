using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pausePanel;

<<<<<<< HEAD
    // --- NEW: The Instant Pause ---
=======
    void Start()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

>>>>>>> parent of f595f90 (Merge branch 'Speed-Multiplier' into dev)
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

<<<<<<< HEAD
    // --- The Delayed Un-Pauses ---
=======
    // UPDATED: Now using Coroutine for smooth visual feedback
>>>>>>> parent of f595f90 (Merge branch 'Speed-Multiplier' into dev)
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
<<<<<<< HEAD
        UniversalUIManager.Instance.TriggerCustomAction(() =>
        {
            Time.timeScale = 1f; // Always unfreeze before loading a scene!
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
=======
        // Wait for 0.15 seconds (Real time) so we see the button click animation
        yield return new WaitForSecondsRealtime(0.15f);

        pausePanel.SetActive(false);
        Time.timeScale = 1f;
>>>>>>> parent of f595f90 (Merge branch 'Speed-Multiplier' into dev)
    }

    public void BackToMainMenu()
    {
<<<<<<< HEAD
        UniversalUIManager.Instance.TriggerCustomAction(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("A1_Main Menu"); // Replace with your actual menu scene name
        });
=======
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("A1_Main Menu");
>>>>>>> parent of f595f90 (Merge branch 'Speed-Multiplier' into dev)
    }
}