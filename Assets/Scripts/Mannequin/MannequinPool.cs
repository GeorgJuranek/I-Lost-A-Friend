using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannequinPool : MonoBehaviour
{
    // Static Reference
    public static MannequinPool Current { get; private set; }

    private Queue<GameObject> pool = new Queue<GameObject>();

    private List<GameObject> allEntries;

    [SerializeField]
    int maximumAmount = 30;

    void Awake()
    {
        if (Current != null && Current != this)
        {
            Debug.LogWarning("More than one pool in scene, new pool is used");
        }

        Current = this;


        allEntries = new List<GameObject>();
    }

    public void Insert(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    public GameObject Release(Vector3 releasePosition)
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            if (obj != null)
            {
                ActivateAt(obj, releasePosition);
                SetSpecificToListEnd(obj, allEntries);
            }
            return obj;
        }
        else
        {
            return null;
        }
    }

    private void ActivateAt(GameObject obj, Vector3 releasePosition)
    {
        obj.SetActive(true);
        RagdollController controller = obj.GetComponent<RagdollController>();
        if (controller != null)
        {
            controller.TeleportRigidbodies(releasePosition);
        }
        else
        {
            Debug.LogWarning($"{obj.name} has no RagdollController!");
        }
    }

    public bool HasObjectInPool()
    {
        return pool.Count > 0;
    }

    public GameObject InstantiatePoolObject(GameObject prefab, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject newEntry;

        if (allEntries.Count < maximumAmount) //creates new
        {
            newEntry = Instantiate(prefab, spawnPosition, spawnRotation);
            allEntries.Add(newEntry);
        }
        else //using the oldest in List
        {
            newEntry = allEntries[0];
            allEntries.RemoveAt(0);
            allEntries.Add(newEntry);
        }

        ActivateAt(newEntry, spawnPosition);
        return newEntry;
    }

    void SetSpecificToListEnd(GameObject selected, List<GameObject> allEntries)
    {
        if (allEntries.Remove(selected))
        {
            allEntries.Add(selected);
        }
    }
}

