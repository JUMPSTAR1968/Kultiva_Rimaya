using UnityEngine;

public class UniversalVoiceManager : MonoBehaviour
{
    // --- THE SINGLETON ---
    public static UniversalVoiceManager Instance;

    [Header("Audio Components")]
    public AudioSource voiceSource;

    private void Awake()
    {
        // Ensure only one Voice Manager exists across all scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return; // CRITICAL: Stop running if this is a duplicate!
        }

        // --- MOVED TO AWAKE: Sets up the audio before any other script can ask for it ---
        if (voiceSource == null)
        {
            voiceSource = gameObject.AddComponent<AudioSource>();
        }

        // Immunity from the Pause Menu mute!
        voiceSource.ignoreListenerPause = true;

        // Ensure it doesn't play randomly on awake
        voiceSource.playOnAwake = false;
    }

    // You can completely delete the Start() function now!

    // --- THE UNIVERSAL TRIGGER ---
    public void PlayVoice(AudioClip clipToPlay)
    {
        if (clipToPlay == null) return;

        // CRITICAL: Stop any currently playing voice so they don't talk over each other!
        if (voiceSource.isPlaying)
        {
            voiceSource.Stop();
        }

        // Load the new voice and play it
        voiceSource.clip = clipToPlay;
        voiceSource.Play();
    }

    // Optional: Call this if you want a "Stop Talking" button
    public void StopVoice()
    {
        if (voiceSource.isPlaying)
        {
            voiceSource.Stop();
        }
    }
}