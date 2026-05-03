using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events; // Required to make actions universal!
using System.Collections;

public class UI_ButtonUniversal : MonoBehaviour
{
    [Header("Settings")]
    public float transitionDelay = 0.6f;
    public AudioClip clickSound;

    [Header("Non-Scene Actions (Panels, UI, etc.)")]
    [Tooltip("Put the things you want to happen AFTER the delay here!")]
    public UnityEvent delayedActions;

    // 1. For Scene Transitions (Your original code, untouched)
    public void GoToScene(string sceneName)
    {
        StartCoroutine(ExecuteWithDelay(() => SceneManager.LoadScene(sceneName)));
    }

    // 2. For Quitting Game (Now with a delay!)
    public void QuitGameDelayed()
    {
        StartCoroutine(ExecuteWithDelay(() => {
            Debug.Log("Quitting...");
            Application.Quit();
        }));
    }

    // 3. For EVERYTHING ELSE! (Closing panels, Meet the Team clicks, etc.)
    public void TriggerDelayedAction()
    {
        StartCoroutine(ExecuteWithDelay(() => delayedActions.Invoke()));
    }

    // --- THE UNIVERSAL COROUTINE ---
    // This takes ANY action (Scene load, Quit, or UnityEvent) and delays it
    private IEnumerator ExecuteWithDelay(System.Action actionToPerform)
    {
        // 1. Play the Ghost Sound
        if (clickSound != null)
        {
            GameObject soundObj = new GameObject("TempSound");
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = clickSound;
            source.Play();
            DontDestroyOnLoad(soundObj);
            Destroy(soundObj, clickSound.length);
        }

        // 2. Wait for the visual Ghost Effect
        yield return new WaitForSeconds(transitionDelay);

        // 3. Execute whatever action was passed in!
        actionToPerform?.Invoke();
    }
}