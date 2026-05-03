using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSelectManager : MonoBehaviour
{
    public void SelectTatlongBibe()
    {
        GameSettings.TargetScene = "C1_May Tatlong Bibe";
    }

    public void SelectBahayKubo()
    {
        GameSettings.TargetScene = "C2_Bahay Kubo";
    }

    public void PlayEasy() { LaunchGame(Difficulty.Easy); }
    public void PlayMedium() { LaunchGame(Difficulty.Medium); }
    public void PlayHard() { LaunchGame(Difficulty.Hard); }

    private void LaunchGame(Difficulty diff)
    {
        if (string.IsNullOrEmpty(GameSettings.TargetScene))
        {
            Debug.LogWarning("Wait! You need to pick a game first.");
            return;
        }

        GameSettings.CurrentDifficulty = diff;

        // This instantly borrows the audio and delay from the Universal Manager!
        UniversalUIManager.Instance.TriggerCustomAction(() =>
        {
            SceneManager.LoadScene(GameSettings.TargetScene);
        });
    }
}