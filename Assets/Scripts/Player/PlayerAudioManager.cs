using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAudioManager : MonoBehaviour
{

    [SerializeField] AudioContainer walkingSounds;
    [SerializeField] AudioContainer jumpSounds;
    [SerializeField] AudioContainer fallSounds;
    [SerializeField] AudioContainer painSounds;
    [SerializeField] AudioContainer annoyanceSounds;
    [SerializeField] AudioContainer checkpointSounds;
    [SerializeField] AudioContainer dropSounds;

    AudioSource audioSource;
    PlayerInput playerInput;
    InputAction move, sprint;

    bool isInAir = false;
    int count = 0;
    float startPitch;
    int currentPriority = -1;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        startPitch = audioSource.pitch;

        playerInput = new PlayerInput();
        playerInput.Enable();

        move = playerInput.Player.Move;
        sprint = playerInput.Player.Sprint;

    }


    private void OnEnable()
    {
        move.Enable();
        sprint.Enable();

        PlayerMovement.OnIsInAir += PlayRandomJump;
        PlayerMovement.OnHasLanded += PlayRandomLanded;

        PlayerReseter.OnPlayerReset += PlayRandomAnnoyance;


        sprint.started += RegulatePitchUp;
        sprint.canceled += RegulatePitchDown;


        PlayerDeathInitiator.OnPlayerPain += PlayRandomPain;
        SavePoint.OnPlayerWasSaved += PlayPassedCheckpoint;


        Holder.OnDropDenied += PlayRandomAnnoyance;
        Holder.OnDropSuccess += PlayRandomDrop;
    }

    private void OnDisable()
    {
        move.Disable();
        sprint.Disable();

        PlayerMovement.OnIsInAir -= PlayRandomJump;
        PlayerMovement.OnHasLanded -= PlayRandomLanded;

        PlayerReseter.OnPlayerReset -= PlayRandomAnnoyance;

        sprint.started -= RegulatePitchUp;
        sprint.canceled -= RegulatePitchDown;


        PlayerDeathInitiator.OnPlayerPain -= PlayRandomPain;
        SavePoint.OnPlayerWasSaved -= PlayPassedCheckpoint;


        Holder.OnDropDenied -= PlayRandomAnnoyance;
        Holder.OnDropSuccess -= PlayRandomDrop;
    }

    private void Update()
    {
        if (move.IsPressed() && !isInAir)
        {
            PlayWalking();
        }
    }

    private void RegulatePitchUp(InputAction.CallbackContext context)
    {
        audioSource.pitch = startPitch - 0.1f;

    }

    private void RegulatePitchDown(InputAction.CallbackContext context)
    {
        audioSource.pitch = startPitch;
    }



    void PlayWalking()
    {
        if (Time.timeScale == 0) return;
        if (audioSource.isPlaying) return;
    
    
        if (count >= walkingSounds.audioClips.Length)
            count = 0;
    
    
        audioSource.clip = walkingSounds.audioClips[count];
    
        if (walkingSounds.audioClips[count] != null)
        {
            audioSource.Play();
            currentPriority = walkingSounds.priority;
            count++;
        }
    }


    void PlayRandomDrop()
    {
        PlaySoundOneShot(dropSounds);
    }

    void PlayPassedCheckpoint()
    {
        PlaySoundOneShot(checkpointSounds);
    }


    void PlayRandomAnnoyance()
    {
        PlaySoundOneShot(annoyanceSounds);
    }

    void PlayRandomPain()
    {
        PlaySoundOneShot(painSounds);
    }

    void PlayRandomJump()
    {
        isInAir = true;
        PlaySoundOneShot(jumpSounds);
    }

    void PlayRandomLanded()
    {
        isInAir = false;
        PlaySoundOneShot(fallSounds);
    }


    void PlaySound(AudioContainer audioContainer)
    {
        if (Time.timeScale == 0 || audioContainer.audioClips.Length == 0) return;

        int next = Random.Range(0, audioContainer.audioClips.Length);
        AudioClip selectedClip = audioContainer.audioClips[next];

        if (selectedClip != null && audioContainer.priority >= currentPriority)
        {
            audioSource.Stop();
            audioSource.clip = selectedClip;
            audioSource.Play();
            currentPriority = audioContainer.priority;
        }

        currentPriority = -1;
    }

    void PlaySoundOneShot(AudioContainer audioContainer)
    {
        if (Time.timeScale == 0 || audioContainer.audioClips.Length == 0) return;

        int next = Random.Range(0, audioContainer.audioClips.Length);
        AudioClip selectedClip = audioContainer.audioClips[next];

        if (selectedClip != null && audioContainer.priority > currentPriority)
        {
            audioSource.PlayOneShot(selectedClip);
            currentPriority = audioContainer.priority;
        }

        currentPriority = -1;
    }


    void PlayRandom(AudioClip[] audioClips)
    {
        if (Time.timeScale == 0) return;

        audioSource.Stop();

        int next = Random.Range(0, audioClips.Length);

        audioSource.clip = audioClips[next];

        if (audioClips[next] != null)
            audioSource.Play();
    }

    void PlayRandomOneshot(AudioClip[] audioClips)
    {
        if (Time.timeScale == 0) return;

        audioSource.Stop();

        int next = Random.Range(0, audioClips.Length);

        audioSource.clip = audioClips[next];

        if (audioClips[next] != null)
            audioSource.PlayOneShot(audioClips[next]);
    }


    //Container
    [System.Serializable]
    public class AudioContainer
    {
        public int priority;
        public AudioClip[] audioClips;
    }
}
