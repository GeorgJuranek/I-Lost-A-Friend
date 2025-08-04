using System.Collections;
using UnityEngine;

public class FallDetector : MonoBehaviour
{
    BaseReseter reseter;

    private Coroutine fallCoroutine;

    [SerializeField]
    float maxFallTimeInSeconds = 3;

    [SerializeField]
    GroundChecker groundChecker;

    //[SerializeField]
    float checkingDistanceInFall = Mathf.Infinity;

    private void Start()
    {
        reseter = GetComponent<BaseReseter>();
    }

    public void StartFalling()
    {
        if (fallCoroutine == null)
        {
            fallCoroutine = StartCoroutine(CountingForDeath());
        }
    }

    IEnumerator CountingForDeath()
    {
        float countingForDeath = 0;

        while (countingForDeath < maxFallTimeInSeconds)
        {
            if (groundChecker.GroundCheckAny(checkingDistanceInFall))//Pause if anything underneath could stop fall
            {
                if (this.GetComponentInChildren<Rigidbody>().velocity.magnitude < 0.1f) //Break if targets fall was stopped before reaching maxFallTime
                {
                    StopFalling();
                    yield break;
                }

                yield return null;
            }

            countingForDeath += Time.deltaTime;
            yield return null;
        }

        reseter.ResetObject(false);
        fallCoroutine = null;
    }

    public void StopFalling()
    {
        if (fallCoroutine != null)
        {
            StopCoroutine(fallCoroutine);
            fallCoroutine = null;
        }
    }

    public bool IsInCoroutine()
    {
        return fallCoroutine != null;
    }


    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
