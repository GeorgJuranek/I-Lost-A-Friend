using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnAmountActivator : MonoBehaviour
{
    [SerializeField]
    ObjectCounter objectCounter;

    [SerializeField]
    List<Requirement> requirements;

    [SerializeField]
    private UnityEvent OnRequirementsFullfilled;

    [SerializeField]
    private UnityEvent OnRequirementsNotFullfilled;

    Mover mover;

    private void Start()
    {
        CheckRequirements();
    }

    private void OnEnable()
    {
        objectCounter.OnCountChange += CheckRequirements;
    }

    private void OnDisable()
    {
        objectCounter.OnCountChange -= CheckRequirements;
    }

    void CheckRequirements()
    {
        if (HasAllRequirements())
        {
            OnRequirementsFullfilled.Invoke();
        }
        else
        {
            OnRequirementsNotFullfilled.Invoke();
        }
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
}
