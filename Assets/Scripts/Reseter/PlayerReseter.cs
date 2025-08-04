using System;
using UnityEngine;

public class PlayerReseter : BaseReseter
{
    PlayerMovement playerMovement;

    [SerializeField]
    GameObject prefabToSpawn;

    Vector3 lastSafePosition;
    Quaternion lastSafeRotation;

    Quaternion lastCameraRotation;

    public static Action OnPlayerReset;

    [SerializeField]
    Holder holder;

    [SerializeField]
    float spawnObjectYOffsetFactor = 0.6f;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        lastSafePosition = playerMovement.lastSavePosition;
        lastSafeRotation = playerMovement.lastSaveRotation;
    }


    public override void ResetObject(bool shallSpawn)
    {
        lastCameraRotation = playerMovement.LastCameraRotation;//Update

        if (prefabToSpawn!=null && shallSpawn)
        {
            Vector3 deathPosition = transform.position;

            //if IGrabable
            Vector3 startRotation = prefabToSpawn.gameObject.GetComponentInChildren<IGrabable>().getSpawnRotation();// ask prefab how it wants to be rotated in the first place

            SpawnPrefab(prefabToSpawn, deathPosition, Vector3.up * spawnObjectYOffsetFactor, Quaternion.Euler(startRotation.x, lastCameraRotation.eulerAngles.y, startRotation.z)); //puts camera rotation as y-rotation
        }

        lastSafePosition = playerMovement.lastSavePosition;//Update
        lastSafeRotation = playerMovement.lastSaveRotation;//Update

        transform.position = lastSafePosition;
        transform.rotation = lastSafeRotation;

        OnPlayerReset?.Invoke();


        if (holder.Grab != null)
        {
            holder.Drop();
        }
    }


    public override void SpawnPrefab(GameObject prefabToSpawn, Vector3 originalSpawnPosition, Vector3 spawnPositionOffset, Quaternion startRotation)
    {
        Vector3 newSpawnPosition = originalSpawnPosition + spawnPositionOffset;
        GameObject newGameObject = null;

        if (MannequinPool.Current != null && MannequinPool.Current.HasObjectInPool())
        {
            newGameObject = MannequinPool.Current.Release(newSpawnPosition);
        }
        else
        {
            newGameObject = MannequinPool.Current.InstantiatePoolObject(prefabToSpawn, newSpawnPosition, startRotation) ;
        }


        //if IGrabable
        if (newGameObject != null)
            newGameObject.GetComponentInChildren<IGrabable>().OnGrabExit(); //this starts free fall behaviour what is always true on spawn
    }
}
