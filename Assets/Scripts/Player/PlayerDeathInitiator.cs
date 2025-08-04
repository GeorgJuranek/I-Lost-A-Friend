using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathInitiator : MonoBehaviour
{
    float pain = 0f;
    const float deathAmount = 1f;

    Coroutine coroutine = null;

    static public Action OnPlayerPain;

    Nester nester;
    Rigidbody rigidBody;
    PlayerMovement playerMovement;
    PlayerReseter reseter;

    [SerializeField]
    Holder holder;


    private void Awake()
    {
        nester = GetComponent<Nester>();
        rigidBody = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        reseter = GetComponent<PlayerReseter>();
    }

    public void StartDeath()
    {
        if (coroutine != null) return;

            rigidBody.velocity = Vector3.zero;
    }

    public void StayInDeath() //Not OnTriggerEnter bc is more smooth this way
    {
        if (coroutine != null) return;


            OnPlayerPain?.Invoke();//for postrendering Volume Effect

        if (nester.TemporaryParent != null)
            nester.Unnest();

            if (coroutine == null)
                coroutine = StartCoroutine(WaitForDeath());
        
    }

    public void CancelDeath()
    {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            playerMovement.isUnableToMove = false;
            pain = 0f;

    }



    IEnumerator WaitForDeath()
    {
        playerMovement.LastCameraRotation = Camera.main.transform.rotation; //camera rotation in the EXACT moment of death

        pain = 0f;

        playerMovement.GetComponent<PlayerMovement>().isUnableToMove = true;
        holder.Drop();


        while (pain < deathAmount)
        {
            pain += Time.deltaTime;

            yield return null;
        }

        if (nester.TemporaryParent!=null)
            nester.Unnest();

        playerMovement.LastCameraRotation = Camera.main.transform.rotation;
        playerMovement.isUnableToMove = true;

        holder.Drop();


        playerMovement.isUnableToMove = false;
        reseter.ResetObject(true);

        coroutine = null;

    }
}
