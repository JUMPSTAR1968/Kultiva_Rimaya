using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;

    void Start()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        // SAFETY: Make sure audio is always unpaused when a scene starts!
        AudioListener.pause = false;
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    // --- 1. RESUME ---
    public void ResumeGame()
    {
        StartCoroutine(ResumeRoutine());
    }

    private IEnumerator ResumeRoutine()
    {
        yield return new WaitForSecondsRealtime(1f);

        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    // --- 2. RESTART ---
    public void RestartLevel()
    {
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- 3. MAIN MENU ---
    public void LoadMainMenu()
    {
        StartCoroutine(MainMenuRoutine());
    }

    private IEnumerator MainMenuRoutine()
    {
        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("A1_Main Menu");
    }
}