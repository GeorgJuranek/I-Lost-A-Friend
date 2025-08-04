using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyNet : MonoBehaviour
{
    //This class only is a 'plan B' if reseter-logic failed at some point//

    private void OnTriggerEnter(Collider other)
    {
        BaseReseter reseter = null;

        if (other.TryGetComponent<BaseReseter>(out reseter))
        {
            Debug.LogWarning($"{other.name} at {other.transform.position} landed in safetynet");
            reseter.ResetObject(false);
        }
        else if (other.GetComponentInChildren<BaseReseter>())
        {
            Debug.LogWarning($"{other.name} at {other.transform.position} landed in safetynet");

            reseter = other.GetComponentInChildren<BaseReseter>();
            reseter.ResetObject(false);
        }
        else
        {
            Debug.LogError($"Some unexspected fall happened at {other.transform.position} with {other.name} and NO reseter was on it");
            Destroy(other.gameObject);
            Debug.LogWarning($"Destroyed {other.name} at {other.transform.position}.");
        }
    }
}
