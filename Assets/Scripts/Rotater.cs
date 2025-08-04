using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    [SerializeField]
    Vector3 rotationSpeed = new Vector3(10f, 10f, 10f);

    [SerializeField]
    bool shallRandomizeStart, shallRandomizeRotSpeed;


    [SerializeField]
    float startRandomMin = 0, startRandomMax = 180f;

    [SerializeField]
    float speedRandomMin = 5, speedRandomMax = 25f;

    [SerializeField]
    bool shallRotateDuringMenu = false;


    private void Start()
    {
        if (shallRandomizeStart)
            transform.rotation = Quaternion.Euler(RandomizeVector(startRandomMin, startRandomMax));

        if (shallRandomizeRotSpeed)
            rotationSpeed = RandomizeVector(speedRandomMin, speedRandomMax);
    }


    private void Update()
    {
        transform.Rotate(rotationSpeed * (shallRotateDuringMenu ? 1f : Time.deltaTime));
    }


    Vector3 RandomizeVector(float min, float max)
    {
        float newX = Random.Range(min, max);
        float newY = Random.Range(min, max);
        float newZ = Random.Range(min, max);

        return new Vector3(newX, newY, newZ);
    }

}
