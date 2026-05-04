using UnityEngine;

public class SceneIntroVoice : MonoBehaviour
{
    [Header("Intro Audio")]
    public AudioClip introClip;

    void Start()
    {
        // We use Start() so the UniversalVoiceManager has time to wake up first!
        if (introClip != null && UniversalVoiceManager.Instance != null)
        {
            UniversalVoiceManager.Instance.PlayVoice(introClip);
        }
        else if (UniversalVoiceManager.Instance == null)
        {
            Debug.LogWarning("Cannot play intro voice: UniversalVoiceManager is missing!");
        }
    }
}