using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IActivateable activateable))
        {
            activateable.Enable();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IActivateable deactivateable))
        {
            deactivateable.Disable();
        }
    }
}
