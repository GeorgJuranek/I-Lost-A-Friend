using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartphoneController : MonoBehaviour
{
    bool isActivated = false;

    [SerializeField]
    Light saveLight;

    [SerializeField]
    Material materialOn, materialOff;

    [SerializeField]
    MeshRenderer materialsRenderer;


    [SerializeField]
    AudioSource[] audioSources;

    bool wasAlarmStoped;


    private void Start()
    {
        if (saveLight != null)
            saveLight.enabled = false;

        SwitchRepresentation(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isActivated = !isActivated;

            SwitchRepresentation(isActivated);

            StopAlarmSounds();
        }
    }


    void SwitchRepresentation(bool isTurnedOn)
    {
        var mats = materialsRenderer.materials;

        if (isTurnedOn)
        {
            mats[0] = materialOff;
            mats[1] = materialOn;

            if (saveLight != null)
                saveLight.enabled = true;
        }
        else
        {
            mats[0] = materialOff;
            mats[1] = materialOff;

            if (saveLight != null)
                saveLight.enabled = false;
        }

        materialsRenderer.materials = mats;
    }



    void StopAlarmSounds()
    {
        if (wasAlarmStoped) return;


        wasAlarmStoped = true;

        if (audioSources.Length > 0)
        {
            foreach (var alarm in audioSources)
            {
                if (alarm.isPlaying)
                    alarm.Stop();
            }
        }

    }
}

