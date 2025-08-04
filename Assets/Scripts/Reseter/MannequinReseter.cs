using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannequinReseter : BaseReseter
{
    Vector3 lastSafePosition;

    RagdollController ragdollController;

    MannequinPool pool;

    private void Awake()
    {
        pool = MannequinPool.Current;
    }

    private void Start()
    {
        lastSafePosition = transform.position;
        ragdollController = GetComponent<RagdollController>();
    }

    public override void ResetObject(bool shallSpawn)
    {
        if (shallSpawn)
        {
            SpawnPrefab(null, transform.position, Vector3.zero, Quaternion.identity); 
        }

        if (this.GetComponentInChildren<INestable>().TemporaryParent != null)
            this.GetComponentInChildren<INestable>().Unnest();

        ragdollController.TeleportRigidbodies(transform.position);//Resets the ragdoll on current position

        //send to pool
        if (pool != null)
        {
            pool.Insert(this.gameObject);
        }
        else
        {
            Debug.LogError("NO mannequinpool was found by MannequinReseter!");
        }

    }

    public override void SpawnPrefab(GameObject prefabToSpawn, Vector3 originalSpawnPosition, Vector3 spawnPositionOffset, Quaternion startRotation)
    {
        Debug.LogWarning("Mannequins Spawn-method was called.");
    }
}
