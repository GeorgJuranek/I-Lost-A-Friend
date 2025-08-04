using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookController : MonoBehaviour
{
    /// <summary>
    /// This is the first version of the SpoonController, had some issues with using
    /// spoonController on hook after changes and made this script for testing comparison
    /// </summary>
    ///


    [SerializeField]
    ObjectCounter objectCounter;

    //[SerializeField]
    Mover mover;

    [SerializeField]
    List<Requirement> requirements;


    [SerializeField]
    Transform target;

    private void Start()
    {
        mover = GetComponent<Mover>();
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
            Debug.Log("CheckRequirements was succesful");
            mover.MoveToPosition(target);
        }
        else
        {
            Debug.Log("CheckRequirements was NOT succesful");
            //To Do:maybe extra check here if is already home
            mover.ComeBackHome();
        }
    }

    bool HasAllRequirements()
    {
        foreach (var requirement in requirements)
        {
            if (!objectCounter.AnalyseCountedForAmount(requirement.tagToFind, requirement.amountToFind, false))
            {
                Debug.Log("Got NOT all requirements");
                return false;
            }
        }

        Debug.Log("Got all requirements");
        return true;
    }

    [System.Serializable]
    public struct Requirement
    {
        public string tagToFind;
        public int amountToFind;
    }
}
