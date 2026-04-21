using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSelectManager : MonoBehaviour
{




    public void NextMenu(GameObject menuUI)
    {
        this.gameObject.SetActive(false);
        menuUI.SetActive(true);
    }
    public void OpenMenu(GameObject menuUI)
    {
        menuUI.SetActive(true);
    }


    // --- 1. LINK THESE TO YOUR GAME CHOICE BUTTONS ---
    public void SelectTatlongBibe()
    {
        GameSettings.TargetScene = "C1_May Tatlong Bibe"; // Matches your scene name exactly
        Debug.Log("Selected: " + GameSettings.TargetScene);
        // Note: You would open your Difficulty UI Panel here!
    }

    public void SelectBahayKubo()
    {
        GameSettings.TargetScene = "C2_Bahay Kubo"; // Matches your scene name exactly
        Debug.Log("Selected: " + GameSettings.TargetScene);
        // Note: You would open your Difficulty UI Panel here!
    }

    // --- 2. LINK THESE TO YOUR DIFFICULTY BUTTONS ---
    public void PlayEasy() { LaunchGame(Difficulty.Easy); }
    public void PlayMedium() { LaunchGame(Difficulty.Medium); }
    public void PlayHard() { LaunchGame(Difficulty.Hard); }

    private void LaunchGame(Difficulty diff)
    {
        // Safety check to ensure a game was picked first
        if (string.IsNullOrEmpty(GameSettings.TargetScene))
        {
            Debug.LogWarning("Wait! You need to pick a game first.");
            return;
        }

        GameSettings.CurrentDifficulty = diff;
        SceneManager.LoadScene(GameSettings.TargetScene);
    }
}