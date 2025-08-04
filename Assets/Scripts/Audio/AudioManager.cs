using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    AudioMixer audioMixer;

    [SerializeField]
    float maxSensitivity = 100f, minSensitivity = 5f;

    [SerializeField]
    float startValue = 0.5f;

    public AudioMixer AudioMixer { get => audioMixer; private set { audioMixer = value; } }

    private CameraControl cameraControl;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


        //INIT
        UpdateValues(startValue, startValue, startValue);//sets to a default first

        OnSceneLoaded();
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnSceneLoaded();
    }

    private void OnSceneLoaded()
    {
        cameraControl = null;

        SetMusicVolume(values.backgroundVolume);
        SetSFXVolume(values.sfxVolume);

        cameraControl = FindObjectOfType<CameraControl>();
        if (cameraControl != null)
        {
            SetLookSensitivity(values.mouseSensitivity);
        }
        else
        {
            Debug.LogWarning("No CameraControl-script found in scene by SliderToSettingsConnector!");
        }
    }


    SettingsValues values = new SettingsValues();

    public void UpdateValues(float background, float sfx, float sensitivity)
    {
        values.backgroundVolume = background;
        values.sfxVolume = sfx;
        values.mouseSensitivity = sensitivity;
    }
    public SettingsValues GetValues()
    {
        return values;
    }



    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
        {
            volume = Mathf.Clamp01(volume);
            if (volume <= 0)
                volume = 0.0001f;

            audioMixer.SetFloat("BackgroundVolume", Mathf.Log10(volume) * 20); //to decibel
        }
        else
        {
            Debug.LogError("No audioMixer found!");
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
        {
            volume = Mathf.Clamp01(volume);
            if (volume <= 0)
                volume = 0.0001f;

            audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20); //to decibel
        }
        else
        {
            Debug.LogError("No audioMixer found!");
        }
    }

    public void SetLookSensitivity(float sensitivity)
    {
        if (cameraControl != null)
        {
            sensitivity *= maxSensitivity;
            cameraControl.MouseSpeed = Mathf.Clamp(sensitivity * maxSensitivity, minSensitivity, maxSensitivity);
        }
    }

}


public struct SettingsValues
{
    public float backgroundVolume;
    public float sfxVolume;
    public float mouseSensitivity;
}
