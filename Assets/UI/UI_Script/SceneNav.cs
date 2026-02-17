using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Required for Coroutines

public class SceneNav : MonoBehaviour
{
    [Header("Settings")]
    public float transitionDelay = 0.6f; // Time to wait for the Ghost Animation
    public AudioClip clickSound;

    // Link this to your Button
    public void GoToScene(string sceneName)
    {
        StartCoroutine(LoadWithDelay(sceneName));
    }

    IEnumerator LoadWithDelay(string sceneName)
    {
        // 1. Play the sound (using the 'Ghost Speaker' method from before)
        if (clickSound != null)
        {
            GameObject soundObj = new GameObject("TempSound");
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = clickSound;
            source.Play();
            DontDestroyOnLoad(soundObj);
            Destroy(soundObj, clickSound.length);
        }

        // 2. WAIT! Let the visual 'Ghost' effect play out
        // (The button script handles the visual, we just wait for it here)
        yield return new WaitForSeconds(transitionDelay);

        // 3. NOW load the scene
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}