using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Killer : MonoBehaviour
{
    static public Action OnPlayerPain;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerDeathInitiator>().StartDeath();

        }
    }

    private void OnTriggerStay(Collider other) //Not OnTriggerEnter bc is more smooth this way
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerDeathInitiator>().StayInDeath();

        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerDeathInitiator>().CancelDeath();

        }

    }

}
