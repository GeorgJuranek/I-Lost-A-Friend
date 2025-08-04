
using UnityEngine;

public interface IGrabable
{
    abstract public Vector3 getHoldingRotation();

    abstract public Vector3 getSpawnRotation();


    abstract public void OnGrab();

    abstract public void OnGrabExit();

    abstract public GameObject ReflectSelf();

}
