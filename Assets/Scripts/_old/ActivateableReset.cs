using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateableReset : MonoBehaviour, IActivateable
{

    [SerializeField]
    MannequinReseter mannequinReseter;


    public void Enable()
    {
        Debug.Log("ENABLE IS WORKING!");
    }

    public void Disable()
    {
        mannequinReseter.ResetObject(false);
        Debug.Log("DISABLE IS WORKING!");
    }
}
