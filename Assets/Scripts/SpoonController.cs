using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpoonController : MonoBehaviour
{
    [SerializeField]
    ObjectCounter objectCounter;

    [SerializeField]
    List<Requirement> requirements;

    [SerializeField]
    Transform target;

    [SerializeField]
    float upwartsSpeed = 2.5f, downwardsSpeed = 5f;

    Vector3 startposition;


    private void Start()
    {
        startposition = transform.position;
    }

    private void OnEnable()
    {
        objectCounter.OnCountChange += CheckRequirements;
    }

    private void OnDisable()
    {
        objectCounter.OnCountChange -= CheckRequirements;
    }

    Coroutine currentCoroutine = null;
    void CheckRequirements()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        if (HasAllRequirements())
        {
            currentCoroutine = StartCoroutine(ConstantCheckingWhileMoving(target.position));
        }
        else
        {
            currentCoroutine = StartCoroutine(ConstantFalseWhileMoving(startposition));
        }
    }


    IEnumerator ConstantCheckingWhileMoving(Vector3 targetposition)
    {
        while (HasAllRequirements() && Vector3.Distance(transform.position, targetposition) > 0.1f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetposition, upwartsSpeed * Time.deltaTime);

            yield return null;
        }

        currentCoroutine = null;
    }

    IEnumerator ConstantFalseWhileMoving(Vector3 targetposition)
    {
        while (!HasAllRequirements() && Vector3.Distance(transform.position, targetposition) > 0.1f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetposition, downwardsSpeed * Time.deltaTime);

            yield return null;
        }

        currentCoroutine = null;
    }

    bool HasAllRequirements()
    {
        foreach (var requirement in requirements)
        {
            if (!objectCounter.AnalyseCountedForAmount(requirement.tagToFind, requirement.amountToFind, false))
            {
                return false;
            }
        }

        return true;
    }

    [System.Serializable]
    public struct Requirement
    {
        public string tagToFind;
        public int amountToFind;
    }


    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
