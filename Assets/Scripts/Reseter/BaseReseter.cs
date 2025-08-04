using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseReseter : MonoBehaviour
{
    abstract public void ResetObject(bool shallSpawn);

    abstract public void SpawnPrefab(GameObject prefabToSpawn, Vector3 originalSpawnPosition, Vector3 spawnPositionOffset, Quaternion startRotation);
}
