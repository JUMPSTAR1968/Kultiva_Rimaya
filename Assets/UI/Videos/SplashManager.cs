using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    [Header("Next Scene")]
    public string nextSceneName = "A1_Main Menu"; // Make sure this matches exactly!

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        // This tells Unity: "When the video reaches the end, run the OnVideoComplete function"
        videoPlayer.loopPointReached += OnVideoComplete;
    }

    void OnVideoComplete(VideoPlayer vp)
    {
        // Video is done! Load the main menu.
        SceneManager.LoadScene(nextSceneName);
    }
}