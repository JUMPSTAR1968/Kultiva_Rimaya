using UnityEngine;

public class VoiceButton : MonoBehaviour
{
    [Header("Assign the Voice Here!")]
    public AudioClip myVoiceClip;

    // The button will trigger this, and this script will find the true Singleton!
    public void Speak()
    {
        if (myVoiceClip != null && UniversalVoiceManager.Instance != null)
        {
            UniversalVoiceManager.Instance.PlayVoice(myVoiceClip);
        }
    }
}