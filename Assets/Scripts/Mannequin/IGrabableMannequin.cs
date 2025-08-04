using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGrabableMannequin : MonoBehaviour, IGrabable
{
    [SerializeField]
    Vector3 holdingRotation = new Vector3(-90, 0, 90);

    [SerializeField]
    Vector3 firstSpawnRotation = new Vector3(-90, 0, 0);

    [SerializeField]
    RagdollController controller;

    [SerializeField]
    FallDetector fallDetector;

    [SerializeField]
    GroundChecker groundChecker;

    Coroutine waitForSleepCoroutine = null;


    public Vector3 getHoldingRotation()
    {
        return holdingRotation;
    }

    public Vector3 getSpawnRotation()
    {
        return firstSpawnRotation;
    }

    public void OnGrab()
    {
        BringToPositionState();

        if (controller == null)
        {
            Debug.LogError("No Instance of Controller in IGrabableMannequin");
            return;
        }

        //If is nested fror transport must be unnested first
        if (TryGetComponent<INestable>(out INestable nestable))
        {
            if (nestable.TemporaryParent != null)
                nestable.Unnest();
        }
        else
        {
            Debug.LogWarning("No INestable on grabed Mannequin");
        }

        if (waitForSleepCoroutine != null)
            StopCoroutine(waitForSleepCoroutine);

        if (fallDetector.IsInCoroutine())
            fallDetector.StopFalling();

        SetToHoldState();

    }

    public void OnGrabExit()
    {
        if (controller == null)
        {
            Debug.LogError("No Instance of Controller in IGrabableMannequin");
            return;
        }

        if (waitForSleepCoroutine != null)
        {
            StopCoroutine(waitForSleepCoroutine);
            waitForSleepCoroutine = null;
        }

        SetToFalldetectState();
        waitForSleepCoroutine = StartCoroutine(WaitForSleep());
    }

    IEnumerator WaitForSleep()
    {
        bool hasGround = false;

        INestable nest = GetComponent<INestable>();

        do
        {
            hasGround = groundChecker.GroundCheck(Mathf.Infinity);

            if (!fallDetector.IsInCoroutine() && !hasGround
                && nest.TemporaryParent == null)
                fallDetector.StartFalling();

            yield return null;
        }
        while (controller.getAverageVelocity() > 0.2f);

        SetToOffState();
        waitForSleepCoroutine = null;
    }


    public GameObject ReflectSelf()
    {
        return this.gameObject;
    }

    void BringToPositionState()
    {
        controller.SetAllCollision(false); //Disable colliders and joints
    }

    void SetToHoldState()
    {
        controller.SetRigidbodiesKinematic(false); //Set isKinematic false
        controller.SetAllCollision(false); //Disable colliders and joints
    }

    void SetToOffState()
    {
        controller.SetVelocityForAllRigidbodies(Vector3.zero); //necessary? to be sure !condition of while-loop is really true
        controller.SetAllCollision(true);
        controller.SetRigidbodiesKinematic(true); //Sets rigidbodies to kinematic when stoped moving
    }

    void SetToFalldetectState()
    {
        controller.SetRigidbodiesKinematic(false); //makes sure is NOT kinematic
        controller.SetAllCollision(true); //Enable Colliders and Joints
    }


    void OnDestroy()
    {
        StopAllCoroutines();
    }

}