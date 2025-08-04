using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BalloonIGrabable : MonoBehaviour, IGrabable
{
    [SerializeField]
    Mover mover;

    [SerializeField]
    GameObject player;

    [SerializeField]
    float heightToRaiseUp = 50f;

    [SerializeField]
    JointConnector jointConnector;

    bool isActivated = false;

    [SerializeField]
    string nextScene;

    private void Awake()
    {
        if (nextScene==null)
        {
            Debug.LogWarning("Missing Scene Reference in BalloonTarget!");
        }
    }

    public static Action OnPlayerHasReachedGoal;

    public Vector3 getHoldingRotation()
    {
        return Vector3.zero;
    }

    public Vector3 getSpawnRotation()
    {
        return Vector3.zero;
    }

    public void OnGrab()
    {
        if (isActivated) return;

        isActivated = true;

        jointConnector.Hold(player);

        StartCoroutine(ToTheEnd());
    }

    public void OnGrabExit()
    {
        Debug.Log("Cant unnest final balloon!");
    }

    public GameObject ReflectSelf()
    {
        return this.gameObject;
    }

    IEnumerator ToTheEnd()
    {
        Vector3 targetPosition = transform.position + Vector3.up * heightToRaiseUp;

        while (mover.HandOutMoveToPositionCoroutine(targetPosition) != null)
        {
            yield return mover.HandOutMoveToPositionCoroutine(targetPosition);
        }

        OnPlayerHasReachedGoal?.Invoke();

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(nextScene);

        Debug.Log("Blue balloon has moved to its target");
    }
}
