using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UniversalUIManager : MonoBehaviour
{
    // --- THE SINGLETON MAGIC ---
    public static UniversalUIManager Instance;

    [Header("Global Settings")]
    public float transitionDelay = 0.6f;
    public AudioClip globalClickSound;

    private void Awake()
    {
        // This ensures only ONE Universal Manager ever exists
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // --- FOR STANDARD UNITY BUTTONS (Panels & Scenes) ---
    public void LoadScene(string sceneName)
    {
        StartCoroutine(DelayRoutine(() => SceneManager.LoadScene(sceneName)));
    }

    public void OpenPanel(GameObject panel)
    {
        StartCoroutine(DelayRoutine(() => panel.SetActive(true)));
    }

    public void ClosePanel(GameObject panel)
    {
        StartCoroutine(DelayRoutine(() => panel.SetActive(false)));
    }

    // --- FOR INSTANT ACTIONS (No Delay, Just Sound) ---
    public void PlayClickSound()
    {
        if (globalClickSound != null)
        {
            GameObject soundObj = new GameObject("UniversalSound");
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = globalClickSound;
            source.Play();
            DontDestroyOnLoad(soundObj);
            Destroy(soundObj, globalClickSound.length);
        }
    }

    // --- FOR YOUR CUSTOM SCRIPTS (The true universal power) ---
    // Any other script can pass its logic through here to get delayed!
    public void TriggerCustomAction(System.Action customAction)
    {
        StartCoroutine(DelayRoutine(customAction));
    }

    // --- THE ENGINE ---
    private IEnumerator DelayRoutine(System.Action action)
    {
        if (globalClickSound != null)
        {
            GameObject soundObj = new GameObject("UniversalSound");
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = globalClickSound;
            source.Play();
            DontDestroyOnLoad(soundObj);
            Destroy(soundObj, globalClickSound.length);
        }

        // --- THE FIX: Use Realtime so it works while paused! ---
        yield return new WaitForSecondsRealtime(transitionDelay);
        action?.Invoke();
    }
}