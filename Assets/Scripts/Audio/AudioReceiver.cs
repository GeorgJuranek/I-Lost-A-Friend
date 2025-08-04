using UnityEngine;
using UnityEngine.Audio;

public class AudioReceiver : MonoBehaviour
{
    [SerializeField]
    bool isBackgroundMusicSource;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioReceiver NEEDS AudioSource to be on the same gameObject!");
            return;
        }

        if (AudioManager.instance != null)
        {
            AudioMixerGroup targetGroup = isBackgroundMusicSource
                ? AudioManager.instance.AudioMixer.FindMatchingGroups("Master/BackgroundVolume")[0]
                : AudioManager.instance.AudioMixer.FindMatchingGroups("Master/SFXVolume")[0];

            audioSource.outputAudioMixerGroup = targetGroup;
        }
    }
}
