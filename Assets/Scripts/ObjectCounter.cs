using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCounter : MonoBehaviour
{
    List<GameObject> objects = new List<GameObject>();

    public Action OnCountChange;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Mannequin"))
        {
            if (!objects.Contains(other.gameObject))
                objects.Add(other.gameObject);

            OnCountChange?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Mannequin"))
        {
            if (objects.Contains(other.gameObject))
                objects.Remove(other.gameObject);

            OnCountChange?.Invoke();
        }
    }

    public bool AnalyseCountedFor(string tagToSearch)
    {
        return AnalyseCountedForAmount(tagToSearch, 1, false);
    }


    public bool AnalyseCountedForAmount(string tagToSearch, int minAmount, bool shallHasExactAmount)
    {
        int counted = 0;
        foreach (var objectItem in objects)
        {
            if (objectItem.CompareTag(tagToSearch))
            {
                counted++;
            }
        }

        //Check Condition
        if(shallHasExactAmount)
        {
            if (counted == minAmount)
            {
                return true;
            }
        }
        else
        {
            if(counted >= minAmount)
            {
                return true;
            }
        }

        return false;
    }

    public bool AnalyseCountedForComponent(Component toSearch, bool shallCheckChildren)
    {
        return AnalyseCountedForComponentAmount(toSearch, 1, false, shallCheckChildren);
    }


    public bool AnalyseCountedForComponentAmount(Component toSearch, int minAmount, bool shallHasExactAmount, bool shallCheckChildren)
    {
        if (objects.Count == 0)
        {
            Debug.LogWarning("Nothing in ObjectCounter");
            return false;
        }

        int counted = 0;
        foreach (var objectItem in objects)
        {
            if (objectItem.TryGetComponent<Component>(out Component foundComponent)) //for main object
            {
                ++counted;
            }
            else
            {
                if (shallCheckChildren)
                {
                    if (objectItem.GetComponentInChildren<Component>()) //for its children
                    {
                        ++counted;
                    }
                }
            }


            if (shallHasExactAmount)
            {
                if (counted == minAmount)
                {
                    return true;
                }
            }
            else
            {
                if (counted >= minAmount)
                {
                    return true;
                }
            }
        }


        return false;
    }

    // static option
    static public bool AnalyseCountedForMinAmount(string tagToSearch, int minAmount, List<GameObject> listToSearch)
    {
        if (listToSearch.Count == 0)
        {
            Debug.LogWarning("Nothing to search in list");
            return false;
        }

        int counted = 0;
        foreach (var objectItem in listToSearch)
        {
            if (objectItem.CompareTag(tagToSearch))
            {
                ++counted;
            }
        }

        if (counted >= minAmount)
        {
            return true;
        }

        return false;
    }

}
