using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saver : MonoBehaviour
{
    public void TryToSaveFromCollision(Collider other)
    {
        PlayerMovement playerMovement = null;
        GameObject mainObject = null;

        if (!other.TryGetComponent<PlayerMovement>(out playerMovement))
        {
            playerMovement = other.gameObject.transform.root.gameObject.GetComponent<PlayerMovement>();
            mainObject = other.gameObject.transform.root.gameObject;
        }
        else
        {
            playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            mainObject = other.gameObject;
        }

        playerMovement.lastSavePosition = mainObject.transform.position;
        playerMovement.lastSaveRotation = mainObject.transform.rotation;
    }


    public void EasySave()
    {
        GameObject player = FindAnyObjectByType<PlayerMovement>().gameObject;
        TryToSaveFromCollision(player.GetComponent<Collider>());
    }
}
