using UnityEngine;
using UnityEngine.SceneManagement; // Useful for restarting later

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // This lets other scripts find the manager easily

    [Header("Global Stats")]
    public int sharedHealth = 3;
    public bool isGameOver = false;

    void Awake()
    {
        // Singleton pattern: ensures only one manager exists
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LoseHealth()
    {
        if (isGameOver) return;

        sharedHealth--;
        Debug.Log("Shared Health: " + sharedHealth);

        if (sharedHealth <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        isGameOver = true;
        Debug.Log("GAME OVER!");

        // This stops the game physics and time-based movements
        Time.timeScale = 0f;

        // Note: Your scrolling scripts might need a small check for this (see Step 3)
    }
}