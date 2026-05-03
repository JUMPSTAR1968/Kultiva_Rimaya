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

        // NEW: This completely freezes ALL music and sound effects!
        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        StartCoroutine(ResumeRoutine());
    }

    IEnumerator ResumeRoutine()
    {
        yield return new WaitForSecondsRealtime(0.15f);

        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        // NEW: Unpause the audio
        AudioListener.pause = false;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("A1_Main Menu");
    }
}