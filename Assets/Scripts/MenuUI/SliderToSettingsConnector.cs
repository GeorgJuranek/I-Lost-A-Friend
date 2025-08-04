using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderToSettingsConnector : MonoBehaviour
{
    [SerializeField]
    Slider sensitivitySlider;

    [SerializeField]
    Slider musicSlider;

    [SerializeField]
    Slider sfxSlider;


    private void OnEnable()
    {
        SetAllValues();
    }

    private void OnDisable()
    {
        AudioManager.instance.UpdateValues(musicSlider.value, sfxSlider.value, sensitivitySlider.value);
    }

    private void SetAllValues()
    {
        SettingsValues values = AudioManager.instance.GetValues();

        musicSlider.value = values.backgroundVolume;
        sfxSlider.value = values.sfxVolume;
        sensitivitySlider.value = values.mouseSensitivity;

        SetMusicVolume();
        SetSFXVolume();
        SetLookSensitivity();

        AudioManager.instance.UpdateValues(musicSlider.value, sfxSlider.value, sensitivitySlider.value);

    }

    public void SetLookSensitivity()
    {
        AudioManager.instance.SetLookSensitivity(sensitivitySlider.value);
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        AudioManager.instance.SetMusicVolume(volume);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        AudioManager.instance.SetSFXVolume(volume);
    }
}
